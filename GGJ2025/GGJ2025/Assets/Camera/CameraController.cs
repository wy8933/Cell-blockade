using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// This isn't currently used
/// </summary>
public class CameraController : MonoBehaviour
{
    //[SerializeField] private Camera cam;

    //[SerializeField] private Transform customCursor;

    [SerializeField] private bool cutscene = false;

    [SerializeField] private bool isPaused = false;

    [Header("Thresholds")]
    [SerializeField] private float xThreshold;
    [SerializeField] private float yThreshold;

}
