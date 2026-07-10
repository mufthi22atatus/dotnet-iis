/*
 * TaskManager schema (manual creation script).
 *
 * EF6 Code First will create this automatically on first run via DbInitializer.
 * Use this script if you prefer to provision the DB up-front (production deploys,
 * CI test runs, or read-replica setups).
 *
 * Target: SQL Server 2017+, LocalDB, or SQL Server Express.
 */

IF DB_ID('TaskManagerDb') IS NULL
BEGIN
    CREATE DATABASE TaskManagerDb;
END
GO

USE TaskManagerDb;
GO

-- ----- Employees ---------------------------------------------------------
IF OBJECT_ID('dbo.Employees', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Employees (
        Id              INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        FullName        NVARCHAR(120)  NOT NULL,
        Email           NVARCHAR(160)  NOT NULL,
        PasswordHash    NVARCHAR(256)  NOT NULL,
        PasswordSalt    NVARCHAR(64)   NOT NULL,
        Role            NVARCHAR(32)   NOT NULL DEFAULT N'Employee',
        Department      NVARCHAR(80)   NULL,
        IsActive        BIT            NOT NULL DEFAULT 1,
        CreatedAt       DATETIME2(3)   NOT NULL DEFAULT SYSUTCDATETIME(),
        LastLoginAt     DATETIME2(3)   NULL,
        FailedLoginCount INT           NOT NULL DEFAULT 0
    );

    CREATE UNIQUE INDEX IX_Employees_Email ON dbo.Employees(Email);
END
GO

-- ----- Projects ----------------------------------------------------------
IF OBJECT_ID('dbo.Projects', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Projects (
        Id              INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Name            NVARCHAR(200)  NOT NULL,
        Description     NVARCHAR(2000) NULL,
        OwnerId         INT            NOT NULL,
        CreatedAt       DATETIME2(3)   NOT NULL DEFAULT SYSUTCDATETIME(),
        IsActive        BIT            NOT NULL DEFAULT 1,
        CONSTRAINT FK_Projects_Owner FOREIGN KEY (OwnerId) REFERENCES dbo.Employees(Id)
    );
    CREATE INDEX IX_Projects_Owner ON dbo.Projects(OwnerId);
END
GO

-- ----- Tasks -------------------------------------------------------------
IF OBJECT_ID('dbo.Tasks', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Tasks (
        Id              INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Title           NVARCHAR(200)  NOT NULL,
        Description     NVARCHAR(4000) NULL,
        Status          INT            NOT NULL DEFAULT 0,
        Priority        INT            NOT NULL DEFAULT 1,
        CreatedById     INT            NOT NULL,
        AssignedToId    INT            NULL,
        ProjectId       INT            NULL,
        CreatedAt       DATETIME2(3)   NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt       DATETIME2(3)   NOT NULL DEFAULT SYSUTCDATETIME(),
        DueDate         DATETIME2(3)   NULL,
        CompletedAt     DATETIME2(3)   NULL,
        Tag             NVARCHAR(80)   NULL,
        EstimatedHours  INT            NOT NULL DEFAULT 0,
        LoggedHours     INT            NOT NULL DEFAULT 0,
        CONSTRAINT FK_Tasks_CreatedBy   FOREIGN KEY (CreatedById)  REFERENCES dbo.Employees(Id),
        CONSTRAINT FK_Tasks_AssignedTo  FOREIGN KEY (AssignedToId) REFERENCES dbo.Employees(Id),
        CONSTRAINT FK_Tasks_Project     FOREIGN KEY (ProjectId)    REFERENCES dbo.Projects(Id)
    );
    CREATE INDEX IX_Tasks_Status     ON dbo.Tasks(Status);
    CREATE INDEX IX_Tasks_Assigned   ON dbo.Tasks(AssignedToId);
    CREATE INDEX IX_Tasks_DueDate    ON dbo.Tasks(DueDate);
    CREATE INDEX IX_Tasks_Project    ON dbo.Tasks(ProjectId);
END
GO

-- If Tasks table exists but ProjectId column doesn't, add it
IF OBJECT_ID('dbo.Tasks', 'U') IS NOT NULL
   AND COL_LENGTH('dbo.Tasks', 'ProjectId') IS NULL
BEGIN
    ALTER TABLE dbo.Tasks ADD ProjectId INT NULL;
    ALTER TABLE dbo.Tasks ADD CONSTRAINT FK_Tasks_Project FOREIGN KEY (ProjectId) REFERENCES dbo.Projects(Id);
    CREATE INDEX IX_Tasks_Project ON dbo.Tasks(ProjectId);
END
GO

-- ----- Attachments -------------------------------------------------------
IF OBJECT_ID('dbo.TaskAttachments', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.TaskAttachments (
        Id           INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        TaskItemId   INT            NOT NULL,
        FileName     NVARCHAR(260)  NOT NULL,
        ContentType  NVARCHAR(120)  NOT NULL,
        StoredPath   NVARCHAR(512)  NOT NULL,
        SizeBytes    BIGINT         NOT NULL,
        UploadedById INT            NOT NULL,
        UploadedAt   DATETIME2(3)   NOT NULL DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_Attachments_Task   FOREIGN KEY (TaskItemId)   REFERENCES dbo.Tasks(Id) ON DELETE CASCADE,
        CONSTRAINT FK_Attachments_Uploader FOREIGN KEY (UploadedById) REFERENCES dbo.Employees(Id)
    );
    CREATE INDEX IX_Attachments_Task ON dbo.TaskAttachments(TaskItemId);
END
GO

-- ----- Comments ----------------------------------------------------------
IF OBJECT_ID('dbo.TaskComments', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.TaskComments (
        Id          INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        TaskItemId  INT           NOT NULL,
        AuthorId    INT           NOT NULL,
        Body        NVARCHAR(2000) NOT NULL,
        CreatedAt   DATETIME2(3)  NOT NULL DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_Comments_Task   FOREIGN KEY (TaskItemId) REFERENCES dbo.Tasks(Id) ON DELETE CASCADE,
        CONSTRAINT FK_Comments_Author FOREIGN KEY (AuthorId)   REFERENCES dbo.Employees(Id)
    );
    CREATE INDEX IX_Comments_Task ON dbo.TaskComments(TaskItemId);
END
GO

-- ----- AuditLogs ---------------------------------------------------------
IF OBJECT_ID('dbo.AuditLogs', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.AuditLogs (
        Id           BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        EventType    NVARCHAR(64)   NOT NULL,
        EntityName   NVARCHAR(64)   NULL,
        EntityId     NVARCHAR(64)   NULL,
        ActorId      INT            NULL,
        ActorEmail   NVARCHAR(160)  NULL,
        IpAddress    NVARCHAR(64)   NULL,
        Message      NVARCHAR(512)  NULL,
        PayloadJson  NVARCHAR(4000) NULL,
        CreatedAt    DATETIME2(3)   NOT NULL DEFAULT SYSUTCDATETIME()
    );
    CREATE INDEX IX_AuditLogs_CreatedAt ON dbo.AuditLogs(CreatedAt);
    CREATE INDEX IX_AuditLogs_EventType ON dbo.AuditLogs(EventType);
END
GO

-- ----- TaskHistory -------------------------------------------------------
IF OBJECT_ID('dbo.TaskHistory', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.TaskHistory (
        Id           INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        TaskItemId   INT            NOT NULL,
        FieldName    NVARCHAR(64)   NOT NULL,
        OldValue     NVARCHAR(500)  NULL,
        NewValue     NVARCHAR(500)  NULL,
        ChangedById  INT            NOT NULL,
        ChangedAt    DATETIME2(3)   NOT NULL DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_TaskHistory_Task     FOREIGN KEY (TaskItemId)  REFERENCES dbo.Tasks(Id),
        CONSTRAINT FK_TaskHistory_Employee FOREIGN KEY (ChangedById) REFERENCES dbo.Employees(Id)
    );
    CREATE INDEX IX_TaskHistory_Task      ON dbo.TaskHistory(TaskItemId);
    CREATE INDEX IX_TaskHistory_ChangedAt ON dbo.TaskHistory(ChangedAt);
END
GO

-- ----- TaskStatusHistory -------------------------------------------------
IF OBJECT_ID('dbo.TaskStatusHistory', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.TaskStatusHistory (
        Id           INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        TaskItemId   INT            NOT NULL,
        OldStatus    INT            NOT NULL,
        NewStatus    INT            NOT NULL,
        ChangedById  INT            NOT NULL,
        ChangedAt    DATETIME2(3)   NOT NULL DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_StatusHistory_Task     FOREIGN KEY (TaskItemId)  REFERENCES dbo.Tasks(Id),
        CONSTRAINT FK_StatusHistory_Employee FOREIGN KEY (ChangedById) REFERENCES dbo.Employees(Id)
    );
    CREATE INDEX IX_StatusHistory_Task ON dbo.TaskStatusHistory(TaskItemId);
END
GO

-- ----- TaskAssignments ---------------------------------------------------
IF OBJECT_ID('dbo.TaskAssignments', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.TaskAssignments (
        Id           INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        TaskItemId   INT            NOT NULL,
        AssignedToId INT            NOT NULL,
        AssignedById INT            NOT NULL,
        AssignedAt   DATETIME2(3)   NOT NULL DEFAULT SYSUTCDATETIME(),
        UnassignedAt DATETIME2(3)   NULL,
        IsActive     BIT            NOT NULL DEFAULT 1,
        CONSTRAINT FK_Assignments_Task     FOREIGN KEY (TaskItemId)   REFERENCES dbo.Tasks(Id),
        CONSTRAINT FK_Assignments_Assignee FOREIGN KEY (AssignedToId) REFERENCES dbo.Employees(Id),
        CONSTRAINT FK_Assignments_Assigner FOREIGN KEY (AssignedById) REFERENCES dbo.Employees(Id)
    );
    CREATE INDEX IX_Assignments_Task   ON dbo.TaskAssignments(TaskItemId);
    CREATE INDEX IX_Assignments_Active ON dbo.TaskAssignments(IsActive);
END
GO

-- ----- TaskLabels --------------------------------------------------------
IF OBJECT_ID('dbo.TaskLabels', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.TaskLabels (
        Id           INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        TaskItemId   INT            NOT NULL,
        Label        NVARCHAR(80)   NOT NULL,
        AddedById    INT            NOT NULL,
        AddedAt      DATETIME2(3)   NOT NULL DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_Labels_Task     FOREIGN KEY (TaskItemId) REFERENCES dbo.Tasks(Id),
        CONSTRAINT FK_Labels_Employee FOREIGN KEY (AddedById)  REFERENCES dbo.Employees(Id)
    );
    CREATE INDEX IX_Labels_Task  ON dbo.TaskLabels(TaskItemId);
    CREATE INDEX IX_Labels_Label ON dbo.TaskLabels(Label);
END
GO

-- ----- TaskWatchers ------------------------------------------------------
IF OBJECT_ID('dbo.TaskWatchers', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.TaskWatchers (
        Id           INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        TaskItemId   INT            NOT NULL,
        EmployeeId   INT            NOT NULL,
        WatchingSince DATETIME2(3)  NOT NULL DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_Watchers_Task     FOREIGN KEY (TaskItemId) REFERENCES dbo.Tasks(Id),
        CONSTRAINT FK_Watchers_Employee FOREIGN KEY (EmployeeId) REFERENCES dbo.Employees(Id)
    );
    CREATE INDEX IX_Watchers_Task ON dbo.TaskWatchers(TaskItemId);
    CREATE UNIQUE INDEX IX_Watchers_Unique ON dbo.TaskWatchers(TaskItemId, EmployeeId);
END
GO

-- ----- TaskDependencies --------------------------------------------------
IF OBJECT_ID('dbo.TaskDependencies', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.TaskDependencies (
        Id              INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        TaskItemId      INT            NOT NULL,
        DependsOnTaskId INT            NOT NULL,
        DependencyType  NVARCHAR(32)   NOT NULL DEFAULT N'BlockedBy',
        CreatedById     INT            NOT NULL,
        CreatedAt       DATETIME2(3)   NOT NULL DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_Dependencies_Task      FOREIGN KEY (TaskItemId)      REFERENCES dbo.Tasks(Id),
        CONSTRAINT FK_Dependencies_DependsOn FOREIGN KEY (DependsOnTaskId) REFERENCES dbo.Tasks(Id),
        CONSTRAINT FK_Dependencies_Creator   FOREIGN KEY (CreatedById)     REFERENCES dbo.Employees(Id)
    );
    CREATE INDEX IX_Dependencies_Task ON dbo.TaskDependencies(TaskItemId);
    CREATE UNIQUE INDEX IX_Dependencies_Unique ON dbo.TaskDependencies(TaskItemId, DependsOnTaskId);
END
GO

-- ----- Notifications -----------------------------------------------------
IF OBJECT_ID('dbo.Notifications', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Notifications (
        Id                 BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        RecipientId        INT            NOT NULL,
        Type               NVARCHAR(64)   NOT NULL,
        Title              NVARCHAR(200)  NOT NULL,
        Message            NVARCHAR(1000) NULL,
        RelatedEntityType  NVARCHAR(64)   NULL,
        RelatedEntityId    NVARCHAR(64)   NULL,
        IsRead             BIT            NOT NULL DEFAULT 0,
        CreatedAt          DATETIME2(3)   NOT NULL DEFAULT SYSUTCDATETIME(),
        ReadAt             DATETIME2(3)   NULL,
        CONSTRAINT FK_Notifications_Recipient FOREIGN KEY (RecipientId) REFERENCES dbo.Employees(Id)
    );
    CREATE INDEX IX_Notifications_Recipient ON dbo.Notifications(RecipientId);
    CREATE INDEX IX_Notifications_CreatedAt ON dbo.Notifications(CreatedAt);
END
GO

-- ----- TimeLogs ----------------------------------------------------------
IF OBJECT_ID('dbo.TimeLogs', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.TimeLogs (
        Id              INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        TaskItemId      INT            NOT NULL,
        EmployeeId      INT            NOT NULL,
        StartedAt       DATETIME2(3)   NOT NULL DEFAULT SYSUTCDATETIME(),
        StoppedAt       DATETIME2(3)   NULL,
        DurationMinutes INT            NOT NULL DEFAULT 0,
        Description     NVARCHAR(500)  NULL,
        CONSTRAINT FK_TimeLogs_Task     FOREIGN KEY (TaskItemId) REFERENCES dbo.Tasks(Id),
        CONSTRAINT FK_TimeLogs_Employee FOREIGN KEY (EmployeeId) REFERENCES dbo.Employees(Id)
    );
    CREATE INDEX IX_TimeLogs_Task     ON dbo.TimeLogs(TaskItemId);
    CREATE INDEX IX_TimeLogs_Employee ON dbo.TimeLogs(EmployeeId);
END
GO

PRINT 'TaskManager schema ready.';
