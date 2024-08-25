using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DamageEnemy : MonoBehaviour
{
    [SerializeField] private float bullteLifeTime;
    [SerializeField] private AudioSource audioSource;
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        
        if(other.gameObject.CompareTag("ENEMY"))
        {
            audioSource.Play();
           // other.gameObject.GetComponent<Enemy>().ActivateRagdoll();
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
        if(other.gameObject.CompareTag("Player"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);


        }
       
    }
    private void Start()
    {
        
        Destroy(gameObject, bullteLifeTime);

    }
}
