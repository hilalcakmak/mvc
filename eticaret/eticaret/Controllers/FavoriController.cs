using eticaret.Data;
using Microsoft.AspNetCore.Mvc;

public class FavoriController : Controller
{
    private readonly AppDbContext _context;
    public FavoriController(AppDbContext context)
    {
        _context = context;
    }

    // Favori ekle/çıkar (toggle)
    public IActionResult Toggle(int id)
    {
        var kullaniciEmail = HttpContext.Session.GetString("KullaniciEmail");
        if (string.IsNullOrEmpty(kullaniciEmail))
            return RedirectToAction("Index", "Account");
        var favoriler = HttpContext.Session.GetObject<List<int>>("favoriler") ?? new List<int>();
        if (favoriler.Contains(id))
            favoriler.Remove(id); // varsa çıkar
        else
            favoriler.Add(id);    // yoksa ekle

        HttpContext.Session.SetObject("favoriler", favoriler);

        var referer = Request.Headers["Referer"].ToString();
        if (!string.IsNullOrEmpty(referer))
            return Redirect(referer);
        return RedirectToAction("Index", "Home");
    }

    // Favorilerim
    public IActionResult Index()
    {
        var favoriler = HttpContext.Session.GetObject<List<int>>("favoriler") ?? new List<int>();
        var urunler = _context.Urunler.Where(u => favoriler.Contains(u.UrunNo)).ToList();
        return View(urunler);
    }
}
