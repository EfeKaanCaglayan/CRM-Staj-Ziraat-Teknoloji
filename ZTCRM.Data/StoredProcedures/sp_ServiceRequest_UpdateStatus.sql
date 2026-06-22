CREATE OR REPLACE PROCEDURE ZTCRM.sp_ServiceRequest_UpdateStatus(
    p_RequestId   IN NUMBER,
    p_NewStatus   IN VARCHAR2,
    p_StaffId     IN NUMBER,
    p_ResolutionNote IN VARCHAR2
) AS
    v_OldStatus VARCHAR2(30);
BEGIN
SELECT CurrentStatus INTO v_OldStatus
FROM ZTCRM.ServiceRequest
WHERE RequestId = p_RequestId;

UPDATE ZTCRM.ServiceRequest
SET CurrentStatus  = p_NewStatus,
    ResolutionNote = p_ResolutionNote,
    ResolvedAt     = SYSDATE
WHERE RequestId = p_RequestId;

INSERT INTO ZTCRM.StatusLog (RequestId, OldStatus, NewStatus, ChangedByStaffId, SystemGenerated)
VALUES (p_RequestId, v_OldStatus, p_NewStatus, p_StaffId, 0);

COMMIT;
END;