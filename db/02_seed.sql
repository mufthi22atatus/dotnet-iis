/*
 * Seed data for TaskManager — large dataset for rich APM tracing.
 *
 * Default credentials:
 *   admin@taskmanager.local      Admin@12345     (Admin)
 *   manager@taskmanager.local    Manager@12345   (Manager)
 *   alice@taskmanager.local      Alice@12345     (Employee)
 *   bob@taskmanager.local        Bob@12345       (Employee)
 *   charlie@taskmanager.local    Charlie@12345   (Employee)
 *   diana@taskmanager.local      Diana@12345     (Employee)
 *   evan@taskmanager.local       Evan@12345      (Employee)
 *   fiona@taskmanager.local      Fiona@12345     (Manager)
 *   george@taskmanager.local     George@12345    (Employee)
 *   hema@taskmanager.local       Hema@12345      (Employee)
 */

USE TaskManagerDb;
GO

-- ===== EMPLOYEES (10 users) =====
IF NOT EXISTS (SELECT 1 FROM dbo.Employees WHERE Email = 'admin@taskmanager.local')
BEGIN
    INSERT INTO dbo.Employees (FullName, Email, PasswordHash, PasswordSalt, Role, Department, IsActive, CreatedAt)
    VALUES
      (N'Asha Admin',       N'admin@taskmanager.local',   N'__SET_BY_INITIALIZER__', N'__SET_BY_INITIALIZER__', N'Admin',    N'Operations',  1, SYSUTCDATETIME()),
      (N'Mihir Manager',    N'manager@taskmanager.local', N'__SET_BY_INITIALIZER__', N'__SET_BY_INITIALIZER__', N'Manager',  N'Engineering', 1, SYSUTCDATETIME()),
      (N'Alice Anderson',   N'alice@taskmanager.local',   N'__SET_BY_INITIALIZER__', N'__SET_BY_INITIALIZER__', N'Employee', N'Engineering', 1, SYSUTCDATETIME()),
      (N'Bob Bhatt',        N'bob@taskmanager.local',     N'__SET_BY_INITIALIZER__', N'__SET_BY_INITIALIZER__', N'Employee', N'Engineering', 1, SYSUTCDATETIME()),
      (N'Charlie Chen',     N'charlie@taskmanager.local', N'__SET_BY_INITIALIZER__', N'__SET_BY_INITIALIZER__', N'Employee', N'QA',          1, SYSUTCDATETIME()),
      (N'Diana Das',        N'diana@taskmanager.local',   N'__SET_BY_INITIALIZER__', N'__SET_BY_INITIALIZER__', N'Employee', N'Design',      1, SYSUTCDATETIME()),
      (N'Evan Edwards',     N'evan@taskmanager.local',    N'__SET_BY_INITIALIZER__', N'__SET_BY_INITIALIZER__', N'Employee', N'Engineering', 1, SYSUTCDATETIME()),
      (N'Fiona Fernandez',  N'fiona@taskmanager.local',   N'__SET_BY_INITIALIZER__', N'__SET_BY_INITIALIZER__', N'Manager',  N'QA',          1, SYSUTCDATETIME()),
      (N'George Gupta',     N'george@taskmanager.local',  N'__SET_BY_INITIALIZER__', N'__SET_BY_INITIALIZER__', N'Employee', N'Operations',  1, SYSUTCDATETIME()),
      (N'Hema Hegde',       N'hema@taskmanager.local',    N'__SET_BY_INITIALIZER__', N'__SET_BY_INITIALIZER__', N'Employee', N'Design',      1, SYSUTCDATETIME());
END
GO

DECLARE @adminId  INT = (SELECT Id FROM dbo.Employees WHERE Email = 'admin@taskmanager.local');
DECLARE @mgrId    INT = (SELECT Id FROM dbo.Employees WHERE Email = 'manager@taskmanager.local');
DECLARE @aliceId  INT = (SELECT Id FROM dbo.Employees WHERE Email = 'alice@taskmanager.local');
DECLARE @bobId    INT = (SELECT Id FROM dbo.Employees WHERE Email = 'bob@taskmanager.local');
DECLARE @charId   INT = (SELECT Id FROM dbo.Employees WHERE Email = 'charlie@taskmanager.local');
DECLARE @dianaId  INT = (SELECT Id FROM dbo.Employees WHERE Email = 'diana@taskmanager.local');
DECLARE @evanId   INT = (SELECT Id FROM dbo.Employees WHERE Email = 'evan@taskmanager.local');
DECLARE @fionaId  INT = (SELECT Id FROM dbo.Employees WHERE Email = 'fiona@taskmanager.local');
DECLARE @georgeId INT = (SELECT Id FROM dbo.Employees WHERE Email = 'george@taskmanager.local');
DECLARE @hemaId   INT = (SELECT Id FROM dbo.Employees WHERE Email = 'hema@taskmanager.local');

