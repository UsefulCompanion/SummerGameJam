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
    private bool doesOpen;

    private void Start()
    {
        door.transform.position = new Vector3(door.transform.position.x, transform.position.y + 1.92f);
        openPos = transform.position.y + openPos;
        doesOpen = false;
    }

    private void Update()
    {
        if (doesOpen)
        {
            door.transform.position = Vector3.MoveTowards(door.transform.position,
                new Vector3(transform.position.x, openPos), speed * Time.deltaTime);
        }
    }

    public void OpenDoor()
    {
        doesOpen = true;
    }

    void OnDrawGizmosSelected()
    {
        var position = transform.position;
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(position - new Vector3(0.5f, 0), position + new Vector3(0.5f, 0));
        Gizmos.color = Color.green;
        Gizmos.DrawLine(position, new Vector3(position.x, openPos));
    }

}
