using Microsoft.ML;

namespace SmartTabbibk.AI.Training
{
    /// <summary>
    /// مسؤول فقط عن تقييم نموذج مدرَّب مسبقاً على بيانات اختبار منفصلة تماماً
    /// عن بيانات التدريب — هذا هو الاختبار الحقيقي لقدرة النموذج على التعميم
    /// (Generalization) على حالات لم يرها أثناء التدريب.
    /// </summary>
    public static class BatchEvaluator
    {
        public static void Run(MLContext mlContext, ITransformer model, string testDataPath, string reportOutputPath)
        {
            Console.WriteLine("\n=== التقييم الدفعي (Batch Testing) على بيانات اختبار منفصلة ===\n");

            IDataView testData = mlContext.Data.LoadFromTextFile<PriorityInput>(
                testDataPath, hasHeader: true, separatorChar: '\t');

            IDataView predictions = model.Transform(testData);

            // ===== 1. المقاييس الإجمالية (نفس مقاييس التدريب، لكن على بيانات لم يرها النموذج إطلاقاً) =====
            var metrics = mlContext.BinaryClassification.Evaluate(predictions, labelColumnName: nameof(PriorityInput.IsUrgent));

            Console.WriteLine($"Accuracy:  {metrics.Accuracy:P2}");
            Console.WriteLine($"AUC:       {metrics.AreaUnderRocCurve:P2}");
            Console.WriteLine($"F1 Score:  {metrics.F1Score:P2}");
            Console.WriteLine($"Precision: {metrics.PositivePrecision:P2}");
            Console.WriteLine($"Recall:    {metrics.PositiveRecall:P2}");

            // ===== 2. تفصيل كل حالة على حدة — لازم نمرّ سطر سطر عشان نعرف بالظبط مين غلط =====
            var testEnumerable = mlContext.Data.CreateEnumerable<PriorityInput>(testData, reuseRowObject: false).ToList();
            var predictionEnumerable = mlContext.Data.CreateEnumerable<PriorityPrediction>(predictions, reuseRowObject: false).ToList();

            var misclassified = new List<string>
            {
                "Symptoms\tExpected\tPredicted\tProbability"
            };

            int correctCount = 0;

            for (int i = 0; i < testEnumerable.Count; i++)
            {
                var expected = testEnumerable[i].IsUrgent;
                var predicted = predictionEnumerable[i].IsUrgent;
                var probability = predictionEnumerable[i].Probability;

                if (expected == predicted)
                {
                    correctCount++;
                }
                else
                {
                    misclassified.Add(
                        $"{testEnumerable[i].Symptoms}\t{(expected ? "Urgent" : "Normal")}\t{(predicted ? "Urgent" : "Normal")}\t{probability:P1}");
                }
            }

            Console.WriteLine($"\nإجمالي حالات الاختبار: {testEnumerable.Count}");
            Console.WriteLine($"تصنيف صحيح: {correctCount}");
            Console.WriteLine($"تصنيف خاطئ: {testEnumerable.Count - correctCount}");

            // ===== 3. حفظ تقرير الحالات المصنَّفة خطأ =====
            File.WriteAllLines(reportOutputPath, misclassified, System.Text.Encoding.UTF8);
            Console.WriteLine($"\n📄 تقرير الحالات الخاطئة محفوظ في: {reportOutputPath}");

            if (misclassified.Count == 1)
            {
                Console.WriteLine("(لا توجد حالات خاطئة — كل حالات الاختبار صُنّفت بشكل صحيح)");
            }
        }
    }
}
