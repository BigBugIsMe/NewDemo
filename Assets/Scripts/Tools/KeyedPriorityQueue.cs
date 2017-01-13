using UnityEngine;
using System.Collections;
using System.Collections.ObjectModel;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System;
namespace Core.Timer
{
    public class KeyedPriorityQueue<K, V, P> where V : class
    {
        /// <summary>
        /// 队列
        /// </summary>
        private List<HeapNode<K, V, P>> heap;
        /// <summary>
        /// 占坑位
        /// </summary>
        private HeapNode<K, V, P> placeHolder;
        /// <summary>
        /// 比较
        /// </summary>
        private Comparer<P> priorityCompare;
        /// <summary>
        /// 大小
        /// </summary>
        private int size;
        /// <summary>
        /// 事件回调
        /// </summary>
        public event EventHandler<PriorityQueueChangeEventArgs<V>> OnFirstElementChanged;

        /// <summary>
        /// 构造函数
        /// </summary>
        public KeyedPriorityQueue()
        {
            this.heap = new List<HeapNode<K, V, P>>();
            this.priorityCompare = Comparer<P>.Default;
            this.placeHolder = new HeapNode<K, V, P>();
            //先往队列中注入一个元素
            this.heap.Add(new HeapNode<K, V, P>());
        }
        public V Peek()
        {
            if(this.size>=1)
            {
                return this.heap[1].Value;
            }
            return default(V);
        }
        /// <summary>
        /// 入列
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="priority"></param>
        public void Enqueue(K key,V value,P priority)
        {
            V local = (this.size > 0) ? this.heap[1].Value : default(V);
            int num = ++this.size;
            int num2 = num / 2;
            if(num==this.heap.Count)
            {
                this.heap.Add(this.placeHolder);
            }
            while((num>1)&&IsHigher(priority,this.heap[num2].Priority))
            {
                this.heap[num] = this.heap[num2];
                num = num2;
                num2 = num / 2;
            }
            this.heap[num] = new HeapNode<K, V, P>(key, value, priority);
            V newHead = this.heap[1].Value;
            if(!newHead.Equals(local))
            {
                this.RaiseEvent(local, newHead);
            }
        }
        public V Dequeue()
        {
            V local = (this.size < 1) ? default(V) : this.DequeueImpl();
            V newHead = (this.size < 1) ? default(V) : this.heap[1].Value;
            this.RaiseEvent(local, newHead);
            return local;
        }
        private V DequeueImpl()
        {
            V local = this.heap[1].Value;
            this.heap[1] = this.heap[size];
            this.heap[size--] = this.placeHolder;
            this.Heapify(1);
            return local;
        }
        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="i"></param>
        private void Heapify(int i)
        {
            int num = i * 2;
            int num2 = num + 1;
            int j = i;
            if((num<this.size)&&this.IsHigher(this.heap[num].Priority,this.heap[i].Priority))
            {
                j = num;
            }
            if((num2<this.size)&&this.IsHigher(this.heap[num2].Priority,this.heap[j].Priority))
            {
                j = num2;
            }
            if(j!=i)
            {
                this.Swap(i, j);
                this.Heapify(j);
            }
        }
        private void Swap(int i,int j)
        {
            HeapNode<K, V, P> node = this.heap[i];
            this.heap[i] = this.heap[j];
            this.heap[j] = node;
        }
        protected virtual bool IsHigher(P p1,P p2)
        {
            return this.priorityCompare.Compare(p1, p2)<1;
        }
        private void RaiseEvent(V oldElement,V newElement)
        {
            if(oldElement!=newElement)
            {
                EventHandler<PriorityQueueChangeEventArgs<V>> firstElementChanged = this.OnFirstElementChanged;
                if(firstElementChanged!=null)
                {
                    firstElementChanged(this, new PriorityQueueChangeEventArgs<V>(oldElement, newElement));
                }
            }
        }
        private struct HeapNode<KK, VV, PP>
        {
            public KK Key;
            public VV Value;
            public PP Priority;
            public HeapNode(KK k, VV v, PP p)
            {
                this.Key = k;
                this.Value = v;
                this.Priority = p;
            }
        }
    }
}

