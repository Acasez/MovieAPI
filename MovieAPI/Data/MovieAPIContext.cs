using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MovieAPI.Models;

namespace MovieAPI.Data;

public class MovieAPIContext(DbContextOptions<MovieAPIContext> options) : DbContext(options)
{
    public DbSet<Movie> Movie { get; set; } = default!;
    public DbSet<Review> Review { get; set; } = default!;
    public DbSet<Actor> Actor { get; set; } = default!;
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Movie>().HasMany(m => m.Actors).WithMany(a => a.Movies)
            .UsingEntity<Dictionary<string, object>>(
                "MovieActor",
                j => j.HasOne<Actor>().WithMany().HasForeignKey("ActorId"),
                j => j.HasOne<Movie>().WithMany().HasForeignKey("MovieId")
            );
    }
}
