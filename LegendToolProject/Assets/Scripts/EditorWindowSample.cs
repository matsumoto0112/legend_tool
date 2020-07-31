using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorWindowSample : EditorWindow
{
    [MenuItem("Editor/Sample")]
    private static void Create()
    {
        //生成
        GetWindow<EditorWindowSample>("サンプル");
    }

    /// <summary>
    /// ScriptableObjectSampleの変数
    /// </summary>
    private ScriptableObjectSample _sample;

    private void OnGUI()
    {
        if (_sample == null)
        {
            _sample = ScriptableObject.CreateInstance<ScriptableObjectSample>();
        }

        using (new GUILayout.HorizontalScope())
        {
            _sample.SampleIntValue = EditorGUILayout.IntField("サンプル", _sample.SampleIntValue);
        }
    }
}

