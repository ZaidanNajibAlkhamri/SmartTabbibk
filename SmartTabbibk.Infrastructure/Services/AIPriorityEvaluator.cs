using SmartTabbibk.Domain.Enums;
using SmartTabbibk.Domain.Interfaces;

namespace SmartTabbibk.Infrastructure.Services
{
    /// <summary>
    /// V2 — تنفيذ AI حقيقي لـ IPriorityEvaluator باستخدام نموذج ML.NET مدرَّب.
    /// هذا الكلاس بديل مباشر لـ ManualPriorityEvaluator — نفس الـ Interface بالظبط،
    /// بدون أي تعديل في AppointmentService أو الـ Controller أو قاعدة البيانات.
    /// تطبيق حي لمبدأ Open/Closed Principle وStrategy Pattern الموثَّقين في التصميم.
    /// </summary>
    public class AIPriorityEvaluator : IPriorityEvaluator
    {
        private readonly PriorityModelService _modelService;

        public AIPriorityEvaluator(PriorityModelService modelService)
        {
            _modelService = modelService;
        }

        public PriorityLevel Evaluate(string symptoms)
        {
            if (string.IsNullOrWhiteSpace(symptoms))
                return PriorityLevel.Normal;

            bool isUrgent = _modelService.PredictIsUrgent(symptoms);
            return isUrgent ? PriorityLevel.Urgent : PriorityLevel.Normal;
        }
    }
}
