using System.ComponentModel;

namespace O2GEN.Models
{
    /// <summary>
    /// Статус объекта
    /// </summary>
    public class AssetState
    {
        public int Id { get; set; } = -1;

        [DisplayName("Название")]
        public string DisplayName { get; set; }
    }
}
