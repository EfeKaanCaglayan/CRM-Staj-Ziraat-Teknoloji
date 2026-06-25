CREATE OR REPLACE PROCEDURE ZTCRM.sp_Staff_Login(
    p_Username IN VARCHAR2,
    p_Result   OUT SYS_REFCURSOR
) AS
BEGIN
OPEN p_Result FOR
SELECT s.StaffId, s.RoleId, s.FullName, s.Username,
       s.PasswordHash, s.IsActive,
       r.RoleName,
       ou.UnitName
FROM ZTCRM.Staff s
         JOIN ZTCRM.Role r ON r.RoleId = s.RoleId
         LEFT JOIN ZTCRM.StaffUnit su ON su.StaffId = s.StaffId AND su.IsPrimary = 1
         LEFT JOIN ZTCRM.OrgUnit ou ON ou.UnitId = su.UnitId
WHERE s.Username = p_Username
  AND s.IsActive = 1;
END;