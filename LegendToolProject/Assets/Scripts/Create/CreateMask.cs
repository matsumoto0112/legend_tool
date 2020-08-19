using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class CreateMask : EditorWindow
{
    private static CreateMask mask;
    private Texture2D texture;
    private string texName;
    private float seedValue;
    private float relief;
    private Vector2Int texSize;
    private Vector2 texPer;
    private int maskSize;

    [MenuItem("Create/CreateMask")]
    static void Open()
    {
        mask = GetWindow<CreateMask>();
        mask.texture = null;
        mask.texName = "mask";
        mask.RandomSeed();
        mask.relief = 75;
        mask.texSize = Vector2Int.one * 128;
        mask.texPer = Vector2.one;
        mask.maskSize = 128;
    }

    void OnGUI()
    {
        if (mask == null)
        {
            return;
        }

        TexParamater();
        DrawMask();
    }


    private void TexParamater()
    {
        GUI.contentColor = Color.black;
        EditorGUILayout.BeginVertical(GUI.skin.box);
        {
            TexButton();

            texName = EditorGUILayout.TextField("マスク名", texName, GUI.skin.textField);
            seedValue = EditorGUILayout.FloatField("シード値", seedValue);
            relief = EditorGUILayout.FloatField("滑らかさ", relief);
            texSize = EditorGUILayout.Vector2IntField("画像サイズ", texSize);
            texSize = new Vector2Int(Mathf.Max(texSize.x, 32), Mathf.Max(texSize.y, 32));
        }
        EditorGUILayout.EndVertical();
    }

    private void TexButton()
    {
        if (GUILayout.Button("ランダムシード"))
        {
            RandomSeed();
        }
        if (GUILayout.Button("マスクメイク"))
        {
            MaskMake();
        }
        if (GUILayout.Button("ランダムマスクメイク"))
        {
            RandomSeed();
            MaskMake();
        }
    }

    private void DrawMask()
    {
        if(texture)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                maskSize = EditorGUILayout.IntSlider("描画サイズ", maskSize, 32, (int)(mask.position.width));

                var rect = new Rect(0, 0, 1, 1);
                GUILayout.Box(GUIContent.none, GUILayout.Width(texPer.x * maskSize), GUILayout.Height(texPer.y * maskSize));
                GUI.DrawTextureWithTexCoords(GUILayoutUtility.GetLastRect(), texture, rect);

                if (texture && GUILayout.Button("セーブ"))
                {
                    Save();
                }
            }
            EditorGUILayout.EndVertical();
        }
    }

    private void Save()
    {
        Create.SaveTexPNG(texName, texture);
    }

    private void MaskMake()
    {
        texture = CreateTexture();
        texPer = new Vector2(texSize.x, texSize.y) / texSize.magnitude;
    }

    private Texture2D CreateTexture()
    {
        var colors = new Color[(int)texSize.y, (int)texSize.x];

        for (int y = 0; y < (int)texSize.y; y++)
        {
            for (int x = 0; x < (int)texSize.x; x++)
            {
                var noise = GetNoise(Vector2.zero, x, y);
                var color = new Color(noise, noise, noise);
                colors[y, x] = color;
            }
        }

        return Create.GetTex2D(colors);
    }

    private float GetNoise(Vector2 pos, int x, int y)
    {
        float noiseX = (pos.x + x + seedValue) / relief;
        float noiseY = (pos.y + y + seedValue) / relief;
        float noise = Mathf.PerlinNoise(noiseX, noiseY);
        return Mathf.Clamp(noise, 0.0f, 1.0f);
    }

    private void RandomSeed()
    {
        seedValue = (int)(Random.value * 10000);
    }
}
