using Microsoft.ML;
using SmartTabbibk.AI.Training;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("=== تدريب نموذج الأولوية (Priority Model) ===\n");

var mlContext = new MLContext(seed: 42);

// ===== 1. تحميل بيانات التدريب (101 مثال — بديل عن مجموعة الـ 888 المولَّدة سابقاً بأداة AI خارجية) =====
var trainingPath = Path.Combine(AppContext.BaseDirectory, "Data", "priority-training-data.tsv");
IDataView trainingData = mlContext.Data.LoadFromTextFile<PriorityInput>(
    trainingPath, hasHeader: true, separatorChar: '\t');

// ===== 2. بناء خط المعالجة (Pipeline) =====
var pipeline = mlContext.Transforms.Text.FeaturizeText("Features", nameof(PriorityInput.Symptoms))
    .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(
        labelColumnName: nameof(PriorityInput.IsUrgent), featureColumnName: "Features"));

// ===== 3. التدريب على كامل بيانات التدريب =====
// ملاحظة: التقييم الحقيقي بيحصل بعد كده على ملف priority-test-data.tsv المنفصل تماماً،
// مش على جزء مقتطع من نفس بيانات التدريب — هذا هو الفرق الجوهري عن النسخة السابقة
Console.WriteLine("جارِ تدريب النموذج...");
var model = pipeline.Fit(trainingData);

// ===== 4. حفظ النموذج المدرَّب =====
var modelOutputPath = Path.Combine(AppContext.BaseDirectory, "PriorityModel.zip");
mlContext.Model.Save(model, trainingData.Schema, modelOutputPath);
Console.WriteLine($"✅ تم حفظ النموذج في: {modelOutputPath}");
Console.WriteLine("انسخ هذا الملف إلى SmartTabbibk.API/AIModels/");

// ===== 5. التقييم الدفعي الحقيقي على بيانات اختبار منفصلة تماماً =====
var testPath = Path.Combine(AppContext.BaseDirectory, "Data", "priority-test-data.tsv");
var reportPath = Path.Combine(AppContext.BaseDirectory, "misclassified-report.tsv");
BatchEvaluator.Run(mlContext, model, testPath, reportPath);

// ===== 6. اختبار سريع يدوي لعينات إضافية (اختياري، للتأكد البصري السريع) =====
var predictionEngine = mlContext.Model.CreatePredictionEngine<PriorityInput, PriorityPrediction>(model);

Console.WriteLine("\n=== أمثلة سريعة إضافية ===");
TestSample(predictionEngine, "ألم شديد في الصدر مع صعوبة في التنفس");
TestSample(predictionEngine, "صداع خفيف بسيط منذ الصباح");

void TestSample(PredictionEngine<PriorityInput, PriorityPrediction> engine, string symptoms)
{
    var result = engine.Predict(new PriorityInput { Symptoms = symptoms });
    var label = result.IsUrgent ? "مستعجل (Urgent)" : "عادي (Normal)";
    Console.WriteLine($"\"{symptoms}\" → {label} ({result.Probability:P1})");
}
