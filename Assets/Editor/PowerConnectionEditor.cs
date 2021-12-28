using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PowerConnection))]
public class PowerConnectionEditor : Editor
{

    public override void OnInspectorGUI()
    {
        PowerConnection powerConnection = (PowerConnection)target;

        if (DrawDefaultInspector())
        {

        }

        if (GUILayout.Button("Refresh line"))
        {
            powerConnection.SetConnectionInterface(powerConnection.GetOutput().GetComponent<IConnectable>());
            powerConnection.SetUpLines();
        }
    }


}
