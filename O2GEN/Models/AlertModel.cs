using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace O2GEN.Models
{
    public class AlertModel
    {
        public AlertType? AlertType { get; set; }
        public string AlertMessage { get; set; }
    }
    public enum AlertType
    {
        Success = 0,
        Warning = 1,
        Error = 2
    }
}
