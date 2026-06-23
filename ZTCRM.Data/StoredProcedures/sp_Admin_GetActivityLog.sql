CREATE OR REPLACE PROCEDURE ZTCRM.sp_Admin_GetActivityLog(
    p_Result OUT SYS_REFCURSOR
) AS
BEGIN
OPEN p_Result FOR
SELECT sl.LogId,
       sl.RequestId,
       CASE sl.OldStatus
           WHEN 'Open'            THEN 'Açık'
           WHEN 'Categorized'     THEN 'Kategorize Edildi'
           WHEN 'InPool'          THEN 'Havuzda'
           WHEN 'InProgress'      THEN 'İşlemde'
           WHEN 'Resolved'        THEN 'Çözüldü'
           WHEN 'CannotResolve'   THEN 'Çözülemez'
           WHEN 'PendingApproval' THEN 'Onay Bekliyor'
           WHEN 'Closed'          THEN 'Kapatıldı'
           WHEN 'Canceled'        THEN 'İptal'
           WHEN 'Rejected'        THEN 'Reddedildi'
           ELSE sl.OldStatus
           END AS OldStatus,
       CASE sl.NewStatus
           WHEN 'Open'            THEN 'Açık'
           WHEN 'Categorized'     THEN 'Kategorize Edildi'
           WHEN 'InPool'          THEN 'Havuzda'
           WHEN 'InProgress'      THEN 'İşlemde'
           WHEN 'Resolved'        THEN 'Çözüldü'
           WHEN 'CannotResolve'   THEN 'Çözülemez'
           WHEN 'PendingApproval' THEN 'Onay Bekliyor'
           WHEN 'Closed'          THEN 'Kapatıldı'
           WHEN 'Canceled'        THEN 'İptal'
           WHEN 'Rejected'        THEN 'Reddedildi'
           ELSE sl.NewStatus
           END AS NewStatus,
       sl.ChangedAt,
       sl.SystemGenerated,
       s.FullName AS ChangedBy
FROM ZTCRM.StatusLog sl
         LEFT JOIN ZTCRM.Staff s ON s.StaffId = sl.ChangedByStaffId
ORDER BY sl.ChangedAt DESC;
END;