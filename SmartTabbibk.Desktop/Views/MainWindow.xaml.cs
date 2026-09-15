using System.Windows;
using SmartTabbibk.Desktop.ViewModels;

namespace SmartTabbibk.Desktop.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
