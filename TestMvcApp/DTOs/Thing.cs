using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ImpulseReSTCore.DTO;

namespace TestMvcApp.DTOs
{
    public class Thing : RestEntity<int>
    {
        public string Name { get; set; }

        public override string Path
        {
            get { return "Things"; }
        }

        public override bool IsSecureDTO
        {
            get { return false; }
        }
    }
}