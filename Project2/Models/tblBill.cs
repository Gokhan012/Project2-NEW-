using SQLite;

namespace Project2.Models
{
    public class tblBill
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Title { get; set; }       // Fatura Adı (Elektrik, Netflix vb.)
        public double Amount { get; set; }      // Tutar
        public DateTime DueDate { get; set; }   // Son Ödeme Tarihi
        public string Category { get; set; }    // Kategori (Fatura, Kira vb.)
        public bool IsPaid { get; set; }        // Ödendi mi? (Varsayılan Hayır)
        public int UserId { get; set; }         // Hangi kullanıcıya ait?
    }
}