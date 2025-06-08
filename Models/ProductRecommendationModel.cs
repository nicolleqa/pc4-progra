using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using System.Collections.Generic;
using System.Linq;
using pc4_progra.Data;

public class ProductRecommendationModel
{
    private readonly MLContext _mlContext;
    private readonly ITransformer _model;
    private readonly Dictionary<string, string> _productDictionary;

    public ProductRecommendationModel(ApplicationDbContext dbContext)
    {
        _mlContext = new MLContext();

        // Cargar productos desde la base de datos
        _productDictionary = dbContext.Products
            .ToDictionary(p => p.ProductId, p => p.Name);

        var trainingData = _mlContext.Data.LoadFromTextFile<ProductInteraction>(
            "Data/ratings-data.csv", hasHeader: true, separatorChar: ',');

        var dataProcessingPipeline = _mlContext.Transforms.Conversion
            .MapValueToKey(outputColumnName: "UserIdEncoded", inputColumnName: nameof(ProductInteraction.UserId))
            .Append(_mlContext.Transforms.Conversion
                .MapValueToKey(outputColumnName: "ProductIdEncoded", inputColumnName: nameof(ProductInteraction.ProductId)));

        var options = new MatrixFactorizationTrainer.Options
        {
            MatrixColumnIndexColumnName = "UserIdEncoded",
            MatrixRowIndexColumnName = "ProductIdEncoded",
            LabelColumnName = nameof(ProductInteraction.Label),
            NumberOfIterations = 20,
            ApproximationRank = 50
        };

        var trainingPipeline = dataProcessingPipeline.Append(_mlContext.Recommendation().Trainers.MatrixFactorization(options));

        _model = trainingPipeline.Fit(trainingData);
    }

    public IEnumerable<ProductScore> Recommend(string userId, int topN = 5)
    {
        var predictionEngine = _mlContext.Model.CreatePredictionEngine<ProductInteraction, ProductScore>(_model);

        var recommendations = new List<ProductScore>();
        for (int productId = 1; productId <= 100; productId++)
        {
            var productKey = $"P{productId}";
            if (_productDictionary.ContainsKey(productKey))
            {
                var prediction = predictionEngine.Predict(new ProductInteraction { UserId = userId, ProductId = productKey });
                recommendations.Add(new ProductScore
                {
                    ProductId = productKey,
                    ProductName = _productDictionary[productKey], // Obtener el nombre del producto desde la bd
                    Score = prediction.Score
                });
            }
        }

        return recommendations.OrderByDescending(r => r.Score).Take(topN);
    }
}

public class ProductInteraction
{
    [LoadColumn(0)] public string UserId { get; set; }
    [LoadColumn(1)] public string ProductId { get; set; }
    [LoadColumn(2)] public float Label { get; set; }
}

public class ProductScore
{
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public float Score { get; set; }
}