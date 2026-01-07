using SQLite;

namespace Project2.Models
{
    public class tblBudget
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Title { get; set; }      // Açıklama veya Kurum Adı
        public decimal Amount { get; set; }    // Tutar
        public DateTime Date { get; set; }     // Tarih
        public string Category { get; set; }   // Market, Kira vb.
        public bool IsIncome { get; set; }     // Gelir mi Gider mi?
        public bool IsRecurring { get; set; }  // Her ay tekrarlansın mı?
        public int UserId { get; set; }        // Kullanıcı ID

        [Ignore]
        public Color TransactionColor => IsIncome
            ? Color.FromArgb("#00FF85")  // Gelirse Yeşil
            : Color.FromArgb("#FF5252"); // Giderse Kırmızı
    }
}