using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guilder.Shared
{
    public class Meeting
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Instant StartTimeInclusive { get; set; } = Instant.MinValue;
        public Instant EndTimeExclusive { get; set; } = Instant.MaxValue;
    }
}
