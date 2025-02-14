using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    //[SerializeField] private Camera cam;

    [SerializeField] private Transform camPos;

    [SerializeField] private Transform playerLoc;

    //[SerializeField] private Transform customCursor;

    [SerializeField] private bool cutscene = false;

    [SerializeField] private bool isPaused = false;

    [Header("Thresholds")]
    [SerializeField] private float xThreshold;
    [SerializeField] private float yThreshold;

    private void Awake()
    {

    }

    private void Update()
    {
        FollowPlayer();
    }

    private void FollowPlayer()
    {
        camPos.position = new Vector3(playerLoc.position.x, 10.0f, playerLoc.position.z - 5);
    }
}
