using ObjectPoolings;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Timeline.DirectorControlPlayable;

[RequireComponent(typeof(PlayerInput), typeof(PlayerController))]
public class PlayerInputManager : MonoBehaviour
{
    [SerializeField] private PlayerController _player;
    public PlayerInput playerInput;

    public Animator playerAnimator;

    [Header("Input Action References")]
    [SerializeField] private InputActionReference _moveAction;
    [SerializeField] private InputActionReference _fireAction;
    [SerializeField] private InputActionReference _pauseAction;

    private void Start()
    {
        _player = GetComponent<PlayerController>();
        playerInput = GetComponent<PlayerInput>();
    }

    /// <summary>
    /// Enable all action and connect methods 
    /// </summary>
    private void OnEnable()
    {
        _moveAction.action.Enable();
        _fireAction.action.Enable();
        _pauseAction.action.Enable();

        _moveAction.action.performed += OnMovePerformed;
        _moveAction.action.canceled += OnMoveCanceled;

        _fireAction.action.performed += OnAttackPerformed;
        _fireAction.action.canceled += OnAttackCanceled;

        _pauseAction.action.performed += OnPausePerformed;
    }

    /// <summary>
    /// Disable all action, and disconnect all methods
    /// </summary>
    private void OnDisable()
    {
        _moveAction.action.Disable();
        _fireAction.action.Disable();
        _pauseAction.action.Disable();

        _moveAction.action.performed -= OnMovePerformed;
        _moveAction.action.canceled -= OnMoveCanceled;

        _fireAction.action.performed -= OnAttackPerformed;
        _fireAction.action.canceled -= OnAttackCanceled;
    }

    /// <summary>
    /// Set the move direction of player and play walk animation
    /// </summary>
    /// <param name="context"></param>
    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        _player.moveDirection = context.ReadValue<Vector2>().normalized;
        playerAnimator.SetBool("IsWalking", true);
    }

    /// <summary>
    /// Set move direction when cancel to make the direction 0,0 to stop movement
    /// </summary>
    /// <param name="context"></param>
    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        _player.moveDirection = context.ReadValue<Vector2>().normalized;
        playerAnimator.SetBool("IsWalking", false);
    }

    /// <summary>
    /// Depend on the weapon type trigger different attack behavior
    /// </summary>
    /// <param name="context"></param>
    private void OnAttackPerformed(InputAction.CallbackContext context) {
        if (_player.weaponType == WeaponType.ShotGun)
        {
            _player.Attack();
        }
        else
        {
            _player.isShooting = true;
        }
    }

    /// <summary>
    /// Set shooting state to false
    /// </summary>
    /// <param name="context"></param>
    private void OnAttackCanceled(InputAction.CallbackContext context)
    {
        _player.isShooting = false;
    }

    /// <summary>
    /// moved to use old input action, this causing error
    /// </summary>
    /// <param name="context"></param>
    private void OnPausePerformed(InputAction.CallbackContext context) { 
        //GameManager.Instance.Pause();
    }
}


