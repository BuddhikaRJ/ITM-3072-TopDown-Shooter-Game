using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] NavMeshAgent agent;
    // Start is called before the first frame update
    void OnEnable()
    {
        player = FindAnyObjectByType<PlayerBehavior>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(player.position);
    }

    void OnTriggerEnter(Collider other){
        Debug.Log(other.tag);

        if(other.gameObject.CompareTag("bullet")){
            Destroy(gameObject);
        }

        if(other.gameObject.CompareTag("Player")){
            Debug.LogError("GAME OVER");
        }
    }
}
