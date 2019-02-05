using System.Collections.Generic;
using System.Linq;
using CityInfo.API.Entities;

namespace CityInfo.API.Services
{
    public interface ICityInfoRepository
    {
         IEnumerable<City> GetCities();

         City GetCity(int cityId, bool includePointsOfInterest);

         PointOfInterest GetPointOfInterestForCity(int cityId, int poiId);

         IEnumerable<PointOfInterest> GetPointsOfInterestForCity(int poiId);

         void AddPointOfInterestForCity(int cityId, PointOfInterest poi);

         bool Save();
    }
}