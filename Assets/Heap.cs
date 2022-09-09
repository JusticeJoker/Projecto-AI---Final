using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//A Heap is basically a faster method to search nodes through a
// search tree and its weight
// To find a parent we use (n-1)/2
// to find a children we need to calculate 
// the Left Child with 2n+1
// and the Right Child 2n+2

/*       0
       /   \
      1     2
    /  \   / \
   3   4  5   6
*/      


public class Heap<T> where T: IHeapItem<T>
{
    T[] items;
    int currentItemCount;

    //maximum size of the Heap
    public Heap(int maxHeapSize)
    {
        items = new T[maxHeapSize];
    }


    public void Add(T item)
    {
        item.HeapIndex = currentItemCount;
        items[currentItemCount] = item;
        Sort(item);
        currentItemCount++;
    }

    public T RemoveFirst()
    {
        T firstItem = items[0];
        currentItemCount--;
        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;
        SortDown(items[0]);
        return firstItem;
    }

    //Change priority of an item
    //Incase we find a node in the pathfind
    // where we want to update with a lower fCost
    //Since there a new path for it and update its position to the Heap
    public void UpdateItem(T item)
    {
        Sort(item);
    }

    public int Count
    {
        get
        {
            return currentItemCount;
        }
    }


    //Check if the heap contains a specific item
    public bool Contains(T item)
    {
        //if its true it will return true, if not will return false
        //Check if two items are equal in the array that is being passed
        return Equals(items[item.HeapIndex], item); 
    }

    void SortDown(T item)
    {
        while (true)
        {
            int childIndexLeft = item.HeapIndex * 2 + 1;
            int childIndexRight = item.HeapIndex * 2 + 2;
            int swapIndex = 0;

            //Check which child has the highest priority
            if (childIndexLeft < currentItemCount)
            {
                swapIndex = childIndexLeft;
                
                if (childIndexRight < currentItemCount)
                {
                    if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                    {
                        swapIndex = childIndexRight;
                    }
                }

                //check if parent has a lower priority then its highest priority child
                if (item.CompareTo(items[swapIndex]) < 0)
                {
                    Swap(item, items[swapIndex]);
                }
                else //If they're are in their corrent position
                {
                    return;
                }
            }
            else //If they're are in their corrent position
            {
                return;
            }
        }
    }

    void Sort(T item)
    {
        //calculate parent
        int parentIndex = (item.HeapIndex - 1) / 2;

        while(true)
        {
            T parentItem = items[parentIndex];

            //if the item got a higher priority then parent item 
            // means lower fCost
            if (item.CompareTo(parentItem) > 0)
            {
                Swap(item, parentItem);
            }
            else
            {
                break;
            }

            parentIndex = (item.HeapIndex - 1) / 2;
        }
    }

    //Swap method
    void Swap(T itemA, T itemB)
    {
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;
        int itemAIndex = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = itemAIndex;
    }
}

//Compare items in the heap
public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex
    {
        get;
        set;
    }
}
