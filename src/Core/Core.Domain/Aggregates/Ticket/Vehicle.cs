
using Optimus.Core.Domain.Aggregates.Vehicle;

namespace Optimus.Core.Domain.Aggregates.Ticket
{
    public class Vehicle : Entity
    {
        public string Plate { get; private set; }
        public string Brand { get; private set; }
        public string Model { get; private set; }
        public string Color { get; private set; }
        public string Fuel { get; private set; }
        public string Situation { get; private set; }
        public string Image { get; private set; }
        public VehicleTypeEnum Type { get; private set; }
        public DateTime LastChangeDate { get; private set; }
        public int Kilometers { get; private set; }


        public static Vehicle Create(VehicleAgg vehicle, int kms = 0)
        {
            return new Vehicle()
            {
                Plate = vehicle.Plate,
                Brand = vehicle.Brand,
                Model = vehicle.Model,
                Color = vehicle.Color,
                Fuel = vehicle.Fuel,
                Situation = vehicle.Situation,
                Image = vehicle.Image,
                Kilometers = kms,
                LastChangeDate = DateTime.Now,
                Type = VehicleTypeEnum.Truck //TODO: Revisar como mapenar os tipos para o enum
            };
        }
        public static Vehicle Create(int kms)
        {
            return new Vehicle()
            {
                Kilometers = kms
            };
        }

        internal void ChangeKilometers(int kms)
        {
            this.Kilometers = kms;
        }

    }
}
