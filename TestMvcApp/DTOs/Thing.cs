using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ReSTCore.DTO;

namespace TestMvcApp.DTOs
{
    public class Thing : RestEntity<int>
    {
        public string Name { get; set; }

        public string Nameq { get; set; }
        public string Namea{ get; set; }
        public string Namez { get; set; }
        public string Namew { get; set; }
        public string Names { get; set; }
        public string Namex { get; set; }
        public string Namee { get; set; }
        public string Named { get; set; }
        public string Namec { get; set; }
        public string Namer { get; set; }
        public string Namef { get; set; }
        public string Namev { get; set; }
        public string Namet  { get; set; }
        public string Nameb { get; set; }

        public string Nameqq { get; set; }
        public string Nameaa { get; set; }
        public string Namezz { get; set; }
        public string Nameww { get; set; }
        public string Namess { get; set; }
        public string Namexx { get; set; }
        public string Nameee { get; set; }
        public string Namedd { get; set; }
        public string Namecc { get; set; }
        public string Namerr { get; set; }
        public string Nameff { get; set; }
        public string Namevv { get; set; }
        public string Namett { get; set; }
        public string Namebg { get; set; }


        public override string Path
        {
            get { return "Things"; }
        }
    }
}