CREATE OR REPLACE PROCEDURE ZTCRM.sp_ServiceRequest_ReturnToPool(
    p_RequestId IN NUMBER,
    p_StaffId   IN NUMBER
) AS
BEGIN
UPDATE ZTCRM.ServiceRequest
SET CurrentStatus = 'InPool'
WHERE RequestId = p_RequestId;

UPDATE ZTCRM."ASSIGNMENT"
SET IsActive = 0
WHERE RequestId = p_RequestId
  AND TargetStaffId = p_StaffId;

INSERT INTO ZTCRM.StatusLog (RequestId, OldStatus, NewStatus, ChangedByStaffId, SystemGenerated)
VALUES (p_RequestId, 'InProgress', 'InPool', p_StaffId, 0);

COMMIT;
END;