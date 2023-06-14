using System;
using System.Collections;
using System.Collections.Generic;
using Mono.Cecil;
using UnityEngine;
using Random = UnityEngine.Random;

public class Shooter : MonoBehaviour
{
    [SerializeField] private GameObject[] projectilePrefab;
    //[SerializeField] private GameObject projectilePrefab2;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float projectileLifeTime = 5f;
    [SerializeField] private float baseFiringRate = 0.2f;
    [SerializeField] private float firingRateVariance = 0;
    [SerializeField] private float minimumFiringRate = 0.1f;
    [SerializeField] private Vector3 AddToChangePosition;
    [SerializeField] private bool useAI;
    
    [HideInInspector]
    public bool isFiring;

    private Coroutine firingCor;
    private Vector2 moveDirection;
    private void Start()
    {
        if (useAI)
        {
            isFiring = true;
            moveDirection = transform.up * -1;
        }
        else
        {
            moveDirection = transform.up;
        }
    }

    private void Update()
    {
        Fire();
    }

    void Fire()
    {
        if (isFiring && firingCor == null)
        {
            firingCor = StartCoroutine(FireContinuously());
        }
        else if(!isFiring && firingCor != null)
        {
            StopCoroutine(firingCor);
            firingCor = null;
        }
        
    }

    IEnumerator FireContinuously()
    {
        while (true)
        {
            Vector3 addPosition = new(-1, 0, 0);
            for (int i = 0; i < projectilePrefab.Length; i++)
            {

                GameObject[] projectile = new GameObject[projectilePrefab.Length];
                projectile[i] = Instantiate(projectilePrefab[i], transform.position + addPosition, Quaternion.identity);
                //GameObject projectile2 = Instantiate(projectilePrefab2, transform.position + new Vector3(5, 0, 0), Quaternion.identity);
                Rigidbody2D[] rb = new Rigidbody2D[projectilePrefab.Length];
                rb[i] = projectile[i].GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb[i].velocity = moveDirection * projectileSpeed;
                }

                Destroy(projectile[i], projectileLifeTime);
                addPosition += new Vector3(2, 0, 0);
                //Destroy(projectile2, projectileLifeTime);
            }
            float timeToNextProjectile =
                Random.Range(baseFiringRate - firingRateVariance, baseFiringRate + firingRateVariance);

            timeToNextProjectile = Mathf.Clamp(timeToNextProjectile, minimumFiringRate, float.MaxValue);
            
            yield return new WaitForSeconds(timeToNextProjectile);
        }
    }
}
