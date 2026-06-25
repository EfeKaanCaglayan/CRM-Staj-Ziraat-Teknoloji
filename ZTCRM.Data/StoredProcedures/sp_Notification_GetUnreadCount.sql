CREATE OR REPLACE PROCEDURE ZTCRM.sp_Notification_GetUnreadCount(
    p_CustomerId IN NUMBER,
    p_Count      OUT NUMBER
) AS
BEGIN
SELECT COUNT(*) INTO p_Count
FROM ZTCRM.NotificationLog
WHERE CustomerId = p_CustomerId
  AND IsSent = 0;
END;