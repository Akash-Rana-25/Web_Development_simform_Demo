using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Demo.Entity
{
    public class User : IdentityUser
    {
            
            public string Name { get; set; }
            public ICollection<Post> Posts { get; set; }
        
    }
}
