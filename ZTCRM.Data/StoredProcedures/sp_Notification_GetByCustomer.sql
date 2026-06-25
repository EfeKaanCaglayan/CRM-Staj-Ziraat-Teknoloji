CREATE OR REPLACE PROCEDURE ZTCRM.sp_Notification_GetByCustomer(
p_CustomerId IN NUMBER,
p_Result OUT SYS_REFCURSOR)AS

BEGIN
OPEN p_Result FOR

SELECT NotifId, RequestId, Message, IsSent, CreatedAt
FROM ZTCRM.NotificationLog
WHERE CustomerId = p_CustomerId
ORDER BY CreatedAt DESC;
END;