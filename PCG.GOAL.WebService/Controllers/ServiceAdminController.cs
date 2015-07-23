using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Helpers;
using System.Web.Http;
using PCG.GOAL.Common.Interface;
using PCG.GOAL.Common.WebModels;
using PCG.GOAL.WebService.Security;

namespace PCG.GOAL.WebService.Controllers
{
    [ApiAuthorize(Roles = "Admin")]
    public class ServiceAdminController : ApiController
    {
        private readonly IDbService _dbService;

        public ServiceAdminController(IDbService dbService)
        {
            _dbService = dbService;
        }

        #region Credentials

        [Route("api/admin/credentials")]
        public IHttpActionResult GetCredentials()
        {
            try
            {
                var users = _dbService.GetAllCredentials();
                return Ok(new ResponseData<Credentials> { Data = users.ToList(), Done = true });

            }
            catch (Exception e)
            {
                return Ok(new ResponseData<Credentials> { Data = null, Done = false, Message = e.Message });
            }
        }

        [Route("api/admin/credentials/{id}")]
        [HttpGet]
        public IHttpActionResult GetCredentialsById(int id)
        {
            try
            {
                var user = _dbService.GetCredentialsById(id);
                if (user != null)
                {
                    return Ok(new ResponseData<Credentials> { Data = new List<Credentials> { user }, Done = true });
                }
                else
                {
                    return Ok(new ResponseData<Credentials> { Data = null, Done = false });
                }

            }
            catch (Exception e)
            {
                return Ok(new ResponseData<Credentials> { Data = null, Done = false, Message = e.Message });
            }

        }
        [Route("api/admin/DeleteCredentials/{id}")]
        [HttpGet]
        public IHttpActionResult DeleteCredentialsById(int id)
        {
            try
            {
                var user = _dbService.GetCredentialsById(id);
                if (user.Username.ToLower() == "admin")
                {
                    return Ok(new ResponseData<Credentials> { Data = null, Done = false, Message = "Cannot delete Admin!" });
                }
                _dbService.DeleteCredentials(id);
                return Ok(new ResponseData<Credentials> { Data = null, Done = true });
            }
            catch (Exception e)
            {
                return Ok(new ResponseData<Credentials> { Data = null, Done = false, Message = e.Message });
            }
        }
        [HttpPost]
        [Route("api/admin/AddCredentials")]
        public IHttpActionResult SaveCredentials(Credentials credentials)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var isInsert = credentials.Id == 0;
                    credentials.Password = Crypto.HashPassword(credentials.Password);
                    if (isInsert)
                    {
                        _dbService.AddCredentials(credentials);
                    }
                    else
                    {
                        _dbService.UpdateCredentials(credentials);
                    }
                    return Ok(new ResponseData<Credentials> { Data = new List<Credentials> { credentials }, Done = true });

                }
                catch (Exception ex)
                {
                    return Ok(new ResponseData<Credentials> { Data = null, Done = false, Message = ex.Message });

                }
            }
            return Ok(new ResponseData<Credentials> { Data = null, Done = false, Message = "Model invalid" });
        }

        #endregion


        #region ApiAppRegistrations

        [Route("api/admin/apps")]
        public IHttpActionResult GetApps()
        {
            try
            {
                var apps = _dbService.GetAllClientInfo();
                return Ok(new ResponseData<ClientInfo> { Data = apps.ToList(), Done = true });

            }
            catch (Exception e)
            {
                return Ok(new ResponseData<ClientInfo> { Data = null, Done = false, Message = e.Message });
            }

        }
        [HttpPost]
        [Route("api/admin/addapp")]
        public IHttpActionResult SaveClient(ClientInfo clientInfo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var isInsert = clientInfo.Id == 0;
                    clientInfo.ClientSecret = Crypto.HashPassword(clientInfo.ClientSecret);
                    if (isInsert)
                    {
                        _dbService.AddClientInfo(clientInfo);
                    }
                    else
                    {
                        _dbService.UpdateClientInfo(clientInfo);
                    }
                    return Ok(new ResponseData<ClientInfo> { Data = new List<ClientInfo> { clientInfo }, Done = true });

                }
                catch (Exception ex)
                {
                    return Ok(new ResponseData<ClientInfo> { Data = null, Done = false, Message = ex.Message });

                }
            }
            return Ok(new ResponseData<ClientInfo> { Data = null, Done = false, Message = "Model invalid" });


        }

        [Route("api/admin/deleteapp/{id}")]
        [HttpGet]
        public IHttpActionResult DeleteAppById(int id)
        {
            try
            {
                //var client = _dbService.GetClientInfoById(id);
                _dbService.DeleteClientInfo(id);
                return Ok(new ResponseData<ClientInfo> { Data = null, Done = true });
            }
            catch (Exception e)
            {
                return Ok(new ResponseData<ClientInfo> { Data = null, Done = false, Message = e.Message });
            }
        }
        [Route("api/admin/app/{id}")]
        [HttpGet]
        public IHttpActionResult GetAppById(int id)
        {
            try
            {
                var client = _dbService.GetClientInfoById(id);
                if (client != null)
                {
                    return Ok(new ResponseData<ClientInfo> { Data = new List<ClientInfo> { client }, Done = true });
                }
                return Ok(new ResponseData<ClientInfo> { Data = null, Done = false });
            }
            catch (Exception e)
            {
                return Ok(new ResponseData<ClientInfo> { Data = null, Done = false, Message = e.Message });
            }
        }

        #endregion

    }
}
