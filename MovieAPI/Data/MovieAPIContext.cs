using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MovieAPI.Models;

namespace MovieAPI.Data
{
    public class MovieAPIContext : DbContext
    {
        public MovieAPIContext (DbContextOptions<MovieAPIContext> options)
            : base(options)
        {
        }

        public DbSet<MovieAPI.Models.Movie> Movie { get; set; } = default!;
    }
}
