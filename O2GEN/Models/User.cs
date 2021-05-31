
using Microsoft.AspNetCore.Identity;
 
namespace O2GEN.Models
{
    public class User : IdentityUser
    {
        public int Year { get; set; }
    }
}