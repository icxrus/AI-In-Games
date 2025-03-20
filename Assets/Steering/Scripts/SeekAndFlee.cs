using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekAndFlee : BaseSteeringBehavior
{
    public GameObject target;
    public bool flee = false;
    public float maxAcceleration = 2.5f;

    public override SteeringOutput GetSteering()
    {
        SteeringOutput steering;
        steering.linear = Vector3.zero;
        steering.angular = 0f;

        if (target) 
        {
                steering.linear = target.transform.position - transform.position;
                if(flee)
                    steering.linear *= -1;
                steering.linear.Normalize();
                steering.linear *= maxAcceleration;
            
        }

        return steering;
    }
}
