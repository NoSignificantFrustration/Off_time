using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Door : MonoBehaviour
{
    [SerializeField] private GameObject door;
    private SpriteRenderer sr;
    private ShadowCaster2D sc;
    private BoxCollider2D bc;

    private void Awake()
    {
        sr = door.GetComponentInChildren<SpriteRenderer>();
        sc = door.GetComponentInChildren<ShadowCaster2D>();
        bc = door.GetComponentInChildren<BoxCollider2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleDoor(bool open)
    {
        sr.enabled = !open;
        sc.castsShadows = !open;
        bc.enabled = !open;
    }
}
