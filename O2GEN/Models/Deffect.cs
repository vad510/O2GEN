using O2GEN.Helpers.ValidationAttributes;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace O2GEN.Models
{
    /// <summary>
    /// Деффекты
    /// </summary>
    public class Deffect
    {
        [DisplayName("Тех.Позиция")]
        public string AName { get; set; }
        [DisplayName("Узел")]
        public string ACName { get; set; }
        [DisplayName("Контроль")]
        public string APVName { get; set; }
        [DisplayName("Значение")]
        public string Value { get; set; }
        [DisplayName("Дефект обнаружил")]
        public string UserName { get; set; }
        [DisplayName("Время обнаружения")]
        public DateTime Timestamp { get; set; }
        [DisplayName("Комментарий")]
        public string Comment { get; set; }
        
    }
}
