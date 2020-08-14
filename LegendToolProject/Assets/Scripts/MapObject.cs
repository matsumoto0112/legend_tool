using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MapObject : MonoBehaviour
{
    public enum ObjectType {
        None = 0,
        StartPoint,
        Enemy,
        Boss,
        Floor,
        Obstacle,
        Graffiti,
        Stationery,
        SearchAI,
    }

    public ObjectType objectType;

    [Header("物理パラメータ")]
    public float mass;
    public float yg;
    public float dc;

    //関係のあるターンか調べる
    public virtual void CheckActiveTurn(int turnCount)
    { }

    public virtual void Input(float[] index)
    {
        transform.position = new Vector3(index[0], index[1], index[2]);
        transform.rotation = Quaternion.Euler(index[3], index[4], index[5]);
        transform.localScale = new Vector3(index[6], index[7], index[8]);

        mass = index[9];
        yg = index[10];
        dc = index[11];
    }

    //出力時のテキストを返す
    public virtual string Output()
    {
        string text = "";

        switch (objectType)
        {
            case ObjectType.None:
                text += "none";
                break;

            case ObjectType.Floor:
                text += "floor";
                break;
        }

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

        text += "\n";

        return text;
    }

    private void OnValidate()
    {
        CheckActiveTurn(SceneInformation.turnCount);
    }

    //どのオブジェクトタイプかを表示する
    private void OnDrawGizmos()
    {
        GUIStyle style = new GUIStyle(EditorStyles.label);
        style.normal.textColor = Color.yellow;

        switch (objectType)
        {
            case ObjectType.StartPoint:
                UnityEditor.Handles.Label(transform.position, "開始地点", style);
                break;

            case ObjectType.Enemy:
                UnityEditor.Handles.Label(transform.position, "敵", style);
                break;

            case ObjectType.Boss:
                UnityEditor.Handles.Label(transform.position, "ボス", style);
                break;

            case ObjectType.Obstacle:
                UnityEditor.Handles.Label(transform.position, "障害物", style);
                break;

            case ObjectType.Graffiti:
                UnityEditor.Handles.Label(transform.position, "落書き", style);
                break;

            case ObjectType.Stationery:
                UnityEditor.Handles.Label(transform.position, "文房具", style);
                break;
        }
    }
}
