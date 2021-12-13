using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace O2GEN.Models
{
    public class MaximoDefect
    {
        public long Id { get; set; }
        [DisplayName("Подразделение")]
        public string DepartmentName { get; set; }
        [DisplayName("Название обхода")]
        public string TaskName { get; set; }
        [DisplayName("Дата создания")]
        public DateTime CreationTime { get; set; }
        [DisplayName("Код тех. поз.")]
        public string CustomAssetCode { get; set; }
        [DisplayName("Код узла")]
        public string CustomAssetChildCode { get; set; }
        [DisplayName("Описание")]
        [DataType(DataType.MultilineText)]
        public string CustomDescription { get; set; }
        public long MaximoStatusId { get; set; }
        [DisplayName("Статус отправления")]
        public string MaximoStatus { get; set; }
        [DisplayName("Номер заявки MAXIMO")]
        public string TICKETID { get; set; }
        public string TICKETUID { get; set; }
        [DisplayName("Последняя отправка")]
        public DateTime? LastSend { get; set; }
        [DisplayName("Ошибка от системы MAXIMO")]
        [DataType(DataType.MultilineText)]
        public string MaximoError { get; set; }
    }
}
