using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSelfDestruct : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(DestroyAfterTime());
    }

    private IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(.75f);
        Destroy(this.gameObject);
    }

}
