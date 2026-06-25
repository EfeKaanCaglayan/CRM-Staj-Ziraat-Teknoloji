CREATE OR REPLACE PROCEDURE ZTCRM.sp_Admin_GetAllStaff(
    p_Result OUT SYS_REFCURSOR
) AS
BEGIN
OPEN p_Result FOR
SELECT s.StaffId,
       s.RoleId,
       s.FullName,
       s.Username,
       s.PasswordHash,
       s.IsActive,
       s.CreatedAt,
       s.LastLoginAt,
       ou.UnitName,
       CASE s.IsActive WHEN 1 THEN 'Aktif' ELSE 'İnaktif' END AS IsActiveText,
       CASE ou.IsActive WHEN 1 THEN 'Aktif' ELSE 'İnaktif' END AS UnitIsActiveText
FROM ZTCRM.Staff s
         LEFT JOIN ZTCRM.StaffUnit su ON su.StaffId = s.StaffId AND su.IsPrimary = 1
         LEFT JOIN ZTCRM.OrgUnit ou ON ou.UnitId = su.UnitId
ORDER BY s.CreatedAt DESC;
END;