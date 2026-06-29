CREATE OR REPLACE PROCEDURE ZTCRM.sp_ServiceRequest_ReturnToPending(
p_RequestId IN NUMBER
) AS
BEGIN
UPDATE ZTCRM.ServiceRequest
SET CurrentStatus='Open',
    CategoryId=NULL,
    Priority=NULL,
    OperatorId=NULL
WHERE RequestId=p_RequestId;
COMMIT;
END;

