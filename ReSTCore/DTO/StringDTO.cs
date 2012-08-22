using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReSTCore.DTO
{
    public class StringDTO : RestDTO<string>
    {
        public string Value { get; set; }

        public override string Path
        {
            get { return null; }
        }
    }
}
