using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project2.Services
{
    public static class UserSeassion
    {
        public static Project2.Models.Person? CurrentUser { get; set; }

        public static Project2.Models.tblWater? CurrentlWater { get; set; }

        public static Project2.Models.tblMedicine? CurrentMedicine { get; set; }

        public static Project2.Models.tblBudget? CurrentBudget { get; set; }

        public static Project2.Models.tblBill? CurrentBill { get; set; }
    }
}
