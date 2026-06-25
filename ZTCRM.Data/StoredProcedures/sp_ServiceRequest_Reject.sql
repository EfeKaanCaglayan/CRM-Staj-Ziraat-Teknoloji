CREATE OR REPLACE PROCEDURE ZTCRM.sp_ServiceRequest_Reject(
    p_RequestId  IN NUMBER,
    p_RejectionType   IN VARCHAR2,
      p_RejectionReason IN VARCHAR2,
    p_OperatorId IN NUMBER
) AS  v_CustomerId NUMBER;
BEGIN
SELECT CustomerId INTO v_CustomerId
FROM ZTCRM.ServiceRequest
WHERE RequestId=p_RequestId;

UPDATE ZTCRM.ServiceRequest
SET
    CurrentStatus = 'Rejected',
    IsActive=0

WHERE RequestId = p_RequestId;

INSERT INTO ZTCRM.RejectionLog (RequestId, RejectedBy, RejectionType, RejectionReason)
VALUES (p_RequestId, p_OperatorId, p_RejectionType, p_RejectionReason);

INSERT INTO ZTCRM.StatusLog (RequestId, OldStatus, NewStatus, ChangedByStaffId, SystemGenerated)
VALUES (p_RequestId, 'Open', 'Rejected', p_OperatorId, 0);

INSERT INTO ZTCRM.NotificationLog(CustomerId, RequestId, Message, Channel)
VALUES(v_CustomerId, p_RequestId,
       '#' || p_RequestId || ' numaralı başvurunuz reddedildi. Sebep: ' || p_RejectionReason,
       'AppNotification');

COMMIT;
END;