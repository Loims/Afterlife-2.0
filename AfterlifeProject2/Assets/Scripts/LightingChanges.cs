using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class LightingChanges : MonoBehaviour
{
    [SerializeField] private RenderSettings sceneSettings;

    private PlayerMovement movementComp;

    private PlayerMovement.State currentState;

    private bool whaleSettingsSet = false;
    private bool planeSettingsSet = false;
    private bool flareSettingsSet = false;

    void OnEnable()
    {
        sceneSettings = Object.FindObjectOfType<RenderSettings>();

        movementComp = GameObject.Find("Gameplay Plane").GetComponentInChildren<PlayerMovement>();

        currentState = movementComp.playerState;
    }

    // Update is called once per frame
    void Update()
    {
        currentState = movementComp.playerState;

        switch (currentState)
        {
            case PlayerMovement.State.WHALE:
                if (!whaleSettingsSet)
                {
                    WhaleSettings();
                    whaleSettingsSet = true;
                }
                break;

            case PlayerMovement.State.PLANE:
                if (!planeSettingsSet)
                {
                    PlaneSettings();
                    planeSettingsSet = true;
                }
                break;

            case PlayerMovement.State.FLARE:
                if (!flareSettingsSet)
                {
                    FlareSettings();
                    flareSettingsSet = true;
                }
                break;
        }
    }

    private void WhaleSettings()
    {
        Color ambientLightColor;
        Color fogColor;
        Color backgroundColor;
        ColorUtility.TryParseHtmlString("#0D72BA", out ambientLightColor);
        ColorUtility.TryParseHtmlString("#007CC3", out fogColor);
        ColorUtility.TryParseHtmlString("#007BDC", out backgroundColor);

        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientLight = ambientLightColor;

        RenderSettings.fog = true;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogDensity = 0.025f;

        Camera.main.backgroundColor = backgroundColor;
    }

    private void PlaneSettings()
    {
        Color ambientLightColor;
        Color fogColor;
        Color backgroundColor;
        ColorUtility.TryParseHtmlString("#A192C6", out ambientLightColor);
        ColorUtility.TryParseHtmlString("#B6A0FF", out fogColor);
        ColorUtility.TryParseHtmlString("#AA94F9", out backgroundColor);

        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientLight = ambientLightColor;

        RenderSettings.fog = true;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogDensity = 0.012f;

        Camera.main.backgroundColor = backgroundColor;
    }

    private void FlareSettings()
    {
        Color ambientLightColor;
        Color fogColor;
        Color backgroundColor;
        ColorUtility.TryParseHtmlString("#000000", out ambientLightColor);
        ColorUtility.TryParseHtmlString("#000000", out fogColor);
        ColorUtility.TryParseHtmlString("#000000", out backgroundColor);

        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientLight = ambientLightColor;

        RenderSettings.fog = true;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogDensity = 0.012f;

        Camera.main.backgroundColor = backgroundColor;
    }
}
