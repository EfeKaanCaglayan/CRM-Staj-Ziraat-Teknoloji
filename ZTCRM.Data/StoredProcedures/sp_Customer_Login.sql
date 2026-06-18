CREATE OR REPLACE PROCEDURE ZTCRM.sp_Customer_Login(
    p_NationalId IN VARCHAR2,
    p_PassportNo IN VARCHAR2,
    p_Result     OUT SYS_REFCURSOR
) AS
BEGIN
OPEN p_Result FOR
SELECT CustomerId, FullName, CustomerType,
       NotifyChannel, IsActive
FROM ZTCRM.Customer
WHERE (
    (CustomerType != 'Foreign' AND NationalId = p_NationalId)
        OR
    (CustomerType = 'Foreign' AND PassportNo = p_PassportNo)
    )
  AND IsActive = 1;
END;