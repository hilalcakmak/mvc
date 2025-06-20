using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using eticaret.Data;
using eticaret.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace eticaret.Controllers
{
    public class UrunController : Controller
    {
        private readonly AppDbContext _context;

        public UrunController(AppDbContext context)
        {
            _context = context;
        }

        // Listeleme ve arama
        public async Task<IActionResult> Index(string searchString, int? kategoriNo)
        {
            var kategoriList = await _context.Kategoriler
                .OrderBy(k => k.KategoriAdi)
                .Select(k => new SelectListItem
                {
                    Value = k.KategoriNo.ToString(),
                    Text = k.KategoriAdi
                })
                .ToListAsync();

            ViewBag.KategoriList = kategoriList;
            ViewBag.SelectedKategori = kategoriNo;
            ViewBag.SearchString = searchString;

            var urunQuery = _context.Urunler
                .Include(u => u.Kategori)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                urunQuery = urunQuery.Where(u =>
                    u.UrunAdi.ToLower().StartsWith(searchString.ToLower()) ||
                    (u.Satici != null && u.Satici.ToLower().StartsWith(searchString.ToLower())));
            }


            if (kategoriNo.HasValue && kategoriNo.Value > 0)
            {
                urunQuery = urunQuery.Where(u => u.KategoriNo == kategoriNo.Value);
            }

            urunQuery = urunQuery.OrderBy(u => u.UrunAdi);

            return View(await urunQuery.ToListAsync());
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.KategoriList = _context.Kategoriler
                .OrderBy(k => k.KategoriAdi)
                .Select(k => new SelectListItem
                {
                    Value = k.KategoriNo.ToString(),
                    Text = k.KategoriAdi
                }).ToList();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Urun urun, IFormFile imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var extension = Path.GetExtension(imageFile.FileName);
                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                urun.ResimUrl = $"img/{fileName}";
            }

            _context.Add(urun);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
