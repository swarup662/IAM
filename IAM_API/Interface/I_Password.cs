using IAM_API.Models;

namespace IAM_API.Interface
{
    public interface I_Password
    {
        List<object> FetchEmailDtls(int EmailSetUpDtls_Key, int TenantID);
        int UserPasswordReset(ChangePass m);
    }
}
