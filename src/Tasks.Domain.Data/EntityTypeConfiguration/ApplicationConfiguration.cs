using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tasks.Definition.Domain.Entities;

namespace Tasks.Domain.Data.EntityTypeConfiguration
{
    internal class ApplicationConfiguration : IEntityTypeConfiguration<Application>
    {
        public void Configure(EntityTypeBuilder<Application> builder)
        {
            builder.ToTable("Application").HasKey(x => new { x.ApplicationId });
            builder.Property(c => c.Name);            
        }
    }
}
