using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularList<T>
{
    private List<T> elements;
    private int index;

    public CircularList()
    {
        elements = new();
        index = 0;
    }

    public T Current => elements[index];

    public int Count => elements.Count;

    public void Push(T item) => elements.Add(item);

    public void Pop() => elements.RemoveAt(Count - 1);

    public void SetIndexPosition(int pos) => index = pos > -1 ? pos : 0;

    public void MoveNext() => index = index < Count - 1 ? index + 1 : 0;

    public void MoveBack() => index = index > 0 ? index - 1 : Count - 1;
}
