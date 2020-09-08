using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stationery : MapObject
{
    [Space]
    [Header("文房具のパラメータ")]
    public int modelID;
    public int skillID;
    public int generateTurn;

    public override void Input(float[] index)
    {
        base.Input(index);

        modelID = (int)(index[12]);
        skillID = (int)(index[13]);
        generateTurn = (int)(index[14]);
    }

    public override string Output()
    {
        string text = "stationery";

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

        text += "," + modelID;
        text += "," + skillID;
        text += "," + generateTurn;

        text += "\n";

        return text;
    }

    public override void CheckActiveTurn(int turnCount)
    {
        gameObject.SetActive(SceneInformation.isAllView || turnCount >= generateTurn);
    }
}