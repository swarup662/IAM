using IAM_API.Interface;
using IAM_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IAM_API.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class PasswordController : ControllerBase
    {
        private readonly I_Password _Password;
        public PasswordController(I_Password password)
        {
          _Password = password;     
        }
        [HttpPost]
        [Route("GetTimeFromDB")]
        public List<object> GetTimeFromDB(EmailTimeLimit m)
        {

            List<object> dt = _Password.FetchEmailDtls(Convert.ToInt32(m.EmailSetUpDtls_Key), m.TenantID);
            return dt;

        }
        [HttpPost]
        [Route("ChangePasswordAPI")]
        public int ChangePasswordAPI(ChangePass m)
        {
            return _Password.UserPasswordReset(m);

        }
    }
}
