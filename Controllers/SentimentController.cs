using Microsoft.AspNetCore.Mvc;

public class SentimentController : Controller
{
    private readonly SentimentAnalysisModel _sentimentModel;

    public SentimentController()
    {
        _sentimentModel = new SentimentAnalysisModel();
    }

    [HttpGet]
    public IActionResult Analyze()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Analyze(string userOpinion)
    {
        var result = _sentimentModel.Predict(userOpinion);
        return View("SentimentResult", result);
    }
}