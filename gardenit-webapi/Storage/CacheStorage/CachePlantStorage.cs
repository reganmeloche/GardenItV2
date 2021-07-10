using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.Caching.Memory;

using gardenit_webapi.Lib;

namespace gardenit_webapi.Storage.CacheStorage
{
    public class CachePlantStorage : IStorePlants
    {
        private readonly static string _cacheKey = "plants_all";
        private readonly IMemoryCache _cache;

        public CachePlantStorage(IMemoryCache cache) {
            _cache = cache;
            var curr = GetAllPlants();
            if (curr == null) {
                _cache.Set(_cacheKey, new List<Plant>());
            }
        }

        public void CreatePlant(Plant plant) {
            List<Plant> currList = (List<Plant>)_cache.Get(_cacheKey);
            currList.Add(plant);
            _cache.Set(_cacheKey, currList);
        }

        public List<Plant> GetAllPlants() {
            return (List<Plant>)_cache.Get(_cacheKey);
        }

        public Plant GetPlant(Guid id) {
            var allPlants = GetAllPlants();
            return allPlants.First(x => x.Id == id);
        }

        public void DeletePlant(Guid id) {
            var plantToRemove = GetPlant(id);
            var allPlants = GetAllPlants();
            allPlants.Remove(plantToRemove);
            _cache.Set(_cacheKey, allPlants);
        }

        public void UpdatePlant(Plant updatedPlant) {
            var plant = GetPlant(updatedPlant.Id);
            plant.Name = updatedPlant.Name;
            plant.Notes = updatedPlant.Notes;
            plant.Type = updatedPlant.Type;
            plant.Waterings = updatedPlant.Waterings;
            DeletePlant(updatedPlant.Id);
            CreatePlant(plant);
        }


        public void AddWatering(Guid id) {
            var plant = GetPlant(id);
            plant.Waterings.Add(new Watering() {
                WateringDate = DateTime.Now
            });
            UpdatePlant(plant);
        }
    }
}