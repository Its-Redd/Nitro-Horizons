using UnityEngine;

public class CarSounds : MonoBehaviour
{
    public float minSpeed = 0.0f;
    public float maxSpeed = 100.0f;

    public float minPitch = 0.5f;
    public float maxPitch = 3.0f;
    private float pitch;

    private Rigidbody carRb;
    private AudioSource engineSound;

    void Start()
    {
        carRb = GetComponent<Rigidbody>();
        engineSound = GetComponent<AudioSource>();
    }

    void Update()
    {
        EngineSound();
    }

    void EngineSound()
    {
        if (carRb.linearVelocity.magnitude < minSpeed)
        {
            engineSound.pitch = minPitch;
        }
        if (carRb.linearVelocity.magnitude > maxSpeed)
        {
            engineSound.pitch = maxPitch;
        }
        else if (carRb.linearVelocity.magnitude > 0.0f && carRb.linearVelocity.magnitude < minSpeed)
        {
            engineSound.pitch = minPitch;
        }
        else
        {
            pitch = Mathf.Lerp(minPitch, maxPitch, carRb.linearVelocity.magnitude / maxSpeed);
        }
        engineSound.pitch = pitch;
    }




}
