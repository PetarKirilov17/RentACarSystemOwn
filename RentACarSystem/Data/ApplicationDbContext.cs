using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RentACarSystem.Data.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace RentACarSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        //TODO -> organise the composite key
        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    base.OnModelCreating(builder);
              
        //}

        public DbSet<Car> Cars { get; set; }

        public DbSet<Query> Queries { get; set; }
    }
}
