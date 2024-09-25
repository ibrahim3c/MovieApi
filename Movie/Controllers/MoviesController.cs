using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movie.Authorization;
using Movie.Authorization.PermissionBased;
using Movie.Data;
using Movie.DTOs;
using Movie.Models;
using Movie.Services.Interfaces;
using Movie.Settings;
using Serilog.Sinks.File;

namespace Movie.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MoviesController : ControllerBase
    {
        private readonly IGenreService genreService;
        private readonly IFileService fileService;
        private readonly ILogger<MoviesController> logger;
        private readonly IMovieService movieService;
        private readonly IMapper mapper;

        public MoviesController(IGenreService genreService,IFileService fileService,ILogger<MoviesController> logger,
            IMovieService movieService,IMapper mapper)
        {
            this.genreService = genreService;
            this.fileService = fileService;
            this.logger = logger;
            this.movieService = movieService;
            this.mapper = mapper;
        }
        [HttpPost]
        [CheckPermission(PermissionEnum.AddMovie)]
        public async Task<IActionResult> CreateAsync( [FromForm]CreateMovieDTO createMovieDTO)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);
            var fileExtension = Path.GetExtension(createMovieDTO.Poster.FileName).ToLowerInvariant();
            var FileSize = createMovieDTO.Poster.Length;

            if (!FileSettings.AllowedExtensions.Contains(fileExtension)) return BadRequest("Extension not allowed.");

            if (FileSettings.MaxFileSizeInBytes < FileSize) return BadRequest("the max Size is 2MB");

            var genre =await genreService.GetByIdAsync(createMovieDTO.GenreID);
            if (genre is null) return NotFound("the GenreID is not Found");

            var poster =  await fileService.UploadFileAsync(createMovieDTO.Poster, FileSettings.ImagePath);
            if(string.IsNullOrEmpty(poster)) return StatusCode(StatusCodes.Status500InternalServerError, "File upload failed.");
            var movie = new MOvie()
            {
                GenreID = createMovieDTO.GenreID,
                Title = createMovieDTO.Title,
                Year = createMovieDTO.Year,
                Rate = createMovieDTO.Rate,
                Storyline = createMovieDTO.Storyline,
                Poster = poster
            };
          await  movieService.AddAsync(movie);
            return Ok(movie);
        }


        [HttpGet]
        //[CheckPermission(PermissionEnum.GetMovies)]
        //[Authorize(Policy= "isMale")]
        [Authorize(Policy = "IsPassed")]
        public async Task<IActionResult> GetAllAsync()
        {
            logger.LogInformation("This to test Logger");

            var movies = await movieService.GetAllAsync();
            // Map Movies to MoviesDetailsDTO =>use automapper
            var MoviesDetails=mapper.Map<IEnumerable<MovieDetails>>(movies);
            return Ok(MoviesDetails);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            logger.LogInformation("Get The Movie with ID= {id}",id);

            var movie = await movieService.GetByIdAsync(id);

            // map movie to movieDetailsDTO=>use automapper
            // Map Movies to MoviesDetailsDTO =>use automapper
            var MoviesDetails = mapper.Map<MovieDetails>(movie);
            return Ok(MoviesDetails);
        }

        [HttpDelete("{id}")]
        [CheckPermission(PermissionEnum.DeleteMovie)]

        public async Task<IActionResult> DeleteAsync(int id)
        {
            var movie = await movieService.GetByIdAsync(id);
            if (movie == null) return NotFound();

            var affectedRows = await movieService.DeleteAsync(movie);
            if (affectedRows)
            {
                await fileService.DeleteFileAsync(movie.Poster);
            }

            return Ok(movie);
        }

        [HttpPut("{id}")]
        [CheckPermission(PermissionEnum.DeleteMovie)]

        public async Task<IActionResult> EditAsync(int id,EditMovieDTO editMovieDTO)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);
            var movie= await movieService.GetByIdAsync(id);
            if (movie == null) return NotFound("No Movie Found");

            movie.Title = editMovieDTO.Title;
            movie.Year = editMovieDTO.Year;
            movie.Storyline = editMovieDTO.Storyline;
            movie.Rate= editMovieDTO.Rate;

            var oldPoster = movie.Poster;
            var genre=await genreService.GetByIdAsync(editMovieDTO.GenreID);
            if(genre !=null) movie.GenreID = editMovieDTO.GenreID;

            if (editMovieDTO.Poster != null && editMovieDTO.Poster.Length != 0) {

                var fileExtension = Path.GetExtension(editMovieDTO.Poster.FileName).ToLowerInvariant();
                var FileSize = editMovieDTO.Poster.Length;

                if (!FileSettings.AllowedExtensions.Contains(fileExtension)) return BadRequest("Extension not allowed.");

                if (FileSettings.MaxFileSizeInBytes < FileSize) return BadRequest("the max Size is 2MB");

                movie.Poster = await fileService.UploadFileAsync(editMovieDTO.Poster, FileSettings.ImagePath);
            }

              var affectedRows= await movieService.EditAsync(movie);
            if (affectedRows)
            {
                if(oldPoster is not null) await fileService.DeleteFileAsync(oldPoster);
            }
            else
            {
                if (movie.Poster != null) await fileService.DeleteFileAsync(movie.Poster);
            }

            return Ok(new { movie.Id, movie.Title, movie.Year, movie.Storyline, movie.Rate, movie.GenreID });
        }



    }
}
