using System.Linq.Expressions;

namespace Riber.Domain.Specifications.Core;

internal sealed class ReplacerParameter(
    ParameterExpression oldParameter,
    ParameterExpression newParameter) 
    : ExpressionVisitor
{
    protected override Expression VisitParameter(ParameterExpression node) 
        => node == oldParameter ? newParameter : base.VisitParameter(node);
}