using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MapObject
{
    [Space]
    [Header("ボス用パラメータ")]
    public int modelID;
    public int bossID;
    public int generateTurn;
    public int durability;

    public override void Input(float[] index)
    {
        base.Input(index);
        
        modelID = (int)(index[12]);
        bossID = (int)(index[13]);
        generateTurn = (int)(index[14]);
        durability = (int)(index[15]);
    }

    public override string Output()
    {
        string text = "boss";

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
        text += "," + bossID;
        text += "," + generateTurn;
        text += "," + durability;

        text += "\n";

        return text;
    }

    public override void CheckActiveTurn(int turnCount)
    {
        gameObject.SetActive(SceneInformation.isAllView || turnCount == generateTurn);
    }
}
