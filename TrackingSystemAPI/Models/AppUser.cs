using Microsoft.AspNetCore.Identity;

namespace TrackingSystemAPI.Models
{
    public class AppUser : IdentityUser<int>
    {
        public string FullName { get; set; } = null!;
        public DateTime DataAdded { get; set; } = DateTime.Now;
    }
}