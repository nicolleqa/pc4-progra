using Microsoft.AspNetCore.Mvc;

public class RecommendationController : Controller
{
    private readonly ProductRecommendationModel _recommendationModel;

    public RecommendationController()
    {
        _recommendationModel = new ProductRecommendationModel();
    }

    [HttpGet]
    public IActionResult Recommend()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Recommend(string userId)
    {
        var recommendations = _recommendationModel.Recommend(userId);
        return View("RecommendationResult", recommendations);
    }
}