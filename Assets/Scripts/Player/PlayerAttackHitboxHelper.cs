using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackHitboxHelper : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [Range(0.01f,0.2f)] [SerializeField] float maxTimeActive;

    float activeTimer;

    BoxCollider2D _boxCol2D;
    private void Awake()
    {
        _boxCol2D = GetComponent<BoxCollider2D>();
    }
    private void Start()
    {
        activeTimer = 0f;
    }
    private void Update()
    {
        activeTimer += Time.deltaTime;
        if (activeTimer > maxTimeActive)
        {
            activeTimer = 0f;
            gameObject.SetActive(false);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            //call player controller that we hit something
            Debug.Log("we hit enemy!");

        }
    }
}
