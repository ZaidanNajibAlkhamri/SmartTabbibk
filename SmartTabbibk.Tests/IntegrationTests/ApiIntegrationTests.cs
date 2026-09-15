using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace SmartTabbibk.Tests.IntegrationTests
{
    /// <summary>
    /// Integration Tests حقيقية: تشغّل الـ API كاملاً (Program.cs الفعلي، كل الطبقات مسجّلة)
    /// وترسل طلبات HTTP حقيقية، بعكس Unit Tests اللي بتعزل الكود عن أي بنية تحتية.
    /// ملاحظة: هذه الاختبارات تتصل بنفس قاعدة البيانات المحلية المستخدمة في التطوير،
    /// فتأكد من تشغيل SQL Server قبل تنفيذها.
    /// </summary>
    public class ApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ApiIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetSpecialties_ShouldReturnOk_WithoutAuthentication()
        {
            // Specialties API عام (AllowAnonymous) — يجب أن يعمل بدون أي توكن
            var response = await _client.GetAsync("/api/specialties");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetPendingDoctors_ShouldReturnUnauthorized_WithoutToken()
        {
            // Endpoint محمي بـ [Authorize(Roles = "Admin")] — بدون توكن يجب أن يُرفض بـ 401
            var response = await _client.GetAsync("/api/doctors/pending");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Login_ShouldReturnBadRequest_WhenEmailIsInvalid()
        {
            // يختبر تفعيل Input Validation فعلياً عبر HTTP حقيقي (مش قناعة نظرية)
            var content = new StringContent(
                "{\"email\":\"not-an-email\",\"password\":\"12345678\"}",
                System.Text.Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/auth/login", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetSpecialties_ShouldSupportXmlFormat_WhenRequested()
        {
            // يتحقق فعلياً من تفعيل AddXmlSerializerFormatters()
            _client.DefaultRequestHeaders.Add("Accept", "application/xml");

            var response = await _client.GetAsync("/api/specialties");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("xml", response.Content.Headers.ContentType?.MediaType ?? "");
        }
    }
}
