using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioAPI.Data;
using PortfolioAPI.Models;

namespace PortfolioAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly PortfolioDbContext _db;

        public ProjectsController(PortfolioDbContext db)
        {
            _db = db;
        }

        // GET: api/projects
        [HttpGet]
        public IActionResult GetAll()
        {
            var items = _db.Projects
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new {
                    projectId = p.Id,
                    title = p.Title,
                    description = p.Description,
                    imageUrl = p.ImageUrl,
                    githubLink = p.GithubLink,
                    demoLink = p.DemoLink,
                    createdAt = p.CreatedAt
                })
                .ToList();

            return Ok(items);
        }

        // GET: api/projects/{id}
        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            var p = _db.Projects.Find(id);
            if (p == null) return NotFound();
            return Ok(new
            {
                projectId = p.Id,
                title = p.Title,
                description = p.Description,
                imageUrl = p.ImageUrl,
                githubLink = p.GithubLink,
                demoLink = p.DemoLink,
                createdAt = p.CreatedAt
            });
        }

        // POST: api/projects  (admin only)
        [Authorize]
        [HttpPost]
        public IActionResult Create([FromBody] ProjectCreateDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Title)) return BadRequest("Title is required");

            var p = new Project
            {
                Title = dto.Title,
                Description = dto.Description ?? "",
                ImageUrl = dto.ImageUrl ?? "",
                GithubLink = dto.GithubLink ?? "",
                DemoLink = dto.DemoLink ?? "",
                CreatedAt = DateTime.UtcNow
            };

            _db.Projects.Add(p);
            _db.SaveChanges();

            return CreatedAtAction(nameof(Get), new { id = p.Id }, new { projectId = p.Id });
        }

        // PUT: api/projects/{id} (admin only)
        [Authorize]
        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] ProjectCreateDto dto)
        {
            var p = _db.Projects.Find(id);
            if (p == null) return NotFound();
            if (dto == null || string.IsNullOrWhiteSpace(dto.Title)) return BadRequest("Title is required");

            p.Title = dto.Title;
            p.Description = dto.Description ?? "";
            p.ImageUrl = dto.ImageUrl ?? "";
            p.GithubLink = dto.GithubLink ?? "";
            p.DemoLink = dto.DemoLink ?? "";
            _db.SaveChanges();

            return NoContent();
        }

        // DELETE: api/projects/{id} (admin only)
        [Authorize]
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var p = _db.Projects.Find(id);
            if (p == null) return NotFound();
            _db.Projects.Remove(p);
            _db.SaveChanges();
            return NoContent();
        }
    }

    public class ProjectCreateDto
    {
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string ImageUrl { get; set; } = "";
        public string GithubLink { get; set; } = "";
        public string DemoLink { get; set; } = "";
    }
}
