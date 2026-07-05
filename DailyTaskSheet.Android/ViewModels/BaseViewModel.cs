using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DailyTaskSheet.App.ViewModels
{
    /// <summary>
    /// Base ViewModel implementing INotifyPropertyChanged for MVVM data binding.
    /// All ViewModels inherit from this base to get property change notifications.
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        private bool _isBusy;
        private string _title = string.Empty;
        private string _errorMessage = string.Empty;

        /// <summary>Gets or sets whether the ViewModel is performing a background operation.</summary>
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        /// <summary>Gets or sets the page/section title.</summary>
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        /// <summary>Gets or sets the current error message (empty if no error).</summary>
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        /// <summary>Gets whether there is currently an error.</summary>
        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

        /// <inheritdoc />
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Sets a property value and raises PropertyChanged if the value changed.
        /// </summary>
        protected bool SetProperty<T>(ref T backingField, T value, [CallerMemberName] string propertyName = "")
        {
            if (Equals(backingField, value)) return false;
            backingField = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Clears any displayed error message.
        /// </summary>
        protected void ClearError()
        {
            ErrorMessage = string.Empty;
        }
    }
}
