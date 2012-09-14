using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReSTCore.DTO;
using System.ComponentModel.DataAnnotations;

namespace ReSTCore.Test.Fixtures
{
    public class TestDTO : RestDTO<int>
    {
        [StringLength(20)]
        public string Name { get; set; }

        public override string Path
        {
            get { return "Test"; }
        }
    }
}
