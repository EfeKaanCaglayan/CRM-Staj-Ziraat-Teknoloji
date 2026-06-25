CREATE OR REPLACE PROCEDURE ZTCRM.sp_Notification_GetByStaff(
p_StaffId IN NUMBER,
p_Result OUT SYS_REFCURSOR
) AS
BEGIN
OPEN p_Result FOR
SELECT NotifId, RequestId, Message, IsSent, CreatedAt
FROM ZTCRM.NotificationLog
WHERE StaffId=p_StaffId
ORDER BY CreatedAt DESC;

END;
