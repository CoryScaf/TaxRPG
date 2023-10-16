using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGreenShot : MonoBehaviour
{
    public float rotationSpeed = 1.0f;
    public float distanceTillUnlock = 1.0f;
    public float perfectSoundDistance = 3.0f;
    public float maxSoundDistance = 6.0f;
    private EnemyStats enemyStats;
    private bool shoot = false;
    private Transform target;
    private float elapsedTime = 0.0f;
    private bool unlocked = false;
    private Vector3 finalVelocity;
    private AudioSource spinSound;

    void Start() {
        enemyStats = GetComponent<EnemyStats>();
        spinSound = GetComponent<AudioSource>();
    }
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0,0,rotationSpeed));
        if(target != null && !spinSound.isPlaying) {
            float dist = Vector2.Distance(transform.position, target.position);
            float volume = perfectSoundDistance / dist;
            if(volume > 1.0f) volume = 1.0f;
            if(dist < maxSoundDistance) {
                spinSound.volume = volume * 0.1f;
                spinSound.Play();
            }
        }
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
