CREATE OR REPLACE PROCEDURE ZTCRM.sp_ServiceRequest_GetByCustomer(
p_CustomerId IN NUMBER,
p_Result OUT SYS_REFCURSOR) AS
BEGIN
OPEN p_Result FOR
SELECT sr.RequestId,
       CASE sr.RequestType
           WHEN 'Complaint' THEN 'Şikayet'
           WHEN 'Request'   THEN 'Talep'
           ELSE sr.RequestType
           END AS RequestType,
       CASE sr.CurrentStatus
           WHEN 'Open'       THEN 'Açık'
           WHEN 'InProgress' THEN 'İşlemde'
           WHEN 'Resolved'   THEN 'Çözüldü'
           WHEN 'Closed'     THEN 'Kapatıldı'
           WHEN 'Cancelled'  THEN 'İptal'
           WHEN 'Rejected'   THEN 'Reddedildi'
           ELSE sr.CurrentStatus
           END AS CurrentStatus,
       sr.Description,
       sr.Priority, sr.CreatedAt,
       sr.ResolvedAt, sr.ClosedAt,
       NVL(c.CategoryName, 'Henüz atanmadı') AS CategoryName
FROM ZTCRM.ServiceRequest sr
         LEFT JOIN ZTCRM.Category c ON sr.CategoryId = c.CategoryId
WHERE sr.CustomerId = p_CustomerId
  AND sr.IsActive = 1
ORDER BY sr.CreatedAt DESC;
END;