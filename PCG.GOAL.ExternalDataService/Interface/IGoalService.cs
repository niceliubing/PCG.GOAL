using System.Collections.Generic;
using System.Threading.Tasks;
using PCG.GOAL.Common.Models;
using PCG.GOAL.Common.WebModels;

namespace PCG.GOAL.ExternalDataService.Interface
{
    public interface IGoalService
    {
        Credentials Credentials { get; set; }
        IServiceConfig ServiceConfig { get; set; }

        Task<ResponseData<ChildInfo>> GetAllStudentsAsync();
        Task<ResponseData<ChildInfo>> GetStudentByStateNumberAsync(string stateTestNumber);
        Task<ResponseData<ChildInfo>> GetStudentByIdentityAsync(string firstName, string lastName, string dob);
    }
}