-- ===== PROJECTS (5 projects) =====
IF NOT EXISTS (SELECT 1 FROM dbo.Projects)
BEGIN
    INSERT INTO dbo.Projects (Name, Description, OwnerId, CreatedAt, IsActive)
    VALUES
      (N'Platform Modernization', N'Migrate legacy monolith to microservices architecture.',         @mgrId,   DATEADD(day, -60, SYSUTCDATETIME()), 1),
      (N'Mobile App v2',          N'React Native mobile application redesign and feature additions.', @adminId, DATEADD(day, -45, SYSUTCDATETIME()), 1),
      (N'DevOps Pipeline',        N'CI/CD pipeline improvements and infrastructure automation.',      @adminId, DATEADD(day, -30, SYSUTCDATETIME()), 1),
      (N'Customer Portal',        N'Self-service customer portal with dashboards and reporting.',     @fionaId, DATEADD(day, -20, SYSUTCDATETIME()), 1),
      (N'Internal Tools',         N'Admin dashboards, monitoring tools, and developer utilities.',    @mgrId,   DATEADD(day, -90, SYSUTCDATETIME()), 1);
END
GO

DECLARE @projPlatform INT = (SELECT Id FROM dbo.Projects WHERE Name = 'Platform Modernization');
DECLARE @projMobile   INT = (SELECT Id FROM dbo.Projects WHERE Name = 'Mobile App v2');
DECLARE @projDevOps   INT = (SELECT Id FROM dbo.Projects WHERE Name = 'DevOps Pipeline');
DECLARE @projPortal   INT = (SELECT Id FROM dbo.Projects WHERE Name = 'Customer Portal');
DECLARE @projTools    INT = (SELECT Id FROM dbo.Projects WHERE Name = 'Internal Tools');

-- Re-declare user IDs for this batch
DECLARE @adminId2  INT = (SELECT Id FROM dbo.Employees WHERE Email = 'admin@taskmanager.local');
DECLARE @mgrId2    INT = (SELECT Id FROM dbo.Employees WHERE Email = 'manager@taskmanager.local');
DECLARE @aliceId2  INT = (SELECT Id FROM dbo.Employees WHERE Email = 'alice@taskmanager.local');
DECLARE @bobId2    INT = (SELECT Id FROM dbo.Employees WHERE Email = 'bob@taskmanager.local');
DECLARE @charId2   INT = (SELECT Id FROM dbo.Employees WHERE Email = 'charlie@taskmanager.local');
DECLARE @dianaId2  INT = (SELECT Id FROM dbo.Employees WHERE Email = 'diana@taskmanager.local');
DECLARE @evanId2   INT = (SELECT Id FROM dbo.Employees WHERE Email = 'evan@taskmanager.local');
DECLARE @fionaId2  INT = (SELECT Id FROM dbo.Employees WHERE Email = 'fiona@taskmanager.local');
DECLARE @georgeId2 INT = (SELECT Id FROM dbo.Employees WHERE Email = 'george@taskmanager.local');
DECLARE @hemaId2   INT = (SELECT Id FROM dbo.Employees WHERE Email = 'hema@taskmanager.local');

