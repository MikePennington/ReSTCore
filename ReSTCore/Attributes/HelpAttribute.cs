using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReSTCore.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class HelpAttribute : Attribute
    {
        public HelpAttribute()
        {
            Order = int.MaxValue;
        }

        public HelpAttribute(string text)
        {
            Text = text;
            Order = int.MaxValue;
        }

        public HelpAttribute(string text, int order)
        {
            Text = text;
            Order = order;
        }

        public string Text { get; set; }

        public bool Ignore { get; set; }

        public int Order { get; set; }
    }
}
