using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace ReSTCore.DTO
{
    public class Result<T>
    {
        public T Entity { get; set; }

        public ResultType ResultType { get; set; }

        public int? ErrorCode { get; set; }

        public string ErrorMessage { get; set; }

        public HttpStatusCode? HttpStatusCode { get; set; }
    }
}
