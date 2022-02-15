using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tasks.Definition.Domain.Entities;

namespace Tasks.Domain.Data.EntityTypeConfiguration
{
    internal class ProcessEventDefinitionConfiguration : IEntityTypeConfiguration<ProcessEventDefinition>
    {
        public void Configure(EntityTypeBuilder<ProcessEventDefinition> builder)
        {
            builder.ToTable("ProcessEventDefinition").HasKey(x => new { x.ProcessDefinitionId, x.EventDefinitionId });
            builder.Property(c => c.ProcessIdentifierProps);
            builder.Property(c => c.EventDefinitionId);

            builder.HasOne(c => c.ProcessDefinition)
                .WithMany(c => c.ProcessEventDefinitions)
                .HasForeignKey(ped => ped.ProcessDefinitionId);

            builder.HasOne(c => c.EventDefinition)
                .WithMany();
        }
    }
}
