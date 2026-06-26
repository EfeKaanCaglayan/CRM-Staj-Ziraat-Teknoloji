CREATE OR REPLACE PROCEDURE ZTCRM.sp_ServiceRequest_Create(
    p_CustomerId  IN NUMBER,
    p_RequestType IN VARCHAR2,
    p_Description IN VARCHAR2,
    p_CategoryId  IN NUMBER,
    p_Channel     IN VARCHAR2,
    p_RequestId   OUT NUMBER
) AS
BEGIN
INSERT INTO ZTCRM.ServiceRequest (
    CustomerId, RequestType, Description,
    CategoryId, Priority, CurrentStatus, IsActive, CreatedAt, Channel
) VALUES (
             p_CustomerId, p_RequestType, p_Description,
             p_CategoryId, 'Low', 'Open', 1, SYSDATE, p_Channel
         ) RETURNING RequestId INTO p_RequestId;

COMMIT;
END;