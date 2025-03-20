using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Align : BaseSteeringBehavior
{
    public Kinematics target;


    public float maxAngularAcceleration = 90f;
    public float maxRotationSpeed = 45f;

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
            float rotation = target.orientation-character.orientation;

            float rotationSize = Mathf.Abs(rotation);

            if (rotationSize<targetRadius)
            {
                return steering;
            }
            float targetRotation = 0;
            if (rotationSize>slowRadius) 
            {
                targetRotation=maxRotationSpeed;
            }
            else
            {
                targetRotation=maxRotationSpeed*rotationSize/slowRadius;
            }
            targetRotation *= rotation / rotationSize;

            steering.angular=targetRotation-character.rotation;
            steering.angular/=timeToTarget;

            float angularAcceleration = Mathf.Abs(steering.angular);
            if (angularAcceleration>maxAngularAcceleration) 
            {
                steering.angular/=angularAcceleration;
                steering.angular *= maxAngularAcceleration;
            }
        }

        return steering;
    }
}
