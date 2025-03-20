using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kinematics : MonoBehaviour
{
    BaseSteeringBehavior steeringBehavior;

    public float orientation = 0;

    public Vector3 velocity;
    public float rotation;

    void Start()
    {
        steeringBehavior = GetComponent<BaseSteeringBehavior>();
    }

    void Update()
    {
        if (steeringBehavior == null || !steeringBehavior.isActiveAndEnabled) 
        {
            return;
        }

        transform.position += velocity * Time.deltaTime;

        if (steeringBehavior.ignorerotation) 
        {
            if (velocity.sqrMagnitude > 0)
            {
                transform.forward = velocity;
            }
        }
        else
        {
            orientation += rotation * Time.deltaTime;
            if (orientation<0)
            {
                orientation += 360;
            }
            if (orientation>360)
            {
                orientation-=360;
            }
            transform.forward = new Vector3(Mathf.Sin(orientation*Mathf.Deg2Rad),0,Mathf.Cos(orientation*Mathf.Deg2Rad));
        }
        
        

        SteeringOutput steeringOutput = steeringBehavior.GetSteering();
        velocity += steeringOutput.linear * Time.deltaTime;
        rotation += steeringOutput.angular * Time.deltaTime;

        if (transform.position.magnitude>40)
        {
            transform.position = new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f));
            transform.Rotate(Vector3.up, Random.Range(0f, 360f));

            velocity = Vector3.zero;
            rotation = 0;
        }
    }
}
