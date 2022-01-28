using System;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private GameObject player;
    private GameManager gameManager;
    private Rigidbody selfRb;
    
    void Start()
    {
        player = GameObject.Find("Player");
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        selfRb = GetComponent<Rigidbody>();
    }
    
    public void Update()
    {
        if (gameManager.isGameActive)
        {
            if (transform.position.y < -5)
            {
                Destroy(gameObject);
                gameManager.EnemyDestroyed();
            }
            
            Vector3 vectorDifference = player.transform.position - transform.position;
            Vector3 vectorToTravel = new Vector3(vectorDifference.x, 0, vectorDifference.z).normalized;
            if (CompareTag("Enemy"))
            {
                selfRb.AddForce(vectorToTravel * 10, ForceMode.Force);
                selfRb.velocity = Vector3.ClampMagnitude(selfRb.velocity, 8);
            }
            else // it's a large boi
            {
                selfRb.AddForce(vectorToTravel * 90, ForceMode.Force);
                selfRb.velocity = Vector3.ClampMagnitude(selfRb.velocity, 6);
            }
        }
    }
}