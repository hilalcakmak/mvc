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

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var urun = _context.Urunler.Find(id);
            if (urun == null)
                return NotFound();

            ViewBag.KategoriList = _context.Kategoriler
                .OrderBy(k => k.KategoriAdi)
                .Select(k => new SelectListItem
                {
                    Value = k.KategoriNo.ToString(),
                    Text = k.KategoriAdi
                }).ToList();

            return View(urun);
        }

        [HttpPost]
        public IActionResult Edit(int id, Urun urun, IFormFile imageFile, bool ResmiKaldir = false)
        {
            var mevcutUrun = _context.Urunler.Find(id);
            if (mevcutUrun == null)
                return NotFound();

            // Diğer alanları güncelle
            mevcutUrun.UrunAdi = urun.UrunAdi;
            mevcutUrun.Fiyat = urun.Fiyat;
            mevcutUrun.KategoriNo = urun.KategoriNo;
            mevcutUrun.Satici = urun.Satici;
            mevcutUrun.Renk = urun.Renk;
            mevcutUrun.BedenSecenekleri = urun.BedenSecenekleri;

            // Mevcut resmi kaldırma seçiliyse...
            if (ResmiKaldir && !string.IsNullOrEmpty(mevcutUrun.ResimUrl))
            {
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", mevcutUrun.ResimUrl.Replace('/', Path.DirectorySeparatorChar));
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath); // Sunucudan dosyayı da sil
                }
                mevcutUrun.ResimUrl = null;
            }

            // Yeni resim yüklenmişse, dosyayı kaydet
            if (imageFile != null && imageFile.Length > 0)
            {
                var extension = Path.GetExtension(imageFile.FileName);
                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.CopyTo(stream);
                }
                mevcutUrun.ResimUrl = $"img/{fileName}";
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var delete = _context.Urunler.Find(id);
            _context.Remove(delete);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
