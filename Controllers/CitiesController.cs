using System.Collections.Generic;
using System.Linq;
using AutoMapper;
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
            var cityEntities = _cityInfoRepository.GetCities();
            var results = AutoMapper.Mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities);

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
                var cityResult = AutoMapper.Mapper.Map<CityDto>(city);
                return Ok(cityResult);
            }
            else
            {
                var cityResult = AutoMapper.Mapper.Map<CityWithoutPointsOfInterestDto>(city);
                return Ok(cityResult);
            }
        }
    }
}