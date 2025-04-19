using System.Runtime.CompilerServices;

namespace DocumentManagementSystem.Helpers
{
    public class Constants
    {
        //Roles
        public const string SuperAdminRole = "SuperAdmin";
        public const string AdminRole = "Administrator";
        public const string ManagerRole = "Manager";
        public const string ClientRole = "Client";

        //constants
        public const string Admin = "Admin";
        public const string SuperAdmin = "SuperAdmin";
        public const string SuperAdminEmail = "admin@app.com";
        public const string SuperAdminPhoneNumber = "0000000000";
        public const string Privilege = "Privilege";
        public const string UserId = "UserId";
        public const string PoSId = "PoSId";
        public const string FirstName = "FirstName";
        public const string LastName = "LastName";
        public const string UserName = "UserName";
        public const string Email = "Email";
        public const string PhoneNumber = "PhoneNumber";
        public const string Sex = "Sex";
        public const string Role = "Role";
        public const string AllInitRole = "AllInitRole";



        public const string SuccessMessage = "operation executed Successfully";
        public const string FailedMessage = " operation failed";
        public const string IsAllowed = "IsAllowed";

        public static List<string> BaseRole = new () { SuperAdminRole.ToLower(), AdminRole.ToLower(), ManagerRole.ToLower(), ClientRole.ToLower() };
    }
    public class Privileges
    {
        //Permissions on the User Model
        public const string canCreateUsers  = "canCreateUsers";
        public const string canViewUsers  =  "canViewUsers";
        public const string canUpdateUsers = "canUpdateUsers"; 
        public const string canDeleteUsers  = "canDeleteUsers";

        //Permission on Role Model
        public const string canCreateRoles = "canCreateRoles";
        public const string canViewRoles   = "canViewRoles";
        public const string canUpdateRoles= "canUpdateRoles";
        public const string canDeleteRoles = "canDeleteRoles";

        //Permission on settings
        public const string canCreateSetting = "canCreateSetting";
        public const string canViewSettings = "canViewSettings";
        public const string canUpdateSettings = "canViewSettings";
        public const string canDeleteSetting = "canDeleteSettings";

        //Permission on system configuration
        public const string canViewDashboard = "canViewDashboard"; 
        public const string canViewReport = "canViewReports"; 





    }
}
