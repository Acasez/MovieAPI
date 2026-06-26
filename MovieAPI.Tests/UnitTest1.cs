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

        // [Fact]
        // public async Task GetMovieById_ReturnsOkWithMovie()
        // {
        //
        //     // Arrange
        //     MovieDTO movieDto = new MovieDTO
        //     {
        //         Id = 1,
        //         Title = "Test Movie",
        //         Year = 2000,
        //         Duration = 120,
        //         GenreId = 1,
        //     };
        //
        //     Mock<MoviesController> mockService = new();
        //
        //     mockService.Setup(s => s.GetMovie(1)).ReturnsAsync(movieDto);
        //
        //     MoviesController controller = new(mockService.Object);
        //
        //     // Act
        //     ActionResult<MovieDto> result = await controller.GetMovieById(1);
        //
        //     // Assert
        //     OkObjectResult okResult = Assert.IsType<OkObjectResult>(result.Result);
        //
        //     MovieDto returnedMovie = Assert.IsType<MovieDto>(okResult.Value);
        //
        //     Assert.Equal(1, returnedMovie.Id);
        //     Assert.Equal("Test Movie", returnedMovie.Title);
        // }
        //
        // [Fact]
        // public async Task GetMovieDetails_ReturnsOK()
        // {
        //     // Arrange
        //     var movieDto = new MovieDto {
        //         Id = 1,
        //         Title = "Test Movie",
        //         Year = null,
        //         Duration = null,
        //         Details = new MovieDetailsDto {
        //             Synopsis = "SynopsisTest.",
        //             Language = "Engelska",
        //             Budget = "$Test"
        //         },
        //         Actors = new List<ActorDto>(),
        //         Genres = new List<GenreDto>(),
        //         Reviews = new List<ReviewDto>()
        //     };
        //
        //     var mockService = new Mock<IMovieService>();
        //
        //     mockService
        //         .Setup(s => s.GetMovieDetails(1))
        //         .ReturnsAsync(movieDto);
        //
        //     var controller = new MoviesController(mockService.Object);
        //
        //     // Act
        //     var result = await controller.GetMovieDetails(1);
        //
        //     // Assert
        //     var okResult = Assert.IsType<OkObjectResult>(result.Result);
        //     var returnedMovie = Assert.IsType<MovieDetailsDto>(okResult.Value);
        //     Assert.Equal("Test Movie", returnedMovie.Title);
        //
        //     var returnedDetails = Assert.IsType<MovieDetailsDto>(okResult.Value);
        //     Assert.Equal("SynopsisTest.", returnedDetails.Synopsis);
        //     Assert.Equal("Engelska", returnedDetails.Language);
        //     Assert.Equal("$Test", returnedDetails.Budget);
        // }
        //
        // [Fact]
        // public async Task GetMovieReviewByMovieId_ReturnsOKWithReviews()
        // {
        //     // Arrange
        //     var movieDto = new MovieDto {
        //         Id = 1,
        //         Title = "Test Movie",
        //         Year = null,
        //         Duration = null,
        //         Details = new MovieDetailsDto(),
        //         Actors = new List<ActorDto>(),
        //         Genres = new List<GenreDto>(),
        //         Reviews = new List<ReviewDto> {
        //             new ReviewDto {
        //                 ReviewId = 1,
        //                 ReviewerName = "Test Reviewer",
        //                 Rating = 5,
        //                 Comment = "Great movie!"
        //             }
        //         }
        //     };
        //
        //     var mockService = new Mock<IMovieService>();
        //
        //     mockService
        //         .Setup(s => s.GetMovieReviews(1))
        //         .ReturnsAsync(movieDto);
        //
        //     var controller = new MoviesController(mockService.Object);
        //
        //     // Act
        //     var result = await controller.GetMovieReviews(1);
        //
        //     // Assert
        //     var okResult = Assert.IsType<OkObjectResult>(result.Result);
        //     var reviews = Assert.IsType<List<ReviewDto>>(okResult.Value);
        //
        //     var returnedReviews = Assert.IsType<List<ReviewDto>>(reviews);
        //     Assert.Single(returnedReviews);
        //     //Assert.Equal(1, returnedReviews[0].Id);
        //     Assert.Equal("Test Reviewer", returnedReviews[0].ReviewerName);
        //     Assert.Equal(5, returnedReviews[0].Rating);
        //     Assert.Equal("Great movie!", returnedReviews[0].Comment);
        // }
    }
}
