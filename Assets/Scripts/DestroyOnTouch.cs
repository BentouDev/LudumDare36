using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DestroyOnTouch : MonoBehaviour
{
    IEnumerator DelayedDestroy()
    {
        yield return new WaitForEndOfFrame();

        DestroyObject(gameObject);
    }

    void OnTriggerEnter(Collider collider)
    {
        StartCoroutine(DelayedDestroy());
    }
}
