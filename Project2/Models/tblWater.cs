using SQLite; // Bu namespace için sqlite-net-pcl paketi yüklü olmalı
using System;

namespace Project2.Models;

// SQL'deki tablo adını korumak için [Table] niteliğini kullanıyoruz
[Table("tblWater")]
public class tblWater
{
    // SQL: PersonID INTEGER PRIMARY KEY
    // 1-1 ilişki olduğu için AutoIncrement OLMAMALI. 
    // Çünkü ID'yi tblPerson'dan alıp buraya aynen yazacağız.
    [PrimaryKey]
    public int PersonID { get; set; }

    // SQL: WaterDrinkTime TEXT
    // SQLite ORM'i DateTime'ı otomatik olarak veritabanında saklayabilir.
    //public DateTime WaterDrinkTime { get; set; }

    // SQL: WaterDrink REAL
    public double WaterDrink { get; set; }

    // SQL: WaterNeeded REAL
    public double WaterNeeded { get; set; }

    public DateTime Date { get; set; }
}