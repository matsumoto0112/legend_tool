using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class Create2DMap : EditorWindow
{
    private static Create2DMap map;
    private Texture2D texture;
    private string texName;
    private float seedValue;
    private float relief;
    private bool isSmooth;
    private float smoothSize;
    private bool isDot, isEqualDot;

    private Vector2Int dotSize;
    private Vector2Int texSize;
    private Vector2 texPer;

    private float mapSize;
    private bool isMaxView;

    private Vector2 scrollPos;
    private Vector2 pramScrollpos;
    private Vector2 texScrollpos;
    private Vector2 chipScrollPos;

    private List<MapColorChip> colorList;

    [MenuItem("Create/Create2DMap")]
    static void Open()
    {
        map = GetWindow<Create2DMap>();
        map.SetParamater();
    }

    private void SetParamater()
    {
        texture = null;
        texName = "map";
        RandomSeed();
        relief = 75;

        isSmooth = false;
        smoothSize = 0.5f;

        isDot = false;
        isEqualDot = false;
        dotSize = Vector2Int.one * 8;

        texSize = Vector2Int.one * 128;
        texPer = Vector2.one;
        mapSize = 1.0f;
        isMaxView = false;

        scrollPos = Vector2.zero;
        pramScrollpos = Vector2.zero;
        texScrollpos = Vector2.zero;
        chipScrollPos = Vector2.zero;

        colorList = new List<MapColorChip>()
        {
            new MapColorChip(Color.white,0,1)
        };
    }

    void OnGUI()
    {
        if (map == null)
        {
            return;
        }

        if (isMaxView)
        {
            MaxView();
        }
        else
        {
            NormalView();
        }
    }

    #region View
    private void MaxView()
    {
        if (texture == null)
        {
            isMaxView = false;
            return;
        }

        var rect = new Rect(0, 0, 1, 1);
        var per = texPer.x + texPer.y;
        var size = position.width > position.height ? position.width : position.height;
        var width = texPer.x / per * size;
        var height = texPer.y / per * size;

        if (GUILayout.Button(GUIContent.none, GUILayout.Width(width), GUILayout.Height(height)))
        {
            isMaxView = !isMaxView;
        }
        GUI.DrawTextureWithTexCoords(GUILayoutUtility.GetLastRect(), texture, rect);
    }
    private void NormalView()
    {
        map.scrollPos = EditorGUILayout.BeginScrollView(map.scrollPos);
        {
            TexParamater();
            DrawMap();
            SetColorChip();
        }
        EditorGUILayout.EndScrollView();
    }
    #endregion

    #region TexParamater
    private void TexParamater()
    {
        GUI.contentColor = Color.black;
        GUI.backgroundColor = Color.white * 0.5f;

        EditorGUILayout.BeginVertical(GUI.skin.box);
        {
            GUI.backgroundColor = Color.white;
            pramScrollpos = EditorGUILayout.BeginScrollView(pramScrollpos);
            {
                TexButton();

                GUI.backgroundColor = Color.white * 0.75f;
                EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.MinWidth(position.width - 20));
                {
                    GUI.backgroundColor = Color.white;
                    texName = EditorGUILayout.TextField("画像名", texName, GUI.skin.textField);
                    seedValue = EditorGUILayout.FloatField("シード値", seedValue);
                    relief = EditorGUILayout.FloatField("滑らかさ", relief);
                    texSize = EditorGUILayout.Vector2IntField("画像サイズ", texSize);
                    texSize = new Vector2Int(Mathf.Max(texSize.x, 32), Mathf.Max(texSize.y, 32));
                }

                SmoothParamater();
                DotParamater();
            }
            EditorGUILayout.EndScrollView();
        }
        EditorGUILayout.EndVertical();
    }

    private void TexButton()
    {
        var width = (position.width - 20) * 1.0f;
        using (new EditorGUILayout.HorizontalScope(GUILayout.Width(width)))
        {
            if ((texture != null) && GUILayout.Button("画像削除"))
            {
                DestroyImmediate(texture);
            }
            if (GUILayout.Button("ランダムシード"))
            {
                RandomSeed();
            }
        }

        using (new EditorGUILayout.HorizontalScope(GUILayout.Width(width)))
        {
            GUILayout.Label("生成");
            if (GUILayout.Button("生成"))
            {
                MapMake();
            }
            if (GUILayout.Button("通常生成"))
            {
                isSmooth = false;
                isDot = false;
                MapMake();
            }
            if (GUILayout.Button("自然化生成"))
            {
                isSmooth = true;
                MapMake();
            }
            if (GUILayout.Button("ドット生成"))
            {
                isDot = true;
                MapMake();
            }
        }

        using (new EditorGUILayout.HorizontalScope(GUILayout.Width(width)))
        {
            GUILayout.Label("乱数生成");
            if (GUILayout.Button("乱数生成"))
            {
                RandomSeed();
                relief = Random.Range(50.0f, 250.0f);
                smoothSize = Random.Range(0.0f, 0.5f);
                dotSize = new Vector2Int(Random.Range(1, texSize.x), Random.Range(1, texSize.y));
                MapMake();
            }
            if (GUILayout.Button("シード値乱数生成"))
            {
                RandomSeed();
                MapMake();
            }
            if (GUILayout.Button("滑らかさ乱数生成"))
            {
                relief = Random.Range(50.0f, 250.0f);
                MapMake();
            }
            if (GUILayout.Button("自然化乱数生成"))
            {
                isSmooth = true;
                smoothSize = Random.Range(0.0f, 0.5f);
                MapMake();
            }
            if (GUILayout.Button("ドット乱数生成"))
            {
                isDot = true;
                dotSize = new Vector2Int(Random.Range(1, texSize.x), Random.Range(1, texSize.y));
                if (isEqualDot) dotSize = Vector2Int.one * (Random.Range(0, Mathf.Min(texSize.x, texSize.y)));
                MapMake();
            }
        }
    }
    private void SmoothParamater()
    {
        var isView = false;
        if (isSmooth)
        {
            GUI.backgroundColor = Color.white * 0.9f;
            EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.MinWidth(position.width - 20));
            GUI.backgroundColor = Color.white;
            isView = true;
        }

        isSmooth = EditorGUILayout.Toggle("自然化", isSmooth);
        if (isSmooth)
        {
            smoothSize = EditorGUILayout.Slider("範囲", smoothSize, 0.0f, 0.5f);
        }

        if (isView)
        {
            EditorGUILayout.EndVertical();
        }
    }
    private void DotParamater()
    {
        var isView = false;
        if (isDot)
        {
            GUI.backgroundColor = Color.white * 0.9f;
            EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.MinWidth(position.width - 20));
            GUI.backgroundColor = Color.white;
            isView = true;
        }

        isDot = EditorGUILayout.Toggle("ドット化", isDot);
        if (isDot)
        {
            isEqualDot = EditorGUILayout.Toggle("サイズをそろえる", isEqualDot);
            if (isEqualDot)
            {
                var size = dotSize.x;
                size = EditorGUILayout.IntSlider("ドットサイズ", size, 1, Mathf.Min(texSize.x, texSize.y));
                dotSize = Vector2Int.one * size;
            }
            else
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    dotSize.x = EditorGUILayout.IntSlider("ドットサイズ_X", dotSize.x, 1, texSize.x);
                    dotSize.y = EditorGUILayout.IntSlider("ドットサイズ_Y", dotSize.y, 1, texSize.y);
                }
            }
        }

        if (isView)
        {
            EditorGUILayout.EndVertical();
        }
    }
    #endregion

    private void MapMake()
    {
        texture = CreateTexture();
        texPer = new Vector2(texSize.x, texSize.y) / texSize.magnitude;
    }

    private void RandomSeed()
    {
        seedValue = (int)(Random.value * 10000);
    }

    #region DrawMap
    private void DrawMap()
    {
        if (texture)
        {
            texScrollpos = EditorGUILayout.BeginScrollView(texScrollpos);
            EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.MinWidth(position.width - 20), GUILayout.MaxHeight(position.height / 3.0f));
            {
                mapSize = EditorGUILayout.Slider("描画サイズ", mapSize, 0.0f, 1.0f);
                var per = texPer.x + texPer.y;
                var rect = new Rect(0, 0, 1, 1);
                var size = position.width > position.height ? position.width : position.height;
                var width = texPer.x / per * size * mapSize;
                var height = texPer.y / per * size * mapSize;

                if (GUILayout.Button(GUIContent.none, GUILayout.Width(width), GUILayout.Height(height)))
                {
                    isMaxView = !isMaxView;
                }
                GUI.DrawTextureWithTexCoords(GUILayoutUtility.GetLastRect(), texture, rect);

                if (texture && GUILayout.Button("セーブ"))
                {
                    Save();
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }
    }

    private void Save()
    {
        Create.SaveTexPNG(texName, texture);
    }
    #endregion

    #region SetColorChip
    private void SetColorChip()
    {
        GUI.backgroundColor = Color.white / 2.0f;
        // --------- 0 ------------
        EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width((position.width - 20)), GUILayout.MaxHeight(position.height / 4.0f));
        GUI.backgroundColor = Color.white;

        // --------- 1 ------------
        using (new EditorGUILayout.HorizontalScope())
        {
            ChipButton();
            GUI.backgroundColor = Color.white * 0.75f;
            // --------- 2 ------------
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                // --------- 3 ------------
                GUI.backgroundColor = Color.white;
                chipScrollPos = EditorGUILayout.BeginScrollView(chipScrollPos);
                {
                    for (int i = 0; i < colorList.Count; i++)
                    {
                        var obj = colorList[i];
                        var before = ((0 < i) ? colorList[i - 1] : null);
                        var after = ((i < (colorList.Count - 1)) ? colorList[i + 1] : null);

                        obj.OnGUI(i, before, after);
                    }
                }
                EditorGUILayout.EndScrollView();
                // --------- 3 ------------
            }
            EditorGUILayout.EndVertical();
            // --------- 2 ------------
        }
        // --------- 1 ------------

        EditorGUILayout.EndVertical();
        // --------- 0 ------------
    }

    private void ChipButton()
    {
        GUI.backgroundColor = Color.white * 0.75f;
        EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width((position.width - 20) * 0.2f));
        {
            GUI.backgroundColor = Color.white;
            if (GUILayout.Button("Add"))
            {
                var beforeChip = colorList[colorList.Count - 1];
                colorList.Add(new MapColorChip(beforeChip.color, beforeChip.maxValue));
            }

            if (GUILayout.Button("Remove"))
            {
                colorList.RemoveAt(colorList.Count - 1);
                if (colorList.Count == 0)
                {
                    colorList.Add(new MapColorChip(Color.white));
                }
                colorList[colorList.Count - 1].maxValue = 1;
            }

            if (GUILayout.Button("Average"))
            {
                for (int i = 0; i < colorList.Count; i++)
                {
                    var befChip = (i > 0) ? colorList[i - 1] : null;
                    var aftChip = (i < colorList.Count - 1) ? colorList[i + 1] : null;
                    var chip = colorList[i];
                    var value = 1.0f / colorList.Count;

                    chip.minValue = (befChip == null) ? 0 : befChip.maxValue;
                    chip.maxValue = (aftChip == null) ? 1 : (chip.minValue + value);
                }
            }
        }
        EditorGUILayout.EndVertical();
    }
    #endregion

    private Texture2D CreateTexture()
    {
        var colors = new Color[(int)texSize.y, (int)texSize.x];

        for (int y = 0; y < (int)texSize.y; y++)
        {
            for (int x = 0; x < (int)texSize.x; x++)
            {
                var valueX = x;
                var valueY = y;
                if (isDot)
                {
                    valueX = ((int)((float)x / (float)dotSize.x)) * dotSize.x;
                    valueY = ((int)((float)y / (float)dotSize.y)) * dotSize.y;
                }

                colors[y, x] = GetColor(Vector2.zero, valueX, valueY);
            }
        }

        return Create.GetTex2D(colors);
    }

    private Color GetColor(Vector2 pos, int x, int y)
    {
        var noise = GetNoise(pos, x, y);
        var col = Color.white;

        for (int i = 0; i < colorList.Count; i++)
        {
            var chip = colorList[i];
            if (chip.Range(noise))
            {
                col = chip.color;
                if (isSmooth)
                {
                    var bef = (i > 0) ? colorList[i - 1] : null;
                    var aft = (i < colorList.Count - 1) ? colorList[i + 1] : null;
                    col = chip.SmoothColor(bef, aft, noise, smoothSize);
                }
                break;
            }
        }

        return (col == null) ? (Color.white) : col;
    }

    private float GetNoise(Vector2 pos, int x, int y)
    {
        float noiseX = (pos.x + x + seedValue) / relief;
        float noiseY = (pos.y + y + seedValue) / relief;
        float noise = Mathf.PerlinNoise(noiseX, noiseY);
        return Mathf.Clamp(noise, 0.0f, 1.0f);
    }
}

