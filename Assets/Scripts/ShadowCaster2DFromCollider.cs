using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.U2D;

[RequireComponent(typeof(ShadowCaster2D))]
[DefaultExecutionOrder(100)]
public class ShadowCaster2DFromCollider : MonoBehaviour
{
    static readonly FieldInfo _meshField;
    static readonly FieldInfo _shapePathField;
    static readonly MethodInfo _generateShadowMeshMethod;
    ShadowCaster2D _shadowCaster;
    SpriteShapeController spriteShapeController;

    static ShadowCaster2DFromCollider()

    {
        _meshField = typeof(ShadowCaster2D).GetField("m_Mesh", BindingFlags.NonPublic | BindingFlags.Instance);
        _shapePathField = typeof(ShadowCaster2D).GetField("m_ShapePath", BindingFlags.NonPublic | BindingFlags.Instance);
        _generateShadowMeshMethod = typeof(ShadowCaster2D)
                                    .Assembly
                                    .GetType("UnityEngine.Experimental.Rendering.Universal.ShadowUtility")
                                    .GetMethod("GenerateShadowMesh", BindingFlags.Public | BindingFlags.Static);
    }


    private void Awake()
    {
        _shadowCaster = GetComponent<ShadowCaster2D>();
        spriteShapeController = GetComponent<SpriteShapeController>();
        
    }

    public void UpdateShadow()
    {
        _shadowCaster = GetComponent<ShadowCaster2D>();
        spriteShapeController = GetComponent<SpriteShapeController>();

        //var points = _polygonCollider == null ? _edgeCollider.points : _polygonCollider.points;
        Vector3[] points = new Vector3[spriteShapeController.spline.GetPointCount()];
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = spriteShapeController.spline.GetPosition(i);
        }

        _shapePathField.SetValue(_shadowCaster, points);
        _meshField.SetValue(_shadowCaster, new Mesh());
        _generateShadowMeshMethod.Invoke(_shadowCaster, new object[] { _meshField.GetValue(_shadowCaster), _shapePathField.GetValue(_shadowCaster) });
    }

    public Vector3[] V2toV3(Vector2[] v2)
    {
        Vector3[] v3 = new Vector3[v2.Length];
        for (int i = 0; i < v2.Length; i++)
        {
            v3[i] = (Vector3)v2[i];
        }

        return v3;
    }
}
