using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentACarSystem.Data.Entity
{
    public class Query : BaseEntity
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int CarId { get; set; }
        public virtual Car Car { get; set; }

        public string CreatorId { get; set; }
        public virtual AppUser Creator { get; set; }

        public double PriceForThePeriod { get; set; }

        public Status StatusOfQuery { get; set; }
    }

    public enum Status
    {
        Waiting,
        Canceled,
        Active,
        Used,
        Expired
    }
}
