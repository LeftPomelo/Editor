using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class SkillEditorWindow : EditorWindow
{
    class PlayerEditor
    {
        public int _characterIndex = 0;
        public int _folderIndex = 0;
        public string _characterName=string.Empty;
        public string _folderName=string.Empty;
        public List<string> characterNameList=new List<string>();
        public Player player;
    }
    PlayerEditor m_player=new PlayerEditor();
    /// <summary>
    /// 文件名
    /// </summary>
    List<string> m_folderNameList=new List<string>();
    /// <summary>
    /// 预制体名
    /// </summary>
    List<string> m_charaNameList=new List<string>();

    Dictionary<string,List<string>> m_folderPrefab=new Dictionary<string, List<string>>();

    string newskillName=string.Empty;

    SkillWindow skill2Window;

    Vector2 scrollView=new Vector2(0,0);
    [MenuItem("Tools/技能编辑器")]
    static void Init()
    {
        if(Application.isPlaying)
        {
            SkillEditorWindow window=GetWindow<SkillEditorWindow>();
            if(window!=null)
            {
                window.Show();
            }
        }
    }

    private void OnEnable()
    {
        DoSearchFolder();
        DoSearchPrefab();
    }
    void DoSearchFolder()
    {
        m_folderNameList.Clear();
        m_folderNameList.Add("All");
        string[] files = Directory.GetDirectories(GetPath());
        foreach (var item in files)
        {
            m_folderNameList.Add(Path.GetFileName(item));
        }
    }
    void DoSearchPrefab()
    {
        m_charaNameList.Clear();
        string[] files = Directory.GetFiles(GetPath(), "*.prefab", SearchOption.AllDirectories);
        foreach (var item in files)
        {
            m_charaNameList.Add(Path.GetFileNameWithoutExtension(item));
        }
        m_charaNameList.Sort();
        m_charaNameList.Insert(0, "null");
        m_player.characterNameList.AddRange(m_charaNameList);
    }

    private void OnGUI()
    {
        int folderIndex = EditorGUILayout.Popup(m_player._folderIndex, m_folderNameList.ToArray());
        if(folderIndex!=m_player._folderIndex)
        {
            m_player._folderIndex = folderIndex;
            m_player._characterIndex = -1;

            string folderName = m_folderNameList[m_player._folderIndex];
            List<string> lists;
            if(folderName.Equals("All"))
            {
                lists = m_charaNameList;
            }
            else
            {
                if(!m_folderPrefab.TryGetValue(folderName,out lists))
                {
                    lists=new List<string>();
                    string[] files=Directory.GetFiles(GetPath()+"/"+folderName,"*.prefab",SearchOption.AllDirectories);
                    foreach (var item in files)
                    {
                        lists.Add(Path.GetFileNameWithoutExtension(item));
                    }
                }
                m_player.characterNameList.Clear();
                m_player.characterNameList.AddRange(lists);
            }
        }

        int characterIndex = EditorGUILayout.Popup(m_player._characterIndex, m_player.characterNameList.ToArray());
        if(characterIndex!=m_player._characterIndex)
        {
            m_player._characterIndex=characterIndex;
            if(m_player._characterName!=m_player.characterNameList[m_player._characterIndex])
            {
                m_player._characterName = m_player.characterNameList[m_player._characterIndex];
                if(!string.IsNullOrEmpty(m_player._characterName))
                {
                    if (m_player.player != null)
                    {
                        m_player.player.DestroyObj();
                    }
                    m_player.player = Player.Init(m_player._characterName);
                }
            }
        }

        newskillName = GUILayout.TextField(newskillName);
        if(GUILayout.Button("创建技能"))
        {
            GUILayout.BeginHorizontal();
            if(!string.IsNullOrEmpty(newskillName) && m_player.player!=null)
            {
                List<SkillBase> lists = m_player.player.AddSkill(newskillName);
                OpenWindowSkill(newskillName, lists);
                newskillName = "";
            }
        }
        if(m_player.player!=null)
        {
            scrollView = GUILayout.BeginScrollView(scrollView, false, true);
            foreach (var item in m_player.player.skillsDic)
            {
                GUILayout.BeginHorizontal();
                if(GUILayout.Button(item.Key))
                {
                    List<SkillBase> lists = m_player.player.GetSkill(item.Key);
                    foreach (var ite in lists)
                    {
                        ite.Init();
                    }
                    OpenWindowSkill(item.Key, lists);
                }
                GUILayoutOption[] option = new GUILayoutOption[]
                {
                    GUILayout.Width(60),
                    GUILayout.Height(20)
                };

                if(GUILayout.Button("删除",option))
                {
                    m_player.player.skillsDic.Remove(item.Key);
                    break;
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
        }
        
    }

    string GetPath()
    {
        return Application.dataPath + "/GameDate/Models";
    }
    void OpenWindowSkill(string skillname,List<SkillBase> skillsComponents)
    {
        if(skillsComponents!=null)
        {
            if(skill2Window==null)
            {
                skill2Window=GetWindow<SkillWindow>();
            }
            skill2Window.titleContent=new GUIContent(skillname);
            skill2Window.SetInitWindow(m_player.player, skillsComponents);
            skill2Window.Show();
            skill2Window.Repaint();
        }
    }
}
