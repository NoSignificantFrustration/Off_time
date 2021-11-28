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
            powerConnection.connectioInterface = powerConnection.output.GetComponent<IConnectable>();
            powerConnection.SetUpLines();
        }
    }


}
