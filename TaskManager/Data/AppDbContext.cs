using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using TaskManager.Data.Entities;

namespace TaskManager.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("name=AppDbContext")
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<TaskItem> Tasks { get; set; }
        public DbSet<TaskAttachment> TaskAttachments { get; set; }
        public DbSet<TaskComment> TaskComments { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<TaskHistory> TaskHistory { get; set; }
        public DbSet<TaskStatusHistory> TaskStatusHistory { get; set; }
        public DbSet<TaskAssignment> TaskAssignments { get; set; }
        public DbSet<TaskLabel> TaskLabels { get; set; }
        public DbSet<TaskWatcher> TaskWatchers { get; set; }
        public DbSet<TaskDependency> TaskDependencies { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<TimeLog> TimeLogs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // Disable cascading delete on the two FKs from TaskItem to Employee to avoid
            // "may cause cycles or multiple cascade paths" — EF cannot have two cascade paths
            // from one parent to one child. We delete tasks/users explicitly in services.
            modelBuilder.Entity<TaskItem>()
                .HasRequired(t => t.CreatedBy)
                .WithMany(e => e.CreatedTasks)
                .HasForeignKey(t => t.CreatedById)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TaskItem>()
                .HasOptional(t => t.AssignedTo)
                .WithMany(e => e.AssignedTasks)
                .HasForeignKey(t => t.AssignedToId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TaskItem>()
                .HasOptional(t => t.Project)
                .WithMany(p => p.Tasks)
                .HasForeignKey(t => t.ProjectId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TaskAttachment>()
                .HasRequired(a => a.UploadedBy)
                .WithMany()
                .HasForeignKey(a => a.UploadedById)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TaskComment>()
                .HasRequired(c => c.Author)
                .WithMany()
                .HasForeignKey(c => c.AuthorId)
                .WillCascadeOnDelete(false);

            // Project -> Owner
            modelBuilder.Entity<Project>()
                .HasRequired(p => p.Owner)
                .WithMany()
                .HasForeignKey(p => p.OwnerId)
                .WillCascadeOnDelete(false);

            // TaskHistory -> Employee (ChangedBy)
            modelBuilder.Entity<TaskHistory>()
                .HasRequired(h => h.ChangedBy)
                .WithMany()
                .HasForeignKey(h => h.ChangedById)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TaskHistory>()
                .HasRequired(h => h.TaskItem)
                .WithMany(t => t.History)
                .HasForeignKey(h => h.TaskItemId)
                .WillCascadeOnDelete(false);

            // TaskStatusHistory -> Employee
            modelBuilder.Entity<TaskStatusHistory>()
                .HasRequired(s => s.ChangedBy)
                .WithMany()
                .HasForeignKey(s => s.ChangedById)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TaskStatusHistory>()
                .HasRequired(s => s.TaskItem)
                .WithMany()
                .HasForeignKey(s => s.TaskItemId)
                .WillCascadeOnDelete(false);

            // TaskAssignment -> two Employee FKs
            modelBuilder.Entity<TaskAssignment>()
                .HasRequired(a => a.AssignedTo)
                .WithMany()
                .HasForeignKey(a => a.AssignedToId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TaskAssignment>()
                .HasRequired(a => a.AssignedBy)
                .WithMany()
                .HasForeignKey(a => a.AssignedById)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TaskAssignment>()
                .HasRequired(a => a.TaskItem)
                .WithMany(t => t.Assignments)
                .HasForeignKey(a => a.TaskItemId)
                .WillCascadeOnDelete(false);

            // TaskLabel -> Employee
            modelBuilder.Entity<TaskLabel>()
                .HasRequired(l => l.AddedBy)
                .WithMany()
                .HasForeignKey(l => l.AddedById)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TaskLabel>()
                .HasRequired(l => l.TaskItem)
                .WithMany(t => t.Labels)
                .HasForeignKey(l => l.TaskItemId)
                .WillCascadeOnDelete(false);

            // TaskWatcher -> Employee
            modelBuilder.Entity<TaskWatcher>()
                .HasRequired(w => w.Employee)
                .WithMany()
                .HasForeignKey(w => w.EmployeeId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TaskWatcher>()
                .HasRequired(w => w.TaskItem)
                .WithMany(t => t.Watchers)
                .HasForeignKey(w => w.TaskItemId)
                .WillCascadeOnDelete(false);

            // TaskDependency -> two TaskItem FKs
            modelBuilder.Entity<TaskDependency>()
                .HasRequired(d => d.TaskItem)
                .WithMany()
                .HasForeignKey(d => d.TaskItemId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TaskDependency>()
                .HasRequired(d => d.DependsOnTask)
                .WithMany()
                .HasForeignKey(d => d.DependsOnTaskId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TaskDependency>()
                .HasRequired(d => d.CreatedBy)
                .WithMany()
                .HasForeignKey(d => d.CreatedById)
                .WillCascadeOnDelete(false);

            // Notification -> Employee (Recipient)
            modelBuilder.Entity<Notification>()
                .HasRequired(n => n.Recipient)
                .WithMany()
                .HasForeignKey(n => n.RecipientId)
                .WillCascadeOnDelete(false);

            // TimeLog -> Employee
            modelBuilder.Entity<TimeLog>()
                .HasRequired(tl => tl.Employee)
                .WithMany()
                .HasForeignKey(tl => tl.EmployeeId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TimeLog>()
                .HasRequired(tl => tl.TaskItem)
                .WithMany(t => t.TimeLogs)
                .HasForeignKey(tl => tl.TaskItemId)
                .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }
    }
}
