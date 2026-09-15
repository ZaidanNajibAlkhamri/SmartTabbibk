using SmartTabbibk.Desktop.Models;
using SmartTabbibk.Desktop.Services;

namespace SmartTabbibk.Desktop.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly AuthService _authService;

        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        private string _password = string.Empty;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public AuthResponse? LoggedInUser { get; private set; }

        /// يُستدعى بعد نجاح الدخول عشان الـ View يقدر يتنقل لشاشة تانية
        public event Action? LoginSucceeded;

        public RelayCommand LoginCommand { get; }

        public LoginViewModel(AuthService authService)
        {
            _authService = authService;
            LoginCommand = new RelayCommand(LoginAsync);
        }

        private async Task LoginAsync()
        {
            ErrorMessage = string.Empty;
            IsLoading = true;

            try
            {
                LoggedInUser = await _authService.LoginAsync(Email, Password);
                LoginSucceeded?.Invoke();
            }
            catch (ApiException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (Exception)
            {
                ErrorMessage = "تعذّر الاتصال بالخادم. تأكد من تشغيل الـ Backend وصحة البورت.";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
