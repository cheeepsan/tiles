using System;
using Constant;
using UnityEngine;
using Util;
using Zenject;

// https://youtu.be/NFvmfoRnarY
// Events are easier than Zenject messages?

/*
 * TODO: CHANGE TICK FROM INT TO FLOAT OR LONG
 */
namespace Game
{
    public class TimeManager : MonoBehaviour
    {

        public class OnTickEventArgs : EventArgs
        {
            public int month;
            public int currentTick;
            public String monthName;
        }

        public class On10TickEventArgs : EventArgs
        {
            public int tick;
        }
        
        public class On40TickEventArgs : EventArgs
        {
            public int tick;
        }        
        
        public class OnMonthChangeEventArgs : EventArgs
        {
            public int month;
            public int currentTick;
            public String monthName;
        }

        public static event EventHandler<OnTickEventArgs> OnTick;
        public static event EventHandler<On10TickEventArgs> On10Tick;
        public static event EventHandler<On40TickEventArgs> On40Tick;
        public static event EventHandler<OnMonthChangeEventArgs> OnMonthChange;

        [Inject] private Configuration _configuration;
        
        private int _tick;
        private float _tickTimer;
        private const float _tickMax = 0.2f;
        private int _currentMonth;
        private String _currentMonthName;
        private int _ticksPerMonth;
        
        private void Start()
        {
            Settings settings = _configuration.GetSettings();
            Debug.Log("TimeTick is started");
            _tick = 0;
            _ticksPerMonth = settings.tickPerMonth;
            _currentMonth = 0;
            _currentMonthName = MonthConstant.GetMonthName(_currentMonth);
        }

        private void Update()
        {
            _tickTimer += Time.deltaTime;
            if (_tickTimer > _tickMax)
            {
                _tickTimer -= _tickMax;
                _tick++;
                
                if (OnTick != null) OnTick(this, new OnTickEventArgs()
                {
                    currentTick = _tick,
                    month = _currentMonth,
                    monthName = _currentMonthName
                });
                
                if (_tick % 10 == 0)
                {
                    if (On10Tick != null)
                    {
                        On10Tick(this, new On10TickEventArgs() { tick = _tick });
                    }

                }
                // TODO JUST TESTING
                if (_tick % 40 == 0)
                {
                    if (On40Tick != null)
                    {
                        On40Tick(this, new On40TickEventArgs() { tick = _tick });
                    }
                }
                
                ChangeMonth();
            }


        }

        private void ChangeMonth()
        {
            if (_tick % _ticksPerMonth == 0)
            {
                int monthToBe = _currentMonth + 1;
                if (monthToBe <= 11)
                {
                    _currentMonth = monthToBe;
                }
                else
                {
                    _currentMonth = 0;
                }

                String monthNameConstant = MonthConstant.GetMonthName(_currentMonth);
                _currentMonthName = monthNameConstant;
                if (OnMonthChange != null)
                {
                    OnMonthChange(this, new OnMonthChangeEventArgs()
                    {
                        month = _currentMonth,
                        monthName = _currentMonthName,
                        currentTick = _tick
                    });
                }

            }
        }

        public int GetCurrentTick()
        {
            return _tick;
        }

        public int GetCurrentMonth()
        {
            return _currentMonth;
        }
        
        public void SetCurrentTick(int tick)
        {
            _tick = tick;
        }    
        
        public void SetCurrentMonth(int currentMonth)
        {
            _currentMonth = currentMonth;
        }
    }
}