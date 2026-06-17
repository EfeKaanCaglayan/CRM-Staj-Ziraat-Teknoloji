CREATE TABLE ZTCRM.Customer (
    CustomerId     NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    CustomerType   VARCHAR2(20)  NOT NULL,
    NationalId     CHAR(11)      NULL,
    TaxNumber      VARCHAR2(10)  NULL,
    FullName       VARCHAR2(100) NOT NULL,
    BirthDate      DATE          NULL,
    Phone          VARCHAR2(20)  NOT NULL,
    Email          VARCHAR2(100) NULL,
    Address        VARCHAR2(255) NULL,
    NotifyChannel  VARCHAR2(20)  NOT NULL,
    IsActive       NUMBER(1)     DEFAULT 1 NOT NULL,
    CreatedAt      DATE          DEFAULT SYSDATE NOT NULL,
    CONSTRAINT chk_customer_type CHECK (CustomerType IN ('Individual', 'Corporate')),
    CONSTRAINT chk_notify_channel CHECK (NotifyChannel IN ('SMS', 'Email', 'AppNotification', 'None')),
    CONSTRAINT chk_customer_active CHECK (IsActive IN (0, 1))
);

CREATE TABLE ZTCRM.Role (
    RoleId      NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    RoleName    VARCHAR2(50)  NOT NULL,
    Description VARCHAR2(255) NULL,
    CreatedAt   DATE          DEFAULT SYSDATE NOT NULL,
    CONSTRAINT chk_role_name CHECK (RoleName IN ('Customer', 'Operator', 'Staff', 'Manager', 'Admin'))
);

CREATE TABLE ZTCRM.OrgUnit (
    UnitId       NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    UnitName     VARCHAR2(100) NOT NULL,
    UnitType     VARCHAR2(20)  NOT NULL,
    ParentUnitId NUMBER        NULL,
    IsActive     NUMBER(1)     DEFAULT 1 NOT NULL,
    CreatedAt    DATE          DEFAULT SYSDATE NOT NULL,
    CONSTRAINT chk_unit_type CHECK (UnitType IN ('Unit', 'Branch', 'Department', 'Region')),
    CONSTRAINT fk_orgunit_parent FOREIGN KEY (ParentUnitId) REFERENCES ZTCRM.OrgUnit(UnitId)
);

CREATE TABLE ZTCRM.Category (
    CategoryId       NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    CategoryName     VARCHAR2(100) NOT NULL,
    DefaultUnitId    NUMBER        NULL,
    ParentCategoryId NUMBER        NULL,
    IsActive         NUMBER(1)     DEFAULT 1 NOT NULL,
    CreatedAt        DATE          DEFAULT SYSDATE NOT NULL,
    CONSTRAINT fk_category_unit FOREIGN KEY (DefaultUnitId) REFERENCES ZTCRM.OrgUnit(UnitId),
    CONSTRAINT fk_category_parent FOREIGN KEY (ParentCategoryId) REFERENCES ZTCRM.Category(CategoryId)
);

CREATE TABLE ZTCRM.Staff (
    StaffId      NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    RoleId       NUMBER        NOT NULL,
    FullName     VARCHAR2(100) NOT NULL,
    Username     VARCHAR2(50)  NOT NULL,
    PasswordHash VARCHAR2(255) NOT NULL,
    IsActive     NUMBER(1)     DEFAULT 1 NOT NULL,
    CreatedAt    DATE          DEFAULT SYSDATE NOT NULL,
    LastLoginAt  DATE          NULL,
    CONSTRAINT uq_staff_username UNIQUE (Username),
    CONSTRAINT fk_staff_role FOREIGN KEY (RoleId) REFERENCES ZTCRM.Role(RoleId)
);

CREATE TABLE ZTCRM.StaffUnit (
    StaffUnitId NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    StaffId     NUMBER    NOT NULL,
    UnitId      NUMBER    NOT NULL,
    IsPrimary   NUMBER(1) DEFAULT 0 NOT NULL,
    CreatedAt   DATE      DEFAULT SYSDATE NOT NULL,
    CONSTRAINT fk_staffunit_staff FOREIGN KEY (StaffId) REFERENCES ZTCRM.Staff(StaffId),
    CONSTRAINT fk_staffunit_unit FOREIGN KEY (UnitId) REFERENCES ZTCRM.OrgUnit(UnitId)
);

CREATE TABLE ZTCRM.ServiceRequest (
    RequestId      NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    CustomerId     NUMBER         NOT NULL,
    OperatorId     NUMBER         NULL,
    RequestType    VARCHAR2(20)   NOT NULL,
    Description    VARCHAR2(2000) NOT NULL,
    CategoryId     NUMBER         NULL,
    Priority       VARCHAR2(10)   NULL,
    CurrentStatus  VARCHAR2(30)   DEFAULT 'Open' NOT NULL,
    ResolutionNote VARCHAR2(2000) NULL,
    IsActive       NUMBER(1)      DEFAULT 1 NOT NULL,
    CreatedAt      DATE           DEFAULT SYSDATE NOT NULL,
    ResolvedAt     DATE           NULL,
    ClosedAt       DATE           NULL,
    CONSTRAINT chk_request_type CHECK (RequestType IN ('Complaint', 'Request')),
    CONSTRAINT chk_request_priority CHECK (Priority IN ('Low', 'Medium', 'High')),
    CONSTRAINT chk_request_status CHECK (CurrentStatus IN ('Open', 'InProgress', 'Resolved', 'PendingApproval', 'Closed', 'Canceled', 'Rejected')),
    CONSTRAINT fk_request_customer FOREIGN KEY (CustomerId) REFERENCES ZTCRM.Customer(CustomerId),
    CONSTRAINT fk_request_operator FOREIGN KEY (OperatorId) REFERENCES ZTCRM.Staff(StaffId),
    CONSTRAINT fk_request_category FOREIGN KEY (CategoryId) REFERENCES ZTCRM.Category(CategoryId)
);

