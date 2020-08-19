using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformEx
{
    public static List<Transform> GetAllChildren(this Transform t)
    {
        List<Transform> childrenList = new List<Transform>();
        t.GetChildren(ref childrenList);
        return childrenList;
    }

    public static List<Transform> GetChildren(this Transform t)
    {
        List<Transform> list = new List<Transform>();
        for (int i = 0; i < t.childCount; i++)
        {
            var child = t.GetChild(i);
            if (!list.Contains(child)) list.Add(child);
        }
        return list;
    }

    static List<Transform> GetChildren(this Transform t, ref List<Transform> list)
    {
        for (int i = 0; i < t.childCount; i++)
        {
            var child = t.GetChild(i);
            if (!list.Contains(child)) list.Add(child);
            child.GetChildren(ref list);
        }
        return list;
    }
}
