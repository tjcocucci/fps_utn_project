using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(LevelManager)), CanEditMultipleObjects]
public class MapEditor : Editor
{

    public override void OnInspectorGUI() {
        LevelManager levelManager = target as LevelManager;
        levelManager.currentLevelIndex = EditorGUILayout.IntField("Current Level Index", levelManager.currentLevelIndex);
        if (DrawDefaultInspector()) {
            Level level = levelManager.levels[levelManager.currentLevelIndex];
            if (level.map != null) {
                levelManager.mapGenerator.GenerateMap(level.map);
            }
        }
        if (GUILayout.Button("Generate Map")) {
            Level level = levelManager.levels[levelManager.currentLevelIndex];
            if (level.map != null) {
                levelManager.mapGenerator.GenerateMap(level.map);
            }
        }
    }
}
