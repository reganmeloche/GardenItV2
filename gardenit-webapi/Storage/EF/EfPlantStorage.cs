using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

using Lib = gardenit_webapi.Lib;

namespace gardenit_webapi.Storage.EF
{
    public class EfPlantStorage : IStorePlants
    {
        private readonly ApplicationDbContext _context;

        public EfPlantStorage(ApplicationDbContext context) {
            _context = context;
        }

        public void CreatePlant(Lib.Plant plant) {
            var dbModel = Convert(plant);
            _context.Plants.Add(dbModel);
            _context.SaveChanges();
        }

        public List<Lib.Plant> GetAllPlants() {
            return _context.Plants
                .AsNoTracking()
                .Include(x => x.Waterings)
                .Select(Convert)
                .ToList();
        }

        public Lib.Plant GetPlant(Guid id) {
            var dbResult = GetDbPlant(id);
            return Convert(dbResult);
        }

        public void DeletePlant(Guid id) {
            var plantToRemove = GetDbPlant(id);
            _context.Remove(plantToRemove);
        }

        public void UpdatePlant(Lib.Plant updatedPlant) {
            var plantDb = GetDbPlant(updatedPlant.Id);
            plantDb.Name = updatedPlant.Name;
            plantDb.Notes = updatedPlant.Notes;
            plantDb.Type = updatedPlant.Type;
            _context.Update(plantDb);
            _context.SaveChanges();
        }

        public void AddWatering(Guid id) {
            var plantDb = _context.Plants
                .Include(x => x.Waterings)
                .First(x => x.Id == id);
            
            plantDb.Waterings.Add(new Watering(){
                PlantId = id,
                WateringDate = DateTime.Now
            });
            
            _context.SaveChanges();
        }

        private Plant GetDbPlant(Guid id) {
            return _context.Plants
                .AsNoTracking()
                .Include(x => x.Waterings)
                .First(x => x.Id == id);
        }

        // Converters
        private static Plant Convert(Lib.Plant plant) {
            return new Plant() {
                Id = plant.Id,
                Name = plant.Name,
                CreateDate = plant.CreateDate,
                Notes = plant.Notes,
                Type = plant.Type,
                Waterings = plant.Waterings.Select(x => Convert(x, plant.Id)).ToList()
            };
        }

        private static Watering Convert(Lib.Watering watering, Guid plantId) {
            return new Watering() {
                Id = Guid.NewGuid(),
                PlantId = plantId,
                WateringDate = watering.WateringDate
            };
        }

        private static Lib.Plant Convert(Plant dbModel) {
            return new Lib.Plant() {
                Id = dbModel.Id,
                Name = dbModel.Name,
                CreateDate = dbModel.CreateDate,
                Notes = dbModel.Notes,
                Type = dbModel.Type,
                Waterings = dbModel.Waterings.Select(Convert).ToList()
            };
        }

        private static Lib.Watering Convert(Watering watering) {
            return new Lib.Watering() {
                WateringDate = watering.WateringDate
            };
        }
    }
}