CREATE OR REPLACE PROCEDURE ZTCRM.sp_ServiceRequest_Approve(
    p_RequestId IN NUMBER,
    p_ManagerId IN NUMBER,
    p_Note      IN VARCHAR2
) AS
    v_OldStatus VARCHAR2(30);
    v_CustomerId NUMBER;
    v_StaffId NUMBER;
BEGIN
SELECT CustomerId INTO  v_CustomerId
FROM ZTCRM.ServiceRequest
WHERE RequestId=p_RequestId;

SELECT 	TargetStaffId  INTO  v_StaffId
FROM ZTCRM."ASSIGNMENT"
WHERE RequestId=p_RequestId AND IsActive=1;


SELECT CurrentStatus INTO v_OldStatus
FROM ZTCRM.ServiceRequest
WHERE RequestId = p_RequestId;


UPDATE ZTCRM.ServiceRequest
SET CurrentStatus = 'Closed',
    ClosedAt      = SYSDATE
WHERE RequestId = p_RequestId;

INSERT INTO ZTCRM.ApprovalLog (RequestId, ActionType, ActionBy, Note)
VALUES (p_RequestId, 'Approved', p_ManagerId, p_Note);

INSERT INTO ZTCRM.StatusLog (RequestId, OldStatus, NewStatus, ChangedByStaffId, SystemGenerated)
VALUES (p_RequestId, v_OldStatus, 'Closed', p_ManagerId, 0);


INSERT INTO ZTCRM.NotificationLog(RequestId, CustomerId, Message, Channel)
VALUES (p_RequestId, v_CustomerId,
        '#' || p_RequestId || ' numaralı başvurunuz onaylandı ve kapatıldı. Yönetici notu: ' || p_Note,
        'AppNotification');
COMMIT;
END;