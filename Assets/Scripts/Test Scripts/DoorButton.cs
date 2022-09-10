using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorButton : MonoBehaviour
{
    [SerializeField] private GameObject doorFrame;

    public void OnTriggerEnter2D(Collider2D other)
    {
        doorFrame.GetComponent<Door>().doesOpen = true;
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.1f);
    }
}
