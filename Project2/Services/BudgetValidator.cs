using Project2.Models;
using System;

namespace Project2.Services
{
    public static class BudgetValidator
    {
        public static void Validate(tblBudget b)
        {
            // Bütçe limiti negatif olamaz
            if (b.BudgetLimit < 0)
            {
                throw new ArgumentException("Bütçe limiti negatif olamaz.");
            }

            // PersonID kontrolü
            if (b.PersonID <= 0)
            {
                throw new ArgumentException("Bütçe bir kişiye atanmalıdır.");
            }
        }
    }
}
