using System.Linq.Expressions;
using Riber.Domain.Specifications.Core;

namespace Riber.Domain.Specifications.Image;

public sealed class ImagesReadyForCleanupSpecification
    : Specification<Entities.Catalog.Image>
{
    public override Expression<Func<Entities.Catalog.Image, bool>> ToExpression()
        => entity => entity.ShouldDelete && entity.MarkedForDeletionAt.HasValue && !entity.DeletedAt.HasValue;
}