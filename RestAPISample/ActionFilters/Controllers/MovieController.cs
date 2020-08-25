using System;
using System.Linq;
using ActionFilters.ActionFilters;
using ActionFilters.Entities;
using ActionFilters.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace ActionFilters.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly MovieContext _context;

        public MovieController(MovieContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var movies = _context.Movies.ToList();
            return Ok(movies);
        }

        [HttpGet("{id}", Name = "MovieById")]
        [ServiceFilter(typeof(ValidateEntityExistsAttribute<Movie>))]
        public IActionResult Get(Guid id)
        {
            var _dbmovie = HttpContext.Items["entity"] as Movie;

            return Ok(_dbmovie);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public IActionResult Post([FromBody]Movie movie)
        {
            _context.Movies.Add(movie);
            _context.SaveChanges();

            return CreatedAtRoute("MovieById", new { Id = movie.Id }, movie);
        }

        [HttpPut("{id}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [ServiceFilter(typeof(ValidateEntityExistsAttribute<Movie>))]
        public IActionResult Put(Guid id, [FromBody]Movie movie)
        {
            var _dbmovie = HttpContext.Items["entity"] as Movie;

            _dbmovie.Map(movie);

            _context.Movies.Update(_dbmovie);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ServiceFilter(typeof(ValidateEntityExistsAttribute<Movie>))]
        public IActionResult Delete(Guid id)
        {
            var _dbmovie = HttpContext.Items["entity"] as Movie;

            _context.Movies.Remove(_dbmovie);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
