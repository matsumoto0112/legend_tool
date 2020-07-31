using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class SceneInformation
{
    private static string mapName;
    private static int backgroundID = -1;
    private static int turnsCount = 0;
    private static int startPointCount = 0;
    private static int enemyCount = 0;
    private static int bossCount = 0;
    private static int floorCount = 0;
    private static int obstacleCount = 0;
    private static int graffitiCount = 0;
    private static int stationeryCount = 0;

    static SceneInformation()
    {
        SceneView.duringSceneGui += OnGUI;
    }

    //マップオブジェクト情報を更新する処理
    private static void UpdateObjectInfomations()
    {
        turnsCount = 0;
        startPointCount = 0;
        enemyCount = 0;
        bossCount = 0;
        floorCount = 0;
        obstacleCount = 0;

        Object[] allGameObject = Resources.FindObjectsOfTypeAll(typeof(MapObject));

        foreach (MapObject obj in allGameObject)
        {
            switch (obj.objectType)
            {
                case MapObject.ObjectType.None:
                    break;

                case MapObject.ObjectType.StartPoint:
                    startPointCount++;
                    break;

                case MapObject.ObjectType.Enemy:
                    enemyCount++;
                    break;

                case MapObject.ObjectType.Boss:
                    bossCount++;
                    break;

                case MapObject.ObjectType.Floor:
                    floorCount++;
                    break;

                case MapObject.ObjectType.Obstacle:
                    obstacleCount++;
                    break;

                case MapObject.ObjectType.Graffiti:
                    graffitiCount++;
                    break;

                case MapObject.ObjectType.Stationery:
                    stationeryCount++;
                    break;
            }
        }
    }

    //SceneViewへの描画
    private static void OnGUI(SceneView sceneView)
    {
        //左上から描画する
        GUILayout.BeginArea(new Rect(0, 0,
            Screen.width / EditorGUIUtility.pixelsPerPoint,
            Screen.height / EditorGUIUtility.pixelsPerPoint));
        Handles.BeginGUI();

        //マップ名を表示&編集する
        mapName = EditorGUILayout.TextField("Map Name", mapName, 
            GUILayout.MaxWidth(32.0f * 8.0f));

        //現在のターン数を表示&編集する
        backgroundID = EditorGUILayout.IntField("背景ID", backgroundID,
            GUILayout.MaxWidth(32.0f * 8.0f));

        //現在のターン数を表示&編集する
        turnsCount = EditorGUILayout.IntField("表示中のターン数", turnsCount,
            GUILayout.MaxWidth(32.0f * 8.0f));

        //各オブジェクト情報を表示する
        DrawObjectInformations();

        //出力ボタン
        if (GUILayout.Button("出力",
            GUILayout.Width(32.0f * 2)))
        {
            Output();
        }

        //描画終了処理
        Handles.EndGUI();
        GUILayout.EndArea();
    }

    //各オブジェクト情報を表示するメソッド
    private static void DrawObjectInformations()
    {
        GUILayout.Label("オブジェクト情報");

        //開始地点が存在しているかの表示
        if (startPointCount <= 1)
        {
            GUILayout.Label("開始地点　 : " + (startPointCount > 0));
        }
        else
        {
            GUILayout.Label("開始地点　 : " + "Over");
        }

        //敵の数の表示
        GUILayout.Label("敵の数　　 : " + enemyCount);

        //ボスの数の表示
        GUILayout.Label("ボスの数　 : " + bossCount);

        //床の数の表示
        GUILayout.Label("床の数　　 : " + floorCount);

        //障害物の数の表示
        GUILayout.Label("障害物の数 : " + obstacleCount);

        //落書きの数の表示
        GUILayout.Label("落書きの数 : " + graffitiCount);

        //文房具の数の表示
        GUILayout.Label("文房具の数 : " + stationeryCount);

        //オブジェクト情報を更新するボタン
        if (GUILayout.Button("オブジェクト情報を更新",
            GUILayout.Width(32.0f * 5)))
        {
            UpdateObjectInfomations();
        }
    }

    private static void Output()
    {
        UpdateObjectInfomations();

        //マップ名の確認
        if ((mapName == null || mapName == "") && EditorUtility.DisplayDialog("エラー", "マップ名が指定されていません。", "戻る"))
        {
            return;
        }

        //開始地点が正しいか
        if (startPointCount != 1 && EditorUtility.DisplayDialog("エラー", "開始地点が複数存在しています。", "戻る"))
        {
            return;
        }

        //ファイルパスを設定
        string filePath = Application.dataPath + "/StageData/" + mapName + ".bin";

        string[] index = new string[1 + startPointCount + enemyCount + bossCount + floorCount + obstacleCount + graffitiCount + stationeryCount];

        //ステージ情報を文字列に
        index[0] = mapName + "," + backgroundID + "\n";
        int count = 1;

        //開始地点を文字列に
        for(int i = 0; i < startPointCount; i++)
        {
            index[count + i] = "startPoint";
            index[count + i] += "\n";
        }
        count += startPointCount;

        //敵の情報を文字列に
        for (int i = 0; i < enemyCount; i++)
        {
            index[count + i] = "enemy";
            index[count + i] += "\n";
        }
        count += enemyCount;

        //ボスの情報を文字列に
        for (int i = 0; i < bossCount; i++)
        {
            index[count + i] = "boss";
            index[count + i] += "\n";
        }
        count += bossCount;

        //床の情報を文字列に
        for (int i = 0; i < floorCount; i++)
        {
            index[count + i] = "floor";
            index[count + i] += "\n";
        }
        count += floorCount;

        //障害物の情報を文字列に
        for (int i = 0; i < obstacleCount; i++)
        {
            index[count + i] = "obstacle";
            index[count + i] += "\n";
        }
        count += obstacleCount  ;

        //落書きの情報を文字列に
        for (int i = 0; i < graffitiCount; i++)
        {
            index[count + i] = "graffiti";
            index[count + i] += "\n";
        }
        count += graffitiCount;

        //文房具の情報を文字列に
        for (int i = 0; i < stationeryCount; i++)
        {
            index[count + i] = "stationery";
            index[count + i] += "\n";
        }
        count += stationeryCount;

        FileOutput.Output(filePath, index);
    }
}
