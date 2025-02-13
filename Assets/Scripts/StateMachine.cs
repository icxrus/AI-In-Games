using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public enum AgentState
{
    Idle,
    Chasing,
    Partol,
    Attacking
}
public class StateMachine : MonoBehaviour //Navigation Brain
{
    public AgentState currentState;
    [SerializeField] Transform target;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform waypoints;
    [SerializeField] List<Transform> waypointList;
    [SerializeField] float cooldown;
    int currentWaypointIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        for (int i = 0; i < waypoints.childCount; i++)
        {
            waypointList.Add(waypoints.GetChild(i));
        }
        currentState = AgentState.Idle;
        StartCoroutine(MakeDecision());
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case AgentState.Idle:
                break;

            case AgentState.Chasing:
                agent.SetDestination(target.position);
                break;

            case AgentState.Partol:
                Patrol();
                break;

            case AgentState.Attacking:
                //playerhealth--;
                break;
        }
    }

    IEnumerator MakeDecision()
    {
        while (true)
        {
            int rand = Random.Range(0, 9);
            if (rand < 5)
            {
                currentState = AgentState.Idle;
            }
            else if (rand >= 5)
            {
                currentState = AgentState.Partol;
            }
            yield return new WaitForSeconds(cooldown);
        }
    }

    void Patrol()
    {
        if (!agent.hasPath && currentWaypointIndex < waypointList.Count)
        {
            agent.SetDestination(waypointList[currentWaypointIndex].position);
            currentWaypointIndex++;
        }
        else if (!agent.hasPath && currentWaypointIndex == waypointList.Count)
        {
            currentWaypointIndex = 0;
            agent.SetDestination(waypointList[0].position);
        }
    }
}
