namespace ApiCities.Models
{
    public class City
    {
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Region { get; set; }
        public int Population { get; set; }

        public double ToRadians(double degree)
        {
            return degree * Math.PI / 180;
        }

    }
}
