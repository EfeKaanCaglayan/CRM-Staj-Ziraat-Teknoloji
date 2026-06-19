CREATE OR REPLACE PROCEDURE ZTCRM.sp_ServiceRequest_Reject(
    p_RequestId       IN NUMBER,
    p_RejectionType   IN VARCHAR2,
    p_RejectionReason IN VARCHAR2,
    p_OperatorId      IN NUMBER
) AS
BEGIN
UPDATE ZTCRM.ServiceRequest
SET CurrentStatus = 'Rejected',
    IsActive      = 0
WHERE RequestId = p_RequestId;

INSERT INTO ZTCRM.RejectionLog (RequestId, RejectedBy, RejectionType, RejectionReason)
VALUES (p_RequestId, p_OperatorId, p_RejectionType, p_RejectionReason);

INSERT INTO ZTCRM.StatusLog (RequestId, OldStatus, NewStatus, ChangedByStaffId, SystemGenerated)
VALUES (p_RequestId, 'Open', 'Rejected', p_OperatorId, 0);

COMMIT;
END;