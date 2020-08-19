using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class CreateRamp : EditorWindow
{
    private static CreateRamp rampWindow;
    private List<Color> colorList;
    private List<float> areaList;
    private List<int> colCounter;
    private string texName;
    private int size;
    private Texture2D tex, popTex;
    private Vector2 scrollPos;

    [MenuItem("Create/CreateRamp")]
    static void Open()
    {
        rampWindow = GetWindow<CreateRamp>();
        rampWindow.colorList = new List<Color>();
        rampWindow.areaList = new List<float>();
        rampWindow.colCounter = new List<int>();
        rampWindow.titleContent.text = "Ramp";
        rampWindow.scrollPos = Vector2.zero;
    }

    void OnGUI()
    {
        if (rampWindow == null)
        {
            return;
        }

        var befColorList = new List<Color>(rampWindow.colorList);
        SetTexture();
        Create();
        DrawColor();
        PopTexture(befColorList);
    }

    private void SetTexture()
    {
        var befTex = rampWindow.tex;
        rampWindow.tex = (Texture2D)EditorGUILayout.ObjectField(
            "画像", rampWindow.tex, typeof(Texture2D), false);
        //Debug.Log((befTex != rampWindow.tex) + " && " + rampWindow.tex != null);

        if ((befTex != rampWindow.tex) && rampWindow.tex != null)
        {
            var tex = rampWindow.tex;
            var newColList = new List<Color>();
            rampWindow.size = tex.width;

            var assetPath = AssetDatabase.GetAssetPath(tex);
            var ti = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            var oldIsReadable = ti.isReadable;
            var oldImporterFormat = ti.GetDefaultPlatformTextureSettings();
            ti.isReadable = true;
            oldImporterFormat.format = TextureImporterFormat.RGBA32;
            AssetDatabase.ImportAsset(assetPath);
            rampWindow.colCounter = new List<int>();

            for (var i = 0; i < tex.width; i++)
            {
                var color = tex.GetPixel(i, 0);
                if ((newColList.Count <= 0) || (newColList[newColList.Count - 1] != color))
                {
                    newColList.Add(color);
                    rampWindow.colCounter.Add(1);
                }
                else
                {
                    rampWindow.colCounter[rampWindow.colCounter.Count - 1]++;
                }
            }
            rampWindow.colorList = newColList;

            ti.isReadable = oldIsReadable;
            ti.SetPlatformTextureSettings(oldImporterFormat);
            AssetDatabase.ImportAsset(assetPath);
        }

        var name = rampWindow.tex ? rampWindow.tex.name : rampWindow.texName;
        rampWindow.texName = GUILayout.TextField(name);
        //if (rampWindow.tex) rampWindow.name = rampWindow.tex.name;
    }
    byte[] ReadFile(string path)
    {
        var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
        var bin = new BinaryReader(fileStream);
        var values = bin.ReadBytes((int)bin.BaseStream.Length);

        bin.Close();

        return values;
    }

    private void Create()
    {
        if (GUILayout.Button("Create"))
        {
            Debug.Log("create");
            if (string.IsNullOrEmpty(rampWindow.texName))
            {
                Debug.Log("NULL_NAME");
            }
            else
            {
                Save();
            }
        }
        GUILayout.Space(20);
    }

    private void DrawColor()
    {
        rampWindow.scrollPos = EditorGUILayout.BeginScrollView(rampWindow.scrollPos, GUI.skin.box);
        {
            var colorList = rampWindow.colorList;
            var listCount = colorList.Count;
            listCount = Mathf.Max(0, EditorGUILayout.IntField("画素数", listCount));
            rampWindow.size = (int)EditorGUILayout.Slider("画像サイズ", rampWindow.size, listCount, 1024);

            if (colorList.Count < listCount)
            {
                var add = listCount - colorList.Count;
                for (int i = 0; i < add; i++)
                {
                    colorList.Add(Color.white);
                }
            }
            else if (colorList.Count > listCount)
            {
                var minus = colorList.Count - listCount;
                for (int i = 0; i < minus; i++)
                {
                    colorList.RemoveAt(colorList.Count - 1);
                }
            }

            if (colorList.Count < colCounter.Count)
            {
                var count = colorList.Count - colCounter.Count;
                for (int i = 0; i < count; i++)
                {
                    colCounter.RemoveAt(colCounter.Count - 1);
                }
            }
            else if (colorList.Count > colCounter.Count)
            {
                var count = colCounter.Count - colorList.Count;
                for (int i = 0; i < count; i++)
                {
                    var value = colCounter.Count > 0 ? colCounter[colCounter.Count - 1]:0;
                    colCounter.Add(value);
                }
            }

            for (int i = 0; i < colorList.Count; i++)
            {
                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                {
                    var label = "[ " + i + " ]";
                    colorList[i] = EditorGUILayout.ColorField(label, colorList[i]);
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndScrollView();

        rampWindow.colorList = colorList;
    }

    private void PopTexture(List<Color> befColorList)
    {
        //Debug.Log((befColorList.Count + " / " + rampWindow.colorList.Count));
        if (befColorList != rampWindow.colorList)
        {
            popTex = CreateTex();
        }

        EditorGUILayout.BeginVertical(GUI.skin.box);
        {
            EditorGUILayout.LabelField("RampTexture");
            GUILayout.Box(GUIContent.none, GUILayout.Width(50), GUILayout.Height(50));
            var rect = new Rect();
            if (popTex)
            {
                rect = new Rect(0, 0, 1, 1);
                GUI.DrawTextureWithTexCoords(GUILayoutUtility.GetLastRect(), popTex, rect);
                //Debug.Log("Pop");
            }
        }
        EditorGUILayout.EndVertical();
    }

    private void Save()
    {
        var path = DataPath();
        rampWindow.tex = CreateTex();
        File.WriteAllBytes(DataPath(), rampWindow.tex.EncodeToPNG());

        AssetDatabase.Refresh();
    }

    private Texture2D CreateTex()
    {
        var list = rampWindow.colorList;
        var size = rampWindow.size;
        var tex = new Texture2D(size, size, TextureFormat.ARGB32, false);

        for (int y = 0; y < tex.height; y++)
        {
            for (int x = 0; x < tex.width; x++)
            {
                int colorCount = (int)(((float)x / (float)size) * (float)rampWindow.colorList.Count);
                var color = list[colorCount];
                tex.SetPixel(x, y, color);
            }
        }

        tex.Apply();

        return tex;
    }

    private string DataPath(string name = "")
    {
        if (string.IsNullOrEmpty(name))
        {
            name = rampWindow.texName;
        }
        return Application.dataPath + "/" + name + ".png";
    }
}