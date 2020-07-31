using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPoint : MapObject
{
    private void Awake()
    {
        objectType = ObjectType.StartPoint;
    }
}
