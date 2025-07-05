using Microsoft.EntityFrameworkCore;

namespace SnackFlow.Infrastructure.Tests;

/// <summary>
/// Representa uma implementação específica para testes do DbContext para uso em testes unitários.
/// Esta classe é responsável por configurar propriedades DbSet que correspondem às tabelas do banco de dados
/// e interagir com o contexto do banco de dados subjacente durante cenários de teste.
/// </summary>
public class TestDbContext(DbContextOptions<TestDbContext> options) : DbContext(options)
{
    public DbSet<TestEntity> TestEntities { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<TestEntity>()
            .HasKey(e => e.Id);

        modelBuilder
            .Entity<TestEntity>()
            .HasQueryFilter(x => !x.DeletedAt.HasValue);

        base.OnModelCreating(modelBuilder);
    }
}