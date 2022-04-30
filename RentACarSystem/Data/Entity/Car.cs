using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentACarSystem.Data.Entity
{
    public class Car : BaseEntity
    {
        public Car()
        {
            CarQueries = new List<Query>();
        }
        public string Brand { get; set; }

        public string Model { get; set; }

        public int YearOfProduction { get; set; }

        public int Seats { get; set; }

        public string Description { get; set; }

        public double PricePerDay { get; set; }

        public string Image { get; set; }

        public virtual List<Query> CarQueries { get; set; }
    }
}
