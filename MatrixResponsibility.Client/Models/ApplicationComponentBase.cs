using Microsoft.AspNetCore.Components;

namespace MatrixResponsibility.Client.Models
{
    public abstract class ApplicationComponentBase : ComponentBase, IAsyncDisposable
    {
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        // Токен отмены, доступный для использования в производных классах
        protected CancellationToken CancellationToken => _cts.Token;

        // Асинхронная инициализация компонента
        protected override async Task OnInitializedAsync()
        {
            try
            {
                await InitializeAsync(CancellationToken);
            }
            catch (OperationCanceledException)
            {
                // Операция была отменена, ничего не делаем
            }
            catch (Exception ex)
            {
                // Обработка ошибок при инициализации
                Console.WriteLine($"Error during initialization: {ex.Message}");
                throw;
            }
        }

        // Виртуальный метод для асинхронной инициализации в производных классах
        protected virtual Task InitializeAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        // Асинхронное освобождение ресурсов
        public async ValueTask DisposeAsync()
        {
            try
            {
                // Отменяем все операции, связанные с этим компонентом
                _cts.Cancel();
                await DisposeResourcesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during disposal: {ex.Message}");
            }
            finally
            {
                _cts.Dispose();
            }
        }

        // Виртуальный метод для освобождения ресурсов в производных классах
        protected virtual Task DisposeResourcesAsync()
        {
            return Task.CompletedTask;
        }
    }
}