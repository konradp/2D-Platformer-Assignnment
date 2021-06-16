using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackHitboxHelper : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [Range(0.01f,0.2f)] [SerializeField] float maxTimeActive;

    float _activeTimer;
    bool _hasHitEnemy;

    private void Awake()
    {
        _hasHitEnemy = false;
        _activeTimer = 0f;
    }

    private void Update()
    {
        _activeTimer += Time.deltaTime;
        if (_activeTimer > maxTimeActive)
        {
            _activeTimer = 0f;
            _hasHitEnemy = false;
            gameObject.SetActive(false);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !_hasHitEnemy)
        {
            _hasHitEnemy = true; //this could make problems when we try to hit multiple enemies at once
                                 //however without it it detects 2 hits at once
        }
    }


}
