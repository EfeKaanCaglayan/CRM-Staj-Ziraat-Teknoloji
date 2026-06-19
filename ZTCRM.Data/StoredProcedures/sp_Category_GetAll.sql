CREATE OR REPLACE PROCEDURE ZTCRM.sp_Category_GetAll(
    p_Result OUT SYS_REFCURSOR
) AS
BEGIN
OPEN p_Result FOR
SELECT CategoryId,
       CategoryName,
       DefaultUnitId,
       ParentCategoryId
FROM ZTCRM.Category
WHERE IsActive = 1
ORDER BY CategoryName ASC;
END;