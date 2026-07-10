using System;
using System.Data.Entity;
using System.Linq;
using Microsoft.Extensions.Logging;
using TaskManager.Data.Entities;
using TaskManager.Services;

namespace TaskManager.Data
{
    /// <summary>
    /// Creates the schema if missing and seeds a baseline set of employees + tasks so the
    /// app is usable on first run with no manual SQL.
    /// </summary>
    public class DbInitializer : CreateDatabaseIfNotExists<AppDbContext>
    {
        protected override void Seed(AppDbContext context)
        {
            AppLogger.Create<DbInitializer>()?.LogInformation("Seeding database with baseline data");

            var hasher = new PasswordHasher();

            var admin = NewEmployee(hasher, "Asha Admin", "admin@taskmanager.local", "Admin@12345", "Admin", "Operations");
            var manager = NewEmployee(hasher, "Mihir Manager", "manager@taskmanager.local", "Manager@12345", "Manager", "Engineering");
            var alice = NewEmployee(hasher, "Alice Anderson", "alice@taskmanager.local", "Alice@12345", "Employee", "Engineering");
            var bob = NewEmployee(hasher, "Bob Bhatt", "bob@taskmanager.local", "Bob@12345", "Employee", "Engineering");
            var charlie = NewEmployee(hasher, "Charlie Chen", "charlie@taskmanager.local", "Charlie@12345", "Employee", "QA");
            var diana = NewEmployee(hasher, "Diana Das", "diana@taskmanager.local", "Diana@12345", "Employee", "Design");
            var evan = NewEmployee(hasher, "Evan Edwards", "evan@taskmanager.local", "Evan@12345", "Employee", "Engineering");
            var fiona = NewEmployee(hasher, "Fiona Fernandez", "fiona@taskmanager.local", "Fiona@12345", "Manager", "QA");
            var george = NewEmployee(hasher, "George Gupta", "george@taskmanager.local", "George@12345", "Employee", "Operations");
            var hema = NewEmployee(hasher, "Hema Hegde", "hema@taskmanager.local", "Hema@12345", "Employee", "Design");

            context.Employees.AddRange(new[] { admin, manager, alice, bob, charlie, diana, evan, fiona, george, hema });
            context.SaveChanges();

            var now = DateTime.UtcNow;

            var p1 = new Project { Name = "Platform Modernization", Description = "Migrate legacy monolith to microservices architecture.", OwnerId = manager.Id, IsActive = true, CreatedAt = now.AddDays(-60) };
            var p2 = new Project { Name = "Mobile App v2", Description = "React Native mobile application redesign and feature additions.", OwnerId = admin.Id, IsActive = true, CreatedAt = now.AddDays(-45) };
            var p3 = new Project { Name = "DevOps Pipeline", Description = "CI/CD pipeline improvements and infrastructure automation.", OwnerId = admin.Id, IsActive = true, CreatedAt = now.AddDays(-30) };
            var p4 = new Project { Name = "Customer Portal", Description = "Self-service customer portal with dashboards and reporting.", OwnerId = fiona.Id, IsActive = true, CreatedAt = now.AddDays(-20) };
            var p5 = new Project { Name = "Internal Tools", Description = "Admin dashboards, monitoring tools, and developer utilities.", OwnerId = manager.Id, IsActive = true, CreatedAt = now.AddDays(-90) };

            context.Projects.AddRange(new[] { p1, p2, p3, p4, p5 });
            context.SaveChanges();

            context.Tasks.AddRange(new[]
            {
                new TaskItem {
                    Title = "Wire up nightly DB backup",
                    Description = "Configure SQL Agent job + offsite copy.",
                    CreatedById = manager.Id, AssignedToId = alice.Id,
                    Priority = TaskPriority.High, Status = TaskStatus.InProgress,
                    DueDate = now.AddDays(3), Tag = "infra", EstimatedHours = 6,
                    ProjectId = p1.Id
                },
                new TaskItem {
                    Title = "Fix login redirect loop on Edge",
                    Description = "Repro: log in, hit /, get bounced. Cookie suspect.",
                    CreatedById = alice.Id, AssignedToId = bob.Id,
                    Priority = TaskPriority.Critical, Status = TaskStatus.Open,
                    DueDate = now.AddDays(1), Tag = "bug", EstimatedHours = 3,
                    ProjectId = p1.Id
                },
                new TaskItem {
                    Title = "Onboard Charlie to QA pipeline",
                    Description = "Run him through Selenium grid setup.",
                    CreatedById = manager.Id, AssignedToId = charlie.Id,
                    Priority = TaskPriority.Medium, Status = TaskStatus.Open,
                    DueDate = now.AddDays(7), Tag = "onboarding", EstimatedHours = 8,
                    ProjectId = p1.Id
                },
                new TaskItem {
                    Title = "Q3 reporting dashboard",
                    Description = "Stakeholder review by EOQ.",
                    CreatedById = admin.Id, AssignedToId = manager.Id,
                    Priority = TaskPriority.High, Status = TaskStatus.InReview,
                    DueDate = now.AddDays(14), Tag = "reporting", EstimatedHours = 20,
                    ProjectId = p1.Id
                },
                new TaskItem {
                    Title = "Document IIS deploy runbook",
                    Description = "Step-by-step for new ops hires.",
                    CreatedById = admin.Id, AssignedToId = bob.Id,
                    Priority = TaskPriority.Low, Status = TaskStatus.Done,
                    DueDate = now.AddDays(-2), CompletedAt = now.AddDays(-1),
                    Tag = "docs", EstimatedHours = 4, LoggedHours = 5,
                    ProjectId = p1.Id
                },
            });

            context.AuditLogs.Add(new AuditLog
            {
                EventType = "system.seed",
                Message = "Initial database seed completed",
                ActorEmail = "system",
                CreatedAt = DateTime.UtcNow
            });

            context.SaveChanges();
            AppLogger.Create<DbInitializer>()?.LogInformation("Seed complete: {Users} users, {Projects} projects, {Tasks} tasks",
                context.Employees.Count(), context.Projects.Count(), context.Tasks.Count());
        }

        private static Employee NewEmployee(PasswordHasher hasher, string name, string email,
            string password, string role, string department)
        {
            var (hash, salt) = hasher.Hash(password);
            return new Employee
            {
                FullName = name,
                Email = email,
                PasswordHash = hash,
                PasswordSalt = salt,
                Role = role,
                Department = department,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
