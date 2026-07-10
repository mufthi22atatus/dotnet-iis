/*
 * Docker SQL Server initialization script for TaskManager.
 *
 * This script is executed once when the SQL Server container is first provisioned.
 * It creates the TaskManagerDb database, all tables, and populates with rich
 * sample data for the WebForms SqlQueries page and the existing MVC app.
 *
 * SA Password: TaskMgr@2024!
 * Database:    TaskManagerDb
 */

-- ============================================================================
-- 1. CREATE DATABASE
-- ============================================================================
IF DB_ID('TaskManagerDb') IS NULL
BEGIN
    CREATE DATABASE TaskManagerDb;
END
GO

USE TaskManagerDb;
GO

-- ============================================================================
-- 2. CREATE TABLES
-- ============================================================================

-- ----- Employees -----------------------------------------------------------
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

-- ----- Tasks ---------------------------------------------------------------
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
        CreatedAt       DATETIME2(3)   NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt       DATETIME2(3)   NOT NULL DEFAULT SYSUTCDATETIME(),
        DueDate         DATETIME2(3)   NULL,
        CompletedAt     DATETIME2(3)   NULL,
        Tag             NVARCHAR(80)   NULL,
        EstimatedHours  INT            NOT NULL DEFAULT 0,
        LoggedHours     INT            NOT NULL DEFAULT 0,
        CONSTRAINT FK_Tasks_CreatedBy   FOREIGN KEY (CreatedById)  REFERENCES dbo.Employees(Id),
        CONSTRAINT FK_Tasks_AssignedTo  FOREIGN KEY (AssignedToId) REFERENCES dbo.Employees(Id)
    );
    CREATE INDEX IX_Tasks_Status     ON dbo.Tasks(Status);
    CREATE INDEX IX_Tasks_Assigned   ON dbo.Tasks(AssignedToId);
    CREATE INDEX IX_Tasks_DueDate    ON dbo.Tasks(DueDate);
END
GO

-- ----- Attachments ---------------------------------------------------------
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
        CONSTRAINT FK_Attachments_Task     FOREIGN KEY (TaskItemId)   REFERENCES dbo.Tasks(Id) ON DELETE CASCADE,
        CONSTRAINT FK_Attachments_Uploader FOREIGN KEY (UploadedById) REFERENCES dbo.Employees(Id)
    );
    CREATE INDEX IX_Attachments_Task ON dbo.TaskAttachments(TaskItemId);
END
GO

-- ----- Comments ------------------------------------------------------------
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

-- ----- AuditLogs -----------------------------------------------------------
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

-- ============================================================================
-- 3. SEED DATA — Employees (10 users)
-- ============================================================================
-- PasswordHash and PasswordSalt are placeholders. The EF Code-First DbInitializer
-- will overwrite these on first application run. For the SQL Queries WebForms page,
-- only the display columns matter.

