using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MovieAPI.Models;

namespace MovieAPI.Data
{
    public class ActorContext : DbContext
    {
        public ActorContext (DbContextOptions<ActorContext> options)
            : base(options)
        {
        }

        public DbSet<MovieAPI.Models.Actor> Actor { get; set; } = default!;
    }
}
