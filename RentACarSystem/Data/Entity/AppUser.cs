using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RentACarSystem.Data.Entity
{
    public class AppUser : IdentityUser
    {
        public AppUser()
        {
            UserQueries = new List<Query>();
        }
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        [StringLength(10)]
        public string EGN { get; set; }

        public virtual List<Query> UserQueries { get; set; }
    }
}
