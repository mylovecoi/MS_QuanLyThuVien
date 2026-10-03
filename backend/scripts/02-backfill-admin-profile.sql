-- Chạy trên ProfileDB (server: DESKTOP-0LER7OP\SQLEXPRESS)
--
-- Tạo hồ sơ cho tài khoản admin nếu chưa có.
--
-- Luồng bình thường: ProfileDbSeeder tự tạo hồ sơ cho admin ngay khi app khởi
-- động, và ProfileService còn tự tạo nếu hồ sơ bị xoá mền giữa chừng. Script này
-- chỉ cần khi dữ liệu được nạp tay, ví dụ khi chỉ có IdentityDB được seed lại mà
-- ProfileDB vẫn giữ dữ liệu cũ -- khi đó hồ sơ của admin cũ có thể trùng Id với
-- Id cố định và bị unique index chặn.
--
-- @UserId: chạy 01-get-admin-userid.sql trên IdentityDB để lấy.
--           Mặc định là Id cố định trong SeedConstants.AdminUserId.
--           Đổi thành NULL sẽ tự dò tài khoản admin trong IdentityDB.
DECLARE @UserId uniqueidentifier = N'00000000-0000-0000-0000-000000000001';
DECLARE @FullName nvarchar(100) = N'System Administrator';
DECLARE @Email nvarchar(100) = N'admin@example.com';

IF @UserId IS NULL
BEGIN
    -- Cách dùng thay thế khi không muốn hardcode Id:
    -- chạy script này từ SSMS đã kết nối tới ProfileDB, rồi bật cross-database
    -- query trong SSMS (Tools > Options > SQL Server > SQL Server Databases >
    -- Enable cross-database queries) rồi thay khối dưới bằng câu lệnh sau:
    --
    --   SELECT TOP 1 @UserId = Id, @FullName = FullName, @Email = Email
    --   FROM IdentityDb.dbo.Users
    --   WHERE Username = 'admin' AND IsDeleted = 0;
    --
    -- Khối dưới chỉ dùng được khi ProfileDB và IdentityDB nằm cùng một SQL
    -- instance. Với hai instance riêng thì phải dán UserId thủ công.
    SET @UserId = N'00000000-0000-0000-0000-000000000001';
END;

IF @UserId IS NULL
    THROW 50001, N'Khong tim thay UserId cua tai khoan admin. Hay chay 01-get-admin-userid.sql truoc.', 1;

DECLARE @ProfileId uniqueidentifier = NEWID();

-- Bảng Profiles có unique index trên UserId, nên một tài khoản chỉ có một hồ sơ.
-- Lười trước để tránh bắt exception khi chạy lại script lần hai.
IF NOT EXISTS (SELECT 1 FROM Profiles WHERE UserId = @UserId)
BEGIN
    INSERT INTO Profiles (Id, UserId, FullName, Email, CreatedAt, IsDeleted)
    VALUES (@ProfileId, @UserId, @FullName, @Email, SYSUTCDATETIME(), 0);

    PRINT N'Da tao ho so moi cho tai khoan admin.';
END
ELSE IF EXISTS (SELECT 1 FROM Profiles WHERE UserId = @UserId AND IsDeleted = 1)
BEGIN
    -- Hồ sơ bị xoá mền: hồi sinh đúng một hồ sơ cho tài khoản.
    -- Điều kiện EXISTS ở trên đảm bảo không có hồ sơ nào đang hoạt động cho
    -- tài khoản này, nên UPDATE không đụng tới hồ sơ hợp lệ.
    UPDATE Profiles
    SET IsDeleted = 0,
        UpdatedAt = SYSUTCDATETIME()
    WHERE UserId = @UserId;

    PRINT N'Da hoi sinh ho so da bi xoa mem cho tai khoan admin.';
END
ELSE
BEGIN
    PRINT N'Ho so cua tai khoan admin da ton tai va dang hoat dong. Khong can lam gi.';
END;