using CommonUtility.SharedModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using IAM_UI.Models;

namespace IAM_UI.Helpers
{
    public class UserAuth
    {
        public List<UserTypeList> GetUserTypeList(long companyId, long tenantId, DataTable dtUserType)
        {
            if (dtUserType == null) throw new ArgumentNullException(nameof(dtUserType));

            var filteredRows = dtUserType.AsEnumerable()
                .Where(row => row.Field<long>("CompanyId") == companyId &&
                              row.Field<long>("TenantID") == tenantId);

            var lstType = new List<UserTypeList>();

            foreach (DataRow dr in filteredRows)
            {
                lstType.Add(new UserTypeList
                {
                    TenantID = Convert.ToInt32(dr["TenantID"]),
                    CompanyID = Convert.ToInt32(dr["CompanyId"]),
                    UserTypeId = Convert.ToInt32(dr["UserTypeId"]),
                    UserType = dr["UserType"].ToString()
                });
            }

            return lstType;
        }

        public List<MenuModel> GetMenuList(long companyId, long tenantId, long applicationId, long userTypeId, DataTable dtMenuList)
        {
            if (dtMenuList == null) throw new ArgumentNullException(nameof(dtMenuList));

            var filteredRows = dtMenuList.AsEnumerable()
                .Where(row => row.Field<long>("CompanyId") == companyId &&
                              row.Field<long>("TenantID") == tenantId &&
                              row.Field<long>("ApplicationId") == applicationId &&
                              row.Field<long>("UserTypeId") == userTypeId);

            var lst = new List<MenuModel>();

            foreach (DataRow row in filteredRows)
            {
                lst.Add(new MenuModel
                {
                    TenantID = Convert.ToInt32(row["TenantID"]),
                    ApplicationId = Convert.ToInt32(row["ApplicationId"]),
                    ModuleId = Convert.ToInt32(row["ModuleId"]),
                    ModuleParentId = row["ModuleParentId"] != DBNull.Value ? Convert.ToInt32(row["ModuleParentId"]) : 0,
                    ModuleName = Convert.ToString(row["ModuleName"]),
                    ModuleLink = Convert.ToString(row["ModuleLink"]),
                    ModuleHierarchy = row["ModuleHierarchy"] != DBNull.Value ? Convert.ToDecimal(row["ModuleHierarchy"]) : Convert.ToDecimal(row["ModuleParentId"] != DBNull.Value ? Convert.ToInt32(row["ModuleParentId"]) : 0),
                    ModuleTypeName = Convert.ToString(row["ModuleTypeName"]),
                    Controller = Convert.ToString(row["Controller"]),
                    Action = Convert.ToString(row["Action"]),
                    FullHiearchy = Convert.ToString(row["FullHierarchy"]),
                    ModuleTypeId = Convert.ToInt32(row["ModuleTypeId"]),
                    Add = Convert.ToInt32(row["Tag_Add"]),
                    Edit = Convert.ToInt32(row["Tag_Edit"]),
                    Delete = Convert.ToInt32(row["Tag_Delete"]),
                    Print = Convert.ToInt32(row["Tag_Print"]),
                    View = Convert.ToInt32(row["Tag_View"])
                });
            }

            return lst;
        }

        public List<CompanyList> GetCompanyList(DataTable dt)
        {
            if (dt == null) throw new ArgumentNullException(nameof(dt));

            var lst = new List<CompanyList>();
            foreach (DataRow row in dt.Rows)
            {
                lst.Add(new CompanyList
                {
                    CompanyId = Convert.ToInt32(row["CompanyId"]),
                    COMPANY_NAME = row["COMPANY_NAME"].ToString()
                });
            }

            return lst;
        }
    }

    public class LayoutProcessor
    {
        private readonly UserAuth _userAuth;

        public LayoutProcessor(UserAuth userAuth)
        {
            _userAuth = userAuth;
        }