IF NOT EXISTS (SELECT 1 FROM dbo.Employees WHERE Email = 'admin@taskmanager.local')
BEGIN
    INSERT INTO dbo.Employees (FullName, Email, PasswordHash, PasswordSalt, Role, Department, IsActive, CreatedAt)
    VALUES
      (N'Asha Admin',       N'admin@taskmanager.local',     N'DOCKER_SEED_HASH', N'DOCKER_SEED_SALT', N'Admin',    N'Operations',   1, DATEADD(day, -90, SYSUTCDATETIME())),
      (N'Mihir Manager',    N'manager@taskmanager.local',   N'DOCKER_SEED_HASH', N'DOCKER_SEED_SALT', N'Manager',  N'Engineering',  1, DATEADD(day, -85, SYSUTCDATETIME())),
      (N'Alice Anderson',   N'alice@taskmanager.local',     N'DOCKER_SEED_HASH', N'DOCKER_SEED_SALT', N'Employee', N'Engineering',  1, DATEADD(day, -80, SYSUTCDATETIME())),
      (N'Bob Bhatt',        N'bob@taskmanager.local',       N'DOCKER_SEED_HASH', N'DOCKER_SEED_SALT', N'Employee', N'Engineering',  1, DATEADD(day, -75, SYSUTCDATETIME())),
      (N'Charlie Chen',     N'charlie@taskmanager.local',   N'DOCKER_SEED_HASH', N'DOCKER_SEED_SALT', N'Employee', N'QA',           1, DATEADD(day, -70, SYSUTCDATETIME())),
      (N'Diana Davis',      N'diana@taskmanager.local',     N'DOCKER_SEED_HASH', N'DOCKER_SEED_SALT', N'Employee', N'Design',       1, DATEADD(day, -60, SYSUTCDATETIME())),
      (N'Ethan Evans',      N'ethan@taskmanager.local',     N'DOCKER_SEED_HASH', N'DOCKER_SEED_SALT', N'Manager',  N'QA',           1, DATEADD(day, -55, SYSUTCDATETIME())),
      (N'Fiona Fischer',    N'fiona@taskmanager.local',     N'DOCKER_SEED_HASH', N'DOCKER_SEED_SALT', N'Employee', N'Operations',   1, DATEADD(day, -50, SYSUTCDATETIME())),
      (N'George Garcia',    N'george@taskmanager.local',    N'DOCKER_SEED_HASH', N'DOCKER_SEED_SALT', N'Employee', N'Engineering',  1, DATEADD(day, -45, SYSUTCDATETIME())),
      (N'Helen Harper',     N'helen@taskmanager.local',     N'DOCKER_SEED_HASH', N'DOCKER_SEED_SALT', N'Employee', N'Design',       0, DATEADD(day, -30, SYSUTCDATETIME()));
END
GO

-- ============================================================================
-- 4. SEED DATA — Tasks (15 tasks)
-- ============================================================================
DECLARE @adminId INT = (SELECT Id FROM dbo.Employees WHERE Email = 'admin@taskmanager.local');
DECLARE @mgrId   INT = (SELECT Id FROM dbo.Employees WHERE Email = 'manager@taskmanager.local');
DECLARE @aliceId INT = (SELECT Id FROM dbo.Employees WHERE Email = 'alice@taskmanager.local');
DECLARE @bobId   INT = (SELECT Id FROM dbo.Employees WHERE Email = 'bob@taskmanager.local');
DECLARE @charId  INT = (SELECT Id FROM dbo.Employees WHERE Email = 'charlie@taskmanager.local');
DECLARE @dianaId INT = (SELECT Id FROM dbo.Employees WHERE Email = 'diana@taskmanager.local');
DECLARE @ethanId INT = (SELECT Id FROM dbo.Employees WHERE Email = 'ethan@taskmanager.local');
DECLARE @fionaId INT = (SELECT Id FROM dbo.Employees WHERE Email = 'fiona@taskmanager.local');
DECLARE @georgeId INT = (SELECT Id FROM dbo.Employees WHERE Email = 'george@taskmanager.local');

