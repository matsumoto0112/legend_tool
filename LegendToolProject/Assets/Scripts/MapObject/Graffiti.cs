using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graffiti : MapObject
{
    [Space]
    [Header("落書きのパラメータ")]
    public int textureID;

    public override void Input(float[] index)
    {
        base.Input(index);

        textureID = (int)(index[12]);
    }

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

        text += "," + mass;
        text += "," + friction;
        text += "," + restitution;

        text += "," + textureID;

        text += "\n";

        return text;
    }

    private void OnValidate()
    {
        ModelChanger changer = this.transform.GetComponent<ModelChanger>();
        changer.ChangeModel(textureID);
    }
}
