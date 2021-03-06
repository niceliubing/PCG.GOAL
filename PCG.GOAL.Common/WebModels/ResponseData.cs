﻿using System.Collections.Generic;

namespace PCG.GOAL.Common.WebModels
{
    public class ResponseData<T>
    {
        public bool Done { get; set; }
        public IList<T> Data { get; set; }
        public string Message { get; set; }
        public string StatusCode { get; set; }
    }
}
