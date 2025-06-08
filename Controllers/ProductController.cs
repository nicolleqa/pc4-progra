using Microsoft.AspNetCore.Mvc;
using pc4_progra.Data;

public class ProductController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProductController(ApplicationDbContext context)
    {
      _context = context;
    }

    [HttpGet]
    public IActionResult Create()
    {
      return View();
    }

    [HttpPost]
    public IActionResult Create(Product product)
    {
        if (ModelState.IsValid)
        {
            var lastProduct = _context.Products.OrderByDescending(p => p.ProductId).FirstOrDefault();
            int nextId = lastProduct == null ? 1 : int.Parse(lastProduct.ProductId.Substring(1)) + 1;
            product.ProductId = $"P{nextId}";

            _context.Products.Add(product);
            _context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
        return View(product);
    }
}