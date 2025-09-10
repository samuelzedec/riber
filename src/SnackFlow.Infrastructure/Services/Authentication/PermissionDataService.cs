using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SnackFlow.Application.Abstractions.Services;
using SnackFlow.Application.DTOs;
using SnackFlow.Application.Exceptions;
using SnackFlow.Domain.Constants;
using SnackFlow.Infrastructure.Persistence;
using SnackFlow.Infrastructure.Persistence.Identity;

namespace SnackFlow.Infrastructure.Services.Authentication;

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
                ?? throw new NotFoundException(ErrorMessage.NotFound.Permission);

            return permission.IsActive;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ErrorMessage.Exception.Unexpected(ex.GetType().Name, ex.Message));
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
                ?? throw new NotFoundException(ErrorMessage.NotFound.Permission);

            permission.IsActive = !permission.IsActive;
            
            _permissionTable.Update(permission);
            await context.SaveChangesAsync();
            InvalidateCache();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ErrorMessage.Exception.Unexpected(ex.GetType().Name, ex.Message));
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
            logger.LogError(ex, ErrorMessage.Exception.Unexpected(ex.GetType().Name, ex.Message));
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