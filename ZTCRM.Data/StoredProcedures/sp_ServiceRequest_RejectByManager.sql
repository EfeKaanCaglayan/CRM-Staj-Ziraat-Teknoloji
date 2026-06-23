CREATE OR REPLACE PROCEDURE ZTCRM.sp_ServiceRequest_RejectByManager(
    p_RequestId IN NUMBER,
    p_ManagerId IN NUMBER,
    p_Note      IN VARCHAR2
) AS
    v_OldStatus VARCHAR2(30);
BEGIN
SELECT CurrentStatus INTO v_OldStatus
FROM ZTCRM.ServiceRequest
WHERE RequestId = p_RequestId;

UPDATE ZTCRM.ServiceRequest
SET CurrentStatus = 'InProgress'
WHERE RequestId = p_RequestId;

INSERT INTO ZTCRM.ApprovalLog (RequestId, ActionType, ActionBy, Note)
VALUES (p_RequestId, 'Rejected', p_ManagerId, p_Note);

INSERT INTO ZTCRM.StatusLog (RequestId, OldStatus, NewStatus, ChangedByStaffId, SystemGenerated)
VALUES (p_RequestId, v_OldStatus, 'InProgress', p_ManagerId, 0);

COMMIT;
END;