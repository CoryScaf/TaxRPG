using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGreenShot : MonoBehaviour
{
    public float rotationSpeed = 1.0f;
    public float distanceTillUnlock = 1.0f;
    private EnemyStats enemyStats;
    private bool shoot = false;
    private Transform target;
    private float elapsedTime = 0.0f;
    private bool unlocked = false;
    private Vector3 finalVelocity;

    void Start() {
        enemyStats = GetComponent<EnemyStats>();
    }
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0,0,rotationSpeed));
        if(shoot) {
            elapsedTime += Time.deltaTime;
            if(Vector2.Distance(transform.position, target.position) < distanceTillUnlock ) {
                unlocked = true;
                finalVelocity = (target.position - transform.position).normalized * enemyStats.moveSpeed * Time.deltaTime;
            }
            if(!unlocked)
                transform.position += (target.position - transform.position).normalized * enemyStats.moveSpeed * Time.deltaTime;
            else
                transform.position += finalVelocity;
            if(elapsedTime > enemyStats.maxHealth) {
                Destroy(gameObject);
            }
        }
    }

    public void SetTarget(Transform tar) {
        target = tar;
    }

    public void Shoot() {shoot = true;}
}
