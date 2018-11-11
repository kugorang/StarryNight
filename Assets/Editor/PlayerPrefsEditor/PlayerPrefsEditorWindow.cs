#region

using System;
using System.Globalization;
using UnityEditor;
using UnityEngine;

#endregion

namespace Editor.PlayerPrefsEditor
{
    public class KeyEditorWindow : EditorWindow
    {
        // Add key vars
        private string _addKeyM;
        private TrySetResponse _addResponseM;
        private string _addValM;
        private PlayerPrefsValueType _addValTypeM = PlayerPrefsValueType.String;

        // Search vars
        private string _searchKeyM;
        private string _searchNewValM;
        private TrySetResponse _searchResponseM;
        private PlayerPrefsValueType _searchValTypeM = PlayerPrefsValueType.String;

        [MenuItem("PlayerPrefs/Open editor")]
        private static void DeleteKey()
        {
            var editorWindow = (KeyEditorWindow) GetWindow(typeof(KeyEditorWindow), true, "Player prefs editor");
            editorWindow.Show();
        }

        private void OnGUI()
        {
            DrawSearchKey();
            DrawAddKey();
        }

        /// <summary>
        ///     Draw search for PlayerPrefs key controls
        /// </summary>
        private void DrawSearchKey()
        {
            GUILayout.Label("Search for key", EditorStyles.boldLabel);
            _searchKeyM = EditorGUILayout.TextField("Key", _searchKeyM);

            //Edit existing key
            if (PlayerPrefs.HasKey(_searchKeyM))
            {
                var type = GetType(_searchKeyM);

                // Delete
                if (GUILayout.Button("Delete"))
                {
                    PlayerPrefs.DeleteKey(_searchKeyM);
                    Debug.Log("PlayerPrefs key: " + _searchKeyM + ", deleted");
                }

                // Set value
                GUILayout.Label("Set new value", EditorStyles.boldLabel);
                _searchNewValM = EditorGUILayout.TextField("New value", _searchNewValM);
                if (type == PlayerPrefsValueType.Unknown)
                {
                    _searchValTypeM = (PlayerPrefsValueType) EditorGUILayout.EnumPopup("Type", _searchValTypeM);
                    EditorGUILayout.HelpBox(
                        "The value for the key is a default value so the type cannot be determined. It is your responsibility to set the value in correct type.",
                        MessageType.Warning);
                }
                else
                {
                    _searchValTypeM = type;
                    GUILayout.Label("Value type: " + _searchValTypeM, EditorStyles.boldLabel);
                    GUILayout.Label("Current value: " + GetValue(_searchKeyM, _searchValTypeM), EditorStyles.boldLabel);
                }

                if (GUILayout.Button("Set")) _searchResponseM = TrySetValue(_searchKeyM, _searchNewValM, _searchValTypeM);
                if (_searchResponseM != null)
                    EditorGUILayout.HelpBox(_searchResponseM.Message, _searchResponseM.MessageType);
            }
            else
            {
                EditorGUILayout.HelpBox("Key doesn't exist in player prefs", MessageType.Warning);
            }
        }

        /// <summary>
        ///     Draw add key controls
        /// </summary>
        private void DrawAddKey()
        {
            GUILayout.Label("Add key", EditorStyles.boldLabel);
            _addKeyM = EditorGUILayout.TextField("Key", _addKeyM);
            _addValM = EditorGUILayout.TextField("Value", _addValM);
            _addValTypeM = (PlayerPrefsValueType) EditorGUILayout.EnumPopup("Type", _addValTypeM);
            if (GUILayout.Button("Add")) _addResponseM = TrySetValue(_addKeyM, _addValM, _addValTypeM);
            if (_addResponseM != null) EditorGUILayout.HelpBox(_addResponseM.Message, _addResponseM.MessageType);
        }

