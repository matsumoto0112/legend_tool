using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class Create
{

    public static string DataPath(string name, string path = "/", string type = ".txt")
    {
        if (string.IsNullOrEmpty(name))
        {
            name = "new";
        }
        return Application.dataPath + path + name + type;
    }

    public static void SaveTexPNG(string name, Texture2D texture)
    {
#if UNITY_EDITOR
        File.WriteAllBytes(DataPath(name, "/", ".png"), texture.EncodeToPNG());
        UnityEditor.AssetDatabase.Refresh();
#endif
    }
    public static void SaveTexJPG(string name, Texture2D texture)
    {
#if UNITY_EDITOR
        File.WriteAllBytes(DataPath(name, "/", ".jpg"), texture.EncodeToJPG());
        UnityEditor.AssetDatabase.Refresh();
#endif
    }

    public static void SaveText(string[] strArray, string name, string path = "/", string type = ".txt")
    {
        var filePath = DataPath(name, path, type);
        File.WriteAllLines(filePath, strArray);
    }
    public static void SaveText(List<string> strList, string name, string path = "/", string type = ".txt")
    {
        SaveText(strList.ToArray(), name, path, type);
    }
    public static void SaveText(string str, string name, string path = "/", string type = ".txt")
    {
        var filePath = DataPath(name, path, type);
        File.WriteAllText(filePath, str);
    }

    public static Texture2D TexRectangle(int width, int height)
    {
        var tex = new Texture2D(width, height, TextureFormat.ARGB32, false);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                tex.SetPixel(x, y, Color.white);
            }
        }

        tex.Apply();
        return tex;
    }
    public static Texture2D TexRectangle(Vector2 size)
    {
        int width = (int)size.x;
        int height = (int)size.y;

        var tex = new Texture2D(width, height, TextureFormat.ARGB32, false);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                tex.SetPixel(x, y, Color.white);
            }
        }

        tex.Apply();
        return tex;
    }
    public static Texture2D TexRectangle(int size)
    {
        return TexRectangle(Vector2.one * size);
    }

    public static Texture2D GetTex2D(Color[,] colors)
    {
        int width = colors.GetLength(1);
        int height = colors.GetLength(0);
        var tex = new Texture2D(width, height, TextureFormat.ARGB32, false);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                tex.SetPixel(x, y, colors[y, x]);
            }
        }

        tex.Apply();
        return tex;
    }
    public static Texture2D GetTex2D(List<Color> colorList, int size)
    {
        var tex = new Texture2D(size, size, TextureFormat.ARGB32, false);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                var color = ((colorList != null) && (colorList.Count > 0)) ? colorList[Random.Range(0, colorList.Count - 1)] : Color.white;
                tex.SetPixel(x, y, color);
            }
        }

        tex.Apply();
        return tex;
    }

    public static Sprite SprRectangle(int width, int height, float pixelsPerUnit = 100.0f)
    {
        var tex = TexRectangle(width, height);
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one / 2.0f, pixelsPerUnit);
    }
    public static Sprite SprRectangle(Vector2 size, float pixelsPerUnit = 100.0f)
    {
        var tex = TexRectangle(size);
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one / 2.0f, pixelsPerUnit);
    }
    public static Sprite SprRectangle(int size, float pixelsPerUnit = 100.0f)
    {
        var tex = TexRectangle(size);
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one / 2.0f, pixelsPerUnit);
    }
    public static Sprite GetSprite(Color[,] colors, float pixelsPerUnit = 100.0f)
    {
        var tex = GetTex2D(colors);
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one / 2.0f, pixelsPerUnit);
    }
    public static Sprite GetSprite(Texture2D tex, float pixelsPerUnit = 100.0f)
    {
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one / 2.0f, pixelsPerUnit);
    }
}
