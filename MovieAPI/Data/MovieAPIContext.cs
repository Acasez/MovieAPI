using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MovieAPI.Models;

namespace MovieAPI.Data;
// ReSharper disable once InconsistentNaming
public class MovieAPIContext(DbContextOptions<MovieAPIContext> options) : DbContext(options)
{
    public DbSet<Movie> Movie { get; set; } = null!;
    public DbSet<Review> Review { get; set; } = null!;
    public DbSet<Actor> Actor { get; set; } = null!;
    public DbSet<Genre> Genre { get; set; } = null!;
    public DbSet<Setting> Settings { get; set; } = null!;
    public DbSet<MovieDetails> MovieDetails { get; set; } = null!;
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Movie>().HasMany(m => m.Actors).WithMany(m => m.Movies)
            .UsingEntity<Dictionary<string, object>>(
                "MovieActor",
                j => j.HasOne<Actor>().WithMany().HasForeignKey("ActorId"),
                j => j.HasOne<Movie>().WithMany().HasForeignKey("MovieId")
            );
    }
}
