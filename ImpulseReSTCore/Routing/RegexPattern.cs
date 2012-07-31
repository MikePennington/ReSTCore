using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImpulseReSTCore.Routing
{
    public static class RegexPattern
    {
        /// <summary>Matches anything.</summary>
        public const string MatchAny = ".+";

        /// <summary>Matches a base64 encoded <see cref="Guid"/></summary>
        public const string MatchBase64Guid = @"[a-zA-Z0-9+/=]{22,24}";

        /// <summary>Matches a <see cref="Guid"/><c>@"\{?[a-fA-F0-9]{8}(?:-(?:[a-fA-F0-9]){4}){3}-[a-fA-F0-9]{12}\}?"</c></summary>
        public const string MatchGuid = @"\{?[a-fA-F0-9]{8}(?:-(?:[a-fA-F0-9]){4}){3}-[a-fA-F0-9]{12}\}?";

        /// <summary>Matches a Positive <see cref="int"/> <c>@"\d{1,10}"</c></summary>
        public const string MatchPositiveInteger = @"\d{1,10}";

        /// <summary>Matches a Positive <see cref="long"/> <c>@"\d{1,19}"</c></summary>
        public const string MatchPositiveLong = @"\d{1,19}";
    }
}
