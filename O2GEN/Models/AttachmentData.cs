using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace O2GEN.Models
{
    public class AttachmentData
    {
        /// <summary>
        /// Данные
        /// </summary>
        public byte[] Data { get; set; }
        /// <summary>
        /// Имя файла
        /// </summary>
        public string FileName { get; set; }
    }
}
