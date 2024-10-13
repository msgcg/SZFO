using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SZFO
{
    public class Book
    {
        public string Code { get; set; }  // Код книги
        public string Name { get; set; }  // Название книги
        public string Category { get; set; }  // Категория книги
        public string FullDescription { get; set; }  // Полное описание книги

        // Пустой конструктор
        public Book()
        {
        }

        // Конструктор с параметрами (если нужен)
        public Book(string code, string name, string category, string fullDescription)
        {
            Code = code;
            Name = name;
            Category = category;
            FullDescription = fullDescription;
        }
    }
}
