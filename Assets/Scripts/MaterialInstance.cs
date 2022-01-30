using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialInstance : MonoBehaviour
{
    [SerializeField] private Color albeto;
    private SpriteRenderer r;

    private void Awake()
    {
        r = GetComponent<SpriteRenderer>();
    }

    public void UpdateColor()
    {
        if (r == null)
        {
            r = GetComponent<SpriteRenderer>();
        }
        r.material.color = albeto;
    }
}
