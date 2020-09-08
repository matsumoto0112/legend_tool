using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ModelChanger : MonoBehaviour
{
    [SerializeField] private List<GameObject> models;

    public void ChangeModel(int id)
    {
        if (id > models.Count)
        {
            Debug.LogError("指定されたIDが存在していません。");
            return;
        }

        //変更前のオブジェクトを削除
        foreach(Transform child in this.transform.GetChildren())
        {
            EditorApplication.delayCall += () => DestroyImmediate(child.gameObject);
        }

        var obj = GameObject.Instantiate(models[id], this.transform) as GameObject;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.Euler(Vector3.zero);

        Undo.RegisterCreatedObjectUndo(obj, "Change Model");
    }
}
