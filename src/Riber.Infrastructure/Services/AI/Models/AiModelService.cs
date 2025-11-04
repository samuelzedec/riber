using Microsoft.EntityFrameworkCore;
using Riber.Application.Abstractions.Services.AI;
using Riber.Infrastructure.Persistence;

namespace Riber.Infrastructure.Services.AI.Models;

public abstract class AiModelService<TInput, TOutput>(AppDbContext context)
    : IAiModelService<TInput, TOutput>
    where TInput : class
    where TOutput : class
{
    protected readonly DbSet<TInput> _table = context.Set<TInput>();

    public async Task CreateAsync(TInput model, CancellationToken cancellationToken = default)
    {
        await _table.AddAsync(model, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(TInput model, CancellationToken cancellationToken = default)
    {
        _table.Remove(model);
        await context.SaveChangesAsync(cancellationToken);
    }

    public abstract Task<ReadOnlyMemory<TOutput>> FindSimilarAsync(float[] query, CancellationToken cancellationToken = default);
    public abstract Task<TInput?> GetByEntityIdAsync(Guid entityId, CancellationToken cancellationToken = default);
}