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
        Stationery
    }

    public ObjectType objectType;

    [Header("物理パラメータ")]
    public float mass;
    public float yg;
    public float dc;

    private void OnDrawGizmos()
    {
        switch (objectType)
        {
            case ObjectType.StartPoint:
                UnityEditor.Handles.Label(transform.position, "開始地点");
                break;

            case ObjectType.Enemy:
                UnityEditor.Handles.Label(transform.position, "Enemy");
                break;

            case ObjectType.Boss:
                UnityEditor.Handles.Label(transform.position, "Boss");
                break;

            case ObjectType.Obstacle:
                UnityEditor.Handles.Label(transform.position, "障害物");
                break;
        }
    }
}
