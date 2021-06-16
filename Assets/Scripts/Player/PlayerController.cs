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
    [Range(50,200)][SerializeField] float maxSpeed;
    [Range(3,16)] [SerializeField] float moveSpeed;
    [Range(1000,2500)] [SerializeField] float jumpForce;
    [SerializeField] float movementSmoothing;

    #endregion

    #region Variables
    //references
    Rigidbody2D _rb2D;

    //private vars
    float _animSpeed;
    Vector2 _refVelocity = Vector2.zero;
    bool _isFacingRight, _isGrounded, _isTouchingWall, _canJump, _canAttack, _animOnGroundFlag;

    //input vars
    float _movement;
    bool _jumpKey, _attackKey;

    //cached vars
    Vector2 _desMove;

    //timers
    float _jumpTimer, _jumpTimerMax = 0.3f;
    float _attackTimer, _attackTimerMax;
    bool _jumpTimerEnabled, _attackTimerEnabled, _attackFlag;
    #endregion

    #region Game Logic
    private void Start()
    {
        _rb2D = GetComponent<Rigidbody2D>();
        _isFacingRight = true;
        _canJump = _canAttack = true;
        SetUpTimers();
        SetUpFlags();
    }
    private void SetUpTimers()
    {
        _attackTimerMax = attackDelay;
        _jumpTimer = _attackTimer = 0f;
        _jumpTimerEnabled = _attackTimerEnabled = false;
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
        _animSpeed = Mathf.Abs(_movement); //todo: make it better
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

    #endregion
}
