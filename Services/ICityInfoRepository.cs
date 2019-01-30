using System.Collections.Generic;
using System.Linq;
using CityInfo.API.Entities;

namespace CityInfo.API.Services
{
    public interface ICityInfoRepository
    {
         IEnumerable<City> GetCities();

         City GetCity(int cityId, bool includePointsOfInterest);

         IEnumerable<PointOfInterest> GetPointOfInterestForCity(int cityId, int poiId);

         PointOfInterest GetPointsOfInterestForCity(int poiId);
    }
}