using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Callyzer.App.ViewModels
{
    /// <summary>
    /// Base ViewModel using CommunityToolkit.Mvvm source generators.
    /// All ViewModels inherit from this to get IsBusy, Title, ErrorMessage, and navigation support.
    /// </summary>
    public abstract partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        private bool _isBusy;

        [ObservableProperty]
        private string _title = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasError))]
        private string _errorMessage = string.Empty;

        /// <summary>Inverse of IsBusy for UI binding (e.g., enabling buttons).</summary>
        public bool IsNotBusy => !IsBusy;

        /// <summary>Whether there is currently an error to display.</summary>
        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

        /// <summary>Clears any displayed error message.</summary>
        protected void ClearError() => ErrorMessage = string.Empty;

        /// <summary>
        /// Wraps an async operation with IsBusy tracking and error handling.
        /// </summary>
        protected async Task ExecuteBusyAsync(Func<Task> operation, string? errorContext = null)
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                ClearError();
                await operation();
            }
            catch (Exception ex)
            {
                ErrorMessage = errorContext != null
                    ? $"{errorContext}: {ex.Message}"
                    : ex.Message;
                System.Diagnostics.Debug.WriteLine($"[{GetType().Name}] Error: {ex}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
