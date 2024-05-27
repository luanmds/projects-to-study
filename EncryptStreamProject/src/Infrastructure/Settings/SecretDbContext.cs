using Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Settings;

public class SecretDbContext : DbContext
{
    public SecretDbContext(){}
    public SecretDbContext(DbContextOptions<SecretDbContext> options) : base(options) { }

    public DbSet<Secret> Secrets { get; set; }
    public DbSet<HashCryptor> HashCryptors { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new SecretEntityTypeConfiguration());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseNpgsql("Server=localhost;Port=5432;Username=postgres;Password=admin");
}


public class SecretEntityTypeConfiguration : IEntityTypeConfiguration<Secret>
{
    public void Configure(EntityTypeBuilder<Secret> builder)
    {
        builder.ToTable("Secrets");
        builder.HasKey(s => s.Id);
        builder.OwnsOne(s => s.HashCryptor);
    }
}
