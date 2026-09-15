using System.Windows;
using System.Windows.Controls;
using SmartTabbibk.Desktop.Models;
using SmartTabbibk.Desktop.ViewModels;

namespace SmartTabbibk.Desktop.Views
{
    public partial class PendingDoctorsView : UserControl
    {
        private PendingDoctorsViewModel ViewModel => (PendingDoctorsViewModel)DataContext;

        public PendingDoctorsView()
        {
            InitializeComponent();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (ViewModel.Doctors.Count == 0)
                await ViewModel.LoadAsync();
        }

        private async void Approve_Click(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is DoctorDto doctor)
                await ViewModel.ApproveAsync(doctor, approve: true);
        }

        private async void Reject_Click(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is DoctorDto doctor)
                await ViewModel.ApproveAsync(doctor, approve: false);
        }
    }
}
