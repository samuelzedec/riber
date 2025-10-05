namespace Riber.Domain.Abstractions;

/// <summary>
/// Interface marcadora para entidades que utilizam a coluna xmin do PostgreSQL 
/// para controle de concorrência otimista.
/// </summary>
public interface IHasXmin
{
    public uint XminCode { get; set; }
}