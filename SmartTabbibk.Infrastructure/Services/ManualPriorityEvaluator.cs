using SmartTabbibk.Domain.Enums;
using SmartTabbibk.Domain.Interfaces;

namespace SmartTabbibk.Infrastructure.Services
{
    /// <summary>
    /// تنفيذ V1: قيمة افتراضية بسيطة — القرار الفعلي يدوي من المريض عبر الـ DTO
    /// (هذا التنفيذ يُستخدم فقط كـ Fallback إذا لم يُحدَّد شيء)
    /// V2 المستقبلي: AIPriorityEvaluator يحل محل هذا الكلاس فقط، بدون أي تعديل
    /// في IPriorityEvaluator أو Appointment أو الـ Controller
    /// </summary>
    public class ManualPriorityEvaluator : IPriorityEvaluator
    {
        public PriorityLevel Evaluate(string symptoms)
        {
            return PriorityLevel.Normal; // القيمة الافتراضية — المريض يقدر يغيّرها يدوياً وقت الحجز
        }
    }
}
