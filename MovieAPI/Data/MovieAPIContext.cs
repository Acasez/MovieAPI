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
    public DbSet<Movie> Movie { get; set; } = default!;
    public DbSet<Review> Review { get; set; } = default!;
    public DbSet<Actor> Actor { get; set; } = default!;
    public DbSet<Genre> Genre { get; set; } = default!;
    public DbSet<Setting> Settings { get; set; } = default!;
    public DbSet<MovieDetails> MovieDetails { get; set; } = default!;
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Movie>().HasMany(m => m.Actors).WithMany(m => m.Movies)
            .UsingEntity<Dictionary<string, object>>(
                "MovieActor",
                j => j.HasOne<Actor>().WithMany().HasForeignKey("ActorId"),
                j => j.HasOne<Movie>().WithMany().HasForeignKey("MovieId")
            );
        //SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        //Movie kikis = new()
        //{
        //    Id = 20,
        //    Title = "Kiki's Delivery Service",
        //    Genre = "Slice of Life",
        //    Duration = 102,
        //};

        //modelBuilder.Entity<Movie>().HasData(
        //    kikis
        //);
    }
}
