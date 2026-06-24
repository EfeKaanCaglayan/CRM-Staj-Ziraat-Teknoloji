CREATE OR REPLACE PROCEDURE ZTCRM.sp_ServiceRequest_Cancel(
    p_RequestId  IN NUMBER,
    p_CustomerId IN NUMBER
) AS
BEGIN
UPDATE ZTCRM.ServiceRequest
SET CurrentStatus = 'Canceled',
    IsActive = 0
WHERE RequestId = p_RequestId
  AND CustomerId = p_CustomerId
  AND CurrentStatus NOT IN ('Closed', 'Canceled', 'Rejected');
COMMIT;
END;