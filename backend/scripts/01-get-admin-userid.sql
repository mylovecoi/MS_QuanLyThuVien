-- Chạy trên IdentityDB (server: DESKTOP-0LER7OP,50760)
--
-- Lấy Id thật của tài khoản admin. Tài khoản này được DbSeeder tạo ra với Id
-- cố định, nên nếu database được seed lại từ đầu thì giá trị trả về luôn là
-- 00000000-0000-0000-0000-000000000001 và khớp với SeedConstants.AdminUserId.
--
-- Chỉ cần chạy script này khi cột GhiChu báo "KHAC Id co dinh", tức là
-- database còn dữ liệu admin từ trước khi Id cố định được áp dụng.
-- Lấy giá trị UserId đó rồi truyền vào script 02-backfill-admin-profile.sql.
SELECT
    Id              AS UserId,
    Username,
    FullName,
    Email,
    IsDeleted,
    CASE WHEN Id = '00000000-0000-0000-0000-000000000001'
         THEN N'== dung Id co dinh trong SeedConstants'
         ELSE N'!! KHAC Id co dinh - database duoc seed truoc khi doi sang Id co dinh, bat buoc lay gia tri nay'
    END AS GhiChu
FROM Users
WHERE Username = 'admin';