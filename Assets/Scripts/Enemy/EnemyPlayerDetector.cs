using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlayerDetector : MonoBehaviour
{
    [SerializeField] EnemyController enemyController;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            enemyController.OnPlayerInRange(collision.transform);
            Debug.Log("Found Player");
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            enemyController.OnPlayerOutOfRange();
            Debug.Log("Lost Player");

        }
    }
}