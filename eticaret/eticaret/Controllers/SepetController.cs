using eticaret.Data;
using eticaret.Models;
using Microsoft.AspNetCore.Mvc;

public class SepetController : Controller
{
    private readonly AppDbContext _context;
    public SepetController(AppDbContext context)
    {
        _context = context;
    }

    // Sepeti göster
    public IActionResult Index()
    {
        // Sepeti oku: List<Tuple<int, int>> (UrunNo, Adet)
        var sepet = HttpContext.Session.GetObject<List<Tuple<int, int>>>("cart") ?? new List<Tuple<int, int>>();
        // Ürün detaylarını çek
        var urunNoList = sepet.Select(x => x.Item1).ToList();
        var urunler = _context.Urunler.Where(u => urunNoList.Contains(u.UrunNo)).ToList();

        var sepetGorunum = sepet.Select(satir => new
        {
            Urun = urunler.FirstOrDefault(u => u.UrunNo == satir.Item1),
            Adet = satir.Item2
        }).ToList();

        return View(sepetGorunum);
    }

    public IActionResult AddToCart(int id)
    {
        var sepet = HttpContext.Session.GetObject<List<Tuple<int, int>>>("cart") ?? new List<Tuple<int, int>>();
        var satir = sepet.FirstOrDefault(x => x.Item1 == id);
        if (satir != null)
        {
            sepet.Remove(satir);
            sepet.Add(new Tuple<int, int>(id, satir.Item2 + 1));
        }
        else
        {
            sepet.Add(new Tuple<int, int>(id, 1));
        }
        HttpContext.Session.SetObject("cart", sepet);

        // Geri geldiği sayfaya yönlendir
        var referer = Request.Headers["Referer"].ToString();
        if (!string.IsNullOrEmpty(referer))
            return Redirect(referer);

        // Yine de bir sıkıntı olursa ana sayfaya dön
        return RedirectToAction("Index", "Home");
    }

    // Sepetten çıkar
    public IActionResult RemoveFromCart(int id)
    {
        var sepet = HttpContext.Session.GetObject<List<Tuple<int, int>>>("cart") ?? new List<Tuple<int, int>>();
        var satir = sepet.FirstOrDefault(x => x.Item1 == id);
        if (satir != null)
        {
            sepet.Remove(satir);
            HttpContext.Session.SetObject("cart", sepet);
        }
        return RedirectToAction("Index");
    }
    [HttpPost]
    public IActionResult SiparisVer()
    {
        var sepet = HttpContext.Session.GetObject<List<Tuple<int, int>>>("cart") ?? new List<Tuple<int, int>>();
        if (sepet.Count == 0)
        {
            TempData["SepetBos"] = true;
            return RedirectToAction("Index");
        }

        var kullaniciEmail = HttpContext.Session.GetString("KullaniciEmail");
        var musteri = _context.Musteriler.FirstOrDefault(m => m.Email == kullaniciEmail);

        if (musteri == null)
            return RedirectToAction("Login", "Account");

        var siparis = new Siparis
        {
            MusteriNo = musteri.MusteriNo,
            Tarih = DateTime.Now,
            Durum = "Sipariş oluşturuldu",
            ToplamTutar = 0
        };
        _context.Siparisler.Add(siparis);
        _context.SaveChanges();

        decimal toplam = 0;
        foreach (var s in sepet)
        {
            var urun = _context.Urunler.FirstOrDefault(u => u.UrunNo == s.Item1);
            if (urun != null)
            {
                var detay = new SiparisDetay
                {
                    SiparisNo = siparis.SiparisNo,
                    UrunNo = urun.UrunNo,
                    Adet = s.Item2,
                    BirimFiyat = urun.Fiyat,
                    Beden = "-",                // Gerekirse sepetten al
                    TeslimDurumu = "Hazırlanıyor",
                    TeslimTarihi = null,
                    TeslimAlan = null
                };
                toplam += urun.Fiyat * s.Item2;
                _context.SiparisDetaylari.Add(detay);
            }
        }
        siparis.ToplamTutar = toplam;
        _context.SaveChanges();

        HttpContext.Session.Remove("cart");
        TempData["SiparisBasarili"] = true;
        return RedirectToAction("Index");
    }


}
