using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour
{
    public int maxAmmo = 6;
    public int currentAmmo;
    // Start is called before the first frame update

    private void Awake()
    {
        currentAmmo = maxAmmo;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