        public ProcessingResult ProcessLoginData(string loginData)
        {
            Global_Model globalModel = null;
            var result = new ProcessingResult();

            if (!string.IsNullOrEmpty(loginData))
            {
                try
                {
                    globalModel = JsonConvert.DeserializeObject<Global_Model>(loginData);
                }
                catch (JsonException ex)
                {
                    // Handle or log deserialization error
                    Console.WriteLine($"Deserialization error: {ex.Message}");
                    return result; // Or handle according to your needs
                }

                if (globalModel != null)
                {
                    result.CompanyId = globalModel.CompanyID;
                    var compList = JsonConvert.DeserializeObject<DataTable>(globalModel.CompanyList);
                    if (compList != null && compList.Rows.Count > 0)
                    {
                        result.CompanyList = _userAuth.GetCompanyList(compList);
                        
                    }
                    result.Approval = JsonConvert.DeserializeObject<List<ApprovalList>>(globalModel.Approval);
                    result.UserDetail = JsonConvert.DeserializeObject<List<UserDetail>>(globalModel.UserDetail).FirstOrDefault();
                    result.UserDetail.UserCategoryId = result.UserDetail.UserCategoryId;
                    result.UserDetail.CurrentCompanyId = result.CompanyId;
                    result.UserDetail.Company = result.UserDetail.CurrentCompanyId;
                    result.UserDetail.User_Master_Key = result.UserDetail.User_Master_Key;
                    result.UserDetail.FirstName = result.UserDetail.FirstName;
                    result.UserDetail.MiddleName = result.UserDetail.MiddleName;
                    result.UserDetail.DOB = result.UserDetail.DOB;
                    result.UserDetail.Mobile_No = result.UserDetail.Mobile_No;
                    result.UserDetail.Email_ID = result.UserDetail.Email_ID;
                    result.UserDetail.Gender = result.UserDetail.Gender;
                    result.UserDetail.GenderName = result.UserDetail.GenderName;
                    result.UserDetail.CurrentAddress = result.UserDetail.CurrentAddress;
                    result.UserDetail.PermanentAddress = result.UserDetail.PermanentAddress;
                    result.UserDetail.User_Unique_ID = result.UserDetail.User_Unique_ID;
                    result.UserDetail.IsAcceptedTerms = result.UserDetail.IsAcceptedTerms;
                    result.UserDetail.AppID = globalModel.ApplicationId;

                    var ds = JsonConvert.DeserializeObject<DataSet>(globalModel.ModuleAccessData);
                    if (ds?.Tables.Count > 0)
                    {
                        var dtUserType = ds.Tables[0];
                        var MenuUsertypeist = ds.Tables[1];

                        var validUserTypeIds = MenuUsertypeist.AsEnumerable()
                                       .Select(row => row.Field<long>("UserTypeId"))
                                       .ToList();

                        // Filter dtUserType based on the valid UserTypeIds
                        DataTable filteredTable = dtUserType.AsEnumerable()
                                                            .Where(row => validUserTypeIds.Contains(row.Field<long>("UserTypeId")))
                                                            .CopyToDataTable();

                        if (dtUserType.Rows.Count > 0)
                        {
                            var userTypeList = _userAuth.GetUserTypeList(globalModel.CompanyID, globalModel.TenantID, filteredTable);
                            result.UserTypes = userTypeList;
                            if (!userTypeList.Any(u => u.UserTypeId == globalModel.UserTypeId))
                            {
                                // If it exists, assign globalModel.UserTypeId as the first UserTypeId, or 0 if not found
                                globalModel.UserTypeId = userTypeList.FirstOrDefault(u => u.UserTypeId == globalModel.UserTypeId)?.UserTypeId ?? 0;
                            }
                            if (globalModel.UserTypeId != 0)
                            {
                                result.UserTypeId = globalModel.UserTypeId;
                            }
                            else
                            {
                                result.UserTypeId = userTypeList.FirstOrDefault()?.UserTypeId ?? 0;
                            }

                            result.UserDetail.CurrentUserTypeId = result.UserTypeId;

                            result.UserDetail.CurrentApplicationId = globalModel.ApplicationId;
                            result.UserDetail.CurrentTenantId = globalModel.TenantID;

                        }

                        var dtMenuList = ds.Tables.Count > 1 ? ds.Tables[1] : null;

                        if (dtMenuList != null && dtMenuList.Rows.Count > 0)
                        {
                            result.MenuItems = _userAuth.GetMenuList(globalModel.CompanyID, globalModel.TenantID, globalModel.ApplicationId, result.UserTypeId, dtMenuList);
                        }
                        else
                        {
                            Console.WriteLine("Menu DataTable is empty.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("DataSet contains no DataTables.");
                    }
                }
            }

            return result;
        }
    }

}
