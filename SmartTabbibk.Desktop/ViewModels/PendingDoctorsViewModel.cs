using System.Collections.ObjectModel;
using SmartTabbibk.Desktop.Models;
using SmartTabbibk.Desktop.Services;

namespace SmartTabbibk.Desktop.ViewModels
{
    public class PendingDoctorsViewModel : ViewModelBase
    {
        private readonly DoctorService _doctorService;

        public ObservableCollection<DoctorDto> Doctors { get; } = new();

        private string _statusMessage = string.Empty;
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public RelayCommand RefreshCommand { get; }

        public PendingDoctorsViewModel(DoctorService doctorService)
        {
            _doctorService = doctorService;
            RefreshCommand = new RelayCommand(LoadAsync);
        }

        public async Task LoadAsync()
        {
            IsBusy = true;
            StatusMessage = string.Empty;
            try
            {
                var result = await _doctorService.GetPendingAsync() ?? new List<DoctorDto>();
                Doctors.Clear();
                foreach (var doctor in result) Doctors.Add(doctor);

                if (Doctors.Count == 0)
                    StatusMessage = "لا توجد طلبات بانتظار المراجعة.";
            }
            catch (ApiException ex)
            {
                StatusMessage = ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task ApproveAsync(DoctorDto doctor, bool approve)
        {
            try
            {
                await _doctorService.ApproveAsync(doctor.DoctorId, approve);
                Doctors.Remove(doctor);
                StatusMessage = approve ? $"تم اعتماد {doctor.Name}." : $"تم رفض {doctor.Name}.";
            }
            catch (ApiException ex)
            {
                StatusMessage = ex.Message;
            }
        }
    }
}
