CREATE OR REPLACE PROCEDURE ZTCRM.sp_ServiceRequest_GetPending(
p_Result OUT SYS_REFCURSOR
) AS
BEGIN
OPEN p_Result FOR

SELECT    sr.RequestId,
          sr.CustomerId,
          sr.Description,
          sr.CreatedAt,
          c.FullName AS CustomerName,
          sr.CurrentStatus
FROM  ZTCRM.ServiceRequest sr
          JOIN ZTCRM.Customer	 c 	ON c.CustomerId=sr.CustomerId
WHERE sr.CurrentStatus='Open' AND sr.IsActive=1
ORDER BY sr.CreatedAt ASC;
END;