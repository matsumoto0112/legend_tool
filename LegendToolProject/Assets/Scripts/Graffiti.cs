using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graffiti : MapObject
{
    [Space]
    [Header("落書きのパラメータ")]
    public int textureID;

    public override string Output()
    {
        string text = "graffiti";

        text += "," + transform.position.x;
        text += "," + transform.position.y;
        text += "," + transform.position.z;
        text += "," + transform.eulerAngles.x;
        text += "," + transform.eulerAngles.y;
        text += "," + transform.eulerAngles.z;
        text += "," + transform.lossyScale.x;
        text += "," + transform.lossyScale.y;
        text += "," + transform.lossyScale.z;

        text += "," + textureID;

        text += "\n";

        return text;
    }
}
