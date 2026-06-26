CREATE OR REPLACE PROCEDURE ZTCRM.sp_ServiceRequest_GetNotificationInfo(
    p_RequestId IN NUMBER,
    p_Result OUT SYS_REFCURSOR
) AS
BEGIN
OPEN p_Result FOR
SELECT
    sr.Channel,
    c.Email,
    c.Phone,
    c.FullName,
    sr.ResolutionNote,
    rl.RejectionReason,
    al.Note AS ApprovalNote
FROM ZTCRM.ServiceRequest sr
         JOIN ZTCRM.Customer c ON sr.CustomerId = c.CustomerId
         LEFT JOIN ZTCRM.RejectionLog rl ON sr.RequestId = rl.RequestId
         LEFT JOIN ZTCRM.ApprovalLog al ON sr.RequestId = al.RequestId AND al.ActionType = 'Approved'
WHERE sr.RequestId = p_RequestId;
END;