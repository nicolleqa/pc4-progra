using Microsoft.AspNetCore.Mvc;
using pc4_progra.Data;

public class RecommendationController : Controller
{
    private readonly ApplicationDbContext _dbContext;

    public RecommendationController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public IActionResult Recommend()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Recommend(string userId)
    {
        var recommendationModel = new ProductRecommendationModel(_dbContext);
        var recommendations = recommendationModel.Recommend(userId);
        return View("RecommendationResult", recommendations);
    }
}