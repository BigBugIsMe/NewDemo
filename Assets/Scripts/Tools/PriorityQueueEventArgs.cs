using UnityEngine;
using System.Collections;
using System;
using System.Runtime;
namespace Core.Timer
{
    public class PriorityQueueEventArgs
    {
    }
    public sealed class PriorityQueueChangeEventArgs<T> : EventArgs where T : class
    {
        private T newFirstElement;
        private T oldFirstElement;
        public PriorityQueueChangeEventArgs(T oldEle, T newEle)
        {
            this.newFirstElement = newEle;
            this.oldFirstElement = oldEle;
        }

        public T NewFirstElement
        {
            get
            {
                return newFirstElement;
            }
        }
        public T OldFirstElement
        {
            get
            {
                return oldFirstElement;
            }
        }
    }
}


