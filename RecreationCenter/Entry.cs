using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecreationCenter
{
    public class Entry
    {
        public DateTime EntryDate { get; set; }
        public int ID { get; set; }

        public String Name { get; set; }

        public int Adult { get; set; }

        public int Child { get; set; }

        public int VisitorsNum { get; set; }

        
        public DateTime EntryTime { get; set; }

        public DateTime ExitTime { get; set; }

        public int TotalHours { get; set; }

        public int TotalAmount { get; set; }

        public Boolean Exit { get; set; }
    }
}
