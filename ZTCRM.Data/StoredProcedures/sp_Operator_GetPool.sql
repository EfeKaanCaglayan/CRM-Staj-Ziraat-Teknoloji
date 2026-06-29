CREATE OR REPLACE PROCEDURE ZTCRM.sp_Operator_GetPool(
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
           WHEN 'InPool' THEN 'Havuzda'
           ELSE sr.CurrentStatus
           END AS CurrentStatus,
       sr.CreatedAt,
       c.FullName AS CustomerName,
       cat.CategoryName
FROM ZTCRM.ServiceRequest sr
         JOIN ZTCRM.Customer c ON c.CustomerId = sr.CustomerId
         LEFT JOIN ZTCRM.Category cat ON cat.CategoryId = sr.CategoryId
WHERE sr.CurrentStatus = 'InPool'
  AND sr.IsActive = 1
ORDER BY sr.CreatedAt DESC;
END;