CREATE TABLE ZTCRM.Assignment (
    AssignmentId  NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    RequestId     NUMBER        NOT NULL,
    TargetUnitId  NUMBER        NULL,
    TargetStaffId NUMBER        NULL,
    AssignedBy    NUMBER        NOT NULL,
    AssignedAt    DATE          DEFAULT SYSDATE NOT NULL,
    IsActive      NUMBER(1)     DEFAULT 1 NOT NULL,
    Note          VARCHAR2(500) NULL,
    CONSTRAINT fk_assignment_req FOREIGN KEY (RequestId) REFERENCES ZTCRM.ServiceRequest(RequestId),
    CONSTRAINT fk_assignment_unit FOREIGN KEY (TargetUnitId) REFERENCES ZTCRM.OrgUnit(UnitId),
    CONSTRAINT fk_assignment_staff FOREIGN KEY (TargetStaffId) REFERENCES ZTCRM.Staff(StaffId),
    CONSTRAINT fk_assignment_by FOREIGN KEY (AssignedBy) REFERENCES ZTCRM.Staff(StaffId)
);

CREATE TABLE ZTCRM.StatusLog (
    LogId            NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    RequestId        NUMBER        NOT NULL,
    OldStatus        VARCHAR2(30)  NULL,
    NewStatus        VARCHAR2(30)  NOT NULL,
    ChangedByStaffId NUMBER        NULL,
    SystemGenerated  NUMBER(1)     DEFAULT 0 NOT NULL,
    ChangedAt        DATE          DEFAULT SYSDATE NOT NULL,
    Note             VARCHAR2(500) NULL,
    CONSTRAINT fk_statuslog_req FOREIGN KEY (RequestId) REFERENCES ZTCRM.ServiceRequest(RequestId),
    CONSTRAINT fk_statuslog_staff FOREIGN KEY (ChangedByStaffId) REFERENCES ZTCRM.Staff(StaffId)
);

CREATE TABLE ZTCRM.RejectionLog (
    RejectionId     NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    RequestId       NUMBER        NOT NULL,
    RejectedBy      NUMBER        NOT NULL,
    RejectionType   VARCHAR2(50)  NOT NULL,
    RejectionReason VARCHAR2(500) NOT NULL,
    RejectedAt      DATE          DEFAULT SYSDATE NOT NULL,
    CONSTRAINT chk_rejection_type CHECK (RejectionType IN ('MissingInfo', 'OutOfScope', 'Duplicate', 'Other')),
    CONSTRAINT fk_rejection_req FOREIGN KEY (RequestId) REFERENCES ZTCRM.ServiceRequest(RequestId),
    CONSTRAINT fk_rejection_staff FOREIGN KEY (RejectedBy) REFERENCES ZTCRM.Staff(StaffId)
);

CREATE TABLE ZTCRM.ApprovalLog (
    ApprovalId NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    RequestId  NUMBER        NOT NULL,
    ActionType VARCHAR2(20)  NOT NULL,
    ActionBy   NUMBER        NOT NULL,
    ActionAt   DATE          DEFAULT SYSDATE NOT NULL,
    Note       VARCHAR2(500) NULL,
    CONSTRAINT chk_approval_type CHECK (ActionType IN ('Approved', 'Rejected')),
    CONSTRAINT fk_approval_req FOREIGN KEY (RequestId) REFERENCES ZTCRM.ServiceRequest(RequestId),
    CONSTRAINT fk_approval_staff FOREIGN KEY (ActionBy) REFERENCES ZTCRM.Staff(StaffId)
);

CREATE TABLE ZTCRM.NotificationLog (
    NotifId    NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    RequestId  NUMBER         NOT NULL,
    CustomerId NUMBER         NOT NULL,
    Channel    VARCHAR2(20)   NOT NULL,
    Message    VARCHAR2(1000) NOT NULL,
    IsSent     NUMBER(1)      DEFAULT 0 NOT NULL,
    SentAt     DATE           NULL,
    CreatedAt  DATE           DEFAULT SYSDATE NOT NULL,
    CONSTRAINT chk_notif_channel CHECK (Channel IN ('SMS', 'Email', 'AppNotification')),
    CONSTRAINT fk_notif_req FOREIGN KEY (RequestId) REFERENCES ZTCRM.ServiceRequest(RequestId),
    CONSTRAINT fk_notif_cust FOREIGN KEY (CustomerId) REFERENCES ZTCRM.Customer(CustomerId)
);

CREATE TABLE ZTCRM.UserPreference (
    PrefId          NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    StaffId         NUMBER        NOT NULL,
    PreferenceKey   VARCHAR2(100) NOT NULL,
    PreferenceValue VARCHAR2(255) NOT NULL,
    UpdatedAt       DATE          DEFAULT SYSDATE NOT NULL,
    CONSTRAINT fk_pref_staff FOREIGN KEY (StaffId) REFERENCES ZTCRM.Staff(StaffId)
);
