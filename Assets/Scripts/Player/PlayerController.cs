using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Serialized Variables

    [SerializeField] Animator playerAnimator;
    [SerializeField] Transform GFXtransform;
    [SerializeField] GameObject attackHitbox;
    [Range(0.1f, 2f)] [SerializeField] float attackDelay;
    [Range(0.1f, 2f)] [SerializeField] float damageImmunityDelay;
    [Range(50,200)][SerializeField] float maxSpeed;
    [Range(3,16)] [SerializeField] float moveSpeed;
    [Range(1000,2500)] [SerializeField] float jumpForce;
    [Range(100,1000)] [SerializeField] float enemyPushBackForce;
    [SerializeField] float movementSmoothing;

    #endregion

    #region Variables
    //references
    Rigidbody2D _rb2D;
    GameManager _gameManager;

    //private vars
    float _animSpeed;
    int _curHp;
    Vector2 _refVelocity = Vector2.zero;
    bool _isFacingRight, _isGrounded, _isTouchingWall, _canJump, _canAttack, _canTakeDamage, _animOnGroundFlag;

    //input vars
    float _movement;
    bool _jumpKey, _attackKey;

    //cached vars
    Vector2 _desMove;

    //timers
    float _jumpTimer, _jumpTimerMax = 0.3f;
    float _attackTimer, _attackTimerMax;
    float _hitImmunityTimer, _hitImmunityTimerMax;
    bool _jumpTimerEnabled, _attackTimerEnabled, _hitImmunityTimerEnabled, _attackFlag;

    //getters
    public int GetHP { get => _curHp; }
    #endregion

    #region Game Logic
    public void OnCreateFromGameManager(int maxHP, GameManager gameManager)
    {
        _curHp = maxHP;
        _gameManager = gameManager;
    }
    private void Start()
    {
        _rb2D = GetComponent<Rigidbody2D>();
        _isFacingRight = true;
        _canJump = _canAttack = _canTakeDamage = true;
        SetUpTimers();
        SetUpFlags();
    }
    private void SetUpTimers()
    {
        _attackTimerMax = attackDelay;
        _hitImmunityTimerMax = damageImmunityDelay;
        _jumpTimer = _attackTimer = _hitImmunityTimer = 0f;
        _jumpTimerEnabled = _attackTimerEnabled = _hitImmunityTimerEnabled = false;
    }
    private void SetUpFlags()
    {
        _attackFlag = false;
        _animOnGroundFlag = false;
    }

    private void Update()
    {
        AssignMovement();
        UpdateAnimator();
        CheckIfShouldFlipGFX();
        PlayerTimers();
    }
    private void AssignMovement()
    {
        _movement = Input.GetAxis("Horizontal");
        _jumpKey = Input.GetButton("Jump");
        _attackKey = Input.GetButtonDown("Fire1");
        //using flag, because of the difference between physics and game logic clock. 
        if (_attackKey)
            _attackFlag = true;
    }
    private void UpdateAnimator()
    {
        _animSpeed = Mathf.Abs(_movement); //todo: make it not reliable on just input
        playerAnimator.SetFloat("Speed", _animSpeed);
    }
    private void CheckIfShouldFlipGFX()
    {
        //if we try to move right but facing left
        if (_movement > 0f && !_isFacingRight)
            FlipGFXandHitbox();
        //or we try to move left, and player is facing right
        else if (_movement < 0 && _isFacingRight)
            FlipGFXandHitbox();
    }
    private void FlipGFXandHitbox()
    {
        _isFacingRight = !_isFacingRight;
        GFXtransform.localScale = new Vector3(GFXtransform.localScale.x * -1, 1, 1);
        attackHitbox.transform.localScale = new Vector3(attackHitbox.transform.localScale.x * -1, 1, 1);
    }
    private void PlayerTimers()
    {
        if (_jumpTimerEnabled)
        {
            _jumpTimer += Time.deltaTime;
            if (_jumpTimer > _jumpTimerMax)
            {
                _canJump = true;
                _jumpTimerEnabled = false;
                _jumpTimer = 0f;
            }
        }
        if (_attackTimerEnabled)
        {
            _attackTimer += Time.deltaTime;
            if (_attackTimer > _attackTimerMax)
            {
                _canAttack = true;
                _attackTimerEnabled = false;
                _attackTimer = 0f;
            }
        }
        if (_hitImmunityTimerEnabled)
        {
            _hitImmunityTimer += Time.deltaTime;
            if(_hitImmunityTimer > _hitImmunityTimerMax)
            {
                _canTakeDamage = true;
                _hitImmunityTimerEnabled = false;
                _hitImmunityTimer = 0f;
            }
        }
    }

    private void FixedUpdate()
    {
        MoveCharacter();
        TryToJump();
        TryToAttack();
    }
    private void MoveCharacter()
    {
        _desMove = new Vector2(_movement * moveSpeed, _rb2D.velocity.y);
        _rb2D.velocity = Vector2.SmoothDamp(_rb2D.velocity, _desMove, ref _refVelocity, movementSmoothing,maxSpeed);
    }
    private void TryToJump()
    {
        if (_jumpKey && (_isGrounded || _isTouchingWall) && _canJump)
        {
            _canJump = false;
            _jumpTimerEnabled = true;
            _isGrounded = _isTouchingWall = false;
            _rb2D.AddForce(new Vector2(0f, jumpForce * 10f));
            playerAnimator.SetTrigger("JumpTrigger");
        }
    }
    private void TryToAttack()
    {
        if (_canAttack && _attackFlag)
        {
            _canAttack = false;
            _attackFlag = false;
            _attackTimerEnabled = true;
            attackHitbox.SetActive(true);
            playerAnimator.SetTrigger("AttackTrigger");
        }
    }
    public void ChangeGroundedStatus(bool isGrounded)
    {
        //todo: make it better and more readable
        if(isGrounded && !_animOnGroundFlag)
        {
            playerAnimator.SetTrigger("StoodOnGroundTrigger");
            _animOnGroundFlag = true;
        }else if(!isGrounded && _animOnGroundFlag)
        {
            playerAnimator.ResetTrigger("StoodOnGroundTrigger");
            _animOnGroundFlag = false;
        }

        _isGrounded = isGrounded;
    }
    public void ChangeWallStatus(bool isTouchingWall)
    {
        _isTouchingWall = isTouchingWall;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy") && _canTakeDamage)
        {
            Debug.Log("we got hit by enemy");
            _canTakeDamage = false;
            _hitImmunityTimerEnabled = true;
            _curHp--;
            _gameManager.OnHitPointChange();
            _rb2D.AddForce((collision.relativeVelocity + new Vector2(0,2f)) * enemyPushBackForce * 10f);
            if (_curHp == 0)
            {
                //make fancy stuff
                _gameManager.OnPlayerKilled();
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Pickable"))
        {
            _gameManager.OnCoinCollected();
            Destroy(collision.gameObject);
        }
    }

    #endregion
}
