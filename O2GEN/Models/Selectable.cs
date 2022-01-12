namespace O2GEN.Models
{
    public class Selectable<T>
    {
        public T TargetObject { get; set; }
        public long? MatchId { get; set; }
        public bool Selected { get; set; }
        public bool BaseSelected { get; set; }
    }
}
