using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tasks.Definition.Domain.Entities;

namespace Tasks.Domain.Data.EntityTypeConfiguration
{
    internal class ProcessDefinitionConfiguration : IEntityTypeConfiguration<ProcessDefinition>
    {
        public void Configure(EntityTypeBuilder<ProcessDefinition> builder)
        {
            builder.ToTable("ProcessDefinition").HasKey(x => new { x.ProcessDefinitionId });
            builder.Property(c => c.Name);
            builder.Property(c => c.ProcessIdentifierEventProps);
            builder.Property(c => c.ApplicationId);
            builder.HasMany(c => c.ProcessEventDefinitions).WithOne(c => c.ProcessDefinition);
            builder.HasOne(c => c.Application)
                .WithMany()
                .HasForeignKey(c=>c.ApplicationId);
        }
    }
}
