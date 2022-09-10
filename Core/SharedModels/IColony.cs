namespace Core.SharedModels;

public interface IColony : IPosition
{
    bool IsStartingLocation { get; }
    // IEnumerable<IUnit> Minerals { get; }
    // IEnumerable<IUnit> Vespene { get; }
    // ICollection<IUnit> Structures { get; }
    // ICollection<IUnit> Workers { get; }
    // int DesiredVespeneWorkers { get; set; }
}