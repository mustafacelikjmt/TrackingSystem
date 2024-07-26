using Microsoft.AspNetCore.Identity;

namespace TrackingSystemWeb.Models
{
    public class AppUser : IdentityUser
    {
        public string UserId
        {
            get { return this.Id; }
            set { this.Id = value; }
        }

        public string FullName { get; set; } = string.Empty;
        public string? Image { get; set; }
    }
}