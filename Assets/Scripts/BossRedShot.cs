using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRedShot : MonoBehaviour
{
    public float originDistance = 1.0f;
    private EnemyStats enemyStats;
    private bool shoot = false;
    private bool tracking = false;
    private Transform target;
    private Transform origin;

    private Vector3 dir;
    private float elapsedTime = 0.0f;

    void Start() {
        enemyStats = GetComponent<EnemyStats>();
    }
    // Update is called once per frame
    void Update()
    {
        if(tracking) {
            Vector3 moveBy = (target.position - origin.position).normalized;
            transform.position = moveBy * originDistance + origin.position;
            float angle = Vector2.Angle(moveBy, Vector3.right)-90;
            if(moveBy.y < 0.0f) {
                angle *= -1;
                angle += 180;
            }
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        else if(shoot) {
            elapsedTime += Time.deltaTime;
            transform.position += dir * enemyStats.moveSpeed * Time.deltaTime;
            if(elapsedTime > enemyStats.maxHealth) {
                Destroy(gameObject);
            }
        }
    }

    public void SetTarget(Transform tar, Transform orig) {
        target = tar;
        origin = orig;
        tracking = true;
    }

    public void Shoot() {
        dir = (target.position - origin.position).normalized;
        tracking = false;
        shoot = true;
    }
}