-- ===== TASKS (30 tasks across projects, statuses, priorities) =====
IF NOT EXISTS (SELECT 1 FROM dbo.Tasks)
BEGIN
    INSERT INTO dbo.Tasks (Title, Description, Status, Priority, CreatedById, AssignedToId, ProjectId,
                            CreatedAt, UpdatedAt, DueDate, CompletedAt, Tag, EstimatedHours, LoggedHours)
    VALUES
      -- Platform Modernization (project 1)
      (N'Wire up nightly DB backup',          N'Configure SQL Agent job + offsite copy.',                1, 2, @mgrId2,   @aliceId2, @projPlatform, DATEADD(day,-30,SYSUTCDATETIME()), DATEADD(day,-1,SYSUTCDATETIME()), DATEADD(day,3,SYSUTCDATETIME()),  NULL, N'infra',      6, 4),
      (N'Fix login redirect loop on Edge',    N'Cookie suspect. Repro: log in, hit /, get bounced.',    0, 3, @aliceId2, @bobId2,   @projPlatform, DATEADD(day,-25,SYSUTCDATETIME()), DATEADD(day,-2,SYSUTCDATETIME()), DATEADD(day,1,SYSUTCDATETIME()),  NULL, N'bug',        3, 0),
      (N'Onboard Charlie to QA pipeline',     N'Selenium grid setup walkthrough.',                      0, 1, @mgrId2,   @charId2,  @projPlatform, DATEADD(day,-20,SYSUTCDATETIME()), DATEADD(day,-5,SYSUTCDATETIME()), DATEADD(day,7,SYSUTCDATETIME()),  NULL, N'onboarding', 8, 2),
      (N'Q3 reporting dashboard',             N'Stakeholder review by EOQ.',                            3, 2, @adminId2, @mgrId2,   @projPlatform, DATEADD(day,-15,SYSUTCDATETIME()), DATEADD(day,-1,SYSUTCDATETIME()), DATEADD(day,14,SYSUTCDATETIME()), NULL, N'reporting',  20, 12),
      (N'Document IIS deploy runbook',        N'Step-by-step for new ops hires.',                       4, 0, @adminId2, @bobId2,   @projPlatform, DATEADD(day,-40,SYSUTCDATETIME()), DATEADD(day,-3,SYSUTCDATETIME()), DATEADD(day,-2,SYSUTCDATETIME()), DATEADD(day,-3,SYSUTCDATETIME()), N'docs', 4, 5),
      (N'API rate limiting middleware',        N'Implement sliding window rate limiter for public APIs.',1, 2, @mgrId2,   @evanId2,  @projPlatform, DATEADD(day,-10,SYSUTCDATETIME()), DATEADD(day,-1,SYSUTCDATETIME()), DATEADD(day,5,SYSUTCDATETIME()),  NULL, N'security',   12, 6),
      (N'Database index optimization',         N'Analyze slow queries and add missing indexes.',        2, 3, @adminId2, @aliceId2, @projPlatform, DATEADD(day,-8,SYSUTCDATETIME()),  DATEADD(day,-1,SYSUTCDATETIME()), DATEADD(day,2,SYSUTCDATETIME()),  NULL, N'performance',8, 3),
      -- Mobile App v2 (project 2)
      (N'Redesign login screen',               N'New UX with biometric auth support.',                  1, 2, @fionaId2, @dianaId2, @projMobile,   DATEADD(day,-18,SYSUTCDATETIME()), DATEADD(day,-2,SYSUTCDATETIME()), DATEADD(day,10,SYSUTCDATETIME()), NULL, N'design',     16, 8),
      (N'Push notification service',           N'FCM + APNs integration for real-time alerts.',         0, 2, @mgrId2,   @evanId2,  @projMobile,   DATEADD(day,-12,SYSUTCDATETIME()), DATEADD(day,-1,SYSUTCDATETIME()), DATEADD(day,8,SYSUTCDATETIME()),  NULL, N'backend',    20, 0),
      (N'Offline mode data sync',              N'SQLite local cache with conflict resolution.',         0, 3, @mgrId2,   @bobId2,   @projMobile,   DATEADD(day,-7,SYSUTCDATETIME()),  DATEADD(day,-1,SYSUTCDATETIME()), DATEADD(day,15,SYSUTCDATETIME()), NULL, N'feature',    30, 0),
      (N'App store screenshots',               N'Create new marketing screenshots for v2 launch.',      4, 0, @fionaId2, @hemaId2,  @projMobile,   DATEADD(day,-30,SYSUTCDATETIME()), DATEADD(day,-10,SYSUTCDATETIME()),DATEADD(day,-5,SYSUTCDATETIME()), DATEADD(day,-10,SYSUTCDATETIME()),N'design', 6, 7),
      (N'Performance profiling',               N'Profile app startup and screen transitions.',          1, 1, @fionaId2, @charId2,  @projMobile,   DATEADD(day,-5,SYSUTCDATETIME()),  DATEADD(day,-1,SYSUTCDATETIME()), DATEADD(day,12,SYSUTCDATETIME()), NULL, N'qa',         10, 4),
      -- DevOps Pipeline (project 3)
      (N'Terraform state migration',           N'Move from local to S3 backend.',                       4, 2, @adminId2, @georgeId2,@projDevOps,   DATEADD(day,-25,SYSUTCDATETIME()), DATEADD(day,-8,SYSUTCDATETIME()), DATEADD(day,-5,SYSUTCDATETIME()), DATEADD(day,-8,SYSUTCDATETIME()), N'infra', 8, 10),
      (N'GitHub Actions CI pipeline',          N'Replace Jenkins with GitHub Actions.',                 1, 2, @adminId2, @georgeId2,@projDevOps,   DATEADD(day,-15,SYSUTCDATETIME()), DATEADD(day,-1,SYSUTCDATETIME()), DATEADD(day,7,SYSUTCDATETIME()),  NULL, N'ci-cd',      15, 8),
      (N'Docker image size reduction',         N'Multi-stage builds, distroless base images.',          0, 1, @mgrId2,   @evanId2,  @projDevOps,   DATEADD(day,-5,SYSUTCDATETIME()),  DATEADD(day,-1,SYSUTCDATETIME()), DATEADD(day,20,SYSUTCDATETIME()), NULL, N'optimization',6, 0),
      (N'Secrets management with Vault',       N'HashiCorp Vault for all env secrets.',                 0, 3, @adminId2, @georgeId2,@projDevOps,   DATEADD(day,-3,SYSUTCDATETIME()),  DATEADD(day,-1,SYSUTCDATETIME()), DATEADD(day,10,SYSUTCDATETIME()), NULL, N'security',   12, 0),
      (N'Monitoring dashboard in Grafana',     N'Create dashboards for API latency and error rates.',   3, 1, @adminId2, @aliceId2, @projDevOps,   DATEADD(day,-20,SYSUTCDATETIME()), DATEADD(day,-2,SYSUTCDATETIME()), DATEADD(day,3,SYSUTCDATETIME()),  NULL, N'monitoring', 10, 7),
      -- Customer Portal (project 4)
      (N'Customer login OAuth2',               N'Google + Microsoft SSO integration.',                  1, 2, @fionaId2, @bobId2,   @projPortal,   DATEADD(day,-14,SYSUTCDATETIME()), DATEADD(day,-1,SYSUTCDATETIME()), DATEADD(day,7,SYSUTCDATETIME()),  NULL, N'auth',       14, 6),
      (N'Invoice PDF generation',              N'Generate branded invoice PDFs from order data.',       0, 1, @fionaId2, @dianaId2, @projPortal,   DATEADD(day,-10,SYSUTCDATETIME()), DATEADD(day,-1,SYSUTCDATETIME()), DATEADD(day,14,SYSUTCDATETIME()), NULL, N'feature',    10, 0),
      (N'Customer feedback widget',            N'Embedded NPS survey and feedback form.',               5, 0, @fionaId2, @hemaId2,  @projPortal,   DATEADD(day,-30,SYSUTCDATETIME()), DATEADD(day,-15,SYSUTCDATETIME()),DATEADD(day,-10,SYSUTCDATETIME()),NULL, N'ux',         5, 2),
      (N'Usage analytics dashboard',           N'Customer-facing usage metrics and charts.',            0, 2, @fionaId2, @aliceId2, @projPortal,   DATEADD(day,-6,SYSUTCDATETIME()),  DATEADD(day,-1,SYSUTCDATETIME()), DATEADD(day,12,SYSUTCDATETIME()), NULL, N'reporting',  18, 0),
      -- Internal Tools (project 5)
      (N'Admin user management page',          N'CRUD for employee accounts, role assignment.',         4, 1, @mgrId2,   @bobId2,   @projTools,    DATEADD(day,-45,SYSUTCDATETIME()), DATEADD(day,-20,SYSUTCDATETIME()),DATEADD(day,-15,SYSUTCDATETIME()),DATEADD(day,-20,SYSUTCDATETIME()),N'admin', 8, 9),
      (N'System health check endpoint',        N'Deep health checks for DB, cache, external deps.',    4, 2, @adminId2, @georgeId2,@projTools,    DATEADD(day,-35,SYSUTCDATETIME()), DATEADD(day,-18,SYSUTCDATETIME()),DATEADD(day,-12,SYSUTCDATETIME()),DATEADD(day,-18,SYSUTCDATETIME()),N'monitoring',4, 5),
      (N'Log aggregation viewer',              N'Web-based log search for Serilog structured logs.',    2, 1, @mgrId2,   @evanId2,  @projTools,    DATEADD(day,-12,SYSUTCDATETIME()), DATEADD(day,-1,SYSUTCDATETIME()), DATEADD(day,10,SYSUTCDATETIME()), NULL, N'tooling',    15, 5),
      (N'Bulk data import utility',            N'CSV/Excel import for customers and orders.',           0, 1, @mgrId2,   @aliceId2, @projTools,    DATEADD(day,-4,SYSUTCDATETIME()),  DATEADD(day,-1,SYSUTCDATETIME()), DATEADD(day,21,SYSUTCDATETIME()), NULL, N'tooling',    12, 0),
      -- Unassigned project tasks
      (N'Update employee handbook',            N'Annual review of policies and procedures.',            0, 0, @adminId2, @hemaId2,  NULL,          DATEADD(day,-10,SYSUTCDATETIME()), DATEADD(day,-1,SYSUTCDATETIME()), DATEADD(day,30,SYSUTCDATETIME()), NULL, N'docs',       6, 0),
      (N'Security audit preparation',          N'Prepare documentation for annual SOC2 audit.',         1, 3, @adminId2, @georgeId2,NULL,          DATEADD(day,-7,SYSUTCDATETIME()),  DATEADD(day,-1,SYSUTCDATETIME()), DATEADD(day,5,SYSUTCDATETIME()),  NULL, N'compliance', 20, 10),
      (N'Team retrospective action items',     N'Follow up on Q2 retro outcomes.',                     0, 1, @mgrId2,   @fionaId2, NULL,          DATEADD(day,-3,SYSUTCDATETIME()),  DATEADD(day,-1,SYSUTCDATETIME()), DATEADD(day,14,SYSUTCDATETIME()), NULL, N'process',    4, 0),
      (N'Conference talk proposal',            N'Submit talk on distributed tracing at .NET Conf.',     0, 0, @aliceId2, @aliceId2, NULL,          DATEADD(day,-2,SYSUTCDATETIME()),  DATEADD(day,-1,SYSUTCDATETIME()), DATEADD(day,45,SYSUTCDATETIME()), NULL, N'community',  8, 0),
      (N'Overdue legacy cleanup',              N'Remove deprecated API endpoints from v1.',             0, 2, @mgrId2,   @evanId2,  @projPlatform, DATEADD(day,-20,SYSUTCDATETIME()), DATEADD(day,-5,SYSUTCDATETIME()), DATEADD(day,-3,SYSUTCDATETIME()), NULL, N'cleanup',    6, 0);
