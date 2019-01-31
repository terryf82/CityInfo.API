using System.Collections.Generic;
using System.Linq;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    public class CitiesController : Controller
    {
        private ICityInfoRepository _cityInfoRepository;

        public CitiesController(ICityInfoRepository cityInfoRepository)
        {
            _cityInfoRepository = cityInfoRepository;
        }

        [HttpGet("api/cities")]
        public IActionResult GetCities()
        {
            var results = new List<CityWithoutPointsOfInterestDto>();

            foreach (var cityEntity in  _cityInfoRepository.GetCities())
            {
                results.Add(new CityWithoutPointsOfInterestDto
                {
                    Id = cityEntity.Id,
                    Name = cityEntity.Name,
                    Description = cityEntity.Description
                });
            }

            return Ok(results);
        }

        [HttpGet("api/cities/{id}")]
        public IActionResult GetCity(int id, bool includePointsOfInterest = false)
        {
            var city = _cityInfoRepository.GetCity(id, includePointsOfInterest);
            
            if (city == null)
            {
                return NotFound();
            }

            if (includePointsOfInterest)
            {
                var cityResult = new CityDto
                {
                    Id = city.Id,
                    Name = city.Name,
                    Description = city.Description
                };

                foreach (var poi in city.PointsOfInterest)
                {
                    cityResult.PointsOfInterest.Add(new PointOfInterestDto
                    {
                        Id = poi.Id,
                        Name = poi.Name,
                        Description = poi.Description
                    });
                }

                return Ok(cityResult);
            }
            else
            {
                var cityResult = new CityWithoutPointsOfInterestDto
                {
                    Id = city.Id,
                    Name = city.Name,
                    Description = city.Description
                };

                return Ok(cityResult);
            }
        }
    }
}