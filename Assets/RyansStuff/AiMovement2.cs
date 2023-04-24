using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiMovement2 : MonoBehaviour
{
    private Transform playerTransform;
    public float maxTime = 1.0f;
    public float maxDistance = 1.0f;

    NavMeshAgent agent;
    //Animator animator;
    float timer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerTransform = Camera.main.transform;
       // animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if(timer < 0.0f){
            //agent.SetDestination(playerTransform.position);
            float sqDistance = (playerTransform.position - agent.destination).sqrMagnitude;
            if(sqDistance > maxDistance*maxDistance){ 
                agent.destination = playerTransform.position;
            }
            
            timer = maxTime;
        }
        //agent.destination= playerTransform.position; //This fixes it
       // animator.SetFloat("speed", agent.velocity.magnitude);
    }
}
