using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MovieAPI.Models;

namespace MovieAPI.Data;

public class ActorContext(DbContextOptions<ActorContext> options) : DbContext(options)
{
    public DbSet<Actor> Actor { get; set; } = default!;
}
