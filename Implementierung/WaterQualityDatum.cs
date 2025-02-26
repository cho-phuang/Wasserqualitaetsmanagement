using System;
using System.Collections.Generic;

namespace Projekt_Schuler
{
    public partial class WaterQualityDatum
    {
        public int DataId { get; set; }
        public int? UserId { get; set; }
        public int? PoolId { get; set; }
        public decimal? Phvalue { get; set; }
        public decimal? Temperature { get; set; }
        public decimal? ChlorineLevel { get; set; }
        public decimal? Turbidity { get; set; }
        public DateTime? EntryDate { get; set; }

        public virtual Pool? Pool { get; set; }
        public virtual User? User { get; set; }
    }
}
