using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MapObject
{
    [Space]
    [Header("障害物のパラメータ")]
    public int modelID;

    public override void Input(float[] index)
    {
        base.Input(index);

        modelID = (int)(index[12]);
    }

    public override string Output()
    {
        string text = "obstacle";

        text += "," + transform.position.x;
        text += "," + transform.position.y;
        text += "," + transform.position.z;
        text += "," + transform.eulerAngles.x;
        text += "," + transform.eulerAngles.y;
        text += "," + transform.eulerAngles.z;
        text += "," + transform.lossyScale.x;
        text += "," + transform.lossyScale.y;
        text += "," + transform.lossyScale.z;

        text += "," + mass;
        text += "," + yg;
        text += "," + dc;

        text += "," + modelID;

        text += "\n";

        return text;
    }
}
