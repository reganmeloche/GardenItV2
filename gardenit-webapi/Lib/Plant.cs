using System;
using System.Collections.Generic;

using gardenit_api_classes.Plant;

namespace gardenit_webapi.Lib
{
    public class Plant : PlantBase
    {
        public Guid Id { get; set; }
        public List<Watering> Waterings { get; set; }
        public List<MoistureReading> MoistureReadings { get; set; }

        public Plant(NewPlantRequest req) {
            Id = Guid.NewGuid();
            CreateDate = DateTime.Now;
            Notes = req.Notes;
            Name = req.Name;
            Type = req.Type;
            DaysBetweenWatering = req.DaysBetweenWatering;
            ImageName = req.ImageName;
            HasDevice = req.HasDevice;
            PollPeriodMinutes = req.PollPeriodMinutes;
            Waterings = new List<Watering>();
            MoistureReadings = new List<MoistureReading>();
        }

        public Plant() {
        }
    }

    public class Watering {
        public DateTime WateringDate { get; set; }
        public int Seconds { get; set; }
    }

    public class MoistureReading {
        public DateTime ReadDate { get; set; }
        public double Value { get; set; }
    }


}