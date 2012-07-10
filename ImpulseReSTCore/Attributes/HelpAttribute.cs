using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImpulseReSTCore.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class HelpAttribute : Attribute
    {
        public HelpAttribute()
        {
        }

        public HelpAttribute(string description)
        {
            Description = description;
        }

        public string Description { get; set; }

        public bool Ignore { get; set; }
    }
}
