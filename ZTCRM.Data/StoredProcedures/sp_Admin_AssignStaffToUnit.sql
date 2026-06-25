CREATE OR REPLACE PROCEDURE ZTCRM.sp_Admin_AssignStaffToUnit(
    p_StaffId   IN NUMBER,
    p_UnitId    IN NUMBER,
    p_IsPrimary IN NUMBER
) AS
BEGIN

DELETE FROM ZTCRM.StaffUnit
WHERE StaffId = p_StaffId;


INSERT INTO ZTCRM.StaffUnit (StaffId, UnitId, IsPrimary)
VALUES (p_StaffId, p_UnitId, p_IsPrimary);

COMMIT;
END;