using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReSTCore.Util
{
    public static class Constants
    {
        public static class Actions
        {
            public const string Index = "Index";
            public const string Show = "Show";
            public const string ShowProperty = "ShowProperty";
            public const string Create = "Create";
            public const string Update = "Update";
            public const string UpdateProperty = "UpdateProperty";
            public const string Delete = "Delete";
            public const string Help = "Help";
        }

        public static class Headers
        {
            public const string ErrorCode = "X-ServiceErrorCode";
            public const string ErrorMessage = "X-ServiceErrorMessage";
        }
    }
}
