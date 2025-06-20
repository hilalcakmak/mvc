using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eticaret.Models
{
    public class Siparis
    {
        [Key]
        public int SiparisNo { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Sipariş Tarihi")]
        public DateTime Tarih { get; set; }

        [ForeignKey("Musteri")]
        public int MusteriNo { get; set; }

        public virtual Musteri Musteri { get; set; }

        [Required]
        [Display(Name = "Sipariş Durumu")]
        public string Durum { get; set; } // örnek: "Sipariş tamamlandı"

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Toplam Tutar")]
        [DataType(DataType.Currency)]
        public decimal ToplamTutar { get; set; }

        [Display(Name = "Ürün Görseli")]
        public string? UrunResimUrl { get; set; } // görsel için opsiyonel URL alanı
    }
}
