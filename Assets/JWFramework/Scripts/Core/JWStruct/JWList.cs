using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;

/// <summary>
/// This improved version of the System.Collections.Generic.List that doesn't release the buffer on Clear(),
/// resulting in better performance and less garbage collection.
/// PRO: JWList performs faster than List when you Add and Remove items (although slower if you remove from the beginning).
/// CON: JWList performs worse when sorting the list. If your operations involve sorting, use the standard List instead.
/// </summary>
namespace JWFramework
{
	public class JWList<T>
	{
		//
		// Properties
		//
		public int Count { get { return size; } }
		//
		// Constructors
		//
		public JWList ()
		{
		}

		public JWList (IEnumerable<T> collection)
		{
			foreach (var item in collection) {
				Add (item);
			}
		}

		/// <summary>
		/// Direct access to the buffer. Note that you should not use its 'Length' parameter, but instead use BetterList.size.
		/// </summary>
		private T[] buffer;

		/// <summary>
		/// Direct access to the buffer's size. Note that it's only public for speed and efficiency. You shouldn't modify it.
		/// </summary>

		private int size = 0;

		/// <summary>
		/// For 'foreach' functionality.
		/// </summary>

		[DebuggerHidden]
		[DebuggerStepThrough]
		public IEnumerator<T> GetEnumerator ()
		{
			if (buffer != null) {
				for (int i = 0; i < size; ++i) {
					yield return buffer [i];
				}
			}
		}
		

		//
		// Indexer
		//
		/// <summary>
		/// Convenience function. I recommend using .buffer instead.
		/// </summary>
		[DebuggerHidden]
		public T this [int i] {
			get { return buffer [i]; }
			set { buffer [i] = value; }
		}

		/// <summary>
		/// Helper function that expands the size of the array, maintaining the content.
		/// </summary>

		void AllocateMore ()
		{
			T[] newList = (buffer != null) ? new T[System.Math.Max (buffer.Length << 1, 32)] : new T[32];
			if (buffer != null && size > 0)
				buffer.CopyTo (newList, 0);
			buffer = newList;
		}

		/// <summary>
		/// Trim the unnecessary memory, resizing the buffer to be of 'Length' size.
		/// Call this function only if you are sure that the buffer won't need to resize anytime soon.
		/// </summary>

		void Trim ()
		{
			if (size > 0) {
				if (size < buffer.Length) {
					T[] newList = new T[size];
					for (int i = 0; i < size; ++i)
						newList [i] = buffer [i];
					buffer = newList;
				}
			} else
				buffer = null;
		}

		/// <summary>
		/// Clear the array by resetting its size to zero. Note that the memory is not actually released.
		/// </summary>

		public void Clear ()
		{
			size = 0;
		}

		/// <summary>
		/// Clear the array and release the used memory.
		/// </summary>

		public void Release ()
		{
			size = 0;
			buffer = null;
		}

		/// <summary>
		/// Add the specified item to the end of the list.
		/// </summary>

		public void Add (T item)
		{
			if (buffer == null || size == buffer.Length) {
				AllocateMore ();
			}
			buffer [size++] = item;
		}

		public void AddRange (IEnumerable<T> collection)
		{
			foreach (var item in collection) {
				Add (item);
			}
		}

		/// <summary>
		/// Insert an item at the specified index, pushing the entries back.
		/// </summary>

		public void Insert (int index, T item)
		{
			if (buffer == null || size == buffer.Length) {
				AllocateMore ();
			}
			if (index > -1 && index < size) {
				for (int i = size; i > index; --i)
					buffer [i] = buffer [i - 1];
				buffer [index] = item;
				++size;
			} else {
				Add (item);
			}
		}

		/// <summary>
		/// Returns 'true' if the specified item is within the list.
		/// </summary>

