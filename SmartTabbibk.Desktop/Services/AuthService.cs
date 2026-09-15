using SmartTabbibk.Desktop.Models;

namespace SmartTabbibk.Desktop.Services
{
    public class AuthService
    {
        private readonly ApiClient _apiClient;

        public AuthService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<AuthResponse> LoginAsync(string email, string password)
        {
            var request = new LoginRequest { Email = email, Password = password };
            var response = await _apiClient.PostAsync<LoginRequest, AuthResponse>("/auth/login", request)
                ?? throw new ApiException("لم يتم استلام رد صحيح من الخادم.", System.Net.HttpStatusCode.BadGateway);

            if (response.Role != "Admin")
                throw new ApiException("هذا التطبيق مخصص لحسابات الإدارة فقط.", System.Net.HttpStatusCode.Forbidden);

            _apiClient.Token = response.Token;
            return response;
        }
    }
}
