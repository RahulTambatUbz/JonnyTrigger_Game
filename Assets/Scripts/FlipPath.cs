using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipPath : MonoBehaviour
{
    //public Transform pointA;
    public Transform pointB;
    public float flipDuration;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Transform GetPathPoint()
    {
        return (pointB);



    }public float GetFlipDuration()
    {
        return (flipDuration);



    }
}
