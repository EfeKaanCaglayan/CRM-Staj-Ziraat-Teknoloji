CREATE OR REPLACE PROCEDURE ZTCRM.sp_Notification_GetUnreadCountByStaff(
p_StaffId IN NUMBER,
p_Count  OUT NUMBER)
AS
BEGIN
SELECT COUNT(*) INTO p_Count
FROM ZTCRM.NotificationLog
WHERE StaffId=p_StaffId AND IsSent=0;
END;
