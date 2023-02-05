using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingBarrel : MonoBehaviour, IShootable
{
    [SerializeField] private float explosionRange = 2f;

    [SerializeField] private int damageValue = 1;
    [SerializeField] private LayerMask explodableLayerMask;

    //callback to draw gizmos that are pickable and always drawn? not sure what this means, i should ask
    //might cause problems in build??
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }

    public void OnShoot()
    {
        Explode();

        Destroy(gameObject);
    }

    private void Explode()
    {
        Collider[] objectsToExplode = Physics.OverlapSphere(transform.position, explosionRange, explodableLayerMask);

        foreach(Collider objectToExplode in objectsToExplode)
        {
            if(objectToExplode.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                objectToExplode.GetComponent<Enemy>().TakeDamage(damageValue, KillType.environmentalKill);
            }
            else
            {
                Destroy(objectToExplode.gameObject);
            }
            
        }
    }
}
