using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Angelo.Ldap;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Angelo.Ldap.Controllers
{
    public class LdapController : ApiController
    {

        [HttpPost]
        [Route("api/{controler}/AuthenticateUser")]
        public async Task<IHttpActionResult> AuthenticateUser()
        {
            try
            {
                string data = await Request.Content.ReadAsStringAsync();
                var query = JsonConvert.DeserializeObject<LdapUserQuery>(data);
                var ldap = new LdapHelper(query.Server);
                bool result = ldap.AuthenticateUser(query.LoginName, query.Password);
                return Ok(result);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("api/{controler}/GetUserByLogin")]
        public async Task<IHttpActionResult> GetUserByLogin()
        {
            try
            {
                string data = await Request.Content.ReadAsStringAsync();
                var query = JsonConvert.DeserializeObject<LdapUserQuery>(data);
                var ldap = new LdapHelper(query.Server);
                var user = ldap.GetUserByLogin(query.LoginName);
                if (user == null)
                    throw new System.Exception($"Unable to find user: {query.LoginName}");
                return Ok(user);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("api/{controler}/GetAllGroups")]
        public async Task<IHttpActionResult> GetAllGroups()
        {
            try
            {
                string data = await Request.Content.ReadAsStringAsync();
                var query = JsonConvert.DeserializeObject<LdapUserQuery>(data);
                var ldap = new LdapHelper(query.Server);
                var groups = ldap.GetAllGroups();
                return Ok(groups);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("api/{controler}/Test")]
        public IHttpActionResult TestGet()
        {
            string result = "You called the Test Get web method.";
            return Ok(result);
        }

        [HttpPost]
        [Route("api/{controler}/Test")]
        public IHttpActionResult TestPost()
        {
            string result = "You called the Test Post web method.";
            return Ok(result);
        }

    }
}
