using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDetection : MonoBehaviour
{
    [SerializeField] float fieldOfView = 90;
    [SerializeField] float viewDistance = 5;
    [SerializeField] Transform target;
    [SerializeField] Transform eyes;
    public bool playerVisible;
    //StateMachine StateMachine;
    [SerializeField] float attackDistance = 1;

    void Start()
    {
        //StateMachine = GetComponent<StateMachine>();
    }

    void FixedUpdate()
    {
        float distance = Vector3.Distance(transform.position,target.position);
        Vector3 angleFrom = target.position - transform.position;
        float angle = Vector3.Angle(angleFrom,transform.forward);
        RaycastHit hit;
        if (distance<viewDistance && angle < fieldOfView/2) 
        {
            if (Physics.Raycast(eyes.position,target.position-eyes.position,out hit,viewDistance)) 
            {
                if (hit.collider.CompareTag("Player")) 
                {
                    playerVisible = true;
                    //StateMachine.currentState = AgentState.Chasing;
                    Debug.DrawLine(eyes.position, hit.point,Color.cyan);
                    CheckIfCloseEnoughToAttack();
                }
            }
        }
        else
        {
            playerVisible=false;
            Invoke("LosePlayer", 2f);
        }
    }

    public bool LosePlayer() 
    {
        if (!playerVisible)
        {
            //StateMachine.currentState = AgentState.Idle;
        }
        return true;
    }

    void CheckIfCloseEnoughToAttack() 
    {
        float distance = Vector3.Distance(target.position, transform.position);
        if (distance < attackDistance)
        {
            //StateMachine.currentState = AgentState.Attacking;
        }
    }
}
