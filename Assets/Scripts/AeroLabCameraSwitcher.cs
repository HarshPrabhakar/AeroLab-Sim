
using UnityEngine;
using UnityEngine.InputSystem;

public class AeroLabCameraSwitcher : MonoBehaviour
{
    [SerializeField] private Camera fpvCamera;
    [SerializeField] private Camera chaseCamera;
    [SerializeField] private Transform drone;

    [Header("Chase Camera")]
    [SerializeField] private Vector3 chaseOffset = new Vector3(0f, 2f, -5f);
    [SerializeField] private float followSmoothness = 8f;

    private bool chaseMode;

    private void Start()
    {
        if (drone == null)
        {
            Debug.LogError("AeroLabCameraSwitcher: Assign the Drone Transform.");
            enabled = false;
            return;
        }

        if (fpvCamera == null || chaseCamera == null)
        {
            Debug.LogError("AeroLabCameraSwitcher: Assign both cameras.");
            enabled = false;
            return;
        }

        chaseMode = false;
        fpvCamera.enabled = true;
        chaseCamera.enabled = false;
    }

    private void LateUpdate()
    {
        if (Keyboard.current != null &&
            Keyboard.current.cKey.wasPressedThisFrame)
        {
            chaseMode = !chaseMode;
            fpvCamera.enabled = !chaseMode;
            chaseCamera.enabled = chaseMode;
        }

        if (chaseMode)
        {
            Vector3 targetPosition =
                drone.TransformPoint(chaseOffset);

            chaseCamera.transform.position = Vector3.Lerp(
                chaseCamera.transform.position,
                targetPosition,
                followSmoothness * Time.deltaTime
            );

            chaseCamera.transform.LookAt(drone.position);
        }
    }
}
