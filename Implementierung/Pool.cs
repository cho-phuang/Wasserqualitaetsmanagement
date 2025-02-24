using System;
using System.Collections.Generic;

namespace Projekt_Schuler
{
    public partial class Pool
    {
        public int PoolId { get; set; }
        public string PoolName { get; set; } = null!;
        public string? Location { get; set; }
        public int? UserId { get; set; }

        public virtual User? User { get; set; }
    }
}
