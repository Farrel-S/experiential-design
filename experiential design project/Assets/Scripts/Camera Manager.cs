using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject ARCamera;
    public GameObject Hidden;
    public GameObject Visible;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ARCamera.SetActive(false);
        Hidden.SetActive(true);
        Visible.SetActive(false);
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
        Debug.Log("Scanning started...");
    }
    

}
