using UnityEngine;

public class HidingController : MonoBehaviour
{
    [SerializeField] InputHandler _inputHandler;
    [SerializeField] CharacterController _characterController;
    private float _originalHeight;
    private float _originalWidth;
    public bool isHidden;

    private void OnEnable()
    {
        GetComponent<InputHandler>().CrouchTriggeredCallback += Hide;
        Debug.Log("Subscribed hide function to Crouch Triggered");
    }

    private void OnDisable()
    {
        GetComponent<InputHandler>().CrouchTriggeredCallback -= Hide;
        Debug.Log("Unsubscribed hide function to Crouch Triggered");
    }

    private void Awake()
    {
        _inputHandler = GetComponent<InputHandler>();
        _characterController = GetComponent<CharacterController>();
        _originalHeight = _characterController.height;
        _originalWidth = _characterController.radius;
    }

    // Make player able to fit under hiding spots
    private void Hide()
    {
        if (_inputHandler.CrouchingActive())
        {
            _characterController.height /= 2;
            _characterController.radius = 0.2f;
        }
        
    }

    private void Update()
    {
        // Reset controller values if no longer hiding
        if (!_inputHandler.CrouchingActive() && !isHidden)
        {
            _characterController.height = _originalHeight;
            _characterController.radius = _originalWidth;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("HidingSpot"))
        {
            isHidden = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("HidingSpot"))
        {
            isHidden = false;
        }
    }
}
