CREATE OR REPLACE PROCEDURE ZTCRM.sp_Admin_CreateUnit(
p_UnitName IN VARCHAR2,
p_UnitType IN VARCHAR2,
p_ParentUnitId IN NUMBER,
p_IsActive IN NUMBER,
p_UnitId OUT NUMBER
)AS

BEGIN
INSERT INTO ZTCRM.OrgUnit(
    UnitName,UnitType,ParentUnitId,IsActive)
VALUES(p_UnitName,
       p_UnitType,
       p_ParentUnitId,
       p_IsActive  )
    RETURNING UnitId INTO p_UnitId;
COMMIT;
END;