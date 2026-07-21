using System;
using System.Reflection;
using System.Xml.Serialization;

namespace CourseProject_OOPiP
{
    public interface ISearchable
    {
        bool ContainsText(string text);
    }

    public interface IValidatable
    {
        string ValidateField(string propName, string value);
    }

    [XmlInclude(typeof(Book))]
    [XmlInclude(typeof(Magazine))]
    public abstract class Publication : ISearchable, IValidatable
    {
        public string Title { get; set; }
        public int? Year { get; set; }
        public double? Price { get; set; }
        public string Author { get; set; }
        public string Isbn { get; set; }
        public int? IssueNumber { get; set; }
        public string Periodicity { get; set; }

        public bool ContainsText(string text)
        {
            if (string.IsNullOrEmpty(text)) return true;

            var props = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in props)
            {
                if (this is Book && (prop.Name == "IssueNumber" || prop.Name == "Periodicity")) continue;
                if (this is Magazine && (prop.Name == "Author" || prop.Name == "Isbn")) continue;

                var val = prop.GetValue(this, null);
                if (val == null) continue;

                string strVal = val.ToString();
                if (strVal.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
            }

            return false;
        }

        public abstract string ValidateField(string propName, string value);
    }

    public class Book : Publication
    {
        public override string ValidateField(string propName, string value)
        {
            string cleanValue = value?.Trim() ?? string.Empty;

            if (propName == "Title" && string.IsNullOrEmpty(cleanValue))
            {
                return "Название книги не может быть пустым.";
            }

            if (propName == "Year")
            {
                if (string.IsNullOrEmpty(cleanValue)) return "Год издания книги обязателен для заполнения.";
                if (!int.TryParse(cleanValue, out int year)) return "Год должен быть целым числом.";
                if (year < 1450 || year > DateTime.Now.Year + 1) return "Недопустимый год издания книги.";
            }

            if (propName == "Price")
            {
                if (string.IsNullOrEmpty(cleanValue)) return "Цена книги обязательна для заполнения.";
                string normValue = cleanValue.Replace('.', ',');
                if (!double.TryParse(normValue, out double price)) return "Цена должна быть числом.";
                if (price < 0.0 || price > 1000000.0) return "Цена книги не может быть отрицательной или слишком большой.";
            }

            if (propName == "Author" && string.IsNullOrEmpty(cleanValue))
            {
                return "Поле 'Автор' обязательно для заполнения книги.";
            }

            if (propName == "Isbn" && string.IsNullOrEmpty(cleanValue))
            {
                return "Поле 'ISBN код' обязательно для заполнения книги.";
            }

            return null;
        }
    }

    public class Magazine : Publication
    {
        public override string ValidateField(string propName, string value)
        {
            string cleanValue = value?.Trim() ?? string.Empty;

            if (propName == "Title" && string.IsNullOrEmpty(cleanValue))
            {
                return "Название журнала не может быть пустым.";
            }

            if (propName == "Year")
            {
                if (string.IsNullOrEmpty(cleanValue)) return "Год издания журнала обязателен для заполнения.";
                if (!int.TryParse(cleanValue, out int year)) return "Год должен быть целым числом.";
                if (year < 1800 || year > DateTime.Now.Year + 1) return "Недопустимый год издания журнала.";
            }

            if (propName == "Price")
            {
                if (string.IsNullOrEmpty(cleanValue)) return "Цена журнала обязательна для заполнения.";
                string normValue = cleanValue.Replace('.', ',');
                if (!double.TryParse(normValue, out double price)) return "Цена должна быть числом.";
                if (price < 0.0 || price > 50000.0) return "Цена журнала выходит за разумные пределы.";
            }

            if (propName == "IssueNumber")
            {
                if (string.IsNullOrEmpty(cleanValue)) return "Номер выпуска обязателен для заполнения.";
                if (!int.TryParse(cleanValue, out int num)) return "Номер выпуска должен быть целым числом.";
                if (num < 1) return "Номер выпуска журнала должен быть больше нуля.";
            }

            if (propName == "Periodicity" && string.IsNullOrEmpty(cleanValue))
            {
                return "Поле 'Периодичность' обязательно для заполнения журнала.";
            }

            return null;
        }
    }
}
