using System.Collections.ObjectModel;
using SmartTabbibk.Desktop.Models;
using SmartTabbibk.Desktop.Services;

namespace SmartTabbibk.Desktop.ViewModels
{
    public class SpecialtiesViewModel : ViewModelBase
    {
        private readonly SpecialtyService _specialtyService;

        public ObservableCollection<SpecialtyDto> Specialties { get; } = new();

        private string _newName = string.Empty;
        public string NewName { get => _newName; set => SetProperty(ref _newName, value); }

        private string _newKeywords = string.Empty;
        public string NewKeywords { get => _newKeywords; set => SetProperty(ref _newKeywords, value); }

        private string _statusMessage = string.Empty;
        public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }

        private bool _isBusy;
        public bool IsBusy { get => _isBusy; set => SetProperty(ref _isBusy, value); }

        public RelayCommand RefreshCommand { get; }
        public RelayCommand AddCommand { get; }

        public SpecialtiesViewModel(SpecialtyService specialtyService)
        {
            _specialtyService = specialtyService;
            RefreshCommand = new RelayCommand(LoadAsync);
            AddCommand = new RelayCommand(AddAsync);
        }

        public async Task LoadAsync()
        {
            IsBusy = true;
            try
            {
                var result = await _specialtyService.GetAllAsync() ?? new List<SpecialtyDto>();
                Specialties.Clear();
                foreach (var s in result) Specialties.Add(s);
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

        private async Task AddAsync()
        {
            StatusMessage = string.Empty;
            try
            {
                await _specialtyService.CreateAsync(new CreateSpecialtyRequest
                {
                    Name = NewName,
                    Keywords = string.IsNullOrWhiteSpace(NewKeywords) ? null : NewKeywords
                });

                NewName = string.Empty;
                NewKeywords = string.Empty;
                await LoadAsync();
            }
            catch (ApiException ex)
            {
                StatusMessage = ex.Message;
            }
        }

        public async Task DeleteAsync(SpecialtyDto specialty)
        {
            StatusMessage = string.Empty;
            try
            {
                await _specialtyService.DeleteAsync(specialty.SpecialtyId);
                Specialties.Remove(specialty);
            }
            catch (ApiException ex)
            {
                // زي رسالة "لا يمكن حذف التخصص لوجود أطباء مرتبطين به" القادمة من الـ Backend فعلياً
                StatusMessage = ex.Message;
            }
        }
    }
}
