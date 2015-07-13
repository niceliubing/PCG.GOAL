using System.Collections.Generic;
using System.Threading.Tasks;
using PCG.GOAL.Common.Models;
using PCG.GOAL.Common.WebAccess;

namespace PCG.GOAL.ExternalDataService.Interface
{
    public interface IGoalService
    {
        Credentials Credentials { get; set; }
        IServiceConfig ServiceConfig { get; set; }

        Task<ResponseData<ChildInfo>> GetAllChildrenAsync();
        Task<ResponseData<ChildInfo>> GetAllChildByStateNumberAsync(string stateTestNumber);
        Task<ResponseData<ChildInfo>> GetAllChildByIdentityAsync(string firstName, string lastName, string dob);
    }
}
