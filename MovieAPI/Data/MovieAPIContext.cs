using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MovieAPI.Models;

namespace MovieAPI.Data;

public class MovieAPIContext(DbContextOptions<MovieAPIContext> options) : DbContext(options)
{
    public DbSet<Movie> Movies { get; set; } = default!;
}