END
GO

-- ===== TASK ASSIGNMENTS =====
-- Re-declare user IDs
DECLARE @a INT = (SELECT Id FROM dbo.Employees WHERE Email = 'admin@taskmanager.local');
DECLARE @m INT = (SELECT Id FROM dbo.Employees WHERE Email = 'manager@taskmanager.local');
DECLARE @al INT = (SELECT Id FROM dbo.Employees WHERE Email = 'alice@taskmanager.local');
DECLARE @bo INT = (SELECT Id FROM dbo.Employees WHERE Email = 'bob@taskmanager.local');
DECLARE @ch INT = (SELECT Id FROM dbo.Employees WHERE Email = 'charlie@taskmanager.local');
DECLARE @di INT = (SELECT Id FROM dbo.Employees WHERE Email = 'diana@taskmanager.local');
DECLARE @ev INT = (SELECT Id FROM dbo.Employees WHERE Email = 'evan@taskmanager.local');
DECLARE @fi INT = (SELECT Id FROM dbo.Employees WHERE Email = 'fiona@taskmanager.local');
DECLARE @ge INT = (SELECT Id FROM dbo.Employees WHERE Email = 'george@taskmanager.local');
DECLARE @he INT = (SELECT Id FROM dbo.Employees WHERE Email = 'hema@taskmanager.local');

