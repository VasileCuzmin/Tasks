using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tasks.Definition.Domain.Entities;

namespace Tasks.Domain.Data.EntityTypeConfiguration
{
    internal class TaskDefinitionConfiguration : IEntityTypeConfiguration<TaskDefinition>
    {
        public void Configure(EntityTypeBuilder<TaskDefinition> builder)
        {
            builder.ToTable("TaskDefinition").HasKey(x => new { x.TaskDefinitionId });
            builder.Property(c => c.Name);
            builder.Property(c => c.ProcessDefinitionId);
            builder.Property(c => c.StartEventDefinitionId);
            builder.Property(c => c.EndEventDefinitionId);
            builder.Property(c => c.CancelEventDefinitionId);
            builder.Property(c => c.StartExpression);
            builder.Property(c => c.EndExpression);
            builder.Property(c => c.CancelExpression);
            builder.Property(c => c.GroupAllocationExpression);
            builder.Property(c => c.UserAllocationExpression);
            builder.Property(c => c.AutomaticStart);
            builder.HasOne(c => c.StartEventDefinition)
                .WithMany()
                .HasForeignKey(td => td.StartEventDefinitionId);

            builder.HasOne(c => c.EndEventDefinition)
                .WithMany()
                .HasForeignKey(td => td.EndEventDefinitionId);

            builder.HasOne(c => c.CancelEventDefinition)
                .WithMany()
                .HasForeignKey(c => c.CancelEventDefinitionId);
        }
    }
}