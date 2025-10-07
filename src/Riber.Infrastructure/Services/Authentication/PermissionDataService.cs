using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Riber.Application.Abstractions.Services;
using Riber.Application.Exceptions;
using Riber.Application.Models;
using Riber.Domain.Constants.Messages.Common;
using Riber.Infrastructure.Persistence;
using Riber.Infrastructure.Persistence.Identity;

namespace Riber.Infrastructure.Services.Authentication;

public sealed class PermissionDataService(
    AppDbContext context,
    IMemoryCache memoryCache)
    : IPermissionDataService
{
    private readonly DbSet<ApplicationPermission> _permissionTable
        = context.Set<ApplicationPermission>();

    private const string CacheKey = "permissions-cache";

    public async Task<bool> ValidateAsync(string name)
    {
        var permissions = await GetPermissionsCacheAsync();
        var permission = permissions.FirstOrDefault(p => p.Name == name)
                         ?? throw new NotFoundException(NotFoundErrors.Permission);

        return permission.IsActive;
    }

    public async Task UpdatePermissionStatusAsync(string name)
    {
        var permissions = await GetPermissionsCacheAsync();
        var permission = permissions.FirstOrDefault(p => p.Name == name)
                         ?? throw new NotFoundException(NotFoundErrors.Permission);

        permission.IsActive = !permission.IsActive;
        _permissionTable.Update(permission);
        await context.SaveChangesAsync();
        InvalidateCache();
    }

    public async Task<ICollection<PermissionModel>> GetAllWithDescriptionsAsync()
    {
        var permissions = await GetPermissionsCacheAsync();

        return
        [
            .. permissions.Select(p => new PermissionModel(
                Name: p.Name,
                Description: p.Description,
                IsActive: p.IsActive
            ))
        ];
    }

    private async Task<IEnumerable<ApplicationPermission>> GetPermissionsCacheAsync()
    {
        if (memoryCache.TryGetValue(CacheKey, out IEnumerable<ApplicationPermission>? permissions) &&
            permissions is not null)
            return [.. permissions];

        var permissionsDatabase = await _permissionTable
            .AsNoTracking()
            .ToListAsync();

        memoryCache.Set(
            CacheKey,
            permissionsDatabase,
            TimeSpan.FromHours(1)
        );

        return permissionsDatabase;
    }

    private void InvalidateCache()
        => memoryCache.Remove(CacheKey);
}