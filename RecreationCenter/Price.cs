using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecreationCenter
{
    public class Price
    {
        public int Persons { get; set; }

        public int Hours { get; set; }

        public int ChildPriceOnWeekdays { get; set; }

        public int AdultPriceOnWeekdays { get; set; }

        public int ChildPriceOnWeekends { get; set; }

        public int AdultPriceOnWeekends { get; set; }
    }
}
