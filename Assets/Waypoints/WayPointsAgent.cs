using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointsAgent : MonoBehaviour
{
    [SerializeField] private GameObject waypointObject;
    [SerializeField] private List<GameObject> wayPoints = new List<GameObject>();
    [SerializeField] private Transform AIAgent;

    private int index = 0;

    private void Start()
    {
        AIAgent = gameObject.transform;
        for (int i = 0; i < waypointObject.transform.childCount; i++)
        {
            wayPoints.Add(waypointObject.transform.GetChild(i).gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (index < wayPoints.Count)
        {
            MoveTowardsWaypoint();
        }
    }

    private void MoveTowardsWaypoint()
    {
        transform.LookAt(wayPoints[index].transform);
        transform.Translate(Vector3.forward * (10f * Time.deltaTime));
        Debug.DrawLine(AIAgent.position, wayPoints[index].transform.position, Color.green);
    }

    private void OnTriggerEnter(Collider other)
    {
        index++;
    }
}
