using UnityEngine;
using System.Collections.Generic;

public class Hurt : MonoBehaviour
{
    public int Damage = 1;

    void OnTriggerEnter(Collider collider)
    {
        var damageable = collider.gameObject.GetComponentInParent(typeof(IDamageable)) as IDamageable;

        if (damageable != null)
        {
            damageable.TakeDamage(Damage);
        }
    }
}
