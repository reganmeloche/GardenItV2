using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.EntityFrameworkCore;

namespace gardenit_webapi.Storage.EF
{
    public class EfPlantStorage : IStorePlants
    {
        private readonly ApplicationDbContext _context;

        public EfPlantStorage(ApplicationDbContext context) {
            _context = context;
        }

        public void CreatePlant(Lib.Plant plant, Guid userId) {
            var dbModel = Convert(plant, userId);
            _context.Plants.Add(dbModel);
            _context.SaveChanges();
        }

        public List<Lib.Plant> GetAllPlants(Guid userId) {
            return _context.Plants
                .AsNoTracking()
                .Include(x => x.Waterings)
                .Include(x => x.MoistureReadings)
                .Where(x => x.IsDeleted == false)
                .Where(x => x.UserId == userId)
                .Select(Convert)
                .ToList();
        }

        public List<Lib.Plant> GetAllPlants() {
            return _context.Plants
                .AsNoTracking()
                .Include(x => x.Waterings)
                .Include(x => x.MoistureReadings)
                .Where(x => x.IsDeleted == false)
                .Select(Convert)
                .ToList();
        }

        public Lib.Plant GetPlant(Guid id, Guid userId) {
            var dbResult = GetDbPlant(id, userId);
            return Convert(dbResult);
        }

        public void DeletePlant(Guid id, Guid userId) {
            var plantToRemove = GetDbPlant(id, userId);
            plantToRemove.IsDeleted = true;
            _context.Update(plantToRemove);
            _context.SaveChanges();
        }

        public void UpdatePlant(Lib.Plant updatedPlant, Guid userId) {
            var plantDb = GetDbPlant(updatedPlant.Id, userId);
            plantDb.Name = updatedPlant.Name;
            plantDb.Notes = updatedPlant.Notes;
            plantDb.Type = updatedPlant.Type;
            plantDb.DaysBetweenWatering = updatedPlant.DaysBetweenWatering;
            plantDb.ImageName = updatedPlant.ImageName;
            plantDb.HasDevice = updatedPlant.HasDevice;
            plantDb.PollPeriodMinutes = updatedPlant.PollPeriodMinutes;
            
            _context.Update(plantDb);

            _context.SaveChanges();
        }

        public void AddWatering(Guid plantId, Lib.Watering watering, Guid userId) {
            var plantDb = _context.Plants
                .Include(x => x.Waterings)
                .First(x => x.Id == plantId && x.IsDeleted == false && x.UserId == userId);

            var dbWatering = Convert(watering, plantId);
            
            plantDb.Waterings.Add(dbWatering);
            
            _context.SaveChanges();
        }

        public void AddMoistureReading(Guid plantId, Lib.MoistureReading moistureReading) {            
            var plantDb = _context.Plants
                .Include(x => x.MoistureReadings)
                .First(x => x.Id == plantId && x.IsDeleted == false);

            var dbReading = Convert(moistureReading, plantId);

            plantDb.MoistureReadings.Add(dbReading);
            
            _context.SaveChanges();
        }

        private Plant GetDbPlant(Guid id, Guid userId) {
            return _context.Plants
                .AsNoTracking()
                .Include(x => x.Waterings)
                .Include(x => x.MoistureReadings)
                .First(x => x.Id == id && x.IsDeleted == false && x.UserId == userId);
        }

        /*** CONVERT LIB -> DB ***/
        private static Plant Convert(Lib.Plant plant, Guid userId) {
            return new Plant() {
                Id = plant.Id,
                Name = plant.Name,
                CreateDate = plant.CreateDate,
                Notes = plant.Notes,
                Type = plant.Type,
                ImageName = plant.ImageName,
                DaysBetweenWatering = plant.DaysBetweenWatering,
                HasDevice = plant.HasDevice,
                PollPeriodMinutes = plant.PollPeriodMinutes,
                Waterings = plant.Waterings.Select(x => Convert(x, plant.Id)).ToList(),
                MoistureReadings = plant.MoistureReadings.Select(x => Convert(x, plant.Id)).ToList(),
                IsDeleted = false,
                UserId = userId
            };
        }

        private static MoistureReading Convert(Lib.MoistureReading moistureReading, Guid plantId) {
            return new MoistureReading() {
                PlantId = plantId,
                ReadDate = moistureReading.ReadDate,
                Value = moistureReading.Value,
            };
        }

        private static Watering Convert(Lib.Watering watering, Guid plantId) {
            return new Watering() {
                PlantId = plantId,
                WateringDate = watering.WateringDate,
                Seconds = watering.Seconds
            };
        }

        /*** CONVERT DB -> LIB ***/
        private static Lib.Plant Convert(Plant dbModel) {
            return new Lib.Plant() {
                Id = dbModel.Id,
                Name = dbModel.Name,
                CreateDate = dbModel.CreateDate,
                Notes = dbModel.Notes,
                Type = dbModel.Type,
                ImageName = dbModel.ImageName,
                HasDevice = dbModel.HasDevice,
                PollPeriodMinutes = dbModel.PollPeriodMinutes,
                DaysBetweenWatering = dbModel.DaysBetweenWatering,
                Waterings = dbModel.Waterings.Select(Convert).ToList(),
                MoistureReadings = dbModel.MoistureReadings.Select(Convert).ToList()
            };
        }

        private static Lib.Watering Convert(Watering watering) {
            return new Lib.Watering() {
                WateringDate = watering.WateringDate,
                Seconds = watering.Seconds
            };
        }

        private static Lib.MoistureReading Convert(MoistureReading readingDb) {
            return new Lib.MoistureReading() {
                ReadDate = readingDb.ReadDate,
                Value = readingDb.Value
            };
        }
    }
}