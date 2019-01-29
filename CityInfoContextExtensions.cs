using System.Collections.Generic;
using System.Linq;
using CityInfo.API.Entities;

namespace CityInfo.API
{
    public static class CityInfoContextExtensions
    {
        public static void EnsureSeedDataForContext(this CityInfoContext context)
        {
            // Data exists, nothing to do
            if (context.Cities.Count() != 0)
            {
                return;
            }

            var cities = new List<City>()
            {
                new City()
                {
                    Name = "New York City",
                    Description = "The city that never sleeps",
                    PointsOfInterest = new List<PointOfInterest>()
                    {
                        new PointOfInterest()
                        {
                            Name = "Central Park",
                            Description = "Big green park"
                        },
                        new PointOfInterest()
                        {
                            Name = "Empire State Building",
                            Description = "Rockerfeller Center is better"
                        }
                    }
                },
                new City()
                {
                    Name = "Paris",
                    Description = "The city of lights",
                    PointsOfInterest = new List<PointOfInterest>()
                    {
                        new PointOfInterest()
                        {
                            Name = "Eiffel Tower",
                            Description = "The big tower"
                        },
                        new PointOfInterest()
                        {
                            Name = "Arc de Triumphe",
                            Description = "Napoleon's favourite"
                        }
                    }
                }
            };

            context.Cities.AddRange(cities);
            context.SaveChanges();
        }
    }    
}