using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PCG.GOAL.ExternalDataService.Interface
{
    public interface IContent
    {
        Task<string> ReadAsStringAsync();
        HttpContent HttpContent { get; set; }
    }
}
