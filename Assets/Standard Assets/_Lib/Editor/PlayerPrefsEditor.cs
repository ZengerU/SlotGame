using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using TMPro;
using UnityEditor;
using UnityEngine;

public class PlayerPrefsTool : EditorWindow
{
    private const string RegistryKeyPath = "Software\\Unity\\UnityEditor\\";
    private static RegistryKey registryKey;
    private readonly Dictionary<string, string> KeyValue = new Dictionary<string, string>();
    private Vector2 _position;

    [MenuItem("Tools/PlayerPrefs Tool", false, 51)]
    private static void Init()
    {
        var registryPath = RegistryKeyPath + PlayerSettings.companyName + "\\" + PlayerSettings.productName;
        registryKey = Registry.CurrentUser.OpenSubKey(registryPath, true);
        if (registryKey != null)
        {
            var window = GetWindow<PlayerPrefsTool>(true, "PlayerPrefs Tool");
            window.RefreshData();
            window.minSize = new Vector2(800f, 600f);
            window.Show();
        }
    }

    private void RefreshData()
    {
        var keys = registryKey.GetValueNames().OrderBy(var => var);
        foreach (var key in keys)
        {
            var keyName = key.Substring(0, key.LastIndexOf('_'));
            if (PlayerPrefs.HasKey(keyName))
            {
                if (!float.IsPositiveInfinity(PlayerPrefs.GetFloat(keyName, float.PositiveInfinity)))
                {
                    var value = PlayerPrefs.GetFloat(keyName).ToString();
                    if (KeyValue.ContainsKey(keyName))
                        KeyValue[keyName] = value;
                    else
                        KeyValue.Add(keyName, value);
                }
                else if (PlayerPrefs.GetInt(keyName, int.MaxValue) != int.MaxValue)
                {
                    var value = PlayerPrefs.GetInt(keyName).ToString();
                    if (KeyValue.ContainsKey(keyName))
                        KeyValue[keyName] = value;
                    else
                        KeyValue.Add(keyName, value);
                }
                else
                {
                    var value = PlayerPrefs.GetString(keyName);
                    if (KeyValue.ContainsKey(keyName))
                        KeyValue[keyName] = value;
                    else
                        KeyValue.Add(keyName, value);
                }
            }
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Registry Path", registryKey.Name, EditorStyles.boldLabel);
        _position = EditorGUILayout.BeginScrollView(_position);
        DrawKeys();
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private void DrawKeys()
    {
        var keys = KeyValue.Keys.ToList();
        foreach (var keyName in keys)
            if (PlayerPrefs.HasKey(keyName))
            {
                if (!float.IsPositiveInfinity(PlayerPrefs.GetFloat(keyName, float.PositiveInfinity)))
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(keyName, EditorStyles.boldLabel);
                    if (GUILayout.Button("Reload", GUILayout.Width(100)))
                        KeyValue[keyName] = PlayerPrefs.GetFloat(keyName).ToString();
                    if (GUILayout.Button("Save", GUILayout.Width(100)))
                        PlayerPrefs.SetFloat(keyName, float.Parse(KeyValue[keyName]));
                    if (GUILayout.Button("Remove", GUILayout.Width(100)))
                        PlayerPrefs.DeleteKey(keyName);
                    EditorGUILayout.EndHorizontal();
                    KeyValue[keyName] = EditorGUILayout.TextArea(KeyValue[keyName], GUILayout.MaxWidth(800f));
                }
                else if (PlayerPrefs.GetInt(keyName, int.MaxValue) != int.MaxValue)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(keyName, EditorStyles.boldLabel);
                    if (GUILayout.Button("Reload", GUILayout.Width(100)))
                        KeyValue[keyName] = PlayerPrefs.GetInt(keyName).ToString();
                    if (GUILayout.Button("Save", GUILayout.Width(100)))
                        PlayerPrefs.SetInt(keyName, int.Parse(KeyValue[keyName]));
                    if (GUILayout.Button("Remove", GUILayout.Width(100)))
                        PlayerPrefs.DeleteKey(keyName);
                    EditorGUILayout.EndHorizontal();
                    KeyValue[keyName] = EditorGUILayout.TextArea(KeyValue[keyName], GUILayout.MaxWidth(800f));
                }
                else
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(keyName, EditorStyles.boldLabel);
                    if (GUILayout.Button("Reload", GUILayout.Width(100)))
                        KeyValue[keyName] = PlayerPrefs.GetString(keyName);
                    if (GUILayout.Button("Save", GUILayout.Width(100)))
                        PlayerPrefs.SetString(keyName, KeyValue[keyName]);
                    if (GUILayout.Button("Remove", GUILayout.Width(100)))
                        PlayerPrefs.DeleteKey(keyName);
                    EditorGUILayout.EndHorizontal();
                    KeyValue[keyName] = EditorGUILayout.TextArea(KeyValue[keyName], GUILayout.MaxWidth(800f));
                }
            }
    }

    private void OnDisable()
    {
        if (registryKey != null)
        {
            registryKey.Close();
            KeyValue.Clear();
        }

        PlayerPrefs.Save();
    }
}