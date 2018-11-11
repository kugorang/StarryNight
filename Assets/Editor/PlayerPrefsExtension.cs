#region

using UnityEngine;

#endregion

namespace Editor
{
    /// <summary>
    ///     Player prefs extension. Used to add boolean funciton in PlayerPrefs
    /// </summary>
    public class PlayerPrefsExtension
    {
        public static void SetBool(string name, bool booleanValue)
        {
            PlayerPrefs.SetInt(name, booleanValue ? 1 : 0);
        }

        public static bool GetBool(string name)
        {
            return PlayerPrefs.GetInt(name) == 1 ? true : false;
        }

        public static bool GetBool(string name, bool defaultValue)
        {
            return PlayerPrefs.HasKey(name) ? GetBool(name) : defaultValue;
        }
    }
}