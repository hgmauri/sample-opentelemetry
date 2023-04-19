using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Sample.OpenTelemetry.Infrastructure.Context;

public class ClientContext : DbContext
{
    protected ClientContext(DbContextOptions options)
        : base(options) { }

    public ClientContext(DbContextOptions<ClientContext> options)
        : this((DbContextOptions)options)
    {
    }

    public DbSet<Client> Clients { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
	    base.OnModelCreating(builder);

	    builder.ApplyConfiguration(new Clientonfiguration());
    }
}

public class Client 
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }
    public DateTime DataNascimento { get; set; }
}


public class Clientonfiguration : IEntityTypeConfiguration<Client>
{
	public void Configure(EntityTypeBuilder<Client> builder)
	{
        builder.ToTable("Clientes");

        builder.Property(t => t.Id).HasColumnName("Id");

        builder.Property(t => t.Nome).HasColumnName("Nome").HasMaxLength(50).IsRequired();

        builder.Property(t => t.Email).HasColumnName("Email").HasMaxLength(100).IsRequired();

		builder.Property(t => t.DataNascimento).HasColumnName("DataNascimento").IsRequired();
    }
}