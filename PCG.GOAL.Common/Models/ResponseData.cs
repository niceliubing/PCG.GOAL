using System.Collections.Generic;
using System.Threading.Tasks;

namespace PCG.GOAL.Common.Models
{
    public class ResponseData<T>
    {
        public bool Done { get; set; }
        public IList<T> Data { get; set; }
        public string Message { get; set; }
        public string StatusCode { get; set; }
    }
}
