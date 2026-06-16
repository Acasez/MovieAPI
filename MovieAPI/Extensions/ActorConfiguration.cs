using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.VisualBasic;
using MovieAPI.Models;

namespace MovieAPI.Extensions;

public class ActorConfiguration : IEntityTypeConfiguration<Actor>
{
    public void Configure(EntityTypeBuilder<Actor> builder)
    {
        builder.ToTable("Movies");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Name).IsRequired().HasMaxLength(200);
        builder.Property(a => a.YearOfBirth).IsRequired();

        // Seed default movies
        builder.HasData(
            new
            {
                Id = Guid.Parse("d1a7b9c3-4e56-4f89-a123-b456c789d012"),
                Title = "Johnny Depp",
                YearOfBirth = 1963,
            },
            new
            {
                Id = Guid.Parse("e2b8c0d4-5f67-4890-b234-c567d890e123"),
                Title = "Ryan Gosling",
                YearOfBirth = 1963,
            }
        );
    }
}
