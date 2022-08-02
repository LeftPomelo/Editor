using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SkillWindow : EditorWindow
{
    Player player;

    List<SkillBase> skillsList;

    float currSpeed = 1;
    string[] typesName = new string[] { "null", "动画", "音效", "特效" };
    int typeIndex = 0;

    Vector2 scrollView=new Vector2 (0,0);

    public void SetInitWindow(Player _player,List<SkillBase> skills)
    {
        player = _player;
        currSpeed = 1;
        skillsList = skills;
        player.currSkillsComponent = skillsList;
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        if(GUILayout.Button("播放"))
        {
            foreach (var item in skillsList)
            {
                item.Play();
            }
        }
        if(GUILayout.Button("停止"))
        {
            foreach (var item in skillsList)
            {
                item.Stop();
            }
        }
        if (GUILayout.Button("保存"))
        {
            player.SaveSkill();
        }
        GUILayout.EndHorizontal();

        GUILayout.Label("速度");
        float speed = EditorGUILayout.Slider(currSpeed, 0, 5);
        if(speed!=currSpeed)
        {
            currSpeed = speed;
            Time.timeScale = currSpeed;
        }

        GUILayout.BeginHorizontal();
        typeIndex = EditorGUILayout.Popup(typeIndex, typesName);
        if(GUILayout.Button("添加"))
        {
            //判断是哪个类型的技能组件
            switch(typeIndex)
            {
                case 1:
                    skillsList.Add(new Skill_Anim(player));
                    break;
                case 2:
                    skillsList.Add(new Skill_Audio(player));
                    break;
                case 3:
                    skillsList.Add(new Skill_Effect(player));
                    break;
            }
        }
        GUILayout.EndHorizontal();

        scrollView = EditorGUILayout.BeginScrollView(scrollView, false, true);
        foreach (var item in skillsList)
        {
            GUILayout.BeginHorizontal();
            if(item is Skill_Anim)
            {
                ShowSkill_Anim(item as Skill_Anim);
            }
            else if(item is Skill_Audio)
            {
                ShowSkill_Audio(item as Skill_Audio);
            }
            else if(item is Skill_Effect)
            {
                ShowSkill_Effect(item as Skill_Effect);
            }

            GUILayoutOption[] option = new GUILayoutOption[]
               {
                    GUILayout.Width(60),
                    GUILayout.Height(20)
               };
            if (GUILayout.Button("删除",option))
            {
                skillsList.Remove(item);
                break;
            }
            GUILayout.Space(0.5f);
            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();
    }

    void ShowSkill_Anim(Skill_Anim skill)
    {
        skill.trigger = EditorGUILayout.TextField(skill.trigger);
        AnimationClip clip=EditorGUILayout.ObjectField(skill.animClip,typeof(AnimationClip),false) as AnimationClip;
        if(skill.animClip != clip)
        {
            skill.SetAnimClip(clip);
        }
    }
    void ShowSkill_Audio(Skill_Audio skill)
    {
        skill.trigger = EditorGUILayout.TextField(skill.trigger);
        AudioClip clip = EditorGUILayout.ObjectField(skill.audioclip, typeof(AudioClip), false) as AudioClip;
        if (skill.audioclip != clip)
        {
            skill.SetAudioClip(clip);
        }
    }
    void ShowSkill_Effect(Skill_Effect skill)
    {
        skill.trigger = EditorGUILayout.TextField(skill.trigger);
        GameObject clip = EditorGUILayout.ObjectField(skill.gameclip, typeof(GameObject), false) as GameObject;
        if (skill.gameclip != clip)
        {
            skill.SetEffectClip(clip);
        }
    }
}
