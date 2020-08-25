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
            var _dbmovie = _context.Movies.SingleOrDefault(x => x.Id.Equals(id));

            if (_dbmovie == null)
                return NotFound();
            else
                return Ok(_dbmovie);
        }

        [HttpPost]
        public IActionResult Post([FromBody]Movie movie)
        {
            if (movie == null)
                return BadRequest("Movie object is null");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Movies.Add(movie);
            _context.SaveChanges();

            return CreatedAtRoute("MovieById", new { Id = movie.Id }, movie);
        }

        [HttpPut("{id}")]
        [ServiceFilter(typeof(ValidateEntityExistsAttribute<Movie>))]
        public IActionResult Put(Guid id, [FromBody]Movie movie)
        {
            if (movie == null)
                return BadRequest("Movie object is null");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var _dbmovie = _context.Movies.SingleOrDefault(x => x.Id.Equals(id));

            if (_dbmovie == null)
                return NotFound();

            _dbmovie.Map(movie);

            _context.Movies.Update(_dbmovie);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ServiceFilter(typeof(ValidateEntityExistsAttribute<Movie>))]
        public IActionResult Delete(Guid id)
        {
            var _dbmovie = _context.Movies.SingleOrDefault(x => x.Id.Equals(id));

            if (_dbmovie == null)
                return NotFound();

            _context.Movies.Remove(_dbmovie);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
