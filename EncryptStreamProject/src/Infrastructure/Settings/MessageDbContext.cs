using System.Diagnostics.CodeAnalysis;
using Infrastructure.MessageBus.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Settings;

[ExcludeFromCodeCoverage]
public class MessageDbContext : DbContext
{
    public MessageDbContext(){}
    public MessageDbContext(DbContextOptions<MessageDbContext> options) : base(options) { }

    public virtual DbSet<Message> Messages { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new MessageEntityTypeConfiguration());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Server=localhost;Port=5432;Username=postgres;Password=admin");
     
}

[ExcludeFromCodeCoverage]
public class MessageEntityTypeConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("Message");
        builder.HasKey(s => s.Id);
    }
}
