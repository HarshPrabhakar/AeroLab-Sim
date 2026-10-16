
using TMPro;
using UnityEngine;

public class AeroLabHUD : MonoBehaviour
{
    [SerializeField] private TMP_Text telemetryText;

    private AeroLabDroneController drone;
    private Rigidbody droneBody;

    private void Start()
    {
        if (telemetryText == null)
            telemetryText = GetComponent<TMP_Text>();

        drone = FindFirstObjectByType<AeroLabDroneController>();

        if (drone != null)
            droneBody = drone.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (telemetryText == null)
            return;

        if (drone == null || droneBody == null)
        {
            telemetryText.text =
                "AEROLAB SIM\n\nDrone controller not found.";
            return;
        }

        Vector3 angles = drone.transform.eulerAngles;

        // Convert Unity's 0..360 degree angles to -180..180.
        float pitch = Mathf.DeltaAngle(0f, angles.x);
        float yaw = Mathf.DeltaAngle(0f, angles.y);
        float roll = Mathf.DeltaAngle(0f, angles.z);

        float speed = droneBody.linearVelocity.magnitude;
        float altitude = drone.transform.position.y;
        float throttlePercent = drone.throttle * 100f;

        telemetryText.text =
            "AEROLAB SIM - FLIGHT HUD\n" +
            "Controller: " + drone.controllerStatus + "\n" +
            "Speed: " + speed.ToString("F2") + " m/s\n" +
            "Altitude: " + altitude.ToString("F2") + " m\n" +
            "Throttle: " + throttlePercent.ToString("F0") + "%\n" +
            "Pitch: " + pitch.ToString("F1") + " deg\n" +
            "Roll: " + roll.ToString("F1") + " deg\n" +
            "Yaw: " + yaw.ToString("F1") + " deg";
    }
}