		public bool Contains (T item)
		{
			if (buffer == null) {
				return false;
			}
			for (int i = 0; i < size; ++i) {
				if (buffer [i].Equals (item)) {
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Return the index of the specified item.
		/// </summary>

		public int IndexOf (T item)
		{
			if (buffer == null) {
				return -1;
			}
			for (int i = 0; i < size; ++i) {
				if (buffer [i].Equals (item)) {
					return i;
				}
			}
			return -1;
		}

		/// <summary>
		/// Remove the specified item from the list. Note that RemoveAt() is faster and is advisable if you already know the index.
		/// </summary>

		public bool Remove (T item)
		{
			if (buffer != null) {
				EqualityComparer<T> comp = EqualityComparer<T>.Default;

				for (int i = 0; i < size; ++i) {
					if (comp.Equals (buffer [i], item)) {
						--size;
						buffer [i] = default(T);
						for (int b = i; b < size; ++b) {
							buffer [b] = buffer [b + 1];
						}
						buffer [size] = default(T);
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Remove an item at the specified index.
		/// </summary>

		public void RemoveAt (int index)
		{
			if (buffer != null && index > -1 && index < size) {
				--size;
				buffer [index] = default(T);
				for (int b = index; b < size; ++b) {
					buffer [b] = buffer [b + 1];
				}
				buffer [size] = default(T);
			}
		}

		/// <summary>
		/// Remove an item from the end.
		/// </summary>

		public T Pop ()
		{
			if (buffer != null && size != 0) {
				T val = buffer [--size];
				buffer [size] = default(T);
				return val;
			}
			return default(T);
		}

		/// <summary>
		/// Mimic List's ToArray() functionality, except that in this case the list is resized to match the current size.
		/// </summary>

		public T[] ToArray ()
		{
			Trim ();
			return buffer;
		}

		/// <summary>
		/// List.Sort equivalent. Manual sorting causes no GC allocations.
		/// </summary>

		[DebuggerHidden]
		[DebuggerStepThrough]
		public void Sort (System.Comparison<T> comparer)
		{
//			int start = 0;
//			int max = size - 1;
//			bool changed = true;
//
//			while (changed) {
//				changed = false;
//
//				for (int i = start; i < max; ++i) {
//					// Compare the two values
//					if (comparer (buffer [i], buffer [i + 1]) > 0) {
//						// Swap the values
//						T temp = buffer [i];
//						buffer [i] = buffer [i + 1];
//						buffer [i + 1] = temp;
//						changed = true;
//					} else if (!changed) {
//						// Nothing has changed -- we can start here next time
//						start = (i == 0) ? 0 : i - 1;
//					}
//				}
//			}
			HeapSort (comparer);
		}

		/// <summary>
		/// 已知H[s…m]除了H[s] 外均满足堆的定义
		/// 调整H[s],使其成为大顶堆.即将对第s个结点为根的子树筛选
		/// </summary>
		/// <param name="s">待调整的数组元素的位置.</param>
		/// <param name="length">数组的长度.</param>
		/// <param name="comparer">Comparer.</param>
		void HeapAdjust (int s, int length, System.Comparison<T> comparer)
		{  
			T tmp = buffer [s];  
			int child = 2 * s + 1; //左孩子结点的位置。(i+1 为当前调整结点的右孩子结点的位置)  
			while (child < length) {  
				if (child + 1 < length && comparer (buffer [child], buffer [child + 1]) < 0) { // 如果右孩子大于左孩子(找到比当前待调整结点大的孩子结点)  
					++child;
				}
				if (comparer (buffer [s], buffer [child]) < 0) {  // 如果较大的子结点大于父结点  
					buffer [s] = buffer [child]; // 那么把较大的子结点往上移动，替换它的父结点  
					s = child;       // 重新设置s ,即待调整的下一个结点的位置  
					child = 2 * s + 1;  
				} else {            // 如果当前待调整结点大于它的左右孩子，则不需要调整，直接退出  
					break;  
				}  
				buffer [s] = tmp;         // 当前待调整的结点放到比其大的孩子结点位置上  
			}
		}

		/// <summary>
		/// 初始堆进行调整 
		/// 将H[0..length-1]建成堆 
		/// 调整完之后第一个元素是序列的最小的元素 
		/// </summary>
		/// <param name="length">Length.</param>
		/// <param name="comparer">Comparer.</param>
		void BuildingHeap (System.Comparison<T> comparer)
		{   
			//最后一个有孩子的节点的位置 i=  (length -1) / 2  
			for (int i = (size - 1) / 2; i >= 0; --i) {
				HeapAdjust (i, size, comparer);
			}
		}

		/// <summary>
		/// 堆排序算法 
		/// </summary>
		/// <param name="length">Length.</param>
		/// <param name="comparer">Comparer.</param>
		void HeapSort (System.Comparison<T> comparer)
		{  
			//初始堆  
			BuildingHeap (comparer);
			//从最后一个元素开始对序列进行调整  
			for (int i = size - 1; i > 0; --i) {  
				//交换堆顶元素H[0]和堆中最后一个元素  
				T temp = buffer [i];
				buffer [i] = buffer [0];
				buffer [0] = temp;  
				//每次交换堆顶元素和堆中最后一个元素之后，都要对堆进行调整  
				HeapAdjust (0, i, comparer);  
			}  
		}
	}
}