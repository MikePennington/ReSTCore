using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ReSTCore.DTO;

namespace TestMvcApp.DTOs
{
    public class Thing2 : RestDTO<int>
    {
        public string Name { get; set; }

        public int Number { get; set; }

        public char? Letter { get; set; }

        public DateTime? Date { get; set; }

        public List<string> List { get; set; }

        public int[] Array { get; set; }

        public Thing Object { get; set; }

        public List<Thing> ObjectList { get; set; }

        public Thing[] ObjectArray { get; set; }

        public override string Path
        {
            get { return "Things"; }
        }
    }
}