IF NOT EXISTS (SELECT 1 FROM dbo.Tasks)
BEGIN
    INSERT INTO dbo.Tasks (Title, Description, Status, Priority, CreatedById, AssignedToId, CreatedAt, UpdatedAt, DueDate, Tag, EstimatedHours, LoggedHours)
    VALUES
      -- Open tasks
      (N'Wire up nightly DB backup',          N'Configure SQL Agent job + offsite copy to Azure Blob.',
        1, 2, @mgrId,    @aliceId,  DATEADD(day, -14, SYSUTCDATETIME()), DATEADD(day, -2, SYSUTCDATETIME()),  DATEADD(day,  3, SYSUTCDATETIME()),  N'infra',      6,  2),
      (N'Fix login redirect loop on Edge',    N'Cookie suspect. Repro: log in, hit /, get bounced.',
        0, 3, @aliceId,  @bobId,    DATEADD(day, -10, SYSUTCDATETIME()), DATEADD(day, -1, SYSUTCDATETIME()),  DATEADD(day,  1, SYSUTCDATETIME()),  N'bug',        3,  1),
      (N'Onboard Charlie to QA pipeline',     N'Selenium grid setup walkthrough + CI integration.',
        0, 1, @mgrId,    @charId,   DATEADD(day,  -7, SYSUTCDATETIME()), DATEADD(day, -1, SYSUTCDATETIME()),  DATEADD(day,  7, SYSUTCDATETIME()),  N'onboarding', 8,  0),
      (N'Q3 reporting dashboard',             N'Stakeholder review by EOQ. Use Power BI embedded.',
        3, 2, @adminId,  @mgrId,    DATEADD(day, -30, SYSUTCDATETIME()), DATEADD(day, -5, SYSUTCDATETIME()),  DATEADD(day, 14, SYSUTCDATETIME()),  N'reporting',  20, 12),
      -- Completed tasks
      (N'Document IIS deploy runbook',        N'Step-by-step for new ops hires.',
        4, 0, @adminId,  @bobId,    DATEADD(day, -45, SYSUTCDATETIME()), DATEADD(day,-10, SYSUTCDATETIME()),  DATEADD(day, -2, SYSUTCDATETIME()),  N'docs',       4,  5),
      (N'Set up CI/CD pipeline',              N'GitHub Actions → build → test → deploy to staging.',
        4, 2, @mgrId,    @aliceId,  DATEADD(day, -60, SYSUTCDATETIME()), DATEADD(day,-20, SYSUTCDATETIME()),  DATEADD(day,-15, SYSUTCDATETIME()),  N'devops',     16, 18),
      -- Overdue tasks (past due, not completed)
      (N'Security audit for Q2',              N'Run OWASP ZAP scan + manual pen-test review.',
        1, 3, @adminId,  @ethanId,  DATEADD(day, -25, SYSUTCDATETIME()), DATEADD(day, -3, SYSUTCDATETIME()),  DATEADD(day, -5, SYSUTCDATETIME()),  N'security',   12, 4),
      (N'Migrate legacy CSS to Bootstrap 5',  N'Old pages still use Bootstrap 3. Update + test.',
        0, 1, @dianaId,  @dianaId,  DATEADD(day, -20, SYSUTCDATETIME()), DATEADD(day, -8, SYSUTCDATETIME()),  DATEADD(day, -3, SYSUTCDATETIME()),  N'frontend',   10, 2),
      (N'API rate limiting implementation',   N'Add rate limiting middleware for /api/* endpoints.',
        2, 2, @mgrId,    @georgeId, DATEADD(day, -18, SYSUTCDATETIME()), DATEADD(day, -4, SYSUTCDATETIME()),  DATEADD(day, -1, SYSUTCDATETIME()),  N'backend',    8,  3),
      -- More active tasks
      (N'Design new dashboard mockups',       N'Create Figma mockups for the analytics dashboard.',
        1, 1, @ethanId,  @dianaId,  DATEADD(day,  -5, SYSUTCDATETIME()), DATEADD(day, -1, SYSUTCDATETIME()),  DATEADD(day, 10, SYSUTCDATETIME()),  N'design',     6,  2),
      (N'Write unit tests for AuthService',   N'Cover all auth flows: login, register, lockout, reset.',
        0, 2, @mgrId,    @charId,   DATEADD(day,  -3, SYSUTCDATETIME()), DATEADD(day, -1, SYSUTCDATETIME()),  DATEADD(day,  5, SYSUTCDATETIME()),  N'testing',    5,  0),
      (N'Optimize slow dashboard query',      N'Dashboard aggregation takes >2s. Profile and add indexes.',
        1, 3, @adminId,  @aliceId,  DATEADD(day,  -2, SYSUTCDATETIME()), DATEADD(day,  0, SYSUTCDATETIME()),  DATEADD(day,  2, SYSUTCDATETIME()),  N'perf',       4,  1),
      (N'Update NuGet packages',              N'Bump EF6, Newtonsoft, Serilog to latest stable.',
        0, 0, @fionaId,  @fionaId,  DATEADD(day,  -1, SYSUTCDATETIME()), DATEADD(day,  0, SYSUTCDATETIME()),  DATEADD(day, 14, SYSUTCDATETIME()),  N'maintenance',2,  0),
      (N'Implement file preview feature',     N'Allow inline preview of images and PDFs in task details.',
        0, 1, @mgrId,    @georgeId, DATEADD(day,  -4, SYSUTCDATETIME()), DATEADD(day, -2, SYSUTCDATETIME()),  DATEADD(day,  8, SYSUTCDATETIME()),  N'feature',    10, 0),
      -- Cancelled task
      (N'Evaluate NoSQL migration',           N'Explore MongoDB as alternative. CANCELLED — staying with SQL Server.',
        5, 0, @adminId,  @mgrId,    DATEADD(day, -40, SYSUTCDATETIME()), DATEADD(day,-35, SYSUTCDATETIME()),  NULL,                                 N'research',   8,  3);
