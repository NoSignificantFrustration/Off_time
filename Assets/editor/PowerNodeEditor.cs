using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PowerNode))]
public class PowerNodeEditor : Editor
{

    public override void OnInspectorGUI()
    {
        PowerNode powerNode = (PowerNode)target;

        if (DrawDefaultInspector())
        {
            powerNode.LoadSprite();
            powerNode.PreRotate();
        }

        if (GUILayout.Button("Pulse"))
        {
            powerNode.Pulse();
        }
    }

}
