using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace JWFramework.UGUI.Private
{
	[System.Serializable]
	public class QueueList<T>
	{
		[SerializeField]
		T[] datas;
		[SerializeField]
		int size;

		public int Count {
			get {
				return size;
			}
		}

		public T this [int index] {
			get {
				return datas [index];
			}
		}

		public QueueList ()
		{
			size = 0;
		}

		void AllocateMore ()
		{
			T[] newList = (datas != null) ? new T[Mathf.Max (datas.Length << 1, 32)] : new T[Mathf.Max (1, size)];
			if (datas != null && size > 0)
				datas.CopyTo (newList, 0);
			datas = newList;
		}

		public IEnumerator<T> GetEnumerator ()
		{
			if (datas != null) {
				for (int i = 0; i < size; ++i) {
					yield return datas [i];
				}
			}
		}

		public void Enqueue (T item)
		{
			if (datas == null || size == datas.Length)
				AllocateMore ();
			datas [size++] = item;
		}

		public T Dequeue ()
		{
			if (datas != null && size != 0) {
				T val = datas [--size];
				datas [size] = default(T);
				return val;
			}
			return default(T);
		}

		public void Clear ()
		{
			size = 0;
			datas = null;
		}

		public bool Contains (T item)
		{
			if (datas == null)
				return false;
			for (int i = 0; i < size; ++i)
				if (datas [i].Equals (item))
					return true;
			return false;
		}
	}
}