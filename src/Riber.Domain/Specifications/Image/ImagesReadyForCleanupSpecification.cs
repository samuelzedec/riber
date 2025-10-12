using System.Linq.Expressions;
using Riber.Domain.Specifications.Core;

namespace Riber.Domain.Specifications.Image;

public sealed class ImagesReadyForCleanupSpecification
    : Specification<Entities.Image>
{
    public override Expression<Func<Entities.Image, bool>> ToExpression()
        => entity => entity.ShouldDelete == true && entity.MarkedForDeletionAt.HasValue && !entity.DeletedAt.HasValue;
}