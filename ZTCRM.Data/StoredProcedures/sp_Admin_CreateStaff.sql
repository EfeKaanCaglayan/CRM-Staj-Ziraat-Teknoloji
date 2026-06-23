CREATE OR REPLACE PROCEDURE ZTCRM.sp_Admin_CreateStaff(
p_RoleId IN NUMBER,
p_FullName IN VARCHAR2,
p_UserName IN VARCHAR2,
p_PasswordHash IN VARCHAR2,
p_IsActive IN NUMBER,
p_StaffId OUT NUMBER

) AS
BEGIN

INSERT INTO ZTCRM.Staff(
    RoleId,
    FullName,
    UserName,
    PasswordHash,
    IsActive
)VALUES(
           p_RoleId,
           p_FullName,
           p_UserName,
           p_PasswordHash,
           p_IsActive
       )
    RETURNING StaffId INTO p_StaffId;
COMMIT;
END;

