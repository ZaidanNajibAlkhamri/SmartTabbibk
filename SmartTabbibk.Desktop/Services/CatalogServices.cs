using SmartTabbibk.Desktop.Models;

namespace SmartTabbibk.Desktop.Services
{
    public class MedicineService
    {
        private readonly ApiClient _apiClient;

        public MedicineService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public Task<List<MedicineDto>?> GetAllAsync() =>
            _apiClient.GetAsync<List<MedicineDto>>("/medicines/all");

        public Task<MedicineDto?> CreateAsync(CreateMedicineRequest request) =>
            _apiClient.PostAsync<CreateMedicineRequest, MedicineDto>("/medicines", request);

        public Task DeactivateAsync(int id) => _apiClient.PutAsync<object?>($"/medicines/{id}/deactivate", null);
        public Task ActivateAsync(int id) => _apiClient.PutAsync<object?>($"/medicines/{id}/activate", null);
    }

    public class SpecialtyService
    {
        private readonly ApiClient _apiClient;

        public SpecialtyService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public Task<List<SpecialtyDto>?> GetAllAsync() =>
            _apiClient.GetAsync<List<SpecialtyDto>>("/specialties");

        public Task<SpecialtyDto?> CreateAsync(CreateSpecialtyRequest request) =>
            _apiClient.PostAsync<CreateSpecialtyRequest, SpecialtyDto>("/specialties", request);

        public Task DeleteAsync(int id) => _apiClient.DeleteAsync($"/specialties/{id}");
    }
}
