using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AIDetection))]
public class Enemy : MonoBehaviour
{
    [SerializeField] Transform waypointParent;
    [SerializeField]List<Transform> waypoints;
    AIDetection AIDetection;
    NavMeshAgent agent;
    BehaviorTree BehaviorTree;
    [SerializeField]Transform playerTarget;
    [SerializeField] Transform safePlace;
    [SerializeField] Transform safePlace2;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        for (int i = 0; i < waypointParent.childCount; i++) 
        {
            waypoints.Add(waypointParent.GetChild(i).transform);
        }
        AIDetection = GetComponent<AIDetection>();

        BehaviorTree = new BehaviorTree("BT_Enemy");

        PrioritySelector ChaseOrPatrol = new PrioritySelector("ChaseOrPatrol");

        Sequence PatrolSequence = new Sequence("PatrolSequence",80);
        PatrolSequence.AddChild(new Leaf("isPatrolling", new Condition(() => waypoints.Count>0)));
        PatrolSequence.AddChild(new Leaf("Patrol", new PatrolTask(transform,agent,AIDetection, waypoints)));

        Sequence ChasePlayerSequence = new Sequence("ChasePlayerSequence",100);
        Leaf isChasing = new Leaf("isChasing", new Condition(() => AIDetection.playerVisible));
        Leaf ChasePlayer = new Leaf("ChasePlayer", new ChaseTask(AIDetection, playerTarget, agent));
        ChasePlayerSequence.AddChild(isChasing);
        ChasePlayerSequence.AddChild(ChasePlayer);

        Selector goToSafePlace = new RandomSelector("goToSafePlace", 50);
        Sequence goToSafePlaceSeq1 = new Sequence("goToSafePlaceSeq1");
        goToSafePlaceSeq1.AddChild(new Leaf("isSafePlace1?", new Condition(() => safePlace.gameObject.activeSelf)));
        goToSafePlaceSeq1.AddChild(new Leaf("GoToSafePlace1", new ActionTask(() => agent.SetDestination(safePlace.position))));

        goToSafePlace.AddChild(goToSafePlaceSeq1);

        Sequence goToSafePlaceSeq2 = new Sequence("goToSafePlaceSeq2");
        goToSafePlaceSeq1.AddChild(new Leaf("isSafePlace2?", new Condition(() => safePlace2.gameObject.activeSelf)));
        goToSafePlaceSeq1.AddChild(new Leaf("GoToSafePlace2", new ActionTask(() => agent.SetDestination(safePlace2.position))));

        goToSafePlace.AddChild(goToSafePlaceSeq2);

        ChaseOrPatrol.AddChild(ChasePlayerSequence);
        ChaseOrPatrol.AddChild(PatrolSequence);
        ChaseOrPatrol.AddChild(goToSafePlace);

        BehaviorTree.AddChild(ChaseOrPatrol);


        //BehaviorTree.AddChild(new Leaf("Patrol", new PatrolTask(transform, agent, waypoints)));
        //BehaviorTree.AddChild(new Leaf("isChasing", new Condition(() => AIDetection.playerVisible)));
        //BehaviorTree.AddChild(new Leaf("ChasePlayer", new ChaseTask(AIDetection, playerTarget, agent)));
    }

    private void Update()
    {
        if (BehaviorTree.Process() == BT_Node.Status.Success)
        {
            BehaviorTree.Reset();
        }
    }
}
