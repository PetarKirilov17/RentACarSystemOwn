using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RentACarSystem.Models.ViewModels.Users
{
    public class AppUserViewModel
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        [StringLength(10)]
        public string EGN { get; set; }

        public string PhoneNumber { get; set; }

        public string Role { get; set; }
    }
}
