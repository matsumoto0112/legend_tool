﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MapObject
{
    [Space]
    [Header("エネミー用パラメータ")]
    public int modelID;
    public int aiID;
    public int generateTurn;

    public override void Input(float[] index)
    {
        base.Input(index);

        modelID = (int)(index[12]);
        aiID = (int)(index[13]);
        generateTurn = (int)(index[14]);
    }

    public override string Output()
    {
        string text = "enemy";

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
        text += "," + aiID;
        text += "," + generateTurn;

        text += "\n";

        return text;
    }

    public override void CheckActiveTurn(int turnCount)
    {
        gameObject.SetActive(SceneInformation.isAllView || turnCount == generateTurn);
    }
}