        /// <summary>
        ///     Get type for a key that exists in PlayerPrefs. If the key's value is default value, the type cannot be determined.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static PlayerPrefsValueType GetType(string key)
        {
            if (!PlayerPrefs.HasKey(key)) throw new ArgumentException("Key didn't exist in PlayerPrefs");
            var type = PlayerPrefsValueType.Unknown;

            var floatVal = PlayerPrefs.GetFloat(key);
            var intVal = PlayerPrefs.GetInt(key);
            var stringVal = PlayerPrefs.GetString(key);

            if (Math.Abs(floatVal - default(float)) <= 0 && intVal == default(int) && !stringVal.Equals(string.Empty))
                type = PlayerPrefsValueType.String;
            else if (Math.Abs(floatVal - default(float)) <= 0 && intVal != default(int) && stringVal.Equals(string.Empty))
                type = PlayerPrefsValueType.Int;
            else if (Math.Abs(floatVal - default(float)) > 0 && intVal == default(int) && stringVal.Equals(string.Empty))
                type = PlayerPrefsValueType.Float;
            return type;
        }

        /// <summary>
        ///     Tries to set the value to player prefs. If the value is successfully set, PlayerPrefs are saved.
        /// </summary>
        /// <param name="key">Key of value</param>
        /// <param name="value">Value for the key</param>
        /// <param name="type">
        ///     Type of the value. This determines whether PlayerPrefs.SetString(), PlayerPrefs.SetFloat(), or
        ///     PlayerPrefs.SetInt() is used.
        /// </param>
        /// <returns>Response containing info telling if the set was successful or not.</returns>
        private static TrySetResponse TrySetValue(string key, string value, PlayerPrefsValueType type)
        {
            var respone = new TrySetResponse
            {
                Message = "Key: " + key + " with Value: " + value + " was successfully saved to PlayerPrefs as a " + type,
                Success = true,
                MessageType = MessageType.Info
            };
            switch (type)
            {
                case PlayerPrefsValueType.String:
                    PlayerPrefs.SetString(key, value);
                    PlayerPrefs.Save();
                    break;
                case PlayerPrefsValueType.Float:
                    float newValFloat;
                    if (float.TryParse(value, out newValFloat))
                    {
                        PlayerPrefs.SetFloat(key, newValFloat);
                        PlayerPrefs.Save();
                    }
                    else
                    {
                        respone.SetValues(
                            "Couldn't parse input value:" + value + " to target type float. Input a valid float value.",
                            false, MessageType.Error);
                    }

                    break;
                case PlayerPrefsValueType.Int:
                    int newValInt;
                    if (int.TryParse(value, out newValInt))
                    {
                        PlayerPrefs.SetInt(key, newValInt);
                        PlayerPrefs.Save();
                    }
                    else
                    {
                        respone.SetValues(
                            "Couldn't parse input value:" + value + " to target type int. Input a valid int value.", false,
                            MessageType.Error);
                    }

                    break;
                default:
                    respone.SetValues("Unknown PlayerPrefsValueType: " + type, false, MessageType.Error);
                    break;
            }

            return respone;
        }

        /// <summary>
        ///     Get existing PlayerPrefs key value as a string.
        /// </summary>
        /// <param name="key">Key of the value</param>
        /// <param name="type">Type of the value</param>
        /// <returns>Value of the key as a string</returns>
        private string GetValue(string key, PlayerPrefsValueType type)
        {
            if (!PlayerPrefs.HasKey(key)) throw new ArgumentException("Key didn't exist in PlayerPrefs");
            switch (type)
            {
                case PlayerPrefsValueType.String:
                    return PlayerPrefs.GetString(key);
                case PlayerPrefsValueType.Float:
                    return PlayerPrefs.GetFloat(key).ToString(CultureInfo.InvariantCulture);
                case PlayerPrefsValueType.Int:
                    return PlayerPrefs.GetInt(key).ToString(CultureInfo.InvariantCulture);
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }

        private enum PlayerPrefsValueType
        {
            Unknown,
            String,
            Float,
            Int
        }

        /// <summary>
        ///     Helper class to return values from TrySetValue function
        /// </summary>
        private class TrySetResponse
        {
            /// <summary>
            ///     True if the value was successfully set, false otherwise.
            /// </summary>
            public bool Success { get; set; }

            /// <summary>
            ///     Message of the value set. May contain error message or success message.
            /// </summary>
            public string Message { get; set; }

            /// <summary>
            ///     Message type for showing the message in UI
            /// </summary>
            public MessageType MessageType { get; set; }

            public void SetValues(string message, bool success, MessageType messageType)
            {
                Message = message;
                Success = success;
                MessageType = messageType;
            }
        }
    }
}