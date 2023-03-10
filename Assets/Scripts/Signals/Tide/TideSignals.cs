using Zenject;

namespace Game.Tide
{
    public class TideSignals
    {
        private readonly SignalBus _tideSignalBus;

        public TideSignals(SignalBus tideSignalBus)
        {
            _tideSignalBus = tideSignalBus;
        }
    }
}