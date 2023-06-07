using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace Core.Modeles
{
    public static class ModelBuilderExtension
    {



        public static void SeedData(this ModelBuilder modelBuilder)
        {
           
          modelBuilder.Entity<TeamMember>().HasData(
          new TeamMember() { Position = Position.Technician, Id = 1, Name = "KENGELE", PhotoPath=null, Phone="+27814212260" },
          new TeamMember() { Position = Position.Producer, Id = 3, Name = "Johann", PhotoPath = null, Phone="+27824163759" }
          );

        }

    }
}
