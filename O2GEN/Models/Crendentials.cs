using O2GEN.Authorization;

namespace O2GEN.Models
{
    public class Credentials
    {
        /// <summary>
        /// Engineers.Id
        /// </summary>
        public long Id { get; set; }
        public long DeptId { get; set; }
        /// <summary>
        /// Логин
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Пароль
        /// </summary>
        public string Password { get; set; }
        public string Token { get; set; }
        public TokenExceprion TokenException { get; set; }
        public string DisplayName { get; set; }
    }
}
