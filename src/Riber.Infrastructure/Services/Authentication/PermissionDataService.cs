using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Riber.Application.Abstractions.Services;
using Riber.Application.DTOs;
using Riber.Application.Exceptions;
using Riber.Domain.Constants.Messages.Common;
using Riber.Infrastructure.Persistence;
using Riber.Infrastructure.Persistence.Identity;

namespace Riber.Infrastructure.Services.Authentication;

public sealed class PermissionDataService(
    AppDbContext context,
    IMemoryCache memoryCache,
    ILogger<PermissionDataService> logger)
    : IPermissionDataService
{
    private readonly DbSet<ApplicationPermission> _permissionTable
        = context.Set<ApplicationPermission>();

    private const string CacheKey = "permissions-cache";

    public async Task<bool> ValidateAsync(string name)
    {
        try
        {
            var permissions = await GetPermissionsCacheAsync();
            var permission = permissions
                .FirstOrDefault(p => p.Name == name)
                ?? throw new NotFoundException(NotFoundErrors.Permission);

            return permission.IsActive;
        }
        catch (Exception ex)
        {
            logger.LogError(UnexpectedErrors.ForLogging(nameof(PermissionDataService), ex));
            throw;
        }
    }

    public async Task UpdatePermissionStatusAsync(string name)
    {
        try
        {
            var permissions = await GetPermissionsCacheAsync();
            var permission = permissions
                .FirstOrDefault(p => p.Name == name)
                ?? throw new NotFoundException(NotFoundErrors.Permission);

            permission.IsActive = !permission.IsActive;

            _permissionTable.Update(permission);
            await context.SaveChangesAsync();
            InvalidateCache();
        }
        catch (Exception ex)
        {
            logger.LogError(UnexpectedErrors.ForLogging(nameof(PermissionDataService), ex));
            throw;
        }
    }

    public async Task<ICollection<PermissionDTO>> GetAllWithDescriptionsAsync()
    {
        try
        {
            var permissions = await GetPermissionsCacheAsync();

            return [.. permissions.Select(p => new PermissionDTO(
                Name: p.Name,
                Description: p.Description,
                IsActive: p.IsActive
            ))];
        }
        catch (Exception ex)
        {
            logger.LogError(UnexpectedErrors.ForLogging(nameof(PermissionDataService), ex));
            throw;
        }
    }

    private async Task<IEnumerable<ApplicationPermission>> GetPermissionsCacheAsync()
    {
        if (memoryCache.TryGetValue(CacheKey, out IEnumerable<ApplicationPermission>? permissions) && permissions is not null)
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