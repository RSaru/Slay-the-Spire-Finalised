using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform spawnPoint;
    public float fireRate = 2f;
    public float startDelay = 1f;

    private float nextFireTime;

    //initializes the next fire time
    private void Start()
    {
        nextFireTime = Time.time + startDelay;
    }

    //fires a projectile at set intervals
    private void Update()
    {
        if (Time.time >= nextFireTime)
        {
            SpawnProjectile();
            nextFireTime = Time.time + fireRate;
        }
    }

    //spawns a projectile at the spawn point
    private void SpawnProjectile()
    {
        Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);
    }
}
