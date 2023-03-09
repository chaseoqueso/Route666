using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Bullet : MonoBehaviour
{
    public GameObject impactPrefab;

    private UnityAction performOnCompleted;
    private Vector3 velocity;
    private Vector3 hitNormal;
    private Rigidbody rb;
    private float distanceTravelled;
    private float distance;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        distanceTravelled = 0;
    }

    void FixedUpdate()
    {
        if(velocity != Vector3.zero)
        {
            rb.MovePosition(transform.position + velocity * Time.fixedDeltaTime);
        
            distanceTravelled += (velocity * Time.fixedDeltaTime).magnitude;
            if(distanceTravelled > distance)
            {
                if(performOnCompleted != null)
                    performOnCompleted.Invoke();
                    
                if(hitNormal != Vector3.zero)
                {
                    GameObject flash = Instantiate(impactPrefab, transform.position, Quaternion.LookRotation(Vector3.RotateTowards(hitNormal, velocity.normalized, 45f * Mathf.Deg2Rad, 100)));
                    Destroy(flash, 0.05f);
                }
                DestroySelf();
            }
        }
    }

    private void DestroySelf()
    {
        velocity = Vector3.zero;
        Destroy(gameObject, 1);
    }

    public void Initialize(Vector3 velocity, float distance, Vector3 hitNormal, UnityAction performOnCompleted)
    {
        this.velocity = velocity;
        this.distance = distance;
        this.hitNormal = hitNormal;
        this.performOnCompleted = performOnCompleted;
    }
}
