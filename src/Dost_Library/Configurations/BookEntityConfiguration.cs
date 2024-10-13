using Dost_Library.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dost_Library.Configurations
{
    public class BookEntityConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.ToTable("books");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).IsRequired().HasColumnName("name");
            builder.Property(x => x.Id).IsRequired().HasColumnName("id");
        }
    }
}
