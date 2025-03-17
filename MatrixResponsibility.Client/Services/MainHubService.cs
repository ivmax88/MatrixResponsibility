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
        private HubConnection? _connection;

        public event Func<Project, Task>? OnProjectChangedAsync;

        public MainHubService(ILocalStorageService localStorage, ILogger<MainHubService> logger)
        {
            _localStorage = localStorage;
            _logger = logger;
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
                .WithUrl($"http://localhost:5102/hubs/main?{str.access_token}={Uri.EscapeDataString(token)}")
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
                    _logger.LogInformation("Project changed: {Project}", project);
                    if (OnProjectChangedAsync != null)
                    {
                        await OnProjectChangedAsync(project);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error handling ProjectChanged event.");
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


        public async Task ChangeProjectInfo(Project project, CancellationToken cancellationToken = default)
        {
            if (_connection == null || _connection.State == HubConnectionState.Disconnected)
            {
                throw new InvalidOperationException("SignalR connection is not established.");
            }

            try
            {
                await _connection.InvokeAsync(str.ChangeProjectInfo, project, cancellationToken);
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
                _logger.LogInformation("SignalR connection disposed.");
            }
        }
    }
}