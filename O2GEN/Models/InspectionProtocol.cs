using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace O2GEN.Models
{

    public class InspectionProtocol
    {
        public int Id { get; set; } = -1;
        public Guid? ObjectUID { get; set; } = Guid.NewGuid();
        [DisplayName("Тех. позиция")]
        public string Name { get; set; }
        public bool IsNFC { get; set; }
        public List<InspectionProtocolItem> Items { get; set; } = new List<InspectionProtocolItem>();
    }
}
