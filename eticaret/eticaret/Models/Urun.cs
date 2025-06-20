using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using eticaret.Models;

namespace eticaret.Models
{
    public class Urun
    {
        [Key]
        public int UrunNo { get; set; }

        [Required]
        [StringLength(100)]
        public string UrunAdi { get; set; }

        [Required]
        public decimal Fiyat { get; set; }

        [StringLength(20)]
        public string? Renk { get; set; }

        [StringLength(200)]
        public string? ResimUrl { get; set; }

        [StringLength(100)]
        public string? Satici { get; set; }

        [ForeignKey("Kategori")]
        public int KategoriNo { get; set; }

        public virtual Kategori Kategori { get; set; }

        // Ürün detayında gösterilecek beden seçenekleri, örnek: "36,37,38,39"
        [StringLength(100)]
        public string? BedenSecenekleri { get; set; }
    }
}
