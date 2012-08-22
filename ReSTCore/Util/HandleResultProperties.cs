using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReSTCore.Util
{
    public class HandleResultProperties
    {
        public HandleResultProperties()
        {
            NeedsMapping = false;
            IncludeBodyInNonGetRequest = false;
        }

        public bool NeedsMapping { get; set; }

        public bool IncludeBodyInNonGetRequest { get; set; }
    }
}
