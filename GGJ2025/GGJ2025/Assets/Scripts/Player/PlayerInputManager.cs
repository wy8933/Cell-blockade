using ObjectPoolings;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using static UnityEngine.Timeline.DirectorControlPlayable;

[RequireComponent(typeof(PlayerInput), typeof(PlayerController))]
public class PlayerInputManager : MonoBehaviour
{
    [SerializeField] private PlayerController _player;
    public PlayerInput playerInput;

    public Animator playerAnimator;

    [SerializeField] private TowerUIManager _towerUIManager;

    [Header("Input Action References")]
    [SerializeField] private InputActionReference _moveAction;
    [SerializeField] private InputActionReference _pauseAction;

    [SerializeField] private InputActionReference _primaryAction;
    [SerializeField] private InputActionReference _secondaryAction;

    //This is used for tower selection
    [SerializeField] private InputActionReference _numberAction;
    [SerializeField] private InputActionReference _scrollingAction;

    //Wave Skip
    [SerializeField] private InputActionReference _SpaceAction;
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
        _primaryAction.action.Enable();
        _pauseAction.action.Enable();

        _moveAction.action.performed += OnMovePerformed;
        _moveAction.action.canceled += OnMoveCanceled;

        _primaryAction.action.performed += OnPrimaryPreformed;
        _primaryAction.action.canceled += OnPrimaryCanceled;

        _secondaryAction.action.performed += OnSecondaryPreformed;

        _pauseAction.action.performed += OnPausePerformed;

        _numberAction.action.performed += OnNumberClick;

        _scrollingAction.action.performed += OnScroll;

        _SpaceAction.action.performed += OnWaveSkip;
    }

    /// <summary>
    /// Disable all action, and disconnect all methods
    /// </summary>
    private void OnDisable()
    {
        _moveAction.action.Disable();
        _primaryAction.action.Disable();
        _pauseAction.action.Disable();

        _moveAction.action.performed -= OnMovePerformed;
        _moveAction.action.canceled -= OnMoveCanceled;

        _primaryAction.action.performed -= OnPrimaryPreformed;
        _primaryAction.action.canceled -= OnPrimaryCanceled;

        _secondaryAction.action.performed -= OnSecondaryPreformed;

        _numberAction.action.performed -= OnNumberClick;
        
        _scrollingAction.action.performed -= OnScroll;

        _SpaceAction.action.performed -= OnWaveSkip;
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
    private void OnPrimaryPreformed(InputAction.CallbackContext context) {
        if (TowerPlacement.Instance.isBuildingMode)
        {
            TowerPlacement.Instance.StopPlacement();
            TowerPlacement.Instance.isBuildingMode = false;
        }
            
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
    private void OnPrimaryCanceled(InputAction.CallbackContext context)
    {
        _player.isShooting = false;
    }

    /// <summary>
    /// When pressed if the mode is placing a tower it will plcae a tower, and
    /// if removing a tower it will remove it
    /// </summary>
    /// <param name="context"></param>
    private void OnSecondaryPreformed(InputAction.CallbackContext context)
    {
        if (TowerPlacement.Instance.isBuildingMode)
        {
            TowerPlacement.Instance.PlaceTower();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name=""></param>
    private void OnSecondaryCanceled(InputAction.CallbackContext context)
    {

    }

    /// <summary>
    /// moved to use old input action, this causing error
    /// </summary>
    /// <param name="context"></param>
    private void OnPausePerformed(InputAction.CallbackContext context) { 
        //GameManager.Instance.Pause();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    private void OnScroll(InputAction.CallbackContext context) {
        Vector2 scroll = context.ReadValue<Vector2>();

        _towerUIManager.ScrollSelect(scroll);

        if (scroll.y > 0)
        {
            Debug.Log("scroll up");
        }
        else if (scroll.y < 0)
        {
            Debug.Log("scroll down");
        }
    }

    /// <summary>
    /// Detects the inputs from numbers 1 - 6 and calls a method from towerUIManager 
    /// </summary>
    /// <param name="context"></param>
    private void OnNumberClick(InputAction.CallbackContext context) 
    {
        TowerPlacement.Instance.isBuildingMode = true;

        Debug.Log(context.control);

        var control = context.control;

        if (control is KeyControl keyControl)
        {
           _towerUIManager.ChangeTowerUI(keyControl);
        }
    }

    private void OnWaveSkip(InputAction.CallbackContext context)
    {
        EnemyWaveManager.Instance.currenTime = 0;
        EnemyWaveManager.Instance.SkipWave();
    }
}