IF NOT EXISTS (SELECT 1 FROM dbo.TaskAssignments)
BEGIN
    -- Current assignments (matching AssignedToId on Tasks)
    INSERT INTO dbo.TaskAssignments (TaskItemId, AssignedToId, AssignedById, AssignedAt, IsActive)
    SELECT t.Id, t.AssignedToId, t.CreatedById, t.CreatedAt, 1
    FROM dbo.Tasks t WHERE t.AssignedToId IS NOT NULL;
END
GO

-- ===== TASK LABELS =====
DECLARE @a2 INT = (SELECT Id FROM dbo.Employees WHERE Email = 'admin@taskmanager.local');
DECLARE @m2 INT = (SELECT Id FROM dbo.Employees WHERE Email = 'manager@taskmanager.local');

IF NOT EXISTS (SELECT 1 FROM dbo.TaskLabels)
BEGIN
    INSERT INTO dbo.TaskLabels (TaskItemId, Label, AddedById, AddedAt)
    SELECT t.Id, t.Tag, t.CreatedById, t.CreatedAt FROM dbo.Tasks t WHERE t.Tag IS NOT NULL;

    -- Add extra labels to some tasks
    INSERT INTO dbo.TaskLabels (TaskItemId, Label, AddedById, AddedAt)
    SELECT TOP 10 t.Id, N'urgent', @m2, SYSUTCDATETIME()
    FROM dbo.Tasks t WHERE t.Priority >= 2 ORDER BY t.Id;

    INSERT INTO dbo.TaskLabels (TaskItemId, Label, AddedById, AddedAt)
    SELECT TOP 8 t.Id, N'review-needed', @a2, SYSUTCDATETIME()
    FROM dbo.Tasks t WHERE t.Status IN (3) ORDER BY t.Id;
END
GO

-- ===== TASK WATCHERS =====
DECLARE @a3 INT = (SELECT Id FROM dbo.Employees WHERE Email = 'admin@taskmanager.local');
DECLARE @m3 INT = (SELECT Id FROM dbo.Employees WHERE Email = 'manager@taskmanager.local');
DECLARE @al3 INT = (SELECT Id FROM dbo.Employees WHERE Email = 'alice@taskmanager.local');
DECLARE @fi3 INT = (SELECT Id FROM dbo.Employees WHERE Email = 'fiona@taskmanager.local');

IF NOT EXISTS (SELECT 1 FROM dbo.TaskWatchers)
BEGIN
    -- Managers watch high-priority tasks
    INSERT INTO dbo.TaskWatchers (TaskItemId, EmployeeId, WatchingSince)
    SELECT TOP 10 t.Id, @m3, DATEADD(day, -5, SYSUTCDATETIME())
    FROM dbo.Tasks t WHERE t.Priority >= 2 ORDER BY t.Id;

    INSERT INTO dbo.TaskWatchers (TaskItemId, EmployeeId, WatchingSince)
    SELECT TOP 8 t.Id, @a3, DATEADD(day, -3, SYSUTCDATETIME())
    FROM dbo.Tasks t WHERE t.Priority = 3 ORDER BY t.Id;

    -- Creators watch their own tasks
    INSERT INTO dbo.TaskWatchers (TaskItemId, EmployeeId, WatchingSince)
    SELECT TOP 10 t.Id, t.CreatedById, t.CreatedAt
    FROM dbo.Tasks t
    WHERE NOT EXISTS (SELECT 1 FROM dbo.TaskWatchers w WHERE w.TaskItemId = t.Id AND w.EmployeeId = t.CreatedById)
    ORDER BY t.Id;
END
GO

