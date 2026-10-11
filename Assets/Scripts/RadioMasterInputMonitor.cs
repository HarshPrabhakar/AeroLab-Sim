
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class RadioMasterInputMonitor : MonoBehaviour
{
    private Vector2 scrollPosition;

    void OnGUI()
    {
        GUILayout.BeginArea(
            new Rect(10, 10, 420, 650),
            GUI.skin.box
        );

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        GUILayout.Label("AEROLAB SIM - TX16S INPUT DEBUG");
        GUILayout.Space(8);

        if (Joystick.all.Count == 0)
        {
            GUILayout.Label("No joystick detected.");
        }

        foreach (Joystick joystick in Joystick.all)
        {
            GUILayout.Label("Device: " + joystick.displayName);
            GUILayout.Space(5);

            foreach (InputControl control in joystick.allControls)
            {
                if (control is AxisControl ||
                    control is ButtonControl)
                {
                    GUILayout.Label(
                        control.name + " = " +
                        control.ReadValueAsObject()
                    );
                }
            }

            GUILayout.Space(10);
        }

        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }
}
