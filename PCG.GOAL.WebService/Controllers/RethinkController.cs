﻿using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Http;
using PCG.GOAL.Common.Models;
using PCG.GOAL.Common.WebModels;
using PCG.GOAL.ExternalDataService.Interface;
using PCG.GOAL.WebService.Security;

namespace PCG.GOAL.WebService.Controllers
{
    [ApiAuthorize]
    public class RethinkController : ApiController
    {
        private readonly IGoalService _rethinkService;

        public RethinkController(IGoalService rethinkService)
        {
            _rethinkService = rethinkService;
        }

        [Route("api/rethink/student")]
        [HttpGet]
        public async Task<ResponseData<ChildInfo>> Get()
        {
            return await _rethinkService.GetAllStudentsAsync();
        }

        [Route("api/rethink/StudentByStatetestnumber")]
        [HttpGet]
        public async Task<ResponseData<ChildInfo>> GetByStateTestNumber(string statetestnumber)
        {
            return await _rethinkService.GetStudentByStateNumberAsync(statetestnumber);
        }

        [Route("api/rethink/StudentByIdentity")]
        [HttpGet]
        public async Task<ResponseData<ChildInfo>> GetByIdentity(string firstName, string lastName, string dob)
        {
            return await _rethinkService.GetStudentByIdentityAsync(firstName, lastName, dob);
        }



    }
}
