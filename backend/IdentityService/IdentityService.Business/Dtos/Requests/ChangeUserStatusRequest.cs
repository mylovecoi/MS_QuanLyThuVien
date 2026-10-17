using IdentityService.DataAccess.Enums;

namespace IdentityService.Business.Dtos.Requests
{
    /// <summary>
    /// Khóa / mở khóa tài khoản bằng cách đổi trạng thái.
    /// </summary>
    public class ChangeUserStatusRequest
    {
        public AccountStatus Status { get; set; } = AccountStatus.Active;
    }
}
