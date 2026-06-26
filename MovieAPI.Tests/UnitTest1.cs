using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MovieAPI.DataTransferObjects;
using MovieAPI.Services;
using MovieAPI.Controllers;
using MovieAPI.Interfaces;
using MovieAPI.Models;
using Xunit;
using Assert = Xunit.Assert;

namespace MovieAPI.Tests
{
    public class MovieControllerTests
    {
        [Fact]
        public async Task GetMovies_ReturnsOkWithMovies()
        {
            // Arrange
            ICollection<Movie> testMovies = new List<Movie>
            {
                new()
                {
                    Id = 1,
                    Title = "Test Movie",
                    Year = 2000,
                    Duration = 120,
                    GenreId = 0,
                },
                new()
                {
                    Id = 2,
                    Title = "Test Movie 2",
                    Year = 2001,
                    Duration = 130,
                    GenreId = 1,
                }
            };

            ICollection<MovieDTO> expectedMovies = new List<MovieDTO>
            {
                new()
                {
                    Id = 1,
                    Title = "Test Movie",
                    Year = 2000,
                    Duration = 120,
                    GenreId = 0,
                },
                new()
                {
                    Id = 2,
                    Title = "Test Movie 2",
                    Year = 2001,
                    Duration = 130,
                    GenreId = 1,
                }
            };

            // Mock the repository with parameters
            Mock<IMovieService> mockService = new();
            mockService
                .Setup(s => s.GetMoviesAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(testMovies);

            // Mock the mapper
            Mock<IMapper> mockMapper = new();
            mockMapper
                .Setup(m => m.Map<IEnumerable<MovieDTO>>(It.IsAny<IEnumerable<Movie>>()))
                .Returns(expectedMovies.ToArray()); // Return as array to match the controller

            // Create the controller
            MoviesController controller = new(mockService.Object, mockMapper.Object);

            // Act
            ActionResult<IEnumerable<MovieDTO>> result = await controller.GetMovies();

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
            OkObjectResult? okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);

            MovieDTO[] returnedMovies = Assert.IsType<MovieDTO[]>(okResult.Value);
            Assert.Equal(2, returnedMovies.Length);
            Assert.Equal(1, returnedMovies[0].Id);
            Assert.Equal("Test Movie", returnedMovies[0].Title);
            Assert.Equal(2, returnedMovies[1].Id);
            Assert.Equal("Test Movie 2", returnedMovies[1].Title);
        }

        [Fact]
        public async Task GetMovieById_ReturnsOkWithMovie()
        {

            Movie exampleMovie = new()
            {
                Id = 1,
                Title = "Test Movie",
                Year = 2000,
                Duration = 120,
                GenreId = 1,
            };
        
            // Arrange
            MovieDTO movieDto = new()
            {
                Id = 1,
                Title = "Test Movie",
                Year = 2000,
                Duration = 120,
                GenreId = 1,
            };
        
            // Mock the repository with parameters
            Mock<IMovieService> mockService = new();
            mockService
                .Setup(s => s.GetMovieAsync(0)).ReturnsAsync(exampleMovie);
            
            // Mock the mapper
            Mock<IMapper> mockMapper = new();
            mockMapper
                .Setup(m => m.Map<MovieDTO>(It.IsAny<Movie>()))
                .Returns(movieDto); // Return as array to match the controller

        
            MoviesController controller = new(mockService.Object, mockMapper.Object);
        
            // Act
            ActionResult<MovieDTO> result = await controller.GetMovie(0);
        
            // Assert
            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result.Result);
        
            MovieDTO returnedMovie = Assert.IsType<MovieDTO>(okResult.Value);
        
            Assert.Equal(1, returnedMovie.Id);
            Assert.Equal("Test Movie", returnedMovie.Title);
        }
    }
}
