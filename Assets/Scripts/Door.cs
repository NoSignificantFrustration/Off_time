using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

/// <summary>
/// Logic for the doors.
/// </summary>
/// 
public class Door : MonoBehaviour
{
    /// <summary>The door GameObject (The part that opens)</summary>
    [SerializeField] private GameObject door;
    /// <summary>The door's SpriteRenderer component</summary>
    private SpriteRenderer sr;
    /// <summary>The door's ShadowCaster2D component</summary>
    private ShadowCaster2D sc;
    /// <summary>The door's BoxCollider2D component</summary>
    private BoxCollider2D bc;
    
    /// <summary>
    /// Gets references to the door's components.
    /// </summary>
    private void Awake()
    {
        sr = door.GetComponentInChildren<SpriteRenderer>();
        sc = door.GetComponentInChildren<ShadowCaster2D>();
        bc = door.GetComponentInChildren<BoxCollider2D>();
    }

    /// <summary>
    /// Opens and closes the door.
    /// </summary>
    /// <param name="open">New state</param>
    public void ToggleDoor(bool open)
    {
        sr.enabled = !open;
        sc.castsShadows = !open;
        bc.enabled = !open;
    }
}
