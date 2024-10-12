using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SZFO.Models
{
    public class BookInputModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Razdel { get; set; }
        public string FullDescription { get; set; }
    }
}