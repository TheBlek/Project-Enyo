using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class Heap<T> where T : IHeapItem<T>
{
    T[] items;
    int currentItemCount;

    public Heap(int maxHeapSize)
    {
        items = new T[maxHeapSize];
        currentItemCount = 0;
    }

    public void Add(T item)
    {
        items[currentItemCount] = item;
        item.HeapIndex = currentItemCount;
        currentItemCount++;
        SortUp(item);
    }

    public T RemoveFirst()
    {
        T first = items[0];
        currentItemCount--;
        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;
        SortDown(items[0]);
        return first;
    }

    public bool Contains(T item)
    {
        return Equals(items[item.HeapIndex], item);
    }

    public int Count
    {
        get => currentItemCount;
    }

    public void UpdateItem(T item)
    {
        SortUp(item);
    }

    private void SortDown(T item)
    {
        int leftChildIndex = item.HeapIndex * 2 + 1;
        int rightChildIndex = item.HeapIndex * 2 + 2;
        int changeIndex = leftChildIndex;

        while (leftChildIndex < currentItemCount)
        {
            if (rightChildIndex < currentItemCount && items[leftChildIndex].CompareTo(items[rightChildIndex]) < 0)
                changeIndex = rightChildIndex;

            if (item.CompareTo(items[changeIndex]) < 0)
                Swap(item, items[changeIndex]);
            else
                return;

            leftChildIndex = item.HeapIndex * 2 + 1;
            rightChildIndex = item.HeapIndex * 2 + 2;
            changeIndex = leftChildIndex;
        }
    }

    private void SortUp(T item)
    {
        int parentIndex = (item.HeapIndex - 1) / 2;
        T parentItem = items[parentIndex];
        while (item.CompareTo(parentItem) > 0)
        {
            Swap(item, parentItem);
            parentIndex = (item.HeapIndex - 1) / 2;
            parentItem = items[parentIndex];
        }
    }

    private void Swap(T itemA, T itemB)
    {
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;
        int tmp = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = tmp;
    } 

}

public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex
    {
        get;
        set;
    }
}