public class MapColorChip
{
    public Color color;
    public float minValue, maxValue;

    public MapColorChip(Color color, float minValue = 0, float maxValue = 1)
    {
        this.color = color;
        this.minValue = minValue;
        this.maxValue = maxValue;
    }

    public bool Range(float value)
    {
        return minValue <= value && value <= maxValue;
    }

    public Color SmoothColor(MapColorChip before, MapColorChip after, float value, float size)
    {
        var v = (value - minValue) / (maxValue - minValue);
        var col = color;
        if ((before != null) && (after != null))
        {
            var befCol = before.color + (color - before.color) * 0.5f;
            var aftCol = color + (after.color - color) * 0.5f;

            if (v < size)
            {
                col = befCol + (col - befCol) * (v / size);
            }
            else if (v >= (1.0f - size))
            {
                var s = 1.0f - size;
                col = col + (aftCol - col) * ((v - s) / size);
            }
        }
        else if (before != null)
        {
            var befCol = before.color + (color - before.color) * 0.5f;
            if (v < size)
            {
                col = befCol + (col - befCol) * (v / size);
            }
        }
        else if (after != null)
        {
            var aftCol = color + (after.color - color) * 0.5f;
            if (v >= (1.0f - size))
            {
                var s = 1.0f - size;
                col = col + (aftCol - col) * ((v - s) / size);
            }
        }
        return col;
    }

    public void OnGUI(int index, MapColorChip before, MapColorChip after)
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        {
            EditorGUILayout.LabelField("[ " + index + " ]");
            GUILayout.Space(5);
            if (before == null) minValue = 0;
            if (after == null) maxValue = 1;

            var min = minValue;
            var max = maxValue;

            using (new EditorGUILayout.HorizontalScope())
            {
                minValue = EditorGUILayout.FloatField(minValue);
                maxValue = EditorGUILayout.FloatField(maxValue);

                EditorGUILayout.MinMaxSlider(ref minValue, ref maxValue, 0, 1);
                if (/*(min != minValue) &&*/ (before != null))
                {
                    before.maxValue = minValue;
                    before.minValue = Mathf.Min(before.minValue, before.maxValue);
                }
                if (/*(max != maxValue) &&*/ (after != null))
                {
                    after.minValue = maxValue;
                    after.maxValue = Mathf.Max(after.minValue, after.maxValue);
                }

            }

            GUILayout.Space(5);
            color = EditorGUILayout.ColorField(color);
        }
        EditorGUILayout.EndVertical();
    }
}