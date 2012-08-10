using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ReSTCore.Attributes;

namespace TestMvcApp.DTOs
{
    [HelpErrorCodes]
    public enum ErrorCodes
    {
        TestErrorCode = 1,
        AnotherErrorCode = 2,
        LastErrorCode = 3
    }
}