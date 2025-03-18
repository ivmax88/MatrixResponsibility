using Blazored.LocalStorage;
using MatrixResponsibility.Common;
using MatrixResponsibility.Common.Constants;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace MatrixResponsibility.Client.Services
{
    public class MainHubService : IAsyncDisposable
    {
        private readonly ILocalStorageService _localStorage;
        private readonly ILogger<MainHubService> _logger;
        private readonly IConfiguration configuration;
        private HubConnection? _connection;

        public event Func<Project, Task>? OnProjectChanged;
        public event Func<int, Task>? OnConnectedClientsCountChanged;

        public MainHubService(ILocalStorageService localStorage, 
            ILogger<MainHubService> logger,
            IConfiguration configuration)
        {
            _localStorage = localStorage;
            _logger = logger;
            this.configuration=configuration;
        }

        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            if (_connection != null) return;

            var token = await _localStorage.GetItemAsync<string>(str.jwttoken, cancellationToken);
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("No JWT token found in local storage.");
                throw new InvalidOperationException("JWT token is required to connect to SignalR.");
            }

            _connection = new HubConnectionBuilder()
                .WithUrl($"{configuration["ApiUrl"]}/hubs/main?{str.access_token}={Uri.EscapeDataString(token)}", opt =>
                {
                    opt.CloseTimeout = TimeSpan.FromMinutes(2);
                })
                .WithAutomaticReconnect()
                .AddJsonProtocol(o=>
                {
                    o.PayloadSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
                })
                .Build();

            _connection.On<Project?>(str.ProjectChanged, async (project) =>
            {
                try
                {
                    _logger.LogInformation($"Project changed: {project}");
                    if (OnProjectChanged != null)
                    {
                        await OnProjectChanged(project);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error handling {str.ProjectChanged} event.");
                }
            });

            _connection.On<int>(str.ConnectedClientsCount, async (count) =>
            {
                try
                {
                    _logger.LogInformation($"Online connections: {count}");
                    if (OnConnectedClientsCountChanged != null)
                    {
                        await OnConnectedClientsCountChanged(count);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error handling {str.ConnectedClientsCount} event.");
                }
            });

            await StartAsync(cancellationToken);
        }

        public async Task<List<Project>?> GetAllProjects(CancellationToken ct)
        {
            if (_connection == null || _connection.State == HubConnectionState.Disconnected)
            {
                throw new InvalidOperationException("SignalR connection is not established.");
            }
            try
            {
                var r= await _connection.InvokeAsync<List<Project>?>(str.GetAllProjects, ct);
                _logger.LogInformation("GetAllProjects");
                return r;
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("GetAllProjects operation was canceled.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing project via SignalR.");
                throw;
            }
        }


        public async Task ChangeProjectInfo(Project project)
        {
            if (_connection == null || _connection.State == HubConnectionState.Disconnected)
            {
                throw new InvalidOperationException("SignalR connection is not established.");
            }

            try
            {
                await _connection.InvokeAsync(str.ChangeProjectInfo, project);
                _logger.LogInformation("Project info changed: {Project}", project);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("ChangeProjectInfo operation was canceled.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing project via SignalR.");
                throw;
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            if (_connection == null || _connection.State == HubConnectionState.Disconnected)
            {
                try
                {
                    await _connection?.StartAsync(cancellationToken);
                    _logger.LogInformation("SignalR connection started.");
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("StartAsync operation was canceled.");
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error starting SignalR connection.");
                    throw;
                }
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            if (_connection != null && _connection.State != HubConnectionState.Disconnected)
            {
                try
                {
                    await _connection.StopAsync(cancellationToken);
                    _logger.LogInformation("SignalR connection stopped.");
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("StopAsync operation was canceled.");
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error stopping SignalR connection.");
                    throw;
                }
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_connection != null)
            {
                await _connection.DisposeAsync();
                _connection = null;
                _logger.LogInformation("SignalR connection disposed.");
            }
        }
    }
}