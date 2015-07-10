using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCG.GOAL.Common.Models;
using PCG.GOAL.Common.Security;

namespace PCG.GOAL.ExternalDataService.Interface
{
    public interface IGoalService
    {
        Credentials Credentials { get; set; }

        Task<ResponseData<ChildInfo>> GetAllChildrenAsync();
        Task<ResponseData<ChildInfo>> GetAllChildByStateNumberAsync(string stateTestNumber);
        Task<ResponseData<ChildInfo>> GetAllChildByIdentityAsync(string firstName, string lastName, string dob);
        IEnumerable<ChildInfo>  GetAllChildren();
    }
}
