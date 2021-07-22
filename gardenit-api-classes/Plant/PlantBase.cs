using System; 

namespace gardenit_api_classes.Plant
{
    public class PlantBase
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Notes { get; set; }
        public int DaysBetweenWatering { get; set; }
        public string ImageName { get; set; }
        public DateTime CreateDate { get; set; }
    }
}