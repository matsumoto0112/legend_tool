using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class SearchAI : MonoBehaviour
{
    [SerializeField]
    private List<SearchAI> branch;      //  分岐先

    [Space(15)]
    [SerializeField]
    private LineRenderer line;          //  描画用
    [SerializeField]
    private Color color = Color.red;    //  描画色
    [SerializeField, Range(0, 100)]
    private float width = 1;            //  描画線の幅

    void Update()
    {
        SetLine();
    }

    #region Branch // ------------------------------------------------
    /// <summary>
    /// 分岐先
    /// </summary>
    /// <returns></returns>
    public List<SearchAI> GetBranch() { return branch; }
    /// <summary>
    /// 分岐先からランダムで抽出
    /// </summary>
    /// <param name="rem">探索済み削除</param>
    /// <returns>探索箇所</returns>
    public SearchAI GetRandomSearchBranch(params SearchAI[] rem)
    {
        // 分岐先から削除箇所を削除
        var b = branch.ToList();
        foreach (var i in rem) if (b.Contains(i)) b.Remove(i);

        if (b.Count == 0) return null;
        if (b.Count == 1) return b[0];

        var s = b[Random.Range(0, b.Count - 1)];
        return s;
    }
    #endregion // ------------------------------------------------

    #region Line // ------------------------------------------------
    /// <summary>
    /// ラインの設定
    /// </summary>
    public void SetLine()
    {
        if (line == null) return;
        if (branch == null) return;

        // 重複と自信が分岐に含まれていたら削除
        branch = branch.Distinct().ToList();
        branch.RemoveAll(i => (i == this));

        // ラインの設定
        line.positionCount = branch.Count * 2;
        line.startColor = color;
        line.endColor = color;
        line.startWidth = width;
        line.endWidth = width;

        // ラインのポジション設定
        for (int i = 0; i < line.positionCount; i++)
        {
            var pos = transform.position;
            if ((i % 2 == 1) && (branch[i / 2] != null))
            {
                pos = branch[i / 2].transform.position;
            }
            line.SetPosition(i, pos);
        }
    }
    /// <summary>
    /// ラインの整理
    /// 
    /// 自身の分岐先の削除
    /// 分岐先から自身を削除
    /// </summary>
    public void ResetLine()
    {
        branch.ForEach(i =>
        {
            if (i.branch.Contains(this)) i.branch.Remove(this);
        });
        branch.Clear();
        line.positionCount = 0;
    }
    /// <summary>
    /// 分岐先からNullを削除
    /// </summary>
    public void NullRemove()
    {
        branch.RemoveAll(i => i == null);
    }
    /// <summary>
    /// ライン追加
    /// </summary>
    public void AddLine()
    {
        if (line != null) return;
        line = gameObject.AddComponent<LineRenderer>();
        SetLine();
    }
    #endregion // ------------------------------------------------
    /// <summary>
    /// 分岐先設定
    /// </summary>
    public void AddBranch(SearchAI searchAI)
    {
        if (branch == null)
        {
            branch = new List<SearchAI>();
        }
        if (searchAI.transform == transform) return;
        if (branch.Contains(searchAI)) return;
        branch.Add(searchAI);
    }

    #region Editor // ------------------------------------------------
    /// <summary>
    /// 分岐先設定
    /// </summary>
    /// <param name="rem">被り削除</param>
    public void SetBranch(List<SearchAI> rem = null)
    {
        for (int i = 0; i < branch.Count; i++)
        {
            var search = branch[i];
            SearchRemove(search, i, rem);

            if (search == null) continue;
            if (search.branch.Contains(this)) continue;
            search.branch.Add(this);
        }
    }
    /// <summary>
    /// ブランチから削除
    /// </summary>
    /// <param name="search">探索元</param>
    /// <param name="i">配列番号</param>
    /// <param name="rem">被り削除</param>
    private void SearchRemove(SearchAI search, int i, List<SearchAI> rem)
    {
        if (rem == null) return;
        if (rem.Count != branch.Count) return;
        if (rem[i] == search) return;

        var baseSearch = rem[i];
        if ((baseSearch != null) && (search == null))
        {
            baseSearch.branch.Remove(this);
        }
    }
    #endregion // ------------------------------------------------
}


#if UNITY_EDITOR
[CustomEditor(typeof(SearchAI), true)]
public class SearchAIEditor : Editor
{
    /// <summary>
    /// インスペクタの編集
    /// </summary>
    public override void OnInspectorGUI()
    {
        var obj = target as SearchAI;
        var branch = obj.GetBranch();

        if (DrawDefaultInspector())
        {
            obj.SetBranch(branch);
        }
        LineEditor(obj);
        EditorUtility.SetDirty(obj);
    }

    /// <summary>
    /// ライン描画設定
    /// </summary>
    /// <param name="obj">描画元</param>
    private void LineEditor(SearchAI obj)
    {
        GUI.backgroundColor = Color.white / 2;
        using (new GUILayout.VerticalScope(GUI.skin.box))
        {
            GUI.backgroundColor = Color.white;
            if (GUILayout.Button("null_reset"))
            {
                obj.NullRemove();
            }
            GUI.backgroundColor = Color.red / 2;
            if (GUILayout.Button("reset"))
            {
                obj.ResetLine();
            }
            GUI.backgroundColor = Color.white;
        }
    }
}
#endif