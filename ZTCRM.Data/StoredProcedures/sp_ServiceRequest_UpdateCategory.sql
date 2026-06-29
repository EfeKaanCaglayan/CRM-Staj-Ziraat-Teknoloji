CREATE OR REPLACE PROCEDURE ZTCRM.sp_ServiceRequest_UpdateCategory(

p_RequestId IN NUMBER,
p_CategoryId IN NUMBER,
p_Priority IN VARCHAR2

)AS

BEGIN
UPDATE ZTCRM.ServiceRequest
SET CategoryId=p_CategoryId,
    Priority=p_Priority
WHERE RequestId=p_RequestId;
COMMIT;
END;