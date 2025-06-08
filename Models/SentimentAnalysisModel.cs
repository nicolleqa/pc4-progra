using Microsoft.ML;
using Microsoft.ML.Data;

public class SentimentAnalysisModel
{
    private readonly MLContext _mlContext;
    private readonly ITransformer _model;

    public SentimentAnalysisModel()
    {
        _mlContext = new MLContext();

        var trainingData = _mlContext.Data.LoadFromTextFile<SentimentData>(
            "Data/sentiment-data.csv", hasHeader: true, separatorChar: ',');

        var pipeline = _mlContext.Transforms.Text.FeaturizeText("Features", nameof(SentimentData.Text))
            .Append(_mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: "Label", featureColumnName: "Features"));

        _model = pipeline.Fit(trainingData);
    }

    public SentimentPrediction Predict(string text)
    {
        var predictionEngine = _mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(_model);
        return predictionEngine.Predict(new SentimentData { Text = text });
    }
}

public class SentimentData
{
  [LoadColumn(0)] public bool Label { get; set; }
  [LoadColumn(1)] public string Text { get; set; }
}

public class SentimentPrediction : SentimentData
{
    [ColumnName("PredictedLabel")] public bool Prediction { get; set; }
    public float Score { get; set; }
}