CREATE OR REPLACE PROCEDURE ZTCRM.sp_Admin_UpdateStaffRole(
    p_StaffId IN NUMBER,
    p_RoleId  IN NUMBER
) AS
BEGIN
UPDATE ZTCRM.Staff
SET RoleId = p_RoleId
WHERE StaffId = p_StaffId;

COMMIT;
END;