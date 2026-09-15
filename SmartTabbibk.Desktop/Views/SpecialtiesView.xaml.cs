using System.Windows;
using System.Windows.Controls;
using SmartTabbibk.Desktop.Models;
using SmartTabbibk.Desktop.ViewModels;

namespace SmartTabbibk.Desktop.Views
{
    public partial class SpecialtiesView : UserControl
    {
        private SpecialtiesViewModel ViewModel => (SpecialtiesViewModel)DataContext;

        public SpecialtiesView()
        {
            InitializeComponent();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (ViewModel.Specialties.Count == 0)
                await ViewModel.LoadAsync();
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is SpecialtyDto specialty)
                await ViewModel.DeleteAsync(specialty);
        }
    }
}
