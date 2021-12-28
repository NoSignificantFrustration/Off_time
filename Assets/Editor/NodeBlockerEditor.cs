using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NodeBlocker))]
public class NodeBlockerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        NodeBlocker blocker = (NodeBlocker)target;

        DrawDefaultInspector();




    }
}
