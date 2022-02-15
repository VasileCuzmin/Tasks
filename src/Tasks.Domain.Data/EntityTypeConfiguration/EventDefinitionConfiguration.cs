using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tasks.Definition.Domain.Entities;

namespace Tasks.Domain.Data.EntityTypeConfiguration
{
    internal class EventDefinitionConfiguration : IEntityTypeConfiguration<EventDefinition>
    {
        public void Configure(EntityTypeBuilder<EventDefinition> builder)
        {
            builder.ToTable("EventDefinition").HasKey(x => new { x.EventDefinitionId });
            builder.Property(c => c.Name);
            builder.Property(c => c.Topic);
            builder.Property(c => c.ApplicationId);
            builder.Property(c => c.Schema);
            builder.HasOne(c => c.Application)
                .WithMany();
        }
    }
}
