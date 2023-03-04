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
        public string Name { get; set; }
        public string Description { get; set; }
        public Instant StartTimeInclusive { get; set; }
        public Instant EndTimeExclusive { get; set; }
    }
}
