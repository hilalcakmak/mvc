using Microsoft.AspNetCore.Mvc;
using eticaret.Data;
using eticaret.Models;
using System.Linq;

namespace eticaret.Controllers
{
    public class KategoriController : Controller
    {
        private readonly AppDbContext _context;
        public KategoriController(AppDbContext context)
        {
            _context = context;
        }

        // Listele
        public IActionResult Index()
        {
            var kategoriler = _context.Kategoriler.ToList();
            return View(kategoriler);
        }

        // Ekle (GET)
        public IActionResult Create()
        {
            return View();
        }

        // Ekle (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Kategori kategori)
        {
            if (ModelState.IsValid)
            {
                _context.Kategoriler.Add(kategori);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(kategori);
        }

        // Güncelle (GET)
        public IActionResult Edit(int id)
        {
            var kategori = _context.Kategoriler.Find(id);
            if (kategori == null) return NotFound();
            return View(kategori);
        }

        // Güncelle (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Kategori kategori)
        {
            if (id != kategori.KategoriNo) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(kategori);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(kategori);
        }

        // Sil (GET)
        public IActionResult Delete(int id)
        {
            var kategori = _context.Kategoriler.Find(id);
            if (kategori == null) return NotFound();
            return View(kategori);
        }

        // Sil (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var kategori = _context.Kategoriler.Find(id);
            if (kategori == null) return NotFound();

            _context.Kategoriler.Remove(kategori);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}