END
GO

-- ============================================================================
-- 5. SEED DATA — Task Comments (10 comments)
-- ============================================================================
DECLARE @adminId2 INT = (SELECT Id FROM dbo.Employees WHERE Email = 'admin@taskmanager.local');
DECLARE @mgrId2   INT = (SELECT Id FROM dbo.Employees WHERE Email = 'manager@taskmanager.local');
DECLARE @aliceId2 INT = (SELECT Id FROM dbo.Employees WHERE Email = 'alice@taskmanager.local');
DECLARE @bobId2   INT = (SELECT Id FROM dbo.Employees WHERE Email = 'bob@taskmanager.local');
DECLARE @charId2  INT = (SELECT Id FROM dbo.Employees WHERE Email = 'charlie@taskmanager.local');

IF NOT EXISTS (SELECT 1 FROM dbo.TaskComments)
BEGIN
    INSERT INTO dbo.TaskComments (TaskItemId, AuthorId, Body, CreatedAt)
    VALUES
      (1, @aliceId2, N'Started investigating the SQL Agent job configuration. Need SA access to the production server.', DATEADD(day, -12, SYSUTCDATETIME())),
      (1, @mgrId2,   N'SA access granted. Please document the backup schedule once configured.', DATEADD(day, -11, SYSUTCDATETIME())),
      (2, @bobId2,   N'Confirmed the issue is SameSite cookie attribute. Edge Chromium requires Secure flag.', DATEADD(day, -8, SYSUTCDATETIME())),
      (2, @aliceId2, N'Good catch. Can you also check Firefox and Safari while at it?', DATEADD(day, -7, SYSUTCDATETIME())),
      (4, @mgrId2,   N'Power BI license approved. Setting up the embedded workspace now.', DATEADD(day, -20, SYSUTCDATETIME())),
      (4, @adminId2, N'Please include YoY comparison charts. Stakeholders specifically asked for trend analysis.', DATEADD(day, -18, SYSUTCDATETIME())),
      (7, @charId2,  N'OWASP ZAP scan completed. Found 3 medium-severity issues, adding to backlog.', DATEADD(day, -6, SYSUTCDATETIME())),
      (9, @mgrId2,   N'Blocked on this — the rate limiting library has a dependency conflict with our Newtonsoft version.', DATEADD(day, -5, SYSUTCDATETIME())),
      (12, @aliceId2, N'Identified the bottleneck: missing index on Tasks(Status, AssignedToId). Adding composite index.', DATEADD(day, -1, SYSUTCDATETIME())),
      (5, @bobId2,   N'Runbook is complete and published to the internal wiki. Closing this task.', DATEADD(day, -12, SYSUTCDATETIME()));
END
GO

-- ============================================================================
-- 6. SEED DATA — Task Attachments (5 records)
-- ============================================================================
DECLARE @aliceId3 INT = (SELECT Id FROM dbo.Employees WHERE Email = 'alice@taskmanager.local');
DECLARE @bobId3   INT = (SELECT Id FROM dbo.Employees WHERE Email = 'bob@taskmanager.local');
DECLARE @dianaId3 INT = (SELECT Id FROM dbo.Employees WHERE Email = 'diana@taskmanager.local');

