using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace O2GEN.Controllers
{
    public class TestController : Controller
    {
        private readonly List<TestClass> _test;

        public TestController()
        {
            _test = new List<TestClass>();

            _test.Add(new TestClass
            {
                EquipmentType = "1",
                Status = "2",
                Type = "3",
                Bypass = "4",
                Bypasser = "5",
                Start = DateTime.Now,
                End = DateTime.Now
            });
        }

        public IActionResult Test()
        {
            return View(_test);
        }
    }

    public class TestClass
    {
        [DisplayName("Тип оборудований")]
        public string EquipmentType { get; set; }

        [DisplayName("Статус обхода")]
        public string Status { get; set; }

        [DisplayName("Тип обхода")]
        public string Type { get; set; }

        [DisplayName("Обход")]
        public string Bypass { get; set; }

        [DisplayName("Обходчик")]
        public string Bypasser { get; set; }

        [DisplayName("Дата начала (план)")]
        public DateTime Start { get; set; }

        [DisplayName("Дата окончания (план)")]
        public DateTime End { get; set; }
        [DisplayName("Дефект")]
        public string Defect { get; set; }

        [DisplayName("Редактирование")]
        public string Edit { get; set; }
    }
}
