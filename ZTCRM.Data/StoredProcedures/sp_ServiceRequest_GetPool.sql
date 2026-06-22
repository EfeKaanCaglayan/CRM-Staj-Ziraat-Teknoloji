CREATE OR REPLACE PROCEDURE ZTCRM.sp_ServiceRequest_GetPool(
    p_StaffId IN NUMBER,
    p_Result OUT SYS_REFCURSOR
) AS
BEGIN
OPEN p_Result FOR
SELECT sr.RequestId,
       sr.Description,
       CASE sr.Priority
           WHEN 'Low'    THEN 'Düşük'
           WHEN 'Medium' THEN 'Orta'
           WHEN 'High'   THEN 'Yüksek'
           ELSE sr.Priority
           END AS Priority,
       CASE sr.CurrentStatus
           WHEN 'Resolved'      THEN 'Çözüldü'
           WHEN 'CannotResolve' THEN 'Çözülemez'
           WHEN 'InProgress'    THEN 'İşlemde'
           WHEN 'InPool'        THEN 'Havuzda'
           ELSE sr.CurrentStatus
           END AS CurrentStatus,
       sr.CreatedAt,
       c.FullName AS CustomerName,
       s.FullName AS OperatorName,
       cat.CategoryName
FROM ZTCRM.ServiceRequest sr
         JOIN ZTCRM.Customer c ON c.CustomerId = sr.CustomerId
         LEFT JOIN ZTCRM.Staff s ON s.StaffId = sr.OperatorId
         JOIN ZTCRM.Category cat ON cat.CategoryId = sr.CategoryId
         JOIN ZTCRM.OrgUnit ou ON ou.UnitId = cat.DefaultUnitId
         JOIN ZTCRM.StaffUnit su ON su.UnitId = ou.UnitId
WHERE sr.CurrentStatus = 'InPool'
  AND sr.IsActive = 1
  AND su.StaffId = p_StaffId;
END;