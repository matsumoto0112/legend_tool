using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPoint : MapObject
{
    [Space]
    [Header("プレイヤー用パラメータ")]
    public int modelID;

    public override void Input(float[] index)
    {
        base.Input(index);

        modelID = (int)(index[12]);
    }

    public override string Output()
    {
        string text = "startpoint";

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

    public override void CheckActiveTurn(int turnCount)
    {
        gameObject.SetActive(SceneInformation.isAllView || turnCount == 0);
    }
}
