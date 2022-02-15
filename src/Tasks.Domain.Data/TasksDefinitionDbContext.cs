using Microsoft.EntityFrameworkCore;
using Tasks.Definition.Domain.Entities;
using Tasks.Domain.Data.EntityTypeConfiguration;

namespace Tasks.Domain.Data
{
    public class TasksDefinitionDbContext : DbContext
    {
        public DbSet<EventDefinition> EventDefinitions { get; set; }
        public DbSet<ProcessDefinition> ProcessDefinitions { get; set; }
        public DbSet<ProcessEventDefinition> ProcessEventDefinitions { get; set; }
        public DbSet<TaskDefinition> TaskDefinitions { get; set; }
        public DbSet<Application> Applications { get; set; }

        public TasksDefinitionDbContext(DbContextOptions<TasksDefinitionDbContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new ProcessDefinitionConfiguration());
            modelBuilder.ApplyConfiguration(new ProcessEventDefinitionConfiguration());
            modelBuilder.ApplyConfiguration(new EventDefinitionConfiguration());
            modelBuilder.ApplyConfiguration(new ApplicationConfiguration());
            modelBuilder.ApplyConfiguration(new TaskDefinitionConfiguration());
        }
    }
}