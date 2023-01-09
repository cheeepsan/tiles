using System;
using UnityEngine;

// https://youtu.be/NFvmfoRnarY
// Events are easier then Zenject messages?
namespace Game
{
    public class TimeManager : MonoBehaviour
    {

        public class OnTickEventArgs : EventArgs
        {
            public int tick;
        }

        public static event EventHandler<OnTickEventArgs> OnTick;
        
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
            }

        }
    }
}