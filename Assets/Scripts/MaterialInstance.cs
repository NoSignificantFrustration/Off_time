using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component that makes the material of the GameObject's SpriteRenderer it's own seperate instance.
/// </summary>
public class MaterialInstance : MonoBehaviour
{
    /// <summary>Desired color</summary>
    [SerializeField] private Color albeto;
    /// <summary>SpriteRenderer component</summary>
    private SpriteRenderer r;

    /// <summary>
    /// Get a reference to the SpriteRenderer when the script is loaded.
    /// </summary>
    private void Awake()
    {
        r = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Updates the material's color.
    /// </summary>
    public void UpdateColor()
    {
        if (r == null)
        {
            r = GetComponent<SpriteRenderer>();
        }
        r.material.color = albeto;
    }
}
