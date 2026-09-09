namespace IdentityService.Business.Constants
{
    public static class ApplicationConstants
    {
        public static class Roles
        {
            public const string Admin = "Admin";
            public const string User = "User";
        }

        public static class ErrorMessages
        {
            public const string InvalidCredentials = "Tên đăng nhập hoặc mật khẩu không chính xác.";
            public const string UserNotFound = "Không tìm thấy người dùng.";
            public const string UsernameAlreadyExists = "Tên đăng nhập đã tồn tại.";
            public const string EmailAlreadyExists = "Email đã được sử dụng.";
        }
    }
}
