using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject impactPrefab;

    private Vector3 velocity;
    private Rigidbody rb;
    private float distanceTravelled;
    private float maxDistance;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        distanceTravelled = 0;
    }

    void FixedUpdate()
    {
        if(velocity != Vector3.zero)
        {
            RaycastHit hit;
            if(Physics.Raycast(transform.position, velocity, out hit, velocity.magnitude * Time.fixedDeltaTime)
                && hit.transform.gameObject.layer != LayerMask.NameToLayer("Player"))
            {
                if(hit.transform.gameObject.layer == LayerMask.NameToLayer("Environment"))
                {
                    GameObject flash = Instantiate(impactPrefab, transform.position, Quaternion.LookRotation(Vector3.RotateTowards(hit.normal, velocity.normalized, 45f * Mathf.Deg2Rad, 100)));
                    Destroy(flash, 0.05f);
                }

                DestroySelf();
            }
            else
            {
                rb.MovePosition(transform.position + velocity * Time.fixedDeltaTime);
            
                distanceTravelled += (velocity * Time.fixedDeltaTime).magnitude;
                if(distanceTravelled > maxDistance)
                {
                    DestroySelf();
                }
            }
        }
    }

    private void DestroySelf()
    {
        velocity = Vector3.zero;
        Destroy(gameObject, 1);
    }

    public void Initialize(Vector3 velocity, float maxDistance)
    {
        this.velocity = velocity;
        this.maxDistance = maxDistance;
    }
}
