using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Riber.Application.Abstractions.Services.Authentication;
using Riber.Application.Common;
using Riber.Application.Models.Auth;
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

    public async Task<Result<EmptyResult>> ValidateAsync(string name)
    {
        var permissions = await GetPermissionsCacheAsync();
        var permission = permissions.FirstOrDefault(p => p.Name == name);

        if (permission == null)
            return Result.Failure<EmptyResult>(NotFoundErrors.Permission, HttpStatusCode.NotFound);

        return permission.IsActive
            ? Result.Success(new EmptyResult())
            : Result.Failure<EmptyResult>("Permissão está inativa", HttpStatusCode.Forbidden);
    }

    public async Task<Result<EmptyResult>> UpdatePermissionStatusAsync(string name)
    {
        var permissions = await GetPermissionsCacheAsync();
        var permission = permissions.FirstOrDefault(p => p.Name == name);

        if (permission == null)
            return Result.Failure<EmptyResult>(NotFoundErrors.Permission, HttpStatusCode.NotFound);

        permission.IsActive = !permission.IsActive;
        _permissionTable.Update(permission);

        try
        {
            await context.SaveChangesAsync();
            InvalidateCache();
            return Result.Success(new EmptyResult());
        }
        catch (Exception ex)
        {
            return Result.Failure<EmptyResult>($"Erro ao atualizar permissão: {ex.Message}",
                HttpStatusCode.InternalServerError);
        }
    }

    public async Task<Result<IReadOnlyCollection<PermissionModel>>> GetAllWithDescriptionsAsync()
    {
        try
        {
            var permissions = await GetPermissionsCacheAsync();
            var result = permissions.Select(p => new PermissionModel(
                Name: p.Name,
                Description: p.Description,
                IsActive: p.IsActive
            )).ToList();

            return Result.Success<IReadOnlyCollection<PermissionModel>>(result);
        }
        catch (Exception ex)
        {
            return Result.Failure<IReadOnlyCollection<PermissionModel>>(
                $"Erro ao buscar permissões: {ex.Message}",
                HttpStatusCode.InternalServerError);
        }
    }

    private async Task<IEnumerable<ApplicationPermission>> GetPermissionsCacheAsync()
    {
        if (memoryCache.TryGetValue(CacheKey, out IEnumerable<ApplicationPermission>? permissions) &&
            permissions is not null)
            return permissions;

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