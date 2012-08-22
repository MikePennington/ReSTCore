using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReSTCore.DTO;

namespace ReSTCore.Test.Fixtures
{
    public class TestDTO : RestDTO<int>
    {
        public string Name { get; set; }

        public override string Path
        {
            get { return "Test"; }
        }
    }
}
