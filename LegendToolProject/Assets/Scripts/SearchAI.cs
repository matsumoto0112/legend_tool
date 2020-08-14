using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchAI : MapObject
{
    [Space]
    [Header("敵AI用のパラメータ")]
    public int thisID = -1;
    public SearchAI branch1;
    public SearchAI branch2;
    public SearchAI branch3;
    public SearchAI branch4;

    //[SerializeField] private LineRenderer line;

    private string OutputBranch(SearchAI searchAI)
    {
        if (searchAI == null)
            return "-1";

        return searchAI.thisID.ToString();
        
    }

    public override void Input(float[] index)
    {
        base.Input(index);

        //branch1 = (int)(index[12]);
        //branch2 = (int)(index[13]);
        //branch3 = (int)(index[14]);
        //branch4 = (int)(index[15]);
    }

    public override string Output()
    {
        string text = "searchai";

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

        text += "," + OutputBranch(branch1);
        text += "," + OutputBranch(branch2);
        text += "," + OutputBranch(branch3);
        text += "," + OutputBranch(branch4);

        text += "\n";

        return text;
    }

    public override void CheckActiveTurn(int turnCount)
    {
        gameObject.SetActive(SceneInformation.isAllView || turnCount == 0);
    }
}
