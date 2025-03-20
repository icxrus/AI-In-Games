using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public interface ITask //also can be called strategies!!!
{
    BT_Node.Status Process();
    void Reset() 
    {
        //nothing by default
    }
}

public class Condition : ITask 
{
    Func<bool> predicate;

    public Condition(Func<bool> predicate)
    {
        this.predicate = predicate;
        Debug.Log(predicate());
    }

    public BT_Node.Status Process() 
    {
        if (predicate()) 
        {
            return BT_Node.Status.Success;
        }
        else
        {
            return BT_Node.Status.Failure;
        }
    }
}
public class PatrolTask : ITask
{
     Transform enemy;
     NavMeshAgent agent;
    AIDetection AIDetection;
     List<Transform> patrolPoints;
     float patrolSpeed;
    int currentIndex;
    bool isPathCalculated;

    public PatrolTask(Transform enemy, NavMeshAgent agent, AIDetection AIDetection, List<Transform> patrolPoints, float patrolSpeed=2f)
    {
        this.enemy = enemy;
        this.agent = agent;
        this.patrolPoints = patrolPoints;
        this.patrolSpeed = patrolSpeed;
        this.AIDetection= AIDetection;
    }

    public BT_Node.Status Process() 
    {
        Debug.Log(currentIndex);
        if (currentIndex == patrolPoints.Count) 
        {
            return BT_Node.Status.Success;
        }
        if (AIDetection.playerVisible)
        {
            return BT_Node.Status.Failure;
        }
        var target = patrolPoints[currentIndex];
        agent.SetDestination(target.position);

        //enemy.LookAt(target);

        if (isPathCalculated&&agent.remainingDistance<0.1f) 
        {
            
            currentIndex++;
            //Debug.Log(currentIndex);
            isPathCalculated = false;
        }

        if (agent.pathPending)
        {
            isPathCalculated = true;
        }

        return BT_Node.Status.Running;
    }

    public void Reset() 
    {
            currentIndex = 0;
    }
}

public class ChaseTask : ITask
{
    AIDetection AIDetection;
    Transform playerTarget;
    NavMeshAgent agent;
    public ChaseTask(AIDetection AIDetection, Transform playerTarget, NavMeshAgent agent)
    {
        this.AIDetection = AIDetection;
        this.playerTarget = playerTarget;
        this.agent = agent;
    }

    public BT_Node.Status Process() 
    {
        agent.SetDestination(playerTarget.position);
        if (AIDetection.playerVisible)
        {
            return BT_Node.Status.Running;
        }
        return BT_Node.Status.Success;
    }
}

public class ActionTask : ITask 
{
    Action doSomething;

    public ActionTask(Action doSomething) 
    {
        this.doSomething = doSomething;
    }

    public BT_Node.Status Process() 
    {
        doSomething();
        return BT_Node.Status.Success;
    }
}
