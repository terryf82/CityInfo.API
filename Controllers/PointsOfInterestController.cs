using System;
using System.Collections.Generic;
using System.Linq;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace CityInfo.API.Controllers
{
    public class PointsOfInterestController : Controller
    {
        private readonly ILogger<PointsOfInterestController> _logger;

        private readonly IMailService _mailerService;

        private ICityInfoRepository _cityInfoRepository;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailerService, ICityInfoRepository cityInfoRepository)
        {
            _logger = logger;
            _mailerService = mailerService;
            _cityInfoRepository = cityInfoRepository;
        }

        [HttpGet("api/cities/{cityId}/pointsofinterest")]
        public IActionResult GetPointsOfInterest(int cityId)
        {
            var city = _cityInfoRepository.GetCity(cityId, false);
            if (city == null)
            {
                return NotFound();
            }

            var pois = _cityInfoRepository.GetPointsOfInterestForCity(cityId);
            
            var results = AutoMapper.Mapper.Map<IEnumerable<PointOfInterestDto>>(pois);

            return Ok(results);
        }

        [HttpGet("api/cities/{cityId}/pointsofinterest/{poiId}", Name = "GetPointOfInterest")]
        public IActionResult GetPointOfInterest(int cityId, int poiId)
        {
            var city = _cityInfoRepository.GetCity(cityId, true);
            if (city == null)
            {
                return NotFound();
            }

            var poi = city.PointsOfInterest.FirstOrDefault(p => p.Id == poiId);
            
            if (poi == null)
            {
                return NotFound();
            }

            var poiResult = AutoMapper.Mapper.Map<PointOfInterestDto>(poi);
            return Ok(poiResult);
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


            var city = _cityInfoRepository.GetCity(cityId, false);
            if (city == null)
            {
                return NotFound();
            }

            var finalPointOfInterest = AutoMapper.Mapper.Map<Entities.PointOfInterest>(pointOfInterest);

            _cityInfoRepository.AddPointOfInterestForCity(cityId, finalPointOfInterest);

            if ( ! _cityInfoRepository.Save())
            {
                return StatusCode(500, "Error handling request");
            }

            var createdPoi = AutoMapper.Mapper.Map<PointOfInterestDto>(finalPointOfInterest);

            return CreatedAtRoute("GetPointOfInterest",
                new {cityId = city.Id, poiId = createdPoi.Id}, createdPoi);
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

            if (pointOfInterestToPatch.Name == pointOfInterestToPatch.Description)
            {
                ModelState.AddModelError("Description", "Name and description cannot be the same");
            }

            TryValidateModel(pointOfInterestToPatch);

            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
            pointOfInterestFromStore.Description = pointOfInterestToPatch.Description;

            return NoContent();
        }

        [HttpDelete("api/cities/{cityId}/pointsofinterest/{poiId}")]
        public IActionResult DeletePointOfInterest(int cityId, int poiId)
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

            city.PointsOfInterest.Remove(poi);

            // Notify by email of deleted poi
            _mailerService.Send("PoI deleted", $"{poi.Name} was deleted");

            return NoContent();
        }
    }
}