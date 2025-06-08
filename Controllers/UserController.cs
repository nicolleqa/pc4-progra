using Microsoft.AspNetCore.Mvc;
using pc4_progra.Data;

public class UserController : Controller
{
    private readonly ApplicationDbContext _context;

    public UserController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(User user)
    {
        if (ModelState.IsValid)
        {
            
            var lastUser = _context.Users.OrderByDescending(u => u.UserId).FirstOrDefault();
            int nextId = lastUser == null ? 1 : int.Parse(lastUser.UserId.Substring(1)) + 1;
            user.UserId = $"U{nextId}";

            _context.Users.Add(user);
            _context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
        return View(user);
    }
}