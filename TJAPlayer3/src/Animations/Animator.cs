using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TJAPlayer3;
using FDK;

namespace TJAPlayer3.Animations
{
    class Animator : IAnimatable
    {
        public Animator(int startValue, int endValue, int tickInterval, bool isLoop)
        {
            Type = CounterType.Normal;
            StartValue = startValue;
            EndValue = endValue;
            TickInterval = tickInterval;
            IsLoop = isLoop;
            Counter = new Counter();
        }
        public Animator(double startValue, double endValue, double tickInterval, bool isLoop)
        {
            Type = CounterType.Double;
            StartValue = startValue;
            EndValue = endValue;
            TickInterval = tickInterval;
            IsLoop = isLoop;
            Counter = new Counter();
        }
        public void Start()
        {
            if (Counter == null) throw new NullReferenceException();
            switch (Type)
            {
                case CounterType.Normal:
                    Counter.Start((int)StartValue, (int)EndValue, (int)TickInterval, TJAPlayer3.Timer);
                    break;
                case CounterType.Double:
                    Counter.Start((double)StartValue, (double)EndValue, (double)TickInterval, SoundManager.PlayTimer);
                    break;
                default:
                    break;
            }
        }
        public void Stop()
        {
            if (Counter == null) throw new NullReferenceException();
            Counter.Stop();
        }
        public void Reset()
        {
            if (Counter == null) throw new NullReferenceException();
            Start();
        }

        public void Tick()
        {
            if (Counter == null) throw new NullReferenceException();
            switch (Type)
            {
                case CounterType.Normal:
                    if (IsLoop) Counter.TickLoop(); else Counter.Tick();
                    if (!IsLoop && Counter.IsEndValueReached) Stop();
                    break;
                case CounterType.Double:
                    if (IsLoop) Counter.TickLoop_Double(); else Counter.Tick_Double();
                    if (!IsLoop && Counter.IsEndValueReached_Double) Stop();
                    break;
                default:
                    break;
            }
        }

        public virtual object GetAnimation()
        {
            throw new NotImplementedException();
        }



        // プロパティ
        public Counter Counter
        {
            get;
            private set;
        }
        public CounterType Type
        {
            get;
            private set;
        }
        public object StartValue
        {
            get;
            private set;
        }
        public object EndValue
        {
            get;
            private set;
        }
        public object TickInterval
        {
            get;
            private set;
        }
        public bool IsLoop
        {
            get;
            private set;
        }
    }

    enum CounterType
    {
        Normal,
        Double
    }
}
