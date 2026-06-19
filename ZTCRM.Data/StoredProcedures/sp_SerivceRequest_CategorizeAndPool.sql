CREATE OR REPLACE PROCEDURE ZTCRM.sp_ServiceRequest_CategorizeAndPool(
    p_RequestId  IN NUMBER,
    p_CategoryId IN NUMBER,
    p_Priority   IN VARCHAR2,
    p_OperatorId IN NUMBER
) AS
BEGIN
    UPDATE ZTCRM.ServiceRequest
    SET CategoryId    = p_CategoryId,
        Priority      = p_Priority,
        OperatorId    = p_OperatorId,
        CurrentStatus = 'InPool'
    WHERE RequestId = p_RequestId;

    INSERT INTO ZTCRM.StatusLog (RequestId, OldStatus, NewStatus, ChangedByStaffId, SystemGenerated)
    VALUES (p_RequestId, 'Open', 'InPool', p_OperatorId, 0);

    COMMIT;
END;