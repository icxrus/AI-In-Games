using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Arrive : BaseSteeringBehavior
{
    public GameObject target;


    public float maxAcceleration = 2.5f;
    public float maxSpeed = 10f;

    public float targetRadius = 3.0f;
    public float slowRadius = 10f;
    public float timeToTarget = 0.1f;

    public override SteeringOutput GetSteering()
    {
        SteeringOutput steering;
        steering.linear = Vector3.zero;
        steering.angular = 0f;

        if (target)
        {
            Vector3 direction = target.transform.position - character.transform.position;
            float distance = direction.magnitude;

            float targetSpeed = 0;

            if (distance < targetRadius)
            {
                return steering;
            }

            if (distance > slowRadius)
            {
                targetSpeed = maxSpeed;
            }
            else 
            { 
                targetSpeed = maxSpeed * (distance-targetRadius) / (slowRadius-targetRadius); 
            }

            Vector3 targetVelocity = direction;
            targetVelocity.Normalize();
            targetVelocity *= targetSpeed;

            steering.linear = targetVelocity - character.velocity;
            steering.linear /= timeToTarget;

            if (steering.linear.magnitude>maxAcceleration)
            {
                steering.linear.Normalize();
                steering.linear *= maxAcceleration;
            }
        }

        return steering;
    }
}
