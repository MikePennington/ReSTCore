using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReSTCore
{
    public class Configuration
    {
        public Configuration()
        {
            HideRealException = false;
            DefaultExceptionText = "An unknown exception has occurred.";
        }

        public string ServiceName { get; set; }
        
        public Uri ServiceBaseUri { get; set; }

        public bool HideRealException { get; set; }

        public string DefaultExceptionText { get; set; }
    }
}
