using SmartTabbibk.Desktop.Models;

namespace SmartTabbibk.Desktop.Services
{
    public class DoctorService
    {
        private readonly ApiClient _apiClient;

        public DoctorService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public Task<List<DoctorDto>?> GetPendingAsync() =>
            _apiClient.GetAsync<List<DoctorDto>>("/doctors/pending");

        public Task ApproveAsync(int doctorId, bool approve) =>
            _apiClient.PutAsync($"/doctors/{doctorId}/approve", new ApproveDoctorRequest { Approve = approve });
    }
}
