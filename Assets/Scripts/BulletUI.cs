using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletUI : MonoBehaviour
{
    [SerializeField] public Ammo ammo;
    [SerializeField] private GameObject objectToSpawn;
    private int currentIndex = 0;
    private void Awake()
    {
        ReloadBulletUI(); 
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void UpdateUI()
    {

        if(transform.childCount > 0)
        {
            if(currentIndex >= transform.childCount)
            {
                currentIndex = 0;

            }

            Transform child = transform.GetChild(currentIndex);

            // Destroy the child GameObject
            Destroy(child.gameObject);

            // Increment the index for the next call
            currentIndex++;


        }
         else
        {
            Debug.Log("No more children to destroy.");
        }



    }

    public void ReloadBulletUI()
    {

        for (int i = 0; i < ammo.maxAmmo; i++)
        {
            Instantiate(objectToSpawn, transform);

        }

    }
}
