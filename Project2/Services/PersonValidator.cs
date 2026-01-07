using System.Text.RegularExpressions;
using Project2.Models;

namespace Project2.Services
{
    public static class PersonValidator
    {
        public static void Validate(Person p)
        {
            // Name / Surname trim > 0
            if (string.IsNullOrWhiteSpace(p.Name))
                throw new ArgumentException("İsim boş olamaz.");
            if (string.IsNullOrWhiteSpace(p.Surname))
                throw new ArgumentException("Soyad boş olamaz.");

            // Age 0..100
            if (p.Age < 0 || p.Age > 100)
                throw new ArgumentException("Age 0 ile 100 arasında olmalı.");

            // Email basic check like '%@%.%'
            if (string.IsNullOrWhiteSpace(p.Email) || !p.Email.Contains("@") || !p.Email.Contains("."))
                throw new ArgumentException("Email formatı geçersiz.");

            // Password: length 5..15, contains uppercase and digit
            if (p.Password is null || p.Password.Length < 5 || p.Password.Length > 15)
                throw new ArgumentException("Password 5-15 karakter olmalı.");

            if (!Regex.IsMatch(p.Password, "[A-Z]"))
                throw new ArgumentException("Password en az 1 büyük harf içermeli.");

            if (!Regex.IsMatch(p.Password, "[0-9]"))
                throw new ArgumentException("Password en az 1 rakam içermeli.");

            // Height optional 1.40..2.20
            if (p.Height.HasValue && (p.Height.Value < 1.40 || p.Height.Value > 2.20))
                throw new ArgumentException("Height 1.40 ile 2.20 arasında olmalı.");

            // Weight optional 45..200
            if (p.Weight.HasValue && (p.Weight.Value < 45.0 || p.Weight.Value > 200.0))
                throw new ArgumentException("Weight 45 ile 200 arasında olmalı.");

            // Gender in (0,1,2)
            if (p.Gender is not (0 or 1 or 2))
                throw new ArgumentException("Gender sadece 0,1,2 olabilir.");
        }
    }
}