IF NOT EXISTS (SELECT 1 FROM dbo.TaskAttachments)
BEGIN
    INSERT INTO dbo.TaskAttachments (TaskItemId, FileName, ContentType, StoredPath, SizeBytes, UploadedById, UploadedAt)
    VALUES
      (1, N'backup-config.json',    N'application/json', N'~/Uploads/1/backup-config.json',    2048,    @aliceId3, DATEADD(day, -10, SYSUTCDATETIME())),
      (4, N'q3-dashboard-draft.pdf',N'application/pdf',  N'~/Uploads/4/q3-dashboard-draft.pdf',1048576, @bobId3,   DATEADD(day, -15, SYSUTCDATETIME())),
      (5, N'iis-deploy-runbook.docx', N'application/vnd.openxmlformats-officedocument.wordprocessingml.document', N'~/Uploads/5/iis-deploy-runbook.docx', 524288, @bobId3, DATEADD(day, -14, SYSUTCDATETIME())),
      (10, N'dashboard-mockup-v1.png', N'image/png',     N'~/Uploads/10/dashboard-mockup-v1.png', 2097152, @dianaId3, DATEADD(day, -3, SYSUTCDATETIME())),
      (7, N'owasp-scan-report.html',   N'text/html',     N'~/Uploads/7/owasp-scan-report.html',  65536,   @aliceId3, DATEADD(day, -4, SYSUTCDATETIME()));
END
GO

-- ============================================================================
-- 7. SEED DATA — Audit Logs (12 entries)
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM dbo.AuditLogs)
BEGIN
    INSERT INTO dbo.AuditLogs (EventType, EntityName, EntityId, ActorId, ActorEmail, IpAddress, Message, CreatedAt)
    VALUES
      (N'Login',        N'Employee', N'1', 1, N'admin@taskmanager.local',   N'192.168.1.10', N'Admin logged in successfully',         DATEADD(hour, -2, SYSUTCDATETIME())),
      (N'Login',        N'Employee', N'2', 2, N'manager@taskmanager.local', N'192.168.1.11', N'Manager logged in successfully',       DATEADD(hour, -3, SYSUTCDATETIME())),
      (N'TaskCreated',  N'Task',     N'12',1, N'admin@taskmanager.local',   N'192.168.1.10', N'Created task: Optimize slow dashboard query', DATEADD(day, -2, SYSUTCDATETIME())),
      (N'TaskUpdated',  N'Task',     N'1', 3, N'alice@taskmanager.local',   N'192.168.1.12', N'Updated task status to InProgress',    DATEADD(day, -2, SYSUTCDATETIME())),
      (N'TaskUpdated',  N'Task',     N'5', 4, N'bob@taskmanager.local',     N'192.168.1.13', N'Marked task as Done',                  DATEADD(day, -10, SYSUTCDATETIME())),
      (N'FileUploaded', N'Attachment',N'1',3, N'alice@taskmanager.local',   N'192.168.1.12', N'Uploaded backup-config.json to task #1', DATEADD(day, -10, SYSUTCDATETIME())),
      (N'Login',        N'Employee', N'5', 5, N'charlie@taskmanager.local', N'192.168.1.14', N'Charlie logged in successfully',       DATEADD(hour, -5, SYSUTCDATETIME())),
      (N'LoginFailed',  N'Employee', N'10',NULL, N'helen@taskmanager.local',N'192.168.1.20', N'Login failed — account deactivated',   DATEADD(hour, -4, SYSUTCDATETIME())),
      (N'UserUpdated',  N'Employee', N'10',1, N'admin@taskmanager.local',   N'192.168.1.10', N'Deactivated user Helen Harper',        DATEADD(day, -30, SYSUTCDATETIME())),
      (N'TaskCreated',  N'Task',     N'14',2, N'manager@taskmanager.local', N'192.168.1.11', N'Created task: Implement file preview feature', DATEADD(day, -4, SYSUTCDATETIME())),
      (N'CommentAdded', N'Comment',  N'9', 3, N'alice@taskmanager.local',   N'192.168.1.12', N'Added comment on task #12 about index optimization', DATEADD(day, -1, SYSUTCDATETIME())),
      (N'Logout',       N'Employee', N'1', 1, N'admin@taskmanager.local',   N'192.168.1.10', N'Admin logged out',                     DATEADD(hour, -1, SYSUTCDATETIME()));
END
GO

PRINT 'TaskManagerDb Docker initialization complete — schema + seed data applied.';
GO
