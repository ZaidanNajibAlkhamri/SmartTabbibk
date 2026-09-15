using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using SmartTabbibk.Desktop.Models;

namespace SmartTabbibk.Desktop.Services
{
    /// <summary>
    /// طبقة الاتصال المركزية بالـ Backend — نفس دور axiosClient.js في نسخة React تماماً:
    /// تبني الطلب، ترفق التوكن تلقائياً، وتوحّد التعامل مع الأخطاء.
    /// </summary>
    public class ApiClient
    {
        private const string BaseUrl = "https://localhost:7236/api/";

        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public string? Token { get; set; }

        public ApiClient()
        {
            // تجاوز فحص شهادة HTTPS الذاتية التوقيع في بيئة التطوير
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true
            };

            _http = new HttpClient(handler) { BaseAddress = new Uri(BaseUrl) };
        }

        /// <summary>
        /// تنظيف المسار لمنع إلغاء /api/ في حال تم إرسال Endpoint يبدأ بـ /
        /// </summary>
        private static string NormalizeEndpoint(string endpoint)
        {
            return endpoint.StartsWith('/') ? endpoint.TrimStart('/') : endpoint;
        }

        private void AttachToken(HttpRequestMessage request)
        {
            if (!string.IsNullOrEmpty(Token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token);
        }

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, NormalizeEndpoint(endpoint));
            AttachToken(request);

            var response = await _http.SendAsync(request);
            await EnsureSuccessOrThrow(response);

            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }

        public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest body)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, NormalizeEndpoint(endpoint))
            {
                Content = JsonContent.Create(body)
            };
            AttachToken(request);

            var response = await _http.SendAsync(request);
            await EnsureSuccessOrThrow(response);

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return default;

            return await response.Content.ReadFromJsonAsync<TResponse>(_jsonOptions);
        }

        public async Task PutAsync<TRequest>(string endpoint, TRequest body)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, NormalizeEndpoint(endpoint))
            {
                Content = JsonContent.Create(body)
            };
            AttachToken(request);

            var response = await _http.SendAsync(request);
            await EnsureSuccessOrThrow(response);
        }

        public async Task DeleteAsync(string endpoint)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, NormalizeEndpoint(endpoint));
            AttachToken(request);

            var response = await _http.SendAsync(request);
            await EnsureSuccessOrThrow(response);
        }

        private async Task EnsureSuccessOrThrow(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode) return;

            string message = $"خطأ غير متوقع ({(int)response.StatusCode})";
            try
            {
                var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>(_jsonOptions);
                if (!string.IsNullOrEmpty(error?.Message))
                    message = error.Message;
            }
            catch { /* الرد ليس JSON، نستخدم الرسالة الافتراضية */ }

            throw new ApiException(message, response.StatusCode);
        }
    }

    public class ApiException : Exception
    {
        public System.Net.HttpStatusCode StatusCode { get; }

        public ApiException(string message, System.Net.HttpStatusCode statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
