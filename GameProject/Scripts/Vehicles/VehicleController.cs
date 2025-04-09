using UnityEngine;

public class VehicleController : MonoBehaviour
{
    [Header("Vehicle Properties")]
    public float maxSpeed = 10f;
    public float acceleration = 20f;
    public float brakeForce = 30f;
    public float turnSpeed = 180f;
    public float groundCheckDistance = 0.2f;
    public LayerMask groundLayer;
    public Transform[] wheelTransforms;
    public SpriteRenderer vehicleRenderer;
    
    [Header("Vehicle Types")]
    public VehicleType vehicleType;
    public Sprite[] vehicleSprites;
    public AudioClip engineSound;
    public AudioClip hornSound;
    
    private Rigidbody2D rb;
    private AudioSource audioSource;
    private float currentSpeed;
    private bool isOccupied;
    private DogController currentDriver;
    
    public enum VehicleType
    {
        Scooter,
        Car,
        Bicycle,
        Skateboard
    }
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        SetupVehicle();
    }
    
    void SetupVehicle()
    {
        // Set vehicle sprite based on type
        if (vehicleSprites != null && vehicleSprites.Length > (int)vehicleType)
        {
            vehicleRenderer.sprite = vehicleSprites[(int)vehicleType];
        }
        
        // Adjust physics properties based on vehicle type
        switch (vehicleType)
        {
            case VehicleType.Scooter:
                maxSpeed = 8f;
                acceleration = 15f;
                turnSpeed = 200f;
                break;
            case VehicleType.Car:
                maxSpeed = 12f;
                acceleration = 25f;
                turnSpeed = 150f;
                break;
            case VehicleType.Bicycle:
                maxSpeed = 7f;
                acceleration = 12f;
                turnSpeed = 180f;
                break;
            case VehicleType.Skateboard:
                maxSpeed = 6f;
                acceleration = 10f;
                turnSpeed = 220f;
                break;
        }
    }
    
    void Update()
    {
        if (!isOccupied) return;
        
        // Handle input
        float moveInput = Input.GetAxis("Horizontal");
        float accelerationInput = Input.GetAxis("Vertical");
        
        // Apply movement
        if (accelerationInput > 0)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, acceleration * Time.deltaTime);
        }
        else if (accelerationInput < 0)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, -maxSpeed/2, brakeForce * Time.deltaTime);
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, brakeForce/2 * Time.deltaTime);
        }
        
        // Rotate vehicle
        if (Mathf.Abs(currentSpeed) > 0.1f)
        {
            float rotation = moveInput * turnSpeed * Time.deltaTime * Mathf.Sign(currentSpeed);
            transform.Rotate(0, 0, -rotation);
        }
        
        // Update wheel rotations
        if (wheelTransforms != null)
        {
            float wheelRotation = currentSpeed * Time.deltaTime * 360f / (2f * Mathf.PI * 0.5f);
            foreach (Transform wheel in wheelTransforms)
            {
                wheel.Rotate(0, 0, wheelRotation);
            }
        }
        
        // Play engine sound
        if (audioSource != null && engineSound != null)
        {
            audioSource.pitch = Mathf.Lerp(0.5f, 1.5f, Mathf.Abs(currentSpeed/maxSpeed));
        }
        
        // Horn
        if (Input.GetKeyDown(KeyCode.H) && hornSound != null)
        {
            audioSource.PlayOneShot(hornSound);
        }
    }
    
    void FixedUpdate()
    {
        if (!isOccupied) return;
        
        // Apply movement in local forward direction
        Vector2 moveDirection = transform.right * currentSpeed;
        rb.velocity = moveDirection;
    }
    
    public void EnterVehicle(DogController dog)
    {
        if (isOccupied) return;
        
        isOccupied = true;
        currentDriver = dog;
        dog.gameObject.SetActive(false);
        dog.transform.SetParent(transform);
        dog.transform.localPosition = Vector3.zero;
        
        // Start engine sound
        if (audioSource != null && engineSound != null)
        {
            audioSource.clip = engineSound;
            audioSource.loop = true;
            audioSource.Play();
        }
    }
    
    public void ExitVehicle()
    {
        if (!isOccupied) return;
        
        isOccupied = false;
        currentDriver.gameObject.SetActive(true);
        currentDriver.transform.SetParent(null);
        currentDriver.transform.position = transform.position + transform.right * 1.5f;
        currentDriver = null;
        currentSpeed = 0;
        
        // Stop engine sound
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isOccupied && other.CompareTag("Player"))
        {
            DogController dog = other.GetComponent<DogController>();
            if (dog != null)
            {
                UIManager.Instance.ShowMessage("PRESS_E_TO_ENTER_VEHICLE");
            }
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (!isOccupied && other.CompareTag("Player"))
        {
            UIManager.Instance.HideMessage();
        }
    }
}
