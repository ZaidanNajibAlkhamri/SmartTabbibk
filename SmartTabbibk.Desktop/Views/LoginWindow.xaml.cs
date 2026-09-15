using System.Windows;
using SmartTabbibk.Desktop.Services;
using SmartTabbibk.Desktop.ViewModels;

namespace SmartTabbibk.Desktop.Views
{
    public partial class LoginWindow : Window
    {
        private readonly ApiClient _apiClient;
        private readonly LoginViewModel _viewModel;

        public LoginWindow()
        {
            InitializeComponent();

            _apiClient = new ApiClient();
            _viewModel = new LoginViewModel(new AuthService(_apiClient));
            _viewModel.LoginSucceeded += OnLoginSucceeded;

            DataContext = _viewModel;
        }

        // PasswordBox في WPF لا يدعم Binding مباشر لأسباب أمنية (تسريب كلمة المرور عبر Memory Dump)
        // فهذا الكود القليل هو الاستثناء الوحيد المقبول في MVVM للتعامل مع PasswordBox
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.Password = PasswordBox.Password;
        }

        private void OnLoginSucceeded()
        {
            if (_viewModel.LoggedInUser == null) return;

            var mainWindow = new MainWindow(new MainViewModel(_viewModel.LoggedInUser, _apiClient));
            mainWindow.Show();
            Close();
        }
    }
}
