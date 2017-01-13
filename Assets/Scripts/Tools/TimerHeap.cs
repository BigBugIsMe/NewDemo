using UnityEngine;
using System.Collections;
using System.Diagnostics;
namespace Core.Timer
{
    public class TimerHeap
    {
        private static uint m_nNextTimerId;
        private static uint m_unTick;
        private static KeyedPriorityQueue<uint, AbsTimerData, ulong> m_queue;
        private static Stopwatch m_stopWatch;
        private static object m_queueLock;

    }
}

