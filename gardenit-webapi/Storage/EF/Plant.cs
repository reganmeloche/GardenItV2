using System;
using System.Collections.Generic;

namespace gardenit_webapi.Storage.EF
{
    public class Plant
    {
        public Guid Id { get; set; }
        public DateTime CreateDate { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Notes { get; set; }
        public List<Watering> Waterings { get; set; }
        public List<MoistureReading> MoistureReadings { get; set; }
        public int DaysBetweenWatering { get; set; }
        public bool IsDeleted { get; set; }
        public string ImageName { get; set; }
        public int PollPeriodMinutes { get; set; }
        public bool HasDevice { get; set; }
        public Guid UserId { get; set; }
    }

    public class Watering {
        public Guid Id { get; set; }
        public DateTime WateringDate { get; set; }
        public int Seconds { get; set; }
        public Guid PlantId { get; set; }
        public Plant Plant { get; set; }
    }

    public class MoistureReading {
        public Guid Id { get; set; }
        public Guid PlantId { get; set; }
        public Plant Plant { get; set; }
        public DateTime ReadDate { get; set; }
        public double Value { get; set; }
    }
}