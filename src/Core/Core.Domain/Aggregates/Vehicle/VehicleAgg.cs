namespace Optimus.Core.Domain.Aggregates.Vehicle
{
    public class VehicleAgg : AggRoot
    {
        public string Plate { get; private set; }
        public string PlateAlternative { get; private set; }
        public string Brand { get; private set; }
        public string Model { get; private set; }
        public string SubModel { get; private set; }
        public string Segment { get; private set; }
        public string Type { get; private set; }
        public int Year { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }
        public string Color { get; private set; }
        public string Fuel { get; private set; }
        public string Situation { get; private set; }
        public string Image { get; private set; }

        private VehicleAgg()
        {

        }

        public static Result<VehicleAgg> Create(
            string plate,
            string plate_alternative,
            string brand,
            string model,
            string subModel,
            string segment,
            string type,
            int year,
            string city,
            string state,
            string color,
            string fuel,
            string situation,
            string image)
        {
            return new VehicleAgg()
            {
                Plate = plate,
                PlateAlternative = plate_alternative,
                Brand = brand,
                Model = model,
                SubModel = subModel,
                Segment = segment,
                Type = type,
                Year = year,
                City = city,
                State = state,
                Color = color,
                Fuel = fuel,
                Situation = situation,
                Image = image
            };
        }
    }
}
