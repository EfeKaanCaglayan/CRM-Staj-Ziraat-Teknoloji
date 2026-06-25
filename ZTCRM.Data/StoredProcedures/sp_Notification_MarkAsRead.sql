CREATE OR REPLACE PROCEDURE ZTCRM.sp_Notification_MarkAsRead(
    p_CustomerId IN NUMBER
) AS
BEGIN
UPDATE ZTCRM.NotificationLog
SET IsSent = 1
WHERE CustomerId = p_CustomerId
  AND IsSent = 0;
COMMIT;
END;