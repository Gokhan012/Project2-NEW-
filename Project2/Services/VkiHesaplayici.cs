using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Javax.Security.Auth;
using Project2.Services;

namespace Project2.Services
{
    internal class VkiHesaplayici 
    {
        
        public static double hesapla(double kg,double metre)
        {
            if (kg == 0.0 || kg<45 || kg>200 || metre==0 || metre<1.4 || metre>2.2) { return 0; }
            double vkisomuc = 0.0;
            vkisomuc = kg / (metre * metre);
            return vkisomuc;
        }

        public static String aralikGetir(double vkiSonuc)
        {

            if (vkiSonuc == 0)
            {
                return "Kilo veya Boy verisi hatalı yada eksik";
            }

            if (vkiSonuc < 18.5)
            {
                return "Zayıf";
            }
            else if (vkiSonuc < 25)
            {
                return "Normal Kilolu";
            }
            else if (vkiSonuc < 30)
            {
                return "Fazla Kilolu";
            }
            else if (vkiSonuc < 35)
            {
                return "1. Derece Obez";
            }
            else if (vkiSonuc < 40)
            {
                return "2. Derece Obez";
            }
            else
            {
                return "3. Derece Obez";
            }
        }
    }
}
