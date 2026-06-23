CREATE OR REPLACE PROCEDURE ZTCRM.sp_ServiceRequest_GetPendingApproval(
    p_ManagerId IN NUMBER,
    p_Result    OUT SYS_REFCURSOR
) AS
BEGIN
OPEN p_Result FOR
SELECT sr.RequestId,
       sr.CustomerId,
       c.FullName AS CustomerName,
       sr.Description,
       sr.ResolutionNote,
       CASE sr.Priority
           WHEN 'Low'    THEN 'Düşük'
           WHEN 'Medium' THEN 'Orta'
           WHEN 'High'   THEN 'Yüksek'
           ELSE sr.Priority
           END AS Priority,
       CASE sr.CurrentStatus
           WHEN 'Resolved'      THEN 'Çözüldü'
           WHEN 'CannotResolve' THEN 'Çözülemez'
           ELSE sr.CurrentStatus
           END AS CurrentStatus,
       s.FullName AS StaffName,
       cat.CategoryName,
       sr.CreatedAt
FROM ZTCRM.ServiceRequest sr
         JOIN ZTCRM.Customer c ON c.CustomerId = sr.CustomerId
         JOIN ZTCRM.Category cat ON cat.CategoryId = sr.CategoryId
         JOIN ZTCRM."ASSIGNMENT" a ON a.RequestId = sr.RequestId AND a.IsActive = 1
         JOIN ZTCRM.Staff s ON s.StaffId = a.TargetStaffId
         JOIN ZTCRM.StaffUnit su ON su.StaffId = s.StaffId
         JOIN ZTCRM.OrgUnit ou ON ou.UnitId = su.UnitId
         JOIN ZTCRM.StaffUnit msu ON msu.UnitId = ou.UnitId AND msu.StaffId = p_ManagerId
WHERE sr.CurrentStatus IN ('Resolved', 'CannotResolve')
  AND sr.IsActive = 1
ORDER BY sr.CreatedAt ASC;
END;