using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Web;
using System.Diagnostics;
using System.Linq;

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
        public ActionResult Catalog(string selectedCategory = null)
        {
            var categories = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("A", "Продукция сельского, лесного и рыбного хозяйства"),
            new KeyValuePair<string, string>("B", "Продукция горнодобывающих производств"),
            new KeyValuePair<string, string>("C", "Продукция обрабатывающих производств"),
            new KeyValuePair<string, string>("D", "Электроэнергия, газ, пар и кондиционирование воздуха"),
            new KeyValuePair<string, string>("E", "Водоснабжение; водоотведение, услуги по удалению и рекультивации отходов"),
            new KeyValuePair<string, string>("F", "Сооружения и строительные работы"),
            new KeyValuePair<string, string>("G", "Услуги по оптовой и розничной торговле; услуги по ремонту автотранспортных средств и мотоциклов"),
            new KeyValuePair<string, string>("H", "Услуги транспорта и складского хозяйства"),
            new KeyValuePair<string, string>("I", "Услуги гостиничного хозяйства и общественного питания"),
            new KeyValuePair<string, string>("J", "Услуги в области информации и связи"),
            new KeyValuePair<string, string>("K", "Услуги финансовые и страховые"),
            new KeyValuePair<string, string>("L", "Услуги, связанные с недвижимым имуществом"),
            new KeyValuePair<string, string>("M", "Услуги, связанные с научной, инженерно-технической и профессиональной деятельностью"),
            new KeyValuePair<string, string>("N", "Услуги административные и вспомогательные"),
            new KeyValuePair<string, string>("O", "Услуги в сфере государственного управления и обеспечения военной безопасности; услуги по обязательному социальному обеспечению"),
            new KeyValuePair<string, string>("P", "Услуги в области образования"),
            new KeyValuePair<string, string>("Q", "Услуги в области здравоохранения и социальные услуги"),
            new KeyValuePair<string, string>("R", "Услуги в области искусства, развлечений, отдыха и спорта"),
            new KeyValuePair<string, string>("S", "Прочие услуги"),
            new KeyValuePair<string, string>("T", "Товары и услуги различные, производимые домашними хозяйствами для собственного потребления"),
            new KeyValuePair<string, string>("U", "Услуги, предоставляемые экстерриториальными организациями и органами")
        };

            var categoriesSet = new HashSet<string>(categories.Select(c => c.Key));

            // Получаем товары из Excel
            string excelFilePath = Server.MapPath("~/App_Data/Books.xlsx");
            var books = ReadBooksFromExcel(excelFilePath);

            // Собираем уникальные категории из товаров
            var productCategories = books.Select(b => b.Category).Distinct().ToList();

            // Проверяем, есть ли категория товара в предустановленных категориях, и добавляем новую если её нет
            foreach (var productCategory in productCategories)
            {
                if (!categoriesSet.Contains(productCategory))
                {
                    categories.Add(new KeyValuePair<string, string>(productCategory, productCategory));
                    categoriesSet.Add(productCategory); // добавляем новую категорию в набор
                }
            }

            ViewBag.Categories = categories;

            // Фильтруем книги по выбранной категории
            if (!string.IsNullOrEmpty(selectedCategory))
            {
                books = books.Where(b => b.Category == selectedCategory).ToList();
            }

            // Генерируем HTML таблицу
            string htmlTable = GenerateHtmlTable(books);

            // Передаем сгенерированную таблицу в представление
            ViewBag.HtmlTable = htmlTable;

            return View();
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
        public async Task<ActionResult> Add(Book book)
        {
            // Если категория не указана, отправляем запрос к API
            if (string.IsNullOrEmpty(book.Category))
            {
                //try
                {
                    var category = await GetCategoryFromApi(book.Name);
                    if (!string.IsNullOrEmpty(category))
                    {
                        System.Diagnostics.Debug.WriteLine("Категория найдена через API: " + Okpd2Sections[category]);
                        book.Category = Okpd2Sections[category];
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("API не вернул категорию.");
                        book.Category = "Не указано";
                    }
                }
                /*catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Ошибка при запросе к API: " + ex.Message);
                    book.Category = "Ошибка при обращении к API";
                }*/

            }

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
        // Метод для обработки загрузки CSV файла
        [HttpPost]
        public async Task<ActionResult> Upload(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                string filePath2 = Path.Combine(Server.MapPath("~/App_Data/"), Path.GetFileName(file.FileName));
                file.SaveAs(filePath2); // Сохраняем загруженный файл на сервере

                var books = ReadBooksFromCsv(filePath2); // Читаем книги из CSV

                foreach (var book in books)
                {
                    // Если категория не указана, отправляем запрос к API
                    if (string.IsNullOrEmpty(book.Category))
                    {
                        var category = await GetCategoryFromApi(book.Name);
                        book.Category = !string.IsNullOrEmpty(category) ? category : "Не указано";
                    }
                }

                // Здесь можно передать книги в представление для дальнейшей обработки
                return View("Add", books); // Переход на представление с данными книг
            }

            return RedirectToAction("Add");
        }

        // Метод для чтения данных из CSV
        private List<Book> ReadBooksFromCsv(string filePath2)
        {
            var books = new List<Book>();

            using (var reader = new StreamReader(filePath2))
            {
                bool isFirstRow = true;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');

                    if (isFirstRow) // Пропускаем первую строку с заголовками
                    {
                        isFirstRow = false;
                        continue;
                    }

                    var book = new Book
                    {
                        Code = values[0],
                        Name = values[1],
                        Category = values.Length > 2 ? values[2] : string.Empty,
                        FullDescription = values.Length > 3 ? values[3] : string.Empty
                    };
                    System.Diagnostics.Debug.WriteLine(book.Code+"-Code");
                    books.Add(book);
                }
            }

            return books;
        }
        // Метод для получения категории из API
        private async Task<string> GetCategoryFromApi(string productName)
        {
            var query = new { query = productName };
            var jsonQuery = JsonConvert.SerializeObject(query);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Token 7bca959b16a39757579b2242d4aa31d9c401ee7c"); // Вставьте ваш API токен
                var response = await client.PostAsync("http://suggestions.dadata.ru/suggestions/api/4_1/rs/suggest/okpd2",
                    new StringContent(jsonQuery, Encoding.UTF8, "application/json"));
                System.Diagnostics.Debug.WriteLine("Попытка запроса к API DaData");

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine(jsonResponse);
                    var suggestions = JsonConvert.DeserializeObject<ApiResponse>(jsonResponse);

                    // Возвращаем первую найденную категорию
                    if (suggestions != null && suggestions.Suggestions.Count > 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"Найдено: {suggestions.Suggestions[0].Data.Razdel}");
                        return suggestions.Suggestions[0].Data.Razdel;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("API не вернуло значение");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Ошибка при запросе к API DaData: {response.StatusCode}");
                }
            }

            // Если DaData не нашел категорию, обращаемся к Python-скрипту
            string aisug = await GetCategoryFromPython(productName);
            if (!string.IsNullOrEmpty(aisug))
            {
                return aisug;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Не удалось получить категорию от Python-скрипта");
            }

            return null; // Если ничего не найдено
        }



        private async Task<string> GetCategoryFromPython(string productName)
        {
            string pythonExePath = @"C:\Users\myton\AppData\Local\Programs\Python\Python310\python.exe";
            string fileName = @"C:\GPT.py";

            Process p = new Process();
            p.StartInfo = new ProcessStartInfo(pythonExePath, $"{fileName} \"{productName}\"")
            {
                RedirectStandardOutput = true, // Перенаправляем стандартный вывод
                UseShellExecute = false, // Не используем оболочку для перенаправления
                CreateNoWindow = true // Не создаем отдельное окно
            };

            StringBuilder outputBuilder = new StringBuilder(); // Для хранения вывода

            // Обработка вывода по строкам
            p.OutputDataReceived += (sender, args) =>
            {
                if (args.Data != null)
                {
                    Console.WriteLine(args.Data); // Печатаем вывод в консоль
                    outputBuilder.AppendLine(args.Data); // Сохраняем вывод для возвращения
                }
            };

            // Запускаем процесс
            p.Start();
            p.BeginOutputReadLine(); // Начинаем асинхронное чтение вывода

            // Ожидаем завершения процесса
            await Task.Run(() => p.WaitForExit());

            return outputBuilder.ToString().Trim(); // Возвращаем вывод скрипта
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
                // Задание контекста лицензии для EPPlus
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
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

        private string GenerateHtmlTable(List<Book> books)
        {
            var html = new System.Text.StringBuilder();

            html.Append("<h2 style=\"color: #d52b1e;\">Каталог товаров:</h2>");
            html.Append("<table style=\"width: 100%; border-collapse: collapse;\">");
            html.Append("<thead>");
            html.Append("<tr>");
            html.Append("<th style=\"border: 1px solid #ddd; padding: 8px; background-color: #f2f2f2;\">Код</th>");
            html.Append("<th style=\"border: 1px solid #ddd; padding: 8px; background-color: #f2f2f2;\">Название</th>");
            html.Append("<th style=\"border: 1px solid #ddd; padding: 8px; background-color: #f2f2f2;\">Категория</th>");
            html.Append("<th style=\"border: 1px solid #ddd; padding: 8px; background-color: #f2f2f2;\">Полное описание</th>");
            html.Append("</tr>");
            html.Append("</thead>");
            html.Append("<tbody>");

            foreach (var book in books)
            {
                html.Append("<tr>");
                html.Append($"<td style=\"border: 1px solid #ddd; padding: 8px;\">{book.Code}</td>");
                html.Append($"<td style=\"border: 1px solid #ddd; padding: 8px;\">{book.Name}</td>");

                // Получаем категорию по коду из словаря
                if (Okpd2Sections.TryGetValue(book.Category, out var categoryValue))
                {
                    html.Append($"<td style=\"border: 1px solid #ddd; padding: 8px;\">{categoryValue}</td>");
                }
                else
                {
                    // Если категория не найдена, можно вывести какое-то значение по умолчанию
                    html.Append($"<td style=\"border: 1px solid #ddd; padding: 8px;\">{book.Category}</td>");
                }

                html.Append($"<td style=\"border: 1px solid #ddd; padding: 8px;\">{book.FullDescription}</td>");
                html.Append("</tr>");
            }

            html.Append("</tbody></table>");
            return html.ToString();
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

    // Структура для десериализации ответа от API
    public class ApiResponse
    {
        public List<Suggestion> Suggestions { get; set; }
    }

    public class Suggestion
    {
        public string Value { get; set; }
        public Data Data { get; set; }
    }

    public class Data
    {
        [JsonProperty("razdel")] // Указываем имя поля из JSON
        public string Razdel { get; set; } // Буква категории (например, "C")

        [JsonProperty("kod")] // Указываем имя поля из JSON
        public string Kod { get; set; } // Код категории (например, "30.20.20.112")

        [JsonProperty("name")] // Указываем имя поля из JSON
        public string Name { get; set; } // Имя категории (например, "Дизель-поезда")
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
