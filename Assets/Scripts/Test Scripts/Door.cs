using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Door : MonoBehaviour
{
    public GameObject door;
    private float height;
    [SerializeField] private float openPos;
    [SerializeField] private float speed;
    private Vector3 ground;

    private void Start()
    {
        ground = transform.position;
        door.transform.position = new Vector3(door.transform.position.x, ground.y + 1.92f);
        openPos = ground.y + openPos;
    }

    private void Update()
    {
        door.transform.position = Vector3.MoveTowards(door.transform.position, new Vector3(ground.x, openPos), speed * Time.deltaTime);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(ground - new Vector3(0.5f, 0), ground + new Vector3(0.5f, 0));
        Gizmos.color = Color.green;
        Gizmos.DrawLine(ground, new Vector3(ground.x, openPos));
    }

}
