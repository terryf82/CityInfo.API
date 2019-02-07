using System.Collections.Generic;
using System.Linq;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services
{
    public class CityInfoRepository : ICityInfoRepository
    {
        private CityInfoContext _context;

        public CityInfoRepository(CityInfoContext context)
        {
            _context = context;
        }

        public IEnumerable<City> GetCities()
        {
            return _context.Cities.OrderBy(c => c.Name).ToList();
        }

        public City GetCity(int cityId, bool includePointsOfInterest)
        {
            if (includePointsOfInterest == true)
            {
                return _context.Cities.Include(c => c.PointsOfInterest).Where(c => c.Id == cityId).FirstOrDefault();
            }
            else
            {
                return _context.Cities.FirstOrDefault(c => c.Id == cityId);
            }
        }

        public PointOfInterest GetPointOfInterestForCity(int cityId, int poiId)
        {
            return _context.PointsOfInterest
                .Where(p => p.City.Id == cityId && p.Id == poiId).FirstOrDefault();
        }

        public IEnumerable<PointOfInterest> GetPointsOfInterestForCity(int cityId)
        {
            return _context.PointsOfInterest
                .Where(p => p.City.Id == cityId).ToList();
        }

        public void AddPointOfInterestForCity(int cityId, PointOfInterest poi)
        {
            var city = GetCity(cityId, false);
            city.PointsOfInterest.Add(poi);
        }

        public void DeletePointOfInterest(PointOfInterest poi)
        {
            _context.PointsOfInterest.Remove(poi);
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}