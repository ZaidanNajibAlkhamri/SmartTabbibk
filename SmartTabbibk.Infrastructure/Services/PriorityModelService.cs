using Microsoft.Extensions.Configuration;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace SmartTabbibk.Infrastructure.Services
{
    /// <summary>
    /// يحمّل نموذج ML.NET المدرَّب مرة واحدة فقط (Singleton) ويحتفظ به في الذاكرة،
    /// عشان ميحصلش إعادة تحميل الملف من القرص مع كل طلب حجز موعد.
    /// الـ Lock ضروري لأن PredictionEngine في ML.NET غير Thread-Safe بطبيعته.
    /// </summary>
    public class PriorityModelService
    {
        private readonly PredictionEngine<PriorityModelInput, PriorityModelOutput> _predictionEngine;
        private readonly object _lock = new();

        public PriorityModelService(IConfiguration configuration)
        {
            var mlContext = new MLContext();

            var modelPath = configuration["AI:PriorityModelPath"] ?? "AIModels/PriorityModel.zip";
            var fullPath = Path.Combine(AppContext.BaseDirectory, modelPath);

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException(
                    $"ملف نموذج الأولوية غير موجود في: {fullPath}. " +
                    "شغّل مشروع SmartTabbibk.AI.Training أولاً وانسخ PriorityModel.zip للمسار الصحيح.");
            }

            var model = mlContext.Model.Load(fullPath, out _);
            _predictionEngine = mlContext.Model.CreatePredictionEngine<PriorityModelInput, PriorityModelOutput>(model);
        }

        public bool PredictIsUrgent(string symptoms)
        {
            // Lock إجباري: PredictionEngine الواحد مش آمن للاستخدام المتزامن من كذا Thread
            lock (_lock)
            {
                var result = _predictionEngine.Predict(new PriorityModelInput { Symptoms = symptoms });
                return result.IsUrgent;
            }
        }
    }

    public class PriorityModelInput
    {
        public string Symptoms { get; set; } = string.Empty;
    }

    public class PriorityModelOutput
    {
        [ColumnName("PredictedLabel")]
        public bool IsUrgent { get; set; }
        public float Probability { get; set; }
    }
}
