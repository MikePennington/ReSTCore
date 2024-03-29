﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ReSTCore.Attributes;
using ReSTCore.DTO;

namespace TestMvcApp.DTOs
{
    [Help("A DTO just used for testing")]
    public class Thing : RestDTO<int>
    {
        public string Name { get; set; }

        public DateTime? Date { get; set; }

        public override string Path
        {
            get { return "Things"; }
        }
    }
}