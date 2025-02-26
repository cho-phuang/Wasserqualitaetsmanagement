using System;
using System.Collections.Generic;

namespace Projekt_Schuler
{
    public partial class User
    {
        public User()
        {
            Pools = new HashSet<Pool>();
            WaterQualityData = new HashSet<WaterQualityDatum>();
        }

        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public DateTime? LastLogin { get; set; }

        public virtual ICollection<Pool> Pools { get; set; }
        public virtual ICollection<WaterQualityDatum> WaterQualityData { get; set; }
    }
}
