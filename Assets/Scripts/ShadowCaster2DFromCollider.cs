using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.U2D;

/// <summary>
/// Component that copies the object's SpriteShape's points to the ShadowCaster2D.
/// </summary>
[RequireComponent(typeof(ShadowCaster2D))]
[DefaultExecutionOrder(100)]
public class ShadowCaster2DFromCollider : MonoBehaviour
{
    /// <summary>ShadowCaster mesh</summary>
    static readonly FieldInfo _meshField;
    /// <summary>ShadowCaster shape path field</summary>
    static readonly FieldInfo _shapePathField;
    /// <summary>ShadowCaster's inbuilt generate shaadow mesh method</summary>
    static readonly MethodInfo _generateShadowMeshMethod;
    /// <summary>ShadowCaster2D component</summary>
    ShadowCaster2D _shadowCaster;
    /// <summary>SpriteShapeController component</summary>
    SpriteShapeController spriteShapeController;

    /// <summary>
    /// Constructor that gets the needed methods and references.
    /// </summary>
    static ShadowCaster2DFromCollider()

    {
        _meshField = typeof(ShadowCaster2D).GetField("m_Mesh", BindingFlags.NonPublic | BindingFlags.Instance);
        _shapePathField = typeof(ShadowCaster2D).GetField("m_ShapePath", BindingFlags.NonPublic | BindingFlags.Instance);
        _generateShadowMeshMethod = typeof(ShadowCaster2D)
                                    .Assembly
                                    .GetType("UnityEngine.Experimental.Rendering.Universal.ShadowUtility")
                                    .GetMethod("GenerateShadowMesh", BindingFlags.Public | BindingFlags.Static);
    }

    /// <summary>
    /// Gets the two components when the script is loaded.
    /// </summary>
    private void Awake()
    {
        _shadowCaster = GetComponent<ShadowCaster2D>();
        spriteShapeController = GetComponent<SpriteShapeController>();

    }

    /// <summary>
    /// Updates the shadow shape.
    /// </summary>
    public void UpdateShadow()
    {
        _shadowCaster = GetComponent<ShadowCaster2D>();
        spriteShapeController = GetComponent<SpriteShapeController>();

        //var points = _polygonCollider == null ? _edgeCollider.points : _polygonCollider.points;

        //Copy the points from the SplineShape
        Vector3[] points = new Vector3[spriteShapeController.spline.GetPointCount()];
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = spriteShapeController.spline.GetPosition(i);
        }

        //Set the shadow caster's points and mesh, then invoke its mesh generation method
        _shapePathField.SetValue(_shadowCaster, points);
        _meshField.SetValue(_shadowCaster, new Mesh());
        _generateShadowMeshMethod.Invoke(_shadowCaster, new object[] { _meshField.GetValue(_shadowCaster), _shapePathField.GetValue(_shadowCaster) });
    }

    /// <summary>
    /// Method that turns a Vector2 array into a Vector3 array.
    /// </summary>
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
