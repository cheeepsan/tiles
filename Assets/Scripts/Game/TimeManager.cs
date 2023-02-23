using System;
using UnityEngine;

// https://youtu.be/NFvmfoRnarY
// Events are easier than Zenject messages?
namespace Game
{
    public class TimeManager : MonoBehaviour
    {

        public class OnTickEventArgs : EventArgs
        {
            public int tick;
        }

        public class On10TickEventArgs : EventArgs
        {
            public int tick;
        }
        
        public class On40TickEventArgs : EventArgs
        {
            public int tick;
        }

        public static event EventHandler<OnTickEventArgs> OnTick;
        public static event EventHandler<On10TickEventArgs> On10Tick;
        public static event EventHandler<On40TickEventArgs> On40Tick;
        
        private int _tick;
        private float _tickTimer;
        private const float _tickMax = 0.2f;
        private void Start()
        {
            Debug.Log("TimeTick is started");
            _tick = 0;
        }

        private void Update()
        {
            _tickTimer += Time.deltaTime;
            if (_tickTimer > _tickMax)
            {
                _tickTimer -= _tickMax;
                _tick++;
                if (OnTick != null) OnTick(this, new OnTickEventArgs() { tick = _tick });
                if (_tick % 10 == 0)
                {
                    if (On10Tick != null)
                    {
                        On10Tick(this, new On10TickEventArgs() { tick = _tick });
                    }

                }
                // JUST TESTING
                if (_tick % 40 == 0)
                {
                    if (On40Tick != null)
                    {
                        On40Tick(this, new On40TickEventArgs() { tick = _tick });
                    }
                }
            }

        }

        public int GetCurrentTick()
        {
            return _tick;
        }

        public void SetCurrentTick(int tick)
        {
            _tick = tick;
        }
    }
}