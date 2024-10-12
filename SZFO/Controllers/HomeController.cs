using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SZFO.Models; // Не забудьте добавить пространство имен для модели Book
using SZFO.Data; // Не забудьте добавить пространство имен для ApplicationDbContext

namespace SZFO.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context; // Инициализация контекста базы данных
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        // Метод для отображения формы добавления товара
        public ActionResult Add()
        {
            return View();
        }

        // Метод для обработки POST-запроса на добавление товара
        [HttpPost]
        public ActionResult Add(Book book) // Используем модель Book
        {
            if (ModelState.IsValid) // Проверка валидности модели
            {
                _context.Books.Add(book); // Добавление книги в контекст
                _context.SaveChanges(); // Сохранение изменений в базе данных
                return RedirectToAction("Index"); // Перенаправление на главную страницу
            }

            return View(book); // Если модель не валидна, вернуть представление с текущими данными
        }
    }
}
