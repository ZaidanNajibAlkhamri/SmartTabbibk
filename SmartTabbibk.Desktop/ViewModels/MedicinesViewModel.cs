using System.Collections.ObjectModel;
using SmartTabbibk.Desktop.Models;
using SmartTabbibk.Desktop.Services;

namespace SmartTabbibk.Desktop.ViewModels
{
    public class MedicinesViewModel : ViewModelBase
    {
        private readonly MedicineService _medicineService;

        public ObservableCollection<MedicineDto> Medicines { get; } = new();

        private string _newName = string.Empty;
        public string NewName { get => _newName; set => SetProperty(ref _newName, value); }

        private string _newStrength = string.Empty;
        public string NewStrength { get => _newStrength; set => SetProperty(ref _newStrength, value); }

        private string _newForm = "Tablet";
        public string NewForm { get => _newForm; set => SetProperty(ref _newForm, value); }

        private string _statusMessage = string.Empty;
        public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }

        private bool _isBusy;
        public bool IsBusy { get => _isBusy; set => SetProperty(ref _isBusy, value); }

        public RelayCommand RefreshCommand { get; }
        public RelayCommand AddCommand { get; }

        public MedicinesViewModel(MedicineService medicineService)
        {
            _medicineService = medicineService;
            RefreshCommand = new RelayCommand(LoadAsync);
            AddCommand = new RelayCommand(AddAsync);
        }

        public async Task LoadAsync()
        {
            IsBusy = true;
            try
            {
                var result = await _medicineService.GetAllAsync() ?? new List<MedicineDto>();
                Medicines.Clear();
                foreach (var m in result) Medicines.Add(m);
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
                await _medicineService.CreateAsync(new CreateMedicineRequest
                {
                    Name = NewName,
                    Strength = NewStrength,
                    Form = NewForm
                });

                NewName = string.Empty;
                NewStrength = string.Empty;
                await LoadAsync();
            }
            catch (ApiException ex)
            {
                StatusMessage = ex.Message; // زي رسالة "الدواء مسجّل بالفعل" لو تكرار
            }
        }

        public async Task ToggleActiveAsync(MedicineDto medicine)
        {
            try
            {
                if (medicine.IsActive)
                    await _medicineService.DeactivateAsync(medicine.MedicineId);
                else
                    await _medicineService.ActivateAsync(medicine.MedicineId);

                await LoadAsync();
            }
            catch (ApiException ex)
            {
                StatusMessage = ex.Message;
            }
        }
    }
}
