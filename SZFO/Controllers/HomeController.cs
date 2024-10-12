using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace SZFO.Controllers
{
    public class HomeController : Controller
    {
        // Статический словарь категорий
        private static readonly Dictionary<string, string> Okpd2Sections = new Dictionary<string, string>
        {
            { "A", "Продукция сельского, лесного и рыбного хозяйства" },
            { "B", "Продукция горнодобывающих производств" },
            { "C", "Продукция обрабатывающих производств" },
            { "D", "Электроэнергия, газ, пар и кондиционирование воздуха" },
            { "E", "Водоснабжение; водоотведение, услуги по удалению и рекультивации отходов" },
            { "F", "Сооружения и строительные работы" },
            { "G", "Услуги по оптовой и розничной торговле; услуги по ремонту автотранспортных средств и мотоциклов" },
            { "H", "Услуги транспорта и складского хозяйства" },
            { "I", "Услуги гостиничного хозяйства и общественного питания" },
            { "J", "Услуги в области информации и связи" },
            { "K", "Услуги финансовые и страховые" },
            { "L", "Услуги, связанные с недвижимым имуществом" },
            { "M", "Услуги, связанные с научной, инженерно-технической и профессиональной деятельностью" },
            { "N", "Услуги административные и вспомогательные" },
            { "O", "Услуги в сфере государственного управления и обеспечения военной безопасности; услуги по обязательному социальному обеспечению" },
            { "P", "Услуги в области образования" },
            { "Q", "Услуги в области здравоохранения и социальные услуги" },
            { "R", "Услуги в области искусства, развлечений, отдыха и спорта" },
            { "S", "Прочие услуги" },
            { "T", "Товары и услуги различные, производимые домашними хозяйствами для собственного потребления" },
            { "U", "Услуги, предоставляемые экстерриториальными организациями и органами" }
        };
        // Метод для отображения всех книг (чтение из Excel)
        public ActionResult Index()
        {
            string excelFilePath = Server.MapPath("~/App_Data/Books.xlsx"); // Перемещаем сюда
            var books = ReadBooksFromExcel(excelFilePath);
            return View(books);
        }

        // Метод для отображения формы добавления книги (GET)
        [HttpGet]
        public ActionResult Add()
        {
            // Задание контекста лицензии для EPPlus
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            // Передаем словарь в ViewBag для использования в представлении
            ViewBag.Categories = new SelectList(Okpd2Sections, "Key", "Value");
            return View();
        }

        // Метод для обработки формы добавления книги (POST)
        [HttpPost]
        public ActionResult Add(Book book)
        {
            if (ModelState.IsValid)
            {
                string excelFilePath = Server.MapPath("~/App_Data/Books.xlsx");
                var books = ReadBooksFromExcel(excelFilePath); // Читаем существующие книги из Excel
                books.Add(book); // Добавляем новую книгу
                WriteBooksToExcel(excelFilePath, books); // Записываем обновленный список в Excel
                return RedirectToAction("Index");
            }

            // Если форма не валидна, повторно передаем список категорий
            ViewBag.Categories = new SelectList(Okpd2Sections, "Key", "Value");
            return View(book);
        }

        // Метод для чтения данных из файла Excel
        private List<Book> ReadBooksFromExcel(string filePath)
        {
            var books = new List<Book>();

            if (!System.IO.File.Exists(filePath))
            {
                return books; // Если файл не существует, возвращаем пустой список
            }

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows; // Количество строк

                for (int row = 2; row <= rowCount; row++) // Пропускаем первую строку с заголовками
                {
                    var book = new Book
                    {
                        Code = worksheet.Cells[row, 1].Text,
                        Name = worksheet.Cells[row, 2].Text,
                        Category = worksheet.Cells[row, 3].Text,
                        FullDescription = worksheet.Cells[row, 4].Text
                    };
                    books.Add(book);
                }
            }

            return books;
        }

        // Метод для записи данных в файл Excel
        private void WriteBooksToExcel(string filePath, List<Book> books)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Books");

                // Добавляем заголовки столбцов
                worksheet.Cells[1, 1].Value = "Code";
                worksheet.Cells[1, 2].Value = "Name";
                worksheet.Cells[1, 3].Value = "Category";
                worksheet.Cells[1, 4].Value = "FullDescription";

                // Добавляем данные книг
                for (int i = 0; i < books.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = books[i].Code;
                    worksheet.Cells[i + 2, 2].Value = books[i].Name;
                    worksheet.Cells[i + 2, 3].Value = books[i].Category;
                    worksheet.Cells[i + 2, 4].Value = books[i].FullDescription;
                }

                // Сохраняем файл Excel
                package.SaveAs(new FileInfo(filePath));
            }
        }
    }

    // Модель книги
    public class Book
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string FullDescription { get; set; }
    }
}
