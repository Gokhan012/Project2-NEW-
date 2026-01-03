// Her ilacın kendi ID'si var (AutoIncrement), PersonID ile bağlantılı
using SQLite;

namespace Project2.Models;

[Table("tblMedicine")]
public class tblMedicine
{
    [PrimaryKey, AutoIncrement]
    public int MedicineID { get; set; }

    [Indexed] // Aramalarda hızlanması için indexlendi
    public int PersonID { get; set; }

    public string MedicineName { get; set; }
    public double MedicineDose { get; set; }
    public DateTime MedicineTime { get; set; }
    public DateTime LastTakenDate { get; set; }
}
