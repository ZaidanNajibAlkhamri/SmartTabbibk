using System.Windows;
using System.Windows.Controls;
using SmartTabbibk.Desktop.Models;
using SmartTabbibk.Desktop.ViewModels;

namespace SmartTabbibk.Desktop.Views
{
    public partial class MedicinesView : UserControl
    {
        private MedicinesViewModel ViewModel => (MedicinesViewModel)DataContext;

        public MedicinesView()
        {
            InitializeComponent();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (ViewModel.Medicines.Count == 0)
                await ViewModel.LoadAsync();
        }

        private async void ToggleActive_Click(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is MedicineDto medicine)
                await ViewModel.ToggleActiveAsync(medicine);
        }
    }
}
