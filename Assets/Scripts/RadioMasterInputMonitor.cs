
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class RadioMasterInputMonitor : MonoBehaviour
{
    private Vector2 scrollPosition;

    private void OnGUI()
    {
        const float panelWidth = 350f;
        const float panelHeight = 520f;
        const float margin = 10f;

        float panelX = Mathf.Max(
            margin, Screen.width - panelWidth - margin
        );

        GUILayout.BeginArea(
            new Rect(panelX, margin, panelWidth, panelHeight),
            GUI.skin.box
        );

        GUILayout.Label("AEROLAB SIM - RAW TX16S INPUTS");
        GUILayout.Space(5);

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        if (Joystick.all.Count == 0)
        {
            GUILayout.Label("No joystick detected.");
        }
        else
        {
            foreach (Joystick joystick in Joystick.all)
            {
                GUILayout.Label("Device: " + joystick.displayName);
                GUILayout.Space(5);
                GUILayout.Label("AXES - move one stick at a time");

                foreach (InputControl control in joystick.allControls)
                {
                    if (control is AxisControl axis)
                    {
                        GUILayout.Label(
                            control.name + " : " +
                            axis.ReadValue().ToString("F3")
                        );
                    }
                }

                GUILayout.Space(10);
                GUILayout.Label("BUTTONS");

                foreach (InputControl control in joystick.allControls)
                {
                    if (control is ButtonControl button)
                    {
                        GUILayout.Label(
                            control.name + " : " +
                            button.ReadValue().ToString("F2")
                        );
                    }
                }

                GUILayout.Space(12);
            }
        }

        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }
}
