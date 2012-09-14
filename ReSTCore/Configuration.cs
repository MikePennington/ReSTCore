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

        /// <summary>
        /// This is the error code that will be returned in the X-ServiceErrorCode header if the validation
        /// via data annotations fail.
        /// </summary>
        public int? InvalidUserIntputErrorCode { get; set; }

        /// <summary>
        /// This is the error code that will be returned in the X-ServiceErrorCode header if an exception
        /// falls through to the OnException handler.
        /// </summary>
        public int? UnknownServerErrorCode { get; set; }
    }
}
