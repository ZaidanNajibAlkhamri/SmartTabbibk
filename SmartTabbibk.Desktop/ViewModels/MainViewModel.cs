using SmartTabbibk.Desktop.Models;
using SmartTabbibk.Desktop.Services;

namespace SmartTabbibk.Desktop.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public AuthResponse CurrentUser { get; }

        public PendingDoctorsViewModel PendingDoctorsVM { get; }
        public MedicinesViewModel MedicinesVM { get; }
        public SpecialtiesViewModel SpecialtiesVM { get; }

        private int _selectedTabIndex;
        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set => SetProperty(ref _selectedTabIndex, value);
        }

        public MainViewModel(AuthResponse currentUser, ApiClient apiClient)
        {
            CurrentUser = currentUser;

            PendingDoctorsVM = new PendingDoctorsViewModel(new DoctorService(apiClient));
            MedicinesVM = new MedicinesViewModel(new MedicineService(apiClient));
            SpecialtiesVM = new SpecialtiesViewModel(new SpecialtyService(apiClient));

            // تحميل أول تبويب فور فتح الشاشة
            _ = PendingDoctorsVM.LoadAsync();
        }
    }
}
