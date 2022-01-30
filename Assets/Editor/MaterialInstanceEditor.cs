using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MaterialInstance))]
public class MaterialInstanceEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MaterialInstance instance = (MaterialInstance)target;

        if (DrawDefaultInspector())
        {
            instance.UpdateColor();

        }



    }
}
