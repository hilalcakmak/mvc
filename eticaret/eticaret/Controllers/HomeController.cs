using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using eticaret.Models;
using eticaret.Data; // <-- Bunu ekle
using Microsoft.EntityFrameworkCore;

namespace eticaret.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AppDbContext _context; // <-- Bunu ekle

    public HomeController(ILogger<HomeController> logger, AppDbContext context) // <-- DbContext'i parametre olarak al
    {
        _logger = logger;
        _context = context; // <-- DbContext'i ata
    }

    public IActionResult Index(string searchString, int? kategoriNo)
    {
        var urunQuery = _context.Urunler
            .Include(u => u.Kategori)
            .AsQueryable();

        if (!string.IsNullOrEmpty(searchString))
        {
            urunQuery = urunQuery.Where(u =>
                u.UrunAdi.ToLower().Contains(searchString.ToLower()) ||
                (u.Satici != null && u.Satici.ToLower().Contains(searchString.ToLower())) ||
                (u.Kategori != null && u.Kategori.KategoriAdi.ToLower().Contains(searchString.ToLower()))
            );
        }

        if (kategoriNo.HasValue && kategoriNo.Value > 0)
        {
            urunQuery = urunQuery.Where(u => u.KategoriNo == kategoriNo.Value);
        }

        ViewBag.SearchString = searchString;
        ViewBag.SelectedKategori = kategoriNo;

        return View(urunQuery.ToList());
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
