using UnityEngine;
using System.Collections;
using System;
namespace Core.Timer
{

    internal abstract class AbsTimerData
    {
        //Id
        private uint m_nTimerId = 0;
        //总执行次数
        public int Time;
        //已执行次数
        public int CallTime = 0;
        //重复间隔
        private int m_nInterval;
        private ulong m_nNextTick;
        public uint NTimerId
        {
            get { return m_nTimerId; }
            set { m_nTimerId = value; }
        }

        public int NInterval
        {
            get { return m_nInterval; }
            set { m_nInterval = value; }
        }
        public ulong NextTick
        {
            get { return m_nNextTick; }
            set { m_nNextTick = value; }
        } 

        public abstract Delegate Action
        {
            get;set;
        }
        public virtual void DoAction()
        {
            CallTime++;
        }
    }
    internal class TimerData:AbsTimerData
    {
        private Action m_action;
        public override Delegate Action
        {
            get
            {
                return m_action;
            }

            set
            {
                m_action = value as Action;
            }
        }
        public override void DoAction()
        {
            if (this != null && m_action != null)
            {
                m_action();
                base.DoAction();
            }
        }
    }
    internal class TimerData<T> : AbsTimerData
    {
        private Action<T> m_action;
        public override Delegate Action
        {
            get
            {
                return m_action;
            }

            set
            {
                m_action = value as Action<T>;
            }
        }
        public override void DoAction()
        {
            if (this != null && m_action != null)
            {
                m_action(m_args1);
                base.DoAction();
            }
        }
        private T m_args1;
        public T Args1
        {
            get
            {
                return m_args1;
            }
            set
            {
                m_args1 = value;
            }
        }
    }
    internal class TimerData<T,K> : AbsTimerData
    {
        private Action<T,K> m_action;
        public override Delegate Action
        {
            get
            {
                return m_action;
            }

            set
            {
                m_action = value as Action<T,K>;
            }
        }
        public override void DoAction()
        {
            if (this != null && m_action != null)
            {
                m_action(m_args1,m_args2);
                base.DoAction();
            }
        }
        private T m_args1;
        public T Args1
        {
            get
            {
                return m_args1;
            }
            set
            {
                m_args1 =value;
            }
        }
        private K m_args2;
        public K Args2
        {
            get
            {
                return m_args2;
            }
            set
            {
                m_args2 =value;
            }
        }
    }
    internal class TimerData<T, K,V> : AbsTimerData
    {
        private Action<T, K,V> m_action;
        public override Delegate Action
        {
            get
            {
                return m_action;
            }

            set
            {
                m_action = value as Action<T, K,V>;
            }
        }
        public override void DoAction()
        {
            if (this != null && m_action != null)
            {
                m_action(m_args1, m_args2,m_args3);
                base.DoAction();
            }
        }
        private T m_args1;
        public T Args1
        {
            get
            {
                return m_args1;
            }
            set
            {
                m_args1 =value;
            }
        }
        private K m_args2;
        public K Args2
        {
            get
            {
                return m_args2;
            }
            set
            {
                m_args2 = value;
            }
        }
        private V m_args3;
        public V Args3
        {
            get
            {
                return m_args3;
            }
            set
            {
                m_args3 = value;
            }
        }
    }
}

