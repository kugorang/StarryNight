using System;
using UnityEngine;
using UnityEditor;

public class SceneEnum : ScriptableObject
{
    [MenuItem("Tools/MyTool/Do It in C#")]
    static void DoIt()
    {
        EditorUtility.DisplayDialog("MyTool", "Do It in C# !", "OK", "");
    }
    /// <summary>
    /// 
    /// </summary>
    public enum SceneName
    {
        Aries,
        Taurus,
        Gemini,
        Cancer,
        Leo,
        Virgo,
        Libra,
        Scorpio,
        Sagittarius,
        Capricorn,
        Aquarius,
        Pisces
    }
    /// <summary>
    /// 이름으로 index 반환
    /// </summary>
    /// <param name="name">Name of scene</param>
    /// <returns></returns>
    public int GetSceneIndex(string name)
    {
        return (int)Enum.Parse(typeof(SceneName), name);
    }
    /// <summary>
    /// (int)명시적 변환과 같음
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns>sceneName의 int 값</returns>
    public int GetSceneIndex(SceneName sceneName)
    {
        return (int)sceneName;
    }
    /// <summary>
    /// index로 이름찾아 반환.
    /// </summary>
    /// <param name="index">최소 0 최대 11</param>
    /// <returns></returns>
    public string GetSceneName(int index)
    {
        return GetSceneName((SceneName)index);
    }
    /// <summary>
    /// Enum.GetName(type, object)와 동일.
    /// SceneName에 해당하는 이름 반환.
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    public string GetSceneName(SceneName sceneName)
    {
        return Enum.GetName(typeof(SceneName), sceneName);
    }

 }