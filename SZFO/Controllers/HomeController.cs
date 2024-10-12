using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SZFO.Controllers
{
    public class HomeController : Controller
    {
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
    { "T", "Товары и услуги различные, производимые домашними хозяйствами для собственного потребления, включая услуги работодателя для домашнего персонала" },
    { "U", "Услуги, предоставляемые экстерриториальными организациями и органами" }
};

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
        private BooksContext db = new BooksContext();

        public ActionResult Add()
        {
            // Передаем словарь категорий в ViewBag
            ViewBag.Okpd2Sections = new SelectList(Okpd2Sections, "Key", "Value");
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Add(Book book)
        {
            if (ModelState.IsValid)
            {
                db.Books.Add(book);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            // Повторно передаем словарь в случае ошибки валидации
            ViewBag.Okpd2Sections = new SelectList(Okpd2Sections, "Key", "Value");
            return View(book);
        }

    }
}