-- ===== TASK COMMENTS =====
DECLARE @al4 INT = (SELECT Id FROM dbo.Employees WHERE Email = 'alice@taskmanager.local');
DECLARE @bo4 INT = (SELECT Id FROM dbo.Employees WHERE Email = 'bob@taskmanager.local');
DECLARE @ch4 INT = (SELECT Id FROM dbo.Employees WHERE Email = 'charlie@taskmanager.local');
DECLARE @m4  INT = (SELECT Id FROM dbo.Employees WHERE Email = 'manager@taskmanager.local');
DECLARE @a4  INT = (SELECT Id FROM dbo.Employees WHERE Email = 'admin@taskmanager.local');
DECLARE @ev4 INT = (SELECT Id FROM dbo.Employees WHERE Email = 'evan@taskmanager.local');

IF NOT EXISTS (SELECT 1 FROM dbo.TaskComments)
BEGIN
    INSERT INTO dbo.TaskComments (TaskItemId, AuthorId, Body, CreatedAt)
    SELECT TOP 1 Id, @al4, N'Started working on this. Initial analysis looks straightforward.', DATEADD(day,-28,SYSUTCDATETIME()) FROM dbo.Tasks ORDER BY Id;

    INSERT INTO dbo.TaskComments (TaskItemId, AuthorId, Body, CreatedAt)
    SELECT TOP 1 Id, @m4, N'Please prioritize this — it is blocking the release.', DATEADD(day,-24,SYSUTCDATETIME()) FROM dbo.Tasks WHERE Priority = 3 ORDER BY Id;

    INSERT INTO dbo.TaskComments (TaskItemId, AuthorId, Body, CreatedAt)
    SELECT TOP 1 Id, @bo4, N'Found the root cause. The session cookie is not setting SameSite=Lax correctly on Edge.', DATEADD(day,-22,SYSUTCDATETIME()) FROM dbo.Tasks WHERE Priority = 3 ORDER BY Id;

    INSERT INTO dbo.TaskComments (TaskItemId, AuthorId, Body, CreatedAt)
    SELECT TOP 1 Id, @ch4, N'QA environment is set up. Ready for testing.', DATEADD(day,-18,SYSUTCDATETIME()) FROM dbo.Tasks WHERE Tag = N'onboarding' ORDER BY Id;

    INSERT INTO dbo.TaskComments (TaskItemId, AuthorId, Body, CreatedAt)
    SELECT TOP 1 Id, @a4, N'Reviewed and approved. Moving to staging deployment.', DATEADD(day,-14,SYSUTCDATETIME()) FROM dbo.Tasks WHERE Status = 3 ORDER BY Id;

    INSERT INTO dbo.TaskComments (TaskItemId, AuthorId, Body, CreatedAt)
    SELECT TOP 1 Id, @ev4, N'Initial implementation complete. Need code review before merge.', DATEADD(day,-8,SYSUTCDATETIME()) FROM dbo.Tasks WHERE Tag = N'security' ORDER BY Id;

    -- Add more comments across various tasks
    INSERT INTO dbo.TaskComments (TaskItemId, AuthorId, Body, CreatedAt)
    SELECT TOP 5 t.Id, @m4, N'Weekly check-in: any blockers on this task?', DATEADD(day,-3,SYSUTCDATETIME())
    FROM dbo.Tasks t WHERE t.Status IN (0,1) ORDER BY t.Id;

    INSERT INTO dbo.TaskComments (TaskItemId, AuthorId, Body, CreatedAt)
    SELECT TOP 5 t.Id, t.AssignedToId, N'Making good progress. Should be done by end of week.', DATEADD(day,-1,SYSUTCDATETIME())
    FROM dbo.Tasks t WHERE t.Status = 1 AND t.AssignedToId IS NOT NULL ORDER BY t.Id;
END
GO

-- ===== TASK HISTORY =====
DECLARE @m5 INT = (SELECT Id FROM dbo.Employees WHERE Email = 'manager@taskmanager.local');
DECLARE @a5 INT = (SELECT Id FROM dbo.Employees WHERE Email = 'admin@taskmanager.local');

IF NOT EXISTS (SELECT 1 FROM dbo.TaskHistory)
BEGIN
    -- Record creation history for all tasks
    INSERT INTO dbo.TaskHistory (TaskItemId, FieldName, OldValue, NewValue, ChangedById, ChangedAt)
    SELECT t.Id, N'Created', NULL, t.Title, t.CreatedById, t.CreatedAt FROM dbo.Tasks t;

    -- Record status changes for in-progress tasks
    INSERT INTO dbo.TaskHistory (TaskItemId, FieldName, OldValue, NewValue, ChangedById, ChangedAt)
    SELECT t.Id, N'Status', N'Open', N'InProgress', ISNULL(t.AssignedToId, t.CreatedById), DATEADD(day, -10, SYSUTCDATETIME())
    FROM dbo.Tasks t WHERE t.Status = 1;

    -- Record completed tasks
    INSERT INTO dbo.TaskHistory (TaskItemId, FieldName, OldValue, NewValue, ChangedById, ChangedAt)
    SELECT t.Id, N'Status', N'InProgress', N'Done', ISNULL(t.AssignedToId, t.CreatedById), ISNULL(t.CompletedAt, SYSUTCDATETIME())
    FROM dbo.Tasks t WHERE t.Status = 4;
