﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

public class FileOutput
{

    public static bool Output(string filename, string[] index)
    {
        //ファイルが存在するか
        if (File.Exists(filename))
        {
            //上書き保存の確認
            if (!EditorUtility.DisplayDialog("確認", "既にファイルが存在しています。\nファイルを上書きしますか?", "上書きする", "キャンセル"))
            {
                return false;
            }
        }

        //ファイルの書き出し開始
        FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.ReadWrite);
        BinaryWriter write = new BinaryWriter(fs);

        foreach(string text in index)
        {
            write.Write(text);
        }

        write.Close();

        Encoding encoding = Encoding.GetEncoding("Shift_JIS");
        StreamWriter writer = new StreamWriter(filename.Replace(".bin",".txt"), true, encoding);
        foreach (string text in index)
        {
            writer.Write(text);
        }
        writer.Close();
        Debug.Log("書き出し終了");

        return true;
    }
}
