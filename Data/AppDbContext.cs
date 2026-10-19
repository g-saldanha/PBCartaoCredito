using Microsoft.EntityFrameworkCore;

namespace PBCartaoCredito.Data;

public class Cartao
{
    public Guid Id { get; set; }
    public Guid ClienteId { get; set; }
    public decimal Limite { get; set; }
    public string FinalCartao { get; set; } = string.Empty;
}

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Cartao> Cartoes => Set<Cartao>();
}