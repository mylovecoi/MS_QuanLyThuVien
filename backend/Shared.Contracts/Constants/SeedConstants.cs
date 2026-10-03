using System;

namespace Shared.Contracts.Constants
{
    /// <summary>
    /// Giá trị cố định cho tài khoản admin được seed.
    ///
    /// Vì sao cần Id cố định: IdentityService và ProfileService nằm trên hai
    /// SQL Server khác nhau nên không có foreign key xuyên service. Admin được
    /// IdentityService tạo thẳng trong DbSeeder, bỏ qua luồng đăng ký — và
    /// luồng đăng ký là thứ duy nhất gọi sang ProfileService để tạo hồ sơ.
    /// Nếu để DbSeeder sinh Id ngẫu nhiên thì ProfileService không thể biết
    /// cần tạo hồ sơ cho Id nào.
    ///
    /// Vì sao nằm ở Shared.Contracts: đây là hợp đồng xuyên service, cần đúng
    /// một nguồn. Nhưng Shared.Contracts chỉ được reference ở tầng Business/API,
    /// không phải DataAccess, nên các seeder không dùng trực tiếp constant này
    /// mà nhận Id qua tham số từ Program.cs. Nhờ vậy không phải nới ranh giới
    /// tầng cho một hằng số.
    ///
    /// Lưu ý khi triển khai: database đã có từ trước khi đổi sang Id cố định
    /// vẫn giữ Id ngẫu nhiên cũ, vì DbSeeder chỉ chạy khi bảng Users còn trống.
    /// Dùng script trong backend/scripts/ để backfill thủ công cho các DB đó.
    /// </summary>
    public static class SeedConstants
    {
        /// <summary>
        /// Id cố định của tài khoản admin. Không đổi sau khi đã phát hành,
        /// nếu đổi thì hồ sơ đã seed theo Id cũ sẽ mất liên kết.
        /// </summary>
        public static readonly Guid AdminUserId = new("00000000-0000-0000-0000-000000000001");
    }
}