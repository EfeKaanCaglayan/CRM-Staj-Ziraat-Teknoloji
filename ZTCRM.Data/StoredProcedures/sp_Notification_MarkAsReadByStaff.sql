CREATE OR REPLACE PROCEDURE ZTCRM.sp_Notification_MarkAsReadByStaff(
p_StaffId IN NUMBER)
AS
BEGIN
UPDATE ZTCRM.NotificationLog
SET IsSent=1
WHERE StaffId=p_StaffId AND IsSent=0;
COMMIT;
END;
