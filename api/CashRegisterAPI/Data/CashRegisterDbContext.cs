using CashRegisterAPI.Domain;
using Microsoft.EntityFrameworkCore;

namespace CashRegisterAPI.Data;

public class CashRegisterDbContext(DbContextOptions<CashRegisterDbContext> options) : DbContext(options)
{
    public DbSet<Currency> Currency { get; set; }
    public DbSet<Denomination> Denomination { get; set; }
    public DbSet<Country> Country { get; set; }
    public DbSet<CountryCurrency> CountryCurrency { get; set; }
    public DbSet<Domain.Rule> Rule { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CountryCurrency>().HasOne(cc => cc.Country)
             .WithMany(c => c.CountryCurrencies).HasForeignKey(cc => cc.CountryId);

        modelBuilder.Entity<CountryCurrency>()
            .HasOne(cc => cc.Currency)
            .WithMany(c => c.CountryCurrencies)
            .HasForeignKey(cc => cc.CurrencyId);

        modelBuilder.Entity<Currency>().HasIndex(c => c.Name);
        modelBuilder.Entity<Denomination>().HasIndex(d => new { d.Name, d.CurrencyId });
        modelBuilder.Entity<Country>().HasIndex(c => new { c.Name, c.Abbrevation });
        modelBuilder.Entity<Domain.Rule>().HasIndex(r => r.Name);
        modelBuilder.Entity<Domain.Rule>().HasIndex(r => r.Priority);

        // Only 1 primary currency for each Country
        modelBuilder.Entity<CountryCurrency>()
            .HasIndex(cc => cc.CountryId)
            .IsUnique()
            .HasFilter("\"IsPrimary\" = true");
    }
}