END
GO

-- ===== TASK STATUS HISTORY =====
DECLARE @m6 INT = (SELECT Id FROM dbo.Employees WHERE Email = 'manager@taskmanager.local');

IF NOT EXISTS (SELECT 1 FROM dbo.TaskStatusHistory)
BEGIN
    INSERT INTO dbo.TaskStatusHistory (TaskItemId, OldStatus, NewStatus, ChangedById, ChangedAt)
    SELECT t.Id, 0, 1, ISNULL(t.AssignedToId, t.CreatedById), DATEADD(day, -10, SYSUTCDATETIME())
    FROM dbo.Tasks t WHERE t.Status >= 1 AND t.Status != 5;

    INSERT INTO dbo.TaskStatusHistory (TaskItemId, OldStatus, NewStatus, ChangedById, ChangedAt)
    SELECT t.Id, 1, 4, ISNULL(t.AssignedToId, t.CreatedById), ISNULL(t.CompletedAt, SYSUTCDATETIME())
    FROM dbo.Tasks t WHERE t.Status = 4;
END
GO

-- ===== TIME LOGS =====
DECLARE @al7 INT = (SELECT Id FROM dbo.Employees WHERE Email = 'alice@taskmanager.local');
DECLARE @bo7 INT = (SELECT Id FROM dbo.Employees WHERE Email = 'bob@taskmanager.local');
DECLARE @ev7 INT = (SELECT Id FROM dbo.Employees WHERE Email = 'evan@taskmanager.local');
DECLARE @ge7 INT = (SELECT Id FROM dbo.Employees WHERE Email = 'george@taskmanager.local');

IF NOT EXISTS (SELECT 1 FROM dbo.TimeLogs)
BEGIN
    INSERT INTO dbo.TimeLogs (TaskItemId, EmployeeId, StartedAt, StoppedAt, DurationMinutes, Description)
    SELECT TOP 5 t.Id, t.AssignedToId,
           DATEADD(hour, -8, SYSUTCDATETIME()), DATEADD(hour, -4, SYSUTCDATETIME()), 240,
           N'Morning coding session'
    FROM dbo.Tasks t WHERE t.AssignedToId IS NOT NULL AND t.LoggedHours > 0 ORDER BY t.Id;

    INSERT INTO dbo.TimeLogs (TaskItemId, EmployeeId, StartedAt, StoppedAt, DurationMinutes, Description)
    SELECT TOP 5 t.Id, t.AssignedToId,
           DATEADD(hour, -3, SYSUTCDATETIME()), DATEADD(hour, -1, SYSUTCDATETIME()), 120,
           N'Afternoon debugging session'
    FROM dbo.Tasks t WHERE t.AssignedToId IS NOT NULL AND t.LoggedHours > 0 ORDER BY t.Id;

    INSERT INTO dbo.TimeLogs (TaskItemId, EmployeeId, StartedAt, StoppedAt, DurationMinutes, Description)
    SELECT TOP 3 t.Id, t.AssignedToId,
           DATEADD(day, -2, SYSUTCDATETIME()), DATEADD(day, -2, DATEADD(hour, 6, SYSUTCDATETIME())), 360,
           N'Full day on feature implementation'
    FROM dbo.Tasks t WHERE t.AssignedToId IS NOT NULL AND t.EstimatedHours >= 10 ORDER BY t.Id;
END
GO

-- ===== TASK DEPENDENCIES =====
DECLARE @m8 INT = (SELECT Id FROM dbo.Employees WHERE Email = 'manager@taskmanager.local');

IF NOT EXISTS (SELECT 1 FROM dbo.TaskDependencies)
BEGIN
    DECLARE @taskCount INT = (SELECT COUNT(*) FROM dbo.Tasks);
    IF @taskCount >= 10
    BEGIN
        -- Task 2 blocked by task 1
        INSERT INTO dbo.TaskDependencies (TaskItemId, DependsOnTaskId, DependencyType, CreatedById)
        SELECT TOP 1 t2.Id, t1.Id, N'BlockedBy', @m8
        FROM (SELECT Id, ROW_NUMBER() OVER (ORDER BY Id) AS rn FROM dbo.Tasks) t1
        CROSS JOIN (SELECT Id, ROW_NUMBER() OVER (ORDER BY Id) AS rn FROM dbo.Tasks) t2
        WHERE t1.rn = 1 AND t2.rn = 2;

        -- Task 7 blocked by task 6
        INSERT INTO dbo.TaskDependencies (TaskItemId, DependsOnTaskId, DependencyType, CreatedById)
        SELECT TOP 1 t2.Id, t1.Id, N'BlockedBy', @m8
        FROM (SELECT Id, ROW_NUMBER() OVER (ORDER BY Id) AS rn FROM dbo.Tasks) t1
        CROSS JOIN (SELECT Id, ROW_NUMBER() OVER (ORDER BY Id) AS rn FROM dbo.Tasks) t2
        WHERE t1.rn = 6 AND t2.rn = 7;

        -- Task 10 depends on task 9
        INSERT INTO dbo.TaskDependencies (TaskItemId, DependsOnTaskId, DependencyType, CreatedById)
        SELECT TOP 1 t2.Id, t1.Id, N'RelatedTo', @m8
        FROM (SELECT Id, ROW_NUMBER() OVER (ORDER BY Id) AS rn FROM dbo.Tasks) t1
        CROSS JOIN (SELECT Id, ROW_NUMBER() OVER (ORDER BY Id) AS rn FROM dbo.Tasks) t2
        WHERE t1.rn = 9 AND t2.rn = 10;
    END
