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
        //BinaryReader reader = new BinaryReader(fs);
        StreamReader reader = new StreamReader(filename, System.Text.Encoding.GetEncoding("shift_jis"));
        string text = reader.ReadToEnd();

        List<string> indexList = new List<string>();
        index = text.Split('\n');

        //while(reader.BaseStream.Position != reader.BaseStream.Length)
        //{
        //    string text = reader.ReadLine();
        //    Debug.Log(text);

        //    indexList.Add(text);
        //}

        //foreach(string text in reader)

        //index = new string[indexList.Count];

        //for(int i = 0; i < indexList.Count; i++)
        //{
        //    index[i] = indexList[i];
        //}

        reader.Close();
        Debug.Log("読み込み終了");

        return index;
    }
}
