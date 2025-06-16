using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Application.ValidationAttributes
{
    /// <summary>
    /// Sana kelajakdagi sana ekanligini tekshirish uchun validatsiya atributi
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class FutureDateAttribute : ValidationAttribute
    {
        /// <summary>
        /// Validatsiya metodini override qilish
        /// </summary>
        /// <param name="value">Tekshirilayotgan qiymat</param>
        /// <returns>true - agar sana kelajakdagi sana bo'lsa yoki null bo'lsa</returns>
        public override bool IsValid(object? value) // Updated parameter to be nullable
        {
            // Agar qiymat null bo'lsa, valid deb hisoblaymiz (majburiy emas)
            if (value == null)
                return true;

            // Agar qiymat DateTime turida bo'lmasa, noto'g'ri deb hisoblaymiz
            if (!(value is DateTime))
                return false;

            // Qiymatni DateTime ga convert qilamiz
            DateTime date = (DateTime)value;

            // Sana hozirgi vaqtdan keyin bo'lishini tekshiramiz
            return date > DateTime.UtcNow;
        }

        /// <summary>
        /// Formatlash xato xabarini override qilish
        /// </summary>
        /// <param name="name">Maydon nomi</param>
        /// <returns>Formatlangan xato xabari</returns>
        public override string FormatErrorMessage(string name)
        {
            return $"{name} maydoni uchun kelajakdagi sana ko'rsatilishi shart";
        }
    }
}
