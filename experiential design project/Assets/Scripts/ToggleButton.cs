using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButton : MonoBehaviour
{
    [Header("Button Sprites")]
    [SerializeField] private Sprite spriteOn;
    [SerializeField] private Sprite spriteOff;
    [SerializeField] private GameObject TargetObject;
    [SerializeField] private GameObject CameraManagerObject;
    private CameraManager CameraManagerScript;

    private Image buttonImage;
    

    public enum ButtonState
    {
        ON,
        OFF
    }

    private ButtonState currentState;

    void Awake()
    {
        buttonImage = GetComponent<Image>();

        if (buttonImage == null)
        {
            Debug.Log("Image component not found on this GameObject!");
        }

        CameraManagerScript = CameraManagerObject.GetComponent<CameraManager>();

        if (CameraManagerScript == null)
        
        {
            Debug.Log("Camera Manager Script not found on the referenced GameObject!");
        }
    }

    void Start()
    {
        currentState = ButtonState.OFF;
        buttonImage.sprite = spriteOff;

        TargetObject.SetActive(true);

        
    }

    public void ToggleStateSpecial()
    {
        if (currentState == ButtonState.OFF)
        { //Hides the object
            currentState = ButtonState.ON;
            buttonImage.sprite = spriteOn;

            TargetObject.SetActive(false);
        }
        else
        { //Shows the object
            if (TargetObject.activeSelf == false && CameraManagerScript.currentCameraState == CameraManager.CameraState.INACTIVE) // if object is already hidden, and the user hasnt started scanning
            {
                currentState = ButtonState.OFF;
                buttonImage.sprite = spriteOff;
                Debug.Log("Object still hidden since scanning hasn't started.");
            }
            else
            {
                currentState = ButtonState.OFF;
                buttonImage.sprite = spriteOff;

                TargetObject.SetActive(true);
            }
        }
    }

    public void ToggleState()
    {
        if (currentState == ButtonState.OFF)
        { //Hides the object
            currentState = ButtonState.ON;
            buttonImage.sprite = spriteOn;

            TargetObject.SetActive(false);
        }
        else
        { //Shows the object
            
            currentState = ButtonState.OFF;
            buttonImage.sprite = spriteOff;

            TargetObject.SetActive(true);
            
        }
    }
}
