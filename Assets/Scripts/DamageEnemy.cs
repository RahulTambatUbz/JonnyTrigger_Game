using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DamageEnemy : MonoBehaviour
{
    [SerializeField] private float bullteLifeTime;
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        
        if(other.gameObject.CompareTag("ENEMY"))
        {

            Destroy(other.gameObject);

        }
       
    }
    private void Start()
    {
        
        Destroy(gameObject, bullteLifeTime);

    }
}
