using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ShadowCaster2DFromCollider))]
public class ShadowCaster2DFromColliderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ShadowCaster2DFromCollider sc = (ShadowCaster2DFromCollider)target;

        DrawDefaultInspector();


        if (GUILayout.Button("Refresh shadow shape"))
        {
            sc.UpdateShadow();
        }

    }
}
