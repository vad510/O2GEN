using System;
using System.ComponentModel;

namespace O2GEN.Models.HomeModels
{
    public class ReportOnViewedTechPosition
    {
        [DisplayName("Станция")]
        public string StationName { get; set; }

        [DisplayName("Интервал 1")]
        public DateTime FirstInterval { get; set; }

        [DisplayName("Интервал 2")]
        public DateTime SecondInterval { get; set; }

        [DisplayName("Изменения, %")]
        public double Changes { get; set; }
    }
}
