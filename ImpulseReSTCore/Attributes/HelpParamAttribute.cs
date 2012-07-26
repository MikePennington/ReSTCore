using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImpulseReSTCore.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class HelpParamAttribute : Attribute
    {
        public HelpParamAttribute(string name)
        {
            Name = name;
            Order = int.MaxValue;
        }

        public HelpParamAttribute(string name, string text)
        {
            Name = name;
            Text = text;
            Order = int.MaxValue;
        }

        public HelpParamAttribute(string name, string text, int order)
        {
            Name = name;
            Text = text;
            Order = order;
        }

        public string Name { get; set; }

        public string Text { get; set; }

        public int Order { get; set; }
    }
}
