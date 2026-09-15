using Microsoft.ML.Data;

namespace SmartTabbibk.AI.Training
{
    /// <summary>سطر بيانات تدريب واحد — نص الأعراض + هل هي حالة مستعجلة أم لا</summary>
    public class PriorityInput
    {
        [LoadColumn(0)]
        public string Symptoms { get; set; } = string.Empty;

        [LoadColumn(1)]
        public bool IsUrgent { get; set; }
    }

    /// <summary>نتيجة التنبؤ اللي النموذج بيرجّعها</summary>
    public class PriorityPrediction
    {
        [ColumnName("PredictedLabel")]
        public bool IsUrgent { get; set; }

        public float Probability { get; set; }
        public float Score { get; set; }
    }
}
