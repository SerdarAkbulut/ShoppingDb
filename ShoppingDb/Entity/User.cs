using Microsoft.AspNetCore.Identity;

namespace ShoppingApi.Entity
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public List<Address> Addresses { get; set; } = new ();
    }
}
