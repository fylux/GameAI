using UnityEngine;
using System.Collections;
using System;

public interface IHeapItem<T> : IComparable<T> {
    int HeapIndex { get; set; }
}

public class Heap<T> where T : IHeapItem<T> {
	
	T[] items;
	int count;
    public int Count { get { return count; }}

    public Heap(int maxHeapSize) {
		items = new T[maxHeapSize];
	}
	
	public void Add(T item) {
		item.HeapIndex = count;
		items[count] = item;
		SortUp(item);
		count++;
	}

	public T Pop() {
		T firstItem = items[0];
		count--;
		items[0] = items[count];
		items[0].HeapIndex = 0;
		SortDown(items[0]);
		return firstItem;
	}

	public void UpdateItem(T item) {
		SortUp(item);
	}

	public bool Contains(T item) {
		return Equals(items[item.HeapIndex], item);
	}

	void SortDown(T item) {
		while (true) {
			int childIndexLeft = item.HeapIndex * 2 + 1;
			int childIndexRight = item.HeapIndex * 2 + 2;
			int swapIndex = 0;

			if (childIndexLeft < count) {
				swapIndex = childIndexLeft;

				if (childIndexRight < count && items[childIndexLeft].CompareTo(items[childIndexRight]) < 0) {
					swapIndex = childIndexRight;
				}

				if (item.CompareTo(items[swapIndex]) < 0) {
					Swap(item,items[swapIndex]);
				}
				else {
					return;
				}

			}
			else {
				return;
			}

		}
	}
	
	void SortUp(T item) {
		int parentIndex = (item.HeapIndex-1)/2;
		
		while (true) {
			T parentItem = items[parentIndex];
			if (item.CompareTo(parentItem) > 0) {
				Swap(item,parentItem);
			}
			else {
				break;
			}
			parentIndex = (item.HeapIndex-1)/2;
		}
	}
	
	void Swap(T itemA, T itemB) {
		items[itemA.HeapIndex] = itemB;
		items[itemB.HeapIndex] = itemA;
        var temp = itemB.HeapIndex;
        itemB.HeapIndex = itemA.HeapIndex;
        itemA.HeapIndex = temp;
	}
	
}

