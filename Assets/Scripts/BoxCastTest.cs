using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxCastTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var hit = Physics2D.BoxCast(new Vector2(0, -8f), new Vector2(2, 2), 0, Vector2.zero, 0,
            LayerMask.GetMask("Default"));
        if (hit)
            Debug.Log(hit.transform.name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
