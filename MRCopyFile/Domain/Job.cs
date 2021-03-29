using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRCopyFile.Domain
{
    public class Job
    {
        public string Source { get; set; }

        public string Target { get; set; }

        public int Interval { get; set; }

        public DateTime LastChecked { get; set; } = DateTime.MinValue;

    }
}
