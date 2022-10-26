using Zenject;

namespace Game
{
    public class ISignal
    {
        readonly SignalBus _signalBus;

        public ISignal(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }
    }
}