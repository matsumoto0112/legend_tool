using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SearchAI_Manager : MonoBehaviour
{
    private List<SearchAI> searchList;  //  探索箇所
    private List<SearchAI> courseList;  //  道順

    // Start is called before the first frame update
    void Start()
    {
        var searchs = GetComponentsInChildren<SearchAI>();
        searchList = new List<SearchAI>(searchs);
        courseList = new List<SearchAI>();
    }

    public Vector3 SearchCourse(GameObject obj)
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p == null)
        {
            //Debug.Log(obj.name + ": " + (obj.transform.position) + " >> " + (obj.transform.position) + " --------------------------");
            return obj.transform.position;
        }

        var start = obj.transform.position;
        var end = p.transform.position;
        courseList.Clear();

        var vector = (end - start);
        var hitList = (Physics.RaycastAll(start, vector, vector.magnitude)).ToList();
        hitList.RemoveAll(h => ((h.transform.tag == "Player")));

        if (0 < hitList.Count)
        {
            SetCourse(NearSearchAI(start), NearSearchAI(end));
            ChaseCourse(end);
        }

        var result = (courseList.Count <= 0 ? end : NextCouse(start));
        //Debug.Log(obj.name + ": " + (obj.transform.position) + " >> " + (result) + " --------------------------");
        //Debug.Log(obj.name + ": CourseCount[ " + courseList.Count + " ]");
        //for (int i = 0; i < courseList.Count; i++)
        //{
        //    Debug.Log(obj.name + "[ " + i + " ]: " + courseList[i].name + " >> " + courseList[i].transform.position);
        //}
        return result;
    }

    /// <summary>
    /// 道順設定
    /// </summary>
    private void SetCourse(SearchAI cStart, SearchAI cEnd)
    {
        courseList.Clear();                                         //  道順削除
        if ((cStart == null) || (cEnd == null)) return;

        // 生成
        var start = new SearchAI_Course(cStart);                  //  開始位置
        //var end = GetRandomSearch(movStart);                        //  終点設定
        var searched = new SearchAI_Course[] { start };             //  探索済みリスト
        var root = new SearchAI_Course[] { };                       //  道順

        // 探索
        for (int i = 0; i < searched.Length; i++)
        {
            // 探索先を設定（すでに探索されている場合は削除
            var child = SetChild(searched[i], searched);
            var list = searched.ToList();

            // 新たな分岐先を追加
            for (int j = 0; j < child.Length; j++)
            {
                var sPos = searched[i].BaseSearch().transform.position;
                var ePos = child[j].BaseSearch().transform.position;
                var hitList = (Physics.RaycastAll(sPos, (ePos - sPos), (ePos - sPos).magnitude)).ToList();
                hitList.RemoveAll(h => ((h.transform.tag == "Player")));

                if (hitList.Count <= 0)
                {
                    list.Add(child[j]);
                }
            }

            // 原点からの探索距離でソートして探索済みにする
            list = list.OrderBy(j => j.Length()).ToList();
            searched = list.ToArray();

            // 分岐先に終点があれば、そこまでの経路を道順として返す
            if (searched[i].BaseSearch() == cEnd)
            {
                root = searched[i].Parents();
                break;
            }
        }

        // 道順
        foreach (var r in root)
        {
            courseList.Add(r.BaseSearch());
        }
    }

    private void ChaseCourse(Vector3 originPos)
    {
        if (courseList.Count <= 0) return;

        for (int index = 0; index < courseList.Count; index++)
        {
            if (courseList.Count - 1 <= (index + 2))
            {
                break;
            }
            var start = courseList[index].transform.position;
            var end = courseList[index + 2].transform.position;

            var vector = (end - start);
            var hitList = (Physics.RaycastAll(start, vector, vector.magnitude)).ToList();
            hitList.RemoveAll(h => ((h.transform.tag == "Player")));

            if (hitList.Count <= 0)
            {
                courseList.RemoveAt(index + 1);
                index--;
            }
        }
    }

    private Vector3 NextCouse(Vector3 originPos)
    {
        if (courseList.Count <= 0) return originPos;

        for (int index = 0; index < courseList.Count; index++)
        {
            var start = originPos;
            var end = courseList[index].transform.position;

            var vector = (end - start);
            var hitList = (Physics.RaycastAll(start, vector, vector.magnitude)).ToList();
            hitList.RemoveAll(h => ((h.transform.tag == "Player")));

            if (0 < hitList.Count)
            {
                if (index <= 0) break;
                else
                {
                    return courseList[index - 1].transform.position;
                }
            }
        }

        return courseList[0].transform.position;
    }

    /// <summary>
    /// 分岐先から探索先を設定
    /// </summary>
    /// <param name="search">探索箇所</param>
    /// <param name="searched">探索済み</param>
    /// <returns>探索先</returns>
    private SearchAI_Course[] SetChild(SearchAI_Course search, SearchAI_Course[] searched)
    {
        // 探索済みの箇所を削除して探索先を設定
        var child = search.SetChild(searched);
        return child;
    }

    /// <summary>
    /// ランダムで終点を指定
    /// </summary>
    /// <param name="rem">削除箇所</param>
    /// <returns>終点</returns>
    private SearchAI GetRandomSearch(params SearchAI[] rem)
    {
        // 削除箇所を削除してリストを設定
        var sea = searchList.ToList();
        sea.RemoveAll(i => i.GetBranch().Count <= 0);
        foreach (var i in rem) if (sea.Contains(i)) sea.Remove(i);

        if (sea.Count == 0) return null;
        if (sea.Count == 1) return sea[0];

        var s = sea[Random.Range(0, sea.Count - 1)];
        return s;
    }

    private SearchAI NearSearchAI(Vector3 originPos)
    {
        if (searchList.Count <= 0)
        {
            return null;
        }

        SearchAI result = null;
        foreach (var i in searchList)
        {
            if ((result == null) ||
                ((i.transform.position - originPos).magnitude < (result.transform.position - originPos).magnitude))
            {
                var vector = (i.transform.position - originPos);
                var hitList = (Physics.RaycastAll(originPos, vector, vector.magnitude)).ToList();
                hitList.RemoveAll(h => ((h.transform.tag == "Player")));
                if (hitList.Count <= 0)
                {
                    result = i;
                }
            }
        }
        return result;
    }

    public void CreateText(string textFileName)
    {
        List<string> strList = new List<string>();
        var children = transform.GetChildren();
        for (int index = 0; index < children.Count; index++)
        {
            var child = children[index];
            var searchAI = child.GetComponent<SearchAI>();
            if (searchAI == null) continue;
            string addData = "search," + child.position.x + "," + child.position.y + "," + child.position.z;
            var branch = searchAI.GetBranch();
            for (int bIndex = 0; bIndex < branch.Count; bIndex++)
            {
                int fIndex = children.FindIndex(i => i == branch[bIndex].transform);
                if (0 <= fIndex) addData += "," + fIndex;
            }
            Debug.Log(index + ": " + addData);
            strList.Add(addData);
        }
        Create.SaveText(strList, textFileName);
    }

