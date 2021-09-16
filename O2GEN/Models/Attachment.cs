using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace O2GEN.Models
{
    public class Attachment
    {
        public long Id { get; set; }
        public long SCId { get; set; }
        /// <summary>
        /// Имя файла
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime CreationTime { get; set; }
        /// <summary>
        /// Ссылка на файл, будет использоваться для скачивания
        /// </summary>
        public Guid FileRef { get; set; }
        /// <summary>
        /// Название тех позиции
        /// </summary>
        public string IPName { get; set; }
        /// <summary>
        /// Название узла
        /// </summary>
        public string IPIName { get; set; }
        /// <summary>
        /// Название контроля
        /// </summary>
        public string APName { get; set; }
    }
}
