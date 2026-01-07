using Project2.Models;
using System;

namespace Project2.Services
{
    public static class BudgetValidator
    {
        // Parametre olarak tblBill alıyoruz
        public static void Validate(tblBill b)
        {
            // 1. Tutar Kontrolü
            if (b.Amount < 0)
            {
                throw new ArgumentException("Tutar negatif olamaz.");
            }

            // 2. KULLANICI KONTROLÜ (DÜZELTİLEN KISIM)
            // b.Id veritabanına henüz kaydedilmemiş yeni kayıtlarda 0'dır.
            // Bu yüzden b.Id'yi kontrol edersen yeni kayıt ekleyemezsin.
            // Bunun yerine faturanın kime ait olduğunu (UserId) kontrol etmeliyiz.

            if (b.UserId <= 0) // <-- Burası 'Id' değil 'UserId' olmalı
            {
                throw new ArgumentException("Kayıt bir kullanıcıya atanmalıdır (UserId eksik).");
            }

            // 3. Başlık Kontrolü
            if (string.IsNullOrWhiteSpace(b.Title))
            {
                throw new ArgumentException("Açıklama veya başlık boş olamaz.");
            }
        }
    }
}