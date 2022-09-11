using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSelfDestruct : MonoBehaviour
{
    private void Start()
    {
        Destroy(this.gameObject, 1f);
        
    }

}
