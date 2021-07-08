using System;

namespace O2GEN.Models
{
    /// <summary>
    /// Роли
    /// </summary>
    public class Person
    {
        public int Id { get; set; } = -1;
        public string Surname { get; set; } 
        public string GivenName { get; set; } 
        public string MiddleName { get; set; }
        public int UserId { get; set; }
        public int? PersonPositionId { get; set; }
        public string DisplayName => $"{(Surname!= null? Surname:"")}{(GivenName != null && GivenName.Length>0 ? GivenName.Substring(0,1)+". " : "")}{(MiddleName != null && MiddleName.Length > 0 ? MiddleName.Substring(0, 1): "")}";
        public Guid ObjectUID { get; set; } = Guid.NewGuid();
    }
}