#if UNITY_EDITOR // ------------------------------------------------
    private void OnDrawGizmos()
    {
        // エディタ上で再生している際に描画処理
        if (UnityEditor.EditorApplication.isPlaying)
        {
            var size = 0.5f;
            for (int i = 0; i < courseList.Count - 1; i++)
            {
                var from = courseList[i].transform.position;    //  始点
                var to = courseList[i + 1].transform.position;  //  終点

                // 線描画
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(from, to);

                // 球描画
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(courseList[i].transform.position, size / 2.0f);
            }

            // 始点終点を別色別色で描画
            if (courseList.Count <= 0) return;
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(courseList[0].transform.position, size);
            Gizmos.DrawSphere(courseList[courseList.Count - 1].transform.position, size);
        }
    }
#endif // ------------------------------------------------
}


public class SearchAI_Course
{
    private SearchAI baseSearch;            //  原点
    private SearchAI_Course parentSearch;   //  探索元
    private SearchAI_Course[] childSearch;  //  探索先

    /// <summary>
    /// 原点
    /// </summary>
    /// <returns></returns>
    public SearchAI BaseSearch() { return baseSearch; }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="baseSearch">原点</param>
    /// <param name="parentSearch">探索元（視点の場合はなし）</param>
    public SearchAI_Course(SearchAI baseSearch, SearchAI_Course parentSearch = null)
    {
        this.baseSearch = baseSearch;
        this.parentSearch = parentSearch;
    }

    /// <summary>
    /// 分岐先を設定
    /// </summary>
    /// <param name="rem">削除箇所</param>
    /// <returns>分岐先</returns>
    public SearchAI_Course[] SetChild(SearchAI_Course[] rem = null)
    {
        var branch = baseSearch.GetBranch();
        var count = branch.Count;
        var list = new List<SearchAI_Course>();

        for (int i = 0; i < count; i++)
        {
            if (rem.ToList().FindAll(j => j.baseSearch == branch[i]).Count <= 0)
            {
                list.Add(new SearchAI_Course(branch[i], this));
            }
        }

        childSearch = list.ToArray();
        return childSearch;
    }

    /// <summary>
    /// 始点までの探索元を返す
    /// </summary>
    /// <param name="parents">原点まで探索元配列</param>
    /// <returns>道順</returns>
    public SearchAI_Course[] Parents(SearchAI_Course[] parents = null)
    {
        if (parents == null) parents = new SearchAI_Course[] { };
        System.Array.Resize(ref parents, parents.Length + 1);
        parents[parents.Length - 1] = this;

        if (parentSearch == null)
        {
            System.Array.Reverse(parents);
            return parents;
        }
        else
        {
            return parentSearch.Parents(parents);
        }
    }

    /// <summary>
    /// 原点を返す
    /// </summary>
    /// <returns>原点</returns>
    public SearchAI_Course MostParent()
    {
        return (parentSearch != null) ? parentSearch.MostParent() : this;
    }

    /// <summary>
    /// 原点までの距離
    /// </summary>
    /// <returns>距離</returns>
    public float Length()
    {
        var parents = Parents();
        var length = 0.0f;
        for (int i = 0; i < parents.Length - 1; i++)
        {
            var child = parents[i].baseSearch.transform;
            var parent = parents[i + 1].baseSearch.transform;
            length += (parent.position - child.position).magnitude;
        }
        return length;
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(SearchAI_Manager), true)]
public class SearchAI_Manager_Editor : Editor
{
    string textFileName = "map_search_data";

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var obj = target as SearchAI_Manager;
        CreateText(obj);
        EditorUtility.SetDirty(obj);
    }

    void CreateText(SearchAI_Manager obj)
    {
        GUI.backgroundColor = Color.white * 0.75f;
        GUILayout.BeginVertical(GUI.skin.box);
        {
            GUI.backgroundColor = Color.white;
            var text = EditorGUILayout.TextField("File_Name",textFileName);
            if (text != textFileName) textFileName = text;

            if (GUILayout.Button("CreateText"))
            {
                obj.CreateText(textFileName);
            }
        }
        GUILayout.EndVertical();
    }
}
#endif