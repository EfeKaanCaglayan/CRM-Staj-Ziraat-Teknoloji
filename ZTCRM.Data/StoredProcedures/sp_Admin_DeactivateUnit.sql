CREATE OR REPLACE PROCEDURE ZTCRM.sp_Admin_DeactivateUnit(
    p_UnitId IN NUMBER
) AS
BEGIN
UPDATE ZTCRM.OrgUnit
SET IsActive = 0
WHERE UnitId = p_UnitId;


DELETE FROM ZTCRM.StaffUnit
WHERE UnitId = p_UnitId;

COMMIT;
END;