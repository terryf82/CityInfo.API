using System.Linq;
using CityInfo.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CityInfo.API.Controllers
{
    public class PointsOfInterestController : Controller
    {
        [HttpGet("api/cities/{cityId}/pointsofinterest")]
        public IActionResult GetPointsOfInterest(int cityId)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            return Ok(city.PointsOfInterest);
        }

        [HttpGet("api/cities/{cityId}/pointsofinterest/{poiId}", Name = "GetPointOfInterest")]
        public IActionResult GetPointOfInterest(int cityId, int poiId)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            
            if (city == null)
            {
                return NotFound();
            }

            var poi = city.PointsOfInterest.FirstOrDefault(p => p.Id == poiId);
            
            if (poi == null)
            {
                return NotFound();
            }

            return Ok(poi);
        }

        [HttpPost("api/cities/{cityId}/pointsofinterest")]
        public IActionResult CreatePointOfInterest(int cityId,
            [FromBody] PointOfInterestForCreationDto pointOfInterest)
        {
            if (pointOfInterest == null)
            {
                return BadRequest();
            }

            if (pointOfInterest.Name == pointOfInterest.Description)
            {
                ModelState.AddModelError("Description", "Name and description cannot be the same");
            }

            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            
            if (city == null)
            {
                return NotFound();
            }

            var maxPointOfInterestId = CitiesDataStore.Current.Cities.SelectMany(
                c => c.PointsOfInterest).Max(p => p.Id);

            var finalPointOfInterest = new PointOfInterestDto()
            {
                Id = ++maxPointOfInterestId,
                Name = pointOfInterest.Name,
                Description = pointOfInterest.Description
            };

            city.PointsOfInterest.Add(finalPointOfInterest);

            return CreatedAtRoute("GetPointOfInterest",
                new {cityId = city.Id, poiId = finalPointOfInterest.Id}, finalPointOfInterest);
        }

        [HttpPut("api/cities/{cityId}/pointsofinterest/{poiId}")]
        public IActionResult UpdatePointOfInterest(int cityId, int poiId,
            [FromBody] PointOfInterestForUpdateDto pointOfInterest)
        {
            if (pointOfInterest == null)
            {
                return BadRequest();
            }

            if (pointOfInterest.Name == pointOfInterest.Description)
            {
                ModelState.AddModelError("Description", "Name and description cannot be the same");
            }

            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == poiId);

            if (pointOfInterestFromStore == null)
            {
                return NotFound();
            }

            pointOfInterestFromStore.Name = pointOfInterest.Name;
            pointOfInterestFromStore.Description = pointOfInterest.Description;

            // Could return Ok(pointOfInterestFromStore) but convention is NoContent, since returning the
            // same content the client already has is redundant
            return NoContent();
        }

        [HttpPatch("api/cities/{cityId}/pointsofinterest/{poiId}")]
        public IActionResult PartiallyUpdatePointOfInterest(int cityId, int poiId,
        [FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == poiId);

            if (pointOfInterestFromStore == null)
            {
                return NotFound();
            }

            var pointOfInterestToPatch = 
                new PointOfInterestForUpdateDto()
                {
                    Name = pointOfInterestFromStore.Name,
                    Description = pointOfInterestFromStore.Description
                };
            
            patchDoc.ApplyTo(pointOfInterestToPatch, ModelState);

            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            return NoContent();
        }
    }
}