using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyController : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] bool canAttackPlayer;
    [SerializeField] EnemyBehaviourStates enemyState;
    [Range(1, 5)] [SerializeField] int hitPoints;
    [SerializeField] Transform[] patrolPoints;
    [Range(3, 16)] [SerializeField] float moveSpeed;
    [Range(20, 120)] [SerializeField] float maxSpeed;
    [SerializeField] float movementSmoothing;
    [SerializeField] Transform GFXTransform;
    [SerializeField] Animator animator;
    #endregion

    #region Variables
    //ref
    Rigidbody2D _rb2D;
    
    //private vars
    int _curHP, _curCheckpoint;
    bool _isGoingRight;
    Vector2 _refVelocity = Vector2.zero;

    //cache
    Vector3 _relativeVector;
    Vector2 _desMove;
    #endregion

    #region Game Logic
    private void Start()
    {
        _curHP = hitPoints;
        _curCheckpoint = 0;
        _rb2D = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        switch (enemyState)
        {
            case EnemyBehaviourStates.StandingStill:
                break;
            case EnemyBehaviourStates.Patrolling:
                //if we don't have any patrol points, bail
                if (patrolPoints.Length == 0)
                {
                    Debug.LogWarning($"No patrol points for {gameObject.name}, switching state");
                    enemyState = EnemyBehaviourStates.StandingStill;
                    break;
                }
                MoveTowardsCheckpoint();
                CheckpointCheck();
                UpdateAnimations();
                break;
        }
    }

    private void UpdateAnimations()
    {
        animator.SetFloat("Speed", Mathf.Abs(_desMove.x));
        if (_isGoingRight)
            GFXTransform.localScale = new Vector3(1, 1, 1);
        else
            GFXTransform.localScale = new Vector3(-1, 1, 1);
    }
    private void CheckpointCheck()
    {
        if (IsNearCheckpoint(transform.position, patrolPoints[_curCheckpoint].position))
        {
            _curCheckpoint = (_curCheckpoint + 1) % patrolPoints.Length;
        }
    }
    bool IsNearCheckpoint(Vector2 enemyPos, Vector2 checkpointPos)
    {
        //note: only taking X value because we won't have vertical movement
        int _pos1 = Convert.ToInt32(Mathf.Round(enemyPos.x * 10f));
        int _pos2 = Convert.ToInt32(Mathf.Round(checkpointPos.x * 10f));
        if (_pos1==_pos2)
            return true;
        else
            return false;
    }
    private void MoveTowardsCheckpoint()
    {
        //getting vector towards checkpoint, relative to current position
        _relativeVector = transform.InverseTransformPoint(patrolPoints[_curCheckpoint].position).normalized;
        _desMove = new Vector2(_relativeVector.x * moveSpeed, _rb2D.velocity.y);
        _isGoingRight = _relativeVector.x > 0 ? true : false;
        _rb2D.velocity = Vector2.SmoothDamp(_rb2D.velocity, _desMove, ref _refVelocity, movementSmoothing, maxSpeed);
    }
    public void GotHit()
    {
        _curHP--;
        if (_curHP == 0)
        {
            KillEntity();
        }
    }
    void KillEntity()
    {
        animator.SetTrigger("DeathTrigger");
        StartCoroutine(nameof(DestroyAfterTime));
    }
    IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(2f);
        GameManager.instance.SpawnCoin(transform);
        Destroy(this.gameObject);
    }
    #endregion
}
