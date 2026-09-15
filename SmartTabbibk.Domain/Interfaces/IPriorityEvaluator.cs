using SmartTabbibk.Domain.Enums;

namespace SmartTabbibk.Domain.Interfaces
{
    /// <summary>
    /// Strategy Pattern: يفصل منطق "تحديد الأولوية" عن باقي النظام.
    /// V1  → ManualPriorityEvaluator (Infrastructure)
    /// V2+ → AIPriorityEvaluator (Infrastructure) — بدون تعديل هذا الـ Interface
    ///       أو أي طبقة أخرى (Controller / Database / Appointment Workflow)
    /// تطبيق مباشر لـ Open/Closed Principle (SOLID)
    /// </summary>
    public interface IPriorityEvaluator
    {
        PriorityLevel Evaluate(string symptoms);
    }
}