END
GO

-- ===== NOTIFICATIONS =====
DECLARE @al9 INT = (SELECT Id FROM dbo.Employees WHERE Email = 'alice@taskmanager.local');
DECLARE @bo9 INT = (SELECT Id FROM dbo.Employees WHERE Email = 'bob@taskmanager.local');
DECLARE @ch9 INT = (SELECT Id FROM dbo.Employees WHERE Email = 'charlie@taskmanager.local');
DECLARE @m9  INT = (SELECT Id FROM dbo.Employees WHERE Email = 'manager@taskmanager.local');

IF NOT EXISTS (SELECT 1 FROM dbo.Notifications)
BEGIN
    INSERT INTO dbo.Notifications (RecipientId, Type, Title, Message, RelatedEntityType, RelatedEntityId, IsRead, CreatedAt)
    SELECT t.AssignedToId, N'task.assigned', N'Task Assigned', N'You have been assigned to: ' + t.Title,
           N'TaskItem', CAST(t.Id AS NVARCHAR(64)), 0, t.CreatedAt
    FROM dbo.Tasks t WHERE t.AssignedToId IS NOT NULL;

    INSERT INTO dbo.Notifications (RecipientId, Type, Title, Message, IsRead, CreatedAt, ReadAt)
    VALUES
      (@m9,  N'system.info',    N'Weekly Report Ready',     N'Your weekly productivity report is available.',  1, DATEADD(day,-7,SYSUTCDATETIME()), DATEADD(day,-6,SYSUTCDATETIME())),
      (@al9, N'task.commented',  N'New Comment on Your Task', N'Manager left a comment on your task.',          0, DATEADD(day,-2,SYSUTCDATETIME()), NULL),
      (@bo9, N'task.due_soon',   N'Task Due Soon',           N'Your task is due in 24 hours.',                  0, DATEADD(day,-1,SYSUTCDATETIME()), NULL);
END
GO

-- ===== AUDIT LOGS =====
DECLARE @a10 INT = (SELECT Id FROM dbo.Employees WHERE Email = 'admin@taskmanager.local');
DECLARE @m10 INT = (SELECT Id FROM dbo.Employees WHERE Email = 'manager@taskmanager.local');

IF NOT EXISTS (SELECT 1 FROM dbo.AuditLogs WHERE EventType = 'system.seed')
BEGIN
    INSERT INTO dbo.AuditLogs (EventType, ActorId, ActorEmail, Message, EntityName, EntityId, CreatedAt)
    VALUES
      (N'system.seed',    NULL, N'system',                    N'Database seeded with sample data',     NULL,       NULL, DATEADD(day,-60,SYSUTCDATETIME())),
      (N'user.login',     @a10, N'admin@taskmanager.local',   N'Admin logged in',                      N'Employee', CAST(@a10 AS NVARCHAR), DATEADD(day,-30,SYSUTCDATETIME())),
      (N'user.login',     @m10, N'manager@taskmanager.local', N'Manager logged in',                    N'Employee', CAST(@m10 AS NVARCHAR), DATEADD(day,-25,SYSUTCDATETIME())),
      (N'task.create',    @m10, N'manager@taskmanager.local', N'Created task: Wire up nightly DB backup', N'TaskItem', N'1', DATEADD(day,-30,SYSUTCDATETIME())),
      (N'task.assign',    @m10, N'manager@taskmanager.local', N'Assigned task to Alice',               N'TaskItem', N'1', DATEADD(day,-30,SYSUTCDATETIME())),
      (N'task.status',    @a10, N'admin@taskmanager.local',   N'Status changed to InProgress',         N'TaskItem', N'1', DATEADD(day,-28,SYSUTCDATETIME())),
      (N'task.create',    @a10, N'admin@taskmanager.local',   N'Created task: Q3 reporting dashboard', N'TaskItem', N'4', DATEADD(day,-15,SYSUTCDATETIME())),
      (N'task.status',    @m10, N'manager@taskmanager.local', N'Status changed to Done',               N'TaskItem', N'5', DATEADD(day,-3,SYSUTCDATETIME())),
      (N'task.create',    @m10, N'manager@taskmanager.local', N'Created multiple tasks for sprint',    N'TaskItem', NULL, DATEADD(day,-10,SYSUTCDATETIME())),
      (N'user.login',     @a10, N'admin@taskmanager.local',   N'Admin logged in',                      N'Employee', CAST(@a10 AS NVARCHAR), DATEADD(day,-1,SYSUTCDATETIME()));
END
GO

PRINT 'Seed data applied.';
