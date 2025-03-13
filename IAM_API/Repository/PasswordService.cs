using IAM_API.Interface;
using IAM_API.Models;
using IAM_API.Utility;
using System.Data;

namespace IAM_API.Repository
{
    //public class PasswordService:I_Password
    //{
    //    IConfiguration _configuration;
    //    DbAccess _DbAccess;
    //    public PasswordService(IConfiguration configuration)
    //    {
    //        _configuration = configuration;
    //        _DbAccess = new DbAccess(_configuration);
    //    }
    //    //public List<object> FetchEmailDtls(int EmailSetUpDtls_Key, int TenantID)
    //    //{
    //    //    //int EMP_key;
    //    //    try
    //    //    {
    //    //        string[] parameterNames = { "@TenantID", "@EmailSetUpDtls_Key" };
    //    //        string[] parameterValues = { TenantID.ToString(), EmailSetUpDtls_Key.ToString() };
    //    //        DataSet dataSet = _DbAccess.Ds_Process("[SP_GET_TenantMasterEmailDtls]", parameterNames, parameterValues);
    //    //        List<object> types = new List<object>();



    //    //        if (dataSet.Tables.Count > 0)
    //    //        {
    //    //            foreach (DataRow row in dataSet.Tables[0].Rows)
    //    //            {
    //    //                types.Add(
    //    //                   new
    //    //                   {
    //    //                       Mailsubject = row["Mailsubject"].ToString(),
    //    //                       MailBody = row["MailBody"].ToString(),
    //    //                       PageLink = row["PageLink"].ToString(),
    //    //                       parameter1 = row["parameter1"].ToString(),
    //    //                       parameter2 = row["parameter2"].ToString(),
    //    //                       parameter3 = row["parameter3"].ToString(),
    //    //                       timeslotMinute = Convert.ToInt32(row["timeslotMinute"])

    //    //                   });
    //    //            }

    //    //            return types;
    //    //        }
    //    //        else
    //    //        {
    //    //            return null;
    //    //        }



    //    //    }
    //    //    catch
    //    //    {
    //    //        throw;
    //    //    }
    //    //}
    //    //public int UserPasswordReset(ChangePass m)
    //    //{
    //    //    try
    //    //    {
    //    //        string[] pname = { "@EMPLOYEE_MASTER_KEY", "@PASSWORD" };
    //    //        string[] pvalue = { m.Employee_Master_Key.ToString(), m.Confirm_Password };
    //    //        int result = _DbAccess.ExecuteNonQuery("SP_UPDATE_BusinessAdminProfile_PASSWORD", pname, pvalue);
    //    //        if (result > 0)
    //    //        {
    //    //            return result;
    //    //        }
    //    //        else
    //    //        {
    //    //            return 0;
    //    //        }
    //    //    }
    //    //    catch (Exception)
    //    //    {
    //    //        throw;
    //    //    }
    //    //}


    //}
}
