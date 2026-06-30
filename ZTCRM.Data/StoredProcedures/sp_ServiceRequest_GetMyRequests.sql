CREATE OR REPLACE PROCEDURE ZTCRM.sp_ServiceRequest_GetMyRequests(
    p_StaffId IN NUMBER,
    p_Result  OUT SYS_REFCURSOR
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
           WHEN 'Resolved'         THEN 'Çözüldü'
           WHEN 'CannotResolve'    THEN 'Çözülemez'
           WHEN 'InProgress'       THEN 'İşlemde'
           WHEN 'Closed'           THEN 'Kapatıldı'
           WHEN 'PendingApproval'  THEN 'Onay Bekliyor'
           ELSE sr.CurrentStatus
           END AS CurrentStatus,
       sr.CreatedAt,
       c.FullName AS CustomerName,
       cat.CategoryName
FROM ZTCRM.ServiceRequest sr
         JOIN ZTCRM.Customer c ON c.CustomerId = sr.CustomerId
         JOIN ZTCRM.Category cat ON cat.CategoryId = sr.CategoryId
         JOIN ZTCRM."ASSIGNMENT" a ON a.RequestId = sr.RequestId
WHERE a.TargetStaffId = p_StaffId
  AND a.IsActive = 1
  AND sr.IsActive = 1;
END;