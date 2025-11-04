using Microsoft.EntityFrameworkCore;

namespace Riber.Infrastructure.Tests.Persistence.Interceptors.TestModels;

/// <summary>
/// Representa uma implementação específica para testes do DbContext para uso em testes unitários.
/// Esta classe é responsável por configurar propriedades DbSet que correspondem às tabelas do banco de dados
/// e interagir com o contexto do banco de dados subjacente durante cenários de teste.
/// </summary>
public class DbContextTest(DbContextOptions<DbContextTest> options) : DbContext(options)
{
    public DbSet<EntityTest> TestEntities { get; set; }
    public DbSet<ModelTest> TestModels { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<EntityTest>()
            .HasKey(e => e.Id);

        modelBuilder
            .Entity<EntityTest>()
            .HasQueryFilter(x => !x.DeletedAt.HasValue);

        modelBuilder
            .Entity<ModelTest>()
            .HasKey(e => e.Id);

        modelBuilder
            .Entity<ModelTest>()
            .HasQueryFilter(x => !x.DeletedAt.HasValue);

        base.OnModelCreating(modelBuilder);
    }
}