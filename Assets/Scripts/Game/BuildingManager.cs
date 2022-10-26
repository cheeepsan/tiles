using Zenject;

namespace Game
{
    public class BuildingManager : ISignal
    {
        public BuildingManager(SignalBus signalBus) : base(signalBus)
        {
        }
    }
}