using UnityEngine;

public class EnemyHitDetection : MonoBehaviour
{
    [SerializeField] EnemyController enemyController;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerSword"))
        {
            enemyController.GotHit();
        }
    }
}
