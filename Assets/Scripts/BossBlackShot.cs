using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBlackShot : MonoBehaviour
{
    private EnemyStats enemyStats;
    private bool shoot = false;
    private Vector3 dir;
    private float elapsedTime = 0.0f;

    void Start() {
        enemyStats = GetComponent<EnemyStats>();
    }
    // Update is called once per frame
    void Update()
    {
        if(shoot) {
            elapsedTime += Time.deltaTime;
            transform.position += dir * enemyStats.moveSpeed * Time.deltaTime;
            if(elapsedTime > enemyStats.maxHealth) {
                Destroy(gameObject);
            }
        }
    }

    public void SetDirection(Vector3 direction, float angle) {
        dir = direction;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public void Shoot() {shoot = true;}
}
