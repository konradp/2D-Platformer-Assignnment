using System;
using System.Collections;
using UnityEngine;


public class EnemyController : MonoBehaviour, ICoinAcquireable
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
    bool _isGoingRight, _isAlive, _detectedPlayerInRange;
    Vector2 _refVelocity = Vector2.zero;

    //cache
    Vector3 _relativeVector;
    Vector2 _desMove;
    Transform _detectedPlayer;

    #endregion

    #region Game Logic
    private void Start()
    {
        _curHP = hitPoints;
        _curCheckpoint = 0;
        _isAlive = true;
        _rb2D = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        if(canAttackPlayer && _detectedPlayerInRange)
        {
            MoveTowardsPlayer();
            UpdateAnimations();
        }
        else
        {
            switch (enemyState)
            {
                case EnemyBehaviourStates.StandingStill:
                    //do nothing, left as switch-case for easier future expansion
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
    }
    private void MoveTowardsPlayer()
    {
        if (_isAlive)
        {
            MoveTowardsPosition(_detectedPlayer.position);
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
    bool IsNearCheckpoint(Vector2 curPos, Vector2 desPos)
    {
        //note: only taking X value because we won't have vertical movement
        //rounding to first decimal place to compromise between accuarcy and cpu usage
        int _pos1 = Convert.ToInt32(Mathf.Round(curPos.x * 10f));
        int _pos2 = Convert.ToInt32(Mathf.Round(desPos.x * 10f));
        if (_pos1==_pos2)
            return true;
        else
            return false;
    }
    private void MoveTowardsCheckpoint()
    {
        if (_isAlive)
        {
            MoveTowardsPosition(patrolPoints[_curCheckpoint].position);
        }
    }
    private void MoveTowardsPosition(Vector3 desPos)
    {
        //getting vector towards checkpoint/player, relative to current position
        _relativeVector = transform.InverseTransformPoint(desPos).normalized;
        _desMove = new Vector2(_relativeVector.x * moveSpeed, _rb2D.velocity.y);
        _isGoingRight = _relativeVector.x > 0 ? true : false;
        _rb2D.velocity = Vector2.SmoothDamp(_rb2D.velocity, _desMove, ref _refVelocity, movementSmoothing, maxSpeed);
    }
    public void GotHit()
    {
        _curHP--;
        if (_curHP == 0)
            KillEntity();
        else
            animator.SetTrigger("GotHitTrigger");
    }
    void KillEntity()
    {
        animator.SetTrigger("DeathTrigger");
        animator.SetBool("IsDead", true);
        _isAlive = false;
        gameObject.tag = "DeadEnemy";
        gameObject.layer = LayerMask.NameToLayer("DeadEnemy");
        GameManager.instance.OnEnemyDestroyed();
        StartCoroutine(nameof(DestroyAfterTime));
    }
    IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(1.5f);
        GameManager.instance.SpawnCoin(transform);
        Destroy(this.gameObject);
    }
    public void OnPlayerInRange(Transform player)
    {
        _detectedPlayer = player;
        _detectedPlayerInRange = true;
    }
    public void OnPlayerOutOfRange()
    {
        _detectedPlayer = null;
        _detectedPlayerInRange = false;
        animator.SetFloat("Speed", 0); //animation fix
    }
    #endregion
}
