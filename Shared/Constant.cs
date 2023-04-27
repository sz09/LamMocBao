namespace Shared
{
    public class Constants
    {
        public static string DbConnectionString { get; set; }
        public const string UserToken_Key = "user_token";
        public const string UserId_Key = "user_id";
        public const string Username_Key = "username";
    }

    public class LMBRoles
    {
        public const string SUPER_ADMIN = "LMB_SuperAdmin";
        public const string ADMIN = "LMB_Admin";
        public const string Customer = "LMB_Customer";
    }

    public class LMBClaims
    {
        public const string CUSTOMER_CREATE = "LMB_Customer_Can_Create";
    }
}
