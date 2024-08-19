using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletUI : MonoBehaviour
{
    [SerializeField] public Ammo ammo;
    [SerializeField] private GameObject objectToSpawn;

    private void Awake()
    {
        for (int i = 0; i < ammo.maxAmmo; i++)
        {
            Instantiate(objectToSpawn,transform);

        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
