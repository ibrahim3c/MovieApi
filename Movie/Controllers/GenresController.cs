using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movie.Data;
using Movie.DTOs;
using Movie.Models;
using Movie.Services.Interfaces;

namespace Movie.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly IGenreService genreService;

        public GenresController(IGenreService genreService)
        {
            this.genreService = genreService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var genres = await genreService.GetAllAsync();
            return Ok(genres);
        }

        [HttpGet("{id}", Name = "GetGenre")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var genre =await genreService.GetByIdAsync(id);
            if (genre == null) return NotFound();

            return Ok(genre);

        }

        [HttpPost]
        public async Task<IActionResult> AddAsync(CreateGenreDTO createGenreDTO)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);
            var genre = new Genre
            {
                Name = createGenreDTO.Name
            };

           await genreService.AddAsync(genre);

            // Generate the URL for the GetGenre action
            string? url = Url.Action("GetGenre", new { id = genre.Id });

            return Created(url, genre);
        }

        [HttpPut("{id}")] 
        public async Task<IActionResult> EditAsync(int id, [FromBody]EditGenreDTO editGenreDTO)
        {
            var genre=await genreService.GetByIdAsync(id);
            if (genre == null) return NotFound();

            genre.Name = editGenreDTO.Name;

            await genreService.EditAsync(genre);
            


            return Ok(editGenreDTO);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult>DeleteAsync(int id)
        {
            var genre =await genreService.GetByIdAsync(id);
            if (genre == null) return NotFound();

          
            return Ok();
        }
    }
    }
