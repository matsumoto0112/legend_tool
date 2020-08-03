using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class FileInput
{
    public static string[] Input(string filename)
    {
        string[] index;

        //ファイルが存在するか
        if (!File.Exists(filename))
        {
            //上書き保存の確認
            if (!EditorUtility.DisplayDialog("エラー", "ファイルが存在していません。", "戻る"))
            {
                index = new string[1];
                index[0] = "error";
                return index;
            }
        }

        //ファイルの書き出し開始
        FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
        BinaryReader reader = new BinaryReader(fs);

        List<string> indexList = new List<string>();

        while(reader.BaseStream.Position != reader.BaseStream.Length)
        {
            string text = reader.ReadString();
            Debug.Log(text);

            indexList.Add(text);
        }

        index = new string[indexList.Count];

        for(int i = 0; i < indexList.Count; i++)
        {
            index[i] = indexList[i];
        }

        reader.Close();
        Debug.Log("読み込み終了");

        return index;
    }
}
