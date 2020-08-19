using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class SceneInformation
{
    public static int turnCount = 0;
    public static bool isAllView = false;

    private static string mapName;
    private static int backgroundID = -1;
    private static int startPointCount = 0;
    private static int enemyCount = 0;
    private static int bossCount = 0;
    private static int floorCount = 0;
    private static int obstacleCount = 0;
    private static int graffitiCount = 0;
    private static int stationeryCount = 0;

    private static List<GameObject> mapObjects = new List<GameObject>();
    private static SearchAI_Manager searchManager;

    static SceneInformation()
    {
        SceneView.duringSceneGui += OnGUI;
    }

    //マップオブジェクト情報を更新する処理
    private static void UpdateObjectInfomations()
    {
        //各情報を初期化
        startPointCount = 0;
        enemyCount = 0;
        bossCount = 0;
        floorCount = 0;
        obstacleCount = 0;
        graffitiCount = 0;
        stationeryCount = 0;
        mapObjects = new List<GameObject>();

        //MapObjectを全て取得
        Object[] allGameObject = Resources.FindObjectsOfTypeAll(typeof(MapObject));

        foreach (MapObject obj in allGameObject)
        {
            //シーン上にあるオブジェクト以外は除外
            if (!AssetDatabase.GetAssetOrScenePath(obj).Contains(".unity"))
                continue;

            //オブジェクトタイプが指定されていないものは除外
            if (obj.objectType == MapObject.ObjectType.None)
                continue;

            //リストに追加
            mapObjects.Add(obj.gameObject);

            switch (obj.objectType)
            {
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

        Object[] searchAI_manager = Resources.FindObjectsOfTypeAll(typeof(SearchAI_Manager));
        searchManager = (searchAI_manager == null || searchAI_manager.Length <= 0) ? null : searchAI_manager[0] as SearchAI_Manager;
    }

    //SceneViewへの描画
    private static void OnGUI(SceneView sceneView)
    {
        //左上から描画する
        GUILayout.BeginArea(new Rect(0, 0,
            Screen.width / EditorGUIUtility.pixelsPerPoint,
            Screen.height / EditorGUIUtility.pixelsPerPoint));
        Handles.BeginGUI();

        //現在のターン数を表示&編集する
        int beforeTurnCount = turnCount;
        turnCount = EditorGUILayout.DelayedIntField("表示中のターン数", turnCount,
            GUILayout.MaxWidth(32.0f * 8.0f));

        bool beforeIsAllView = isAllView;
        isAllView = EditorGUILayout.Toggle("全て表示するか", isAllView);
        if (beforeIsAllView != isAllView)
            TurnChange();

        //マップ名を表示&編集する
        mapName = EditorGUILayout.TextField("Map Name", mapName,
            GUILayout.MaxWidth(32.0f * 8.0f));

        //現在のターン数を表示&編集する
        backgroundID = EditorGUILayout.DelayedIntField("背景ID", backgroundID,
            GUILayout.MaxWidth(32.0f * 8.0f));

        //ターン数が変わっているなら
        if (turnCount != beforeTurnCount)
            TurnChange();

        //各オブジェクト情報を表示する
        DrawObjectInformations();

        //出力ボタン
        if (GUILayout.Button("出力",
            GUILayout.Width(32.0f * 2)))
        {
            Output();
        }

        string readMapName = "";
        readMapName = EditorGUILayout.DelayedTextField("読み込むマップ名", readMapName,
            GUILayout.MaxWidth(32.0f * 8.0f));
        if (readMapName != "")
        {
            //ファイルパスを設定
            string filePath = Application.dataPath + "/StageData/" + readMapName + ".bin";
            string[] index = FileInput.Input(filePath);

            if (index[0] == "error")
                return;

            //マップを再現
            ObjectRevival(readMapName, index);
        }


        //描画終了処理
        GUILayout.EndArea();
        Handles.EndGUI();
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

    private static void TurnChange()
    {
        //オブジェクト情報を更新
        UpdateObjectInfomations();

        //現在ターンに関係がある場合はアクティ
        foreach (GameObject obj in mapObjects)
        {
            obj.GetComponent<MapObject>().CheckActiveTurn(turnCount);
        }

    }

    private static void ObjectRevival(string readName, string[] index)
    {
        //オブジェクト情報を更新
        UpdateObjectInfomations();

        //現在の設置されているオブジェクトを削除
        foreach (GameObject obj in mapObjects)
        {
            GameObject.DestroyImmediate(obj.gameObject);
        }

        //プレファブのリストを作成
        List<GameObject> prefabs = new List<GameObject>();

        //MapObjectのプレファブを全て取得
        Object[] allGameObject = Resources.FindObjectsOfTypeAll(typeof(MapObject));
        foreach (MapObject obj in allGameObject)
        {
            //プレファブ以外は除外
            if (!AssetDatabase.GetAssetOrScenePath(obj).Contains(".prefab"))
                continue;
            
            prefabs.Add(obj.gameObject);
        }

        foreach (string text in index)
        {
            string[] info = text.Split(',');

            if (info[0] == readName)
            {
                mapName = info[0];
                backgroundID = int.Parse(info[1]);
            }

            //オブジェクト名を判断
            string prefabPath = "error";
            for (int i = 0; i < prefabs.Count; i++)
            {
                string prefabName = System.Enum.GetName(typeof(MapObject.ObjectType), prefabs[i].GetComponent<MapObject>().objectType);

                if (info[0].ToLower() != prefabName.ToLower()) continue;
                
                prefabPath = AssetDatabase.GetAssetOrScenePath(prefabs[i]);
                break;
            }

            if (prefabPath == "error")
            {
                Debug.Log("オブジェクトの生成に失敗しました。\n" + info[0] + "は、プレファブに登録されていません。");
                continue;
            }

            //オブジェクトの生成
            var prefabObject = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            var obj = PrefabUtility.InstantiatePrefab(prefabObject) as GameObject;

            Undo.RegisterCreatedObjectUndo(obj, "Create GameObject");

            //オブジェクトの情報をfloat配列化
            float[] floatInfo = new float[info.Length - 1];
            for(int i = 1; i < info.Length; i++)
            {
                floatInfo[i - 1] = float.Parse(info[i]);
            }

            //オブジェクトの情報を反映
            obj.GetComponent<MapObject>().Input(floatInfo);
            obj.GetComponent<MapObject>().CheckActiveTurn(turnCount);

        }

        //オブジェクト情報を再読み込み
        UpdateObjectInfomations();
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

        string[] index = new string[1 + mapObjects.Count];

        //ステージ情報を文字列に
        index[0] = mapName + "," + backgroundID + "\n";

        for (int i = 0; i < mapObjects.Count; i++)
        {
            index[1 + i] = mapObjects[i].GetComponent<MapObject>().Output();
        }

        FileOutput.Output(filePath, index);

        if (searchManager != null)
        {
            searchManager.CreateText("/StageData/" + mapName + "_searchData");
        }
    }
}
