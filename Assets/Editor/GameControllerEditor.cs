using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameController))]
public class GameControllerEditor : Editor
{
    private GameController controller;
    public override void OnInspectorGUI()
    {
        controller = (GameController)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Generate IDs"))
        {

            GenerateIDs();
            
        }

        
    }
    public void OnPreprocessBuild(BuildTarget target, string path)
    {
        GenerateIDs();
    }

    private void GenerateIDs()
    {
        controller.CollectSaveables();

        foreach (ISaveable item in controller.GetSaveables())
        {
            Object obj = item.GetObject(true);
            if (obj != null)
            {
                SerializedObject so = new SerializedObject(obj);
                SerializedProperty sp = so.FindProperty("uid");
                so.Update();
                sp.stringValue = System.Guid.NewGuid().ToString();
                so.ApplyModifiedProperties();
            }

        }
    }
}
