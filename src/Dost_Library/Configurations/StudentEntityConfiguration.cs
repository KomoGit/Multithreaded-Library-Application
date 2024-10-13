using Dost_Library.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dost_Library.Configurations
{
    public class StudentEntityConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.ToTable("students");
            builder.HasKey(x => x.Id);
            
            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.BookId).HasColumnName("book_id");
            builder.Property(x => x.Name).HasColumnName("name");

            builder.Ignore(x => x.TimeItTakesToRead);
        }
    }
}