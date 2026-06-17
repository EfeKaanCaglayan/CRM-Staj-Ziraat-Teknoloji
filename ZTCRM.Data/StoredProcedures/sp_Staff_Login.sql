CREATE OR REPLACE PROCEDURE ZTCRM.sp_Staff_Login(
    p_Username IN VARCHAR2,
    p_Password IN VARCHAR2,
    p_Result   OUT SYS_REFCURSOR
) AS
BEGIN
OPEN p_Result FOR
SELECT s.StaffId, s.FullName, s.Username,
       s.PasswordHash, s.IsActive,
       s.CreatedAt, s.LastLoginAt,
       r.RoleId, r.RoleName
FROM ZTCRM.Staff s
         JOIN ZTCRM.Role r ON s.RoleId = r.RoleId
WHERE s.Username = p_Username
  AND s.PasswordHash = p_Password
  AND s.IsActive = 1;
END;
