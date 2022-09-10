using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    private Vector3 offset = new Vector3(0f, 2f, -10f);
    [SerializeField]
    private float smoothTime = 0.3f;
    private Vector3 velocity = Vector3.zero;
    [SerializeField]
    private GameObject target;

    private void Start()
    {
        transform.position = target.transform.position + offset;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 targetPosition = target.transform.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
