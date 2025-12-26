using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject ARCamera;
    public GameObject Hidden;
    public GameObject Visible;
    public enum CameraState
    {
        ACTIVE,
        INACTIVE
    }

    public CameraState currentCameraState;

    public GameObject settingsPanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ARCamera.SetActive(false);
        Hidden.SetActive(true);
        Visible.SetActive(false);
        settingsPanel.SetActive(false);

        currentCameraState = CameraState.INACTIVE;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ActivateARCamera()
    {
        ARCamera.SetActive(true);
    }

    public void DeactivateARCamera()
    {
        ARCamera.SetActive(false);
    }

    public void StartScanning()
    {
        Hidden.SetActive(false);
        Visible.SetActive(true);
        ActivateARCamera();

        currentCameraState = CameraState.ACTIVE;

        Debug.Log("Scanning started...");
    }

    public void TargetFound()
    {
        Debug.Log("Target Found!");
    }

    public void ShowSettings()
    {
        if (settingsPanel.activeSelf == false)
        {
            settingsPanel.SetActive(true);
        }
        else
        {
            settingsPanel.SetActive(false);
        }
    }
}
