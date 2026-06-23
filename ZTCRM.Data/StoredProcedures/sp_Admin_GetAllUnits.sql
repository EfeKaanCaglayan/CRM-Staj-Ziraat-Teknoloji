CREATE OR REPLACE PROCEDURE ZTCRM.sp_Admin_GetAllUnits(
    p_Result OUT SYS_REFCURSOR
) AS
BEGIN
OPEN p_Result FOR
SELECT UnitId,
       UnitName,
       CASE UnitType
           WHEN 'Unit'       THEN 'Birim'
           WHEN 'Branch'     THEN 'Şube'
           WHEN 'Department' THEN 'Departman'
           WHEN 'Region'     THEN 'Bölge'
           ELSE UnitType
           END AS UnitType,
       ParentUnitId,
       IsActive,
       CreatedAt,
       CASE IsActive WHEN 1 THEN 'Aktif' ELSE 'İnaktif' END AS IsActiveText
FROM ZTCRM.OrgUnit
ORDER BY UnitName ASC;
END;