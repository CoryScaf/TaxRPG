using UnityEngine;

public class BombDroppingEnemy : EnemyBehavior
{
    public GameObject bombPrefab; // Drag and drop your bomb prefab here

    new public void Start()
    {
        base.Start();
        enemyStats.OnDeath += DropBomb;
    }

    void DropBomb()
    {
        Instantiate(bombPrefab, transform.position, Quaternion.identity);
    }

    private void OnDestroy()
    {
        enemyStats.OnDeath -= DropBomb; // Make sure to unsubscribe to prevent any potential memory leaks
    }
}