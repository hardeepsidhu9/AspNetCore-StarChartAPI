using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [ApiController]
    [Route("")]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.FirstOrDefault(co => co.Id == id);
            if (celestialObject == null)
            {
                return NotFound();
            }
            else
            {
                celestialObject.Satellites = _context.CelestialObjects.Where(co => co.OrbitedObjectId == celestialObject.Id).ToList();
                return Ok(celestialObject);
            }
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects.Where(co => co.Name == name);
            if (celestialObjects != null && celestialObjects.Any())
            {
                foreach (var celestialObject in celestialObjects)
                {
                    celestialObject.Satellites = _context.CelestialObjects.Where(co => co.OrbitedObjectId == celestialObject.Id).ToList();
                }

                return Ok(celestialObjects);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects;

            foreach (var celestialObject in celestialObjects)
            {
                celestialObject.Satellites = _context.CelestialObjects.Where(co => co.OrbitedObjectId == celestialObject.Id).ToList();
            }

            return Ok(celestialObjects);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();

            return CreatedAtRoute("GetById", new { id = celestialObject.Id }, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject modifiedCelestialObject)
        {
            var celestialObject = _context.CelestialObjects.FirstOrDefault(co => co.Id == id);
            if (celestialObject == null)
            {
                return NotFound();
            }
            else
            {
                celestialObject.Name = modifiedCelestialObject.Name;
                celestialObject.OrbitalPeriod = modifiedCelestialObject.OrbitalPeriod;
                celestialObject.OrbitedObjectId = modifiedCelestialObject.OrbitedObjectId;

                _context.CelestialObjects.Update(celestialObject);
                _context.SaveChanges();

                return NoContent();
            }
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var celestialObject = _context.CelestialObjects.FirstOrDefault(co => co.Id == id);
            if (celestialObject == null)
            {
                return NotFound();
            }
            else
            {
                celestialObject.Name = name;

                _context.CelestialObjects.Update(celestialObject);
                _context.SaveChanges();

                return NoContent();
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var celestialObjects = _context.CelestialObjects.Where(co => co.Id == id || co.OrbitedObjectId == id);

            if (celestialObjects != null && celestialObjects.Any())
            {
                _context.CelestialObjects.RemoveRange(celestialObjects);
                _context.SaveChanges();

                return NoContent();
                
            }
            else
            {
                return NotFound();
            }
        }
    }
}
