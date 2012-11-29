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

        public int Integer { get; set; }

        public char? Char { get; set; }

        public short Short { get; set; }

        public float Float { get; set; }

        public double Double { get; set; }

        public decimal Decimal { get; set; }

        public DateTime? NullableDate { get; set; }

        public List<string> ListOfStrings { get; set; }

        public int[] ArrayOfIntegers { get; set; }

        public Thing Object { get; set; }

        public List<Thing> ObjectList { get; set; }

        public Thing[] ObjectArray { get; set; }

        public override string Path
        {
            get { return "Things"; }
        }
    }
}