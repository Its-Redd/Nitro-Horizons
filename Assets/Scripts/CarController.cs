using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;

public class CarController : MonoBehaviour
{
    public enum ControlMode
    {
        Keyboard,
        Buttons
    };

    public enum Axel
    {
        Front,
        Rear
    }

    [Serializable]
    public struct Wheel
    {
        public GameObject wheelModel;
        public WheelCollider wheelCollider;
        public GameObject wheelEffect;
        public GameObject Smoke;
        public Axel axel;
    }

    public AudioSource skidSound;
    public GameObject scoreSystem;
    public ControlMode control;

    public float maxAcceleration = 30.0f;
    public float maxSpeed = 100.0f;
    public float brakeAcceleration = 50.0f;

    public float turnSensitivity = 1.0f;
    public float maxSteerAngle = 30.0f;

    public Vector3 _centerOfMass;

    public List<Wheel> wheels;

    float moveInput;
    float steerInput;

    public float trailActivationSpeed = 0.5f;
    public float sidewaysTrailActivationSpeed = 0.8f;

    private Rigidbody carRb;
    private WheelHit wheelHit = new WheelHit();

    void Start()
    {
        carRb = GetComponent<Rigidbody>();
        carRb.centerOfMass = _centerOfMass;

    }

    void Update()
    {
        GetInputs();
        AnimateWheels();
        Wheeleffects();

        // Check if the car is in the air once a second instead of every frame 
        CheckIfAir();
    }

    void LateUpdate()
    {
        Move();
        Steer();
        Brake();
        ResetCar();
        Rotate();
    }

    public void MoveInput(float input)
    {
        moveInput = input;
    }

    public void SteerInput(float input)
    {
        steerInput = input;
    }

    void GetInputs()
    {
        if (control == ControlMode.Keyboard)
        {
            float newMoveInput = Input.GetAxis("Vertical");
            steerInput = Input.GetAxis("Horizontal");

            // Apply brake if trying to move in the opposite direction
            if (Mathf.Sign(newMoveInput) != Mathf.Sign(moveInput) && moveInput != 0)
            {
                foreach (var wheel in wheels)
                {
                    wheel.wheelCollider.brakeTorque = 300 * brakeAcceleration * Time.deltaTime;
                }
            }
            else
            {
                foreach (var wheel in wheels)
                {
                    wheel.wheelCollider.brakeTorque = 0;
                }
            }

            moveInput = newMoveInput;
        }
    }

    void Move()
    {
        foreach (var wheel in wheels)
        {
            wheel.wheelCollider.motorTorque = moveInput * 6000 * maxAcceleration * Time.deltaTime;
        }

        // if max speed is reached, stop accelerating and keep the speed constant
        if (carRb.linearVelocity.magnitude > maxSpeed)
        {
            foreach (var wheel in wheels)
            {
                wheel.wheelCollider.motorTorque = 0;
            }
        }
    }
    void Steer()
    {
        float actualSteerAngle = maxSteerAngle;

        if (carRb.linearVelocity.magnitude > 25)
        {
            actualSteerAngle = maxSteerAngle * 0.5f;
        }

        if (carRb.linearVelocity.magnitude > 50)
        {
            actualSteerAngle = maxSteerAngle * 0.1f;
        }

        if (carRb.linearVelocity.magnitude > 75)
        {
            actualSteerAngle = maxSteerAngle * 0.01f;
        }

        foreach (var wheel in wheels)
        {
            if (wheel.axel == Axel.Front)
            {
                var _steerAngle = steerInput * turnSensitivity * actualSteerAngle;
                wheel.wheelCollider.steerAngle = Mathf.Lerp(wheel.wheelCollider.steerAngle, _steerAngle, 0.6f);
            }
        }

    }

    void Brake()
    {
        if (Input.GetKey(KeyCode.Space) || moveInput == 0)
        {
            foreach (var wheel in wheels)
            {
                wheel.wheelCollider.brakeTorque = 300 * brakeAcceleration * Time.deltaTime;
            }

        }
        else
        {
            foreach (var wheel in wheels)
            {
                wheel.wheelCollider.brakeTorque = 0;
            }

        }
    }

    void AnimateWheels()
    {
        foreach (var wheel in wheels)
        {
            Quaternion rot;
            Vector3 pos;
            wheel.wheelCollider.GetWorldPose(out pos, out rot);
            wheel.wheelModel.transform.position = pos;
            wheel.wheelModel.transform.rotation = rot;
        }
    }

    void Wheeleffects()
    {
        float timer = 0;
        foreach (var wheel in wheels)
        {
            if (wheel.wheelCollider.GetGroundHit(out wheelHit))
            {
                if (wheelHit.sidewaysSlip > .15 || wheelHit.sidewaysSlip < -.15)
                {
                    wheel.wheelEffect.GetComponentInChildren<TrailRenderer>().emitting = true;
                    wheel.Smoke.SetActive(true);

                    timer =+ Time.deltaTime;
                        scoreSystem.GetComponent<TrickScore>().startTrickCounter(1);





                    if (timer > 3)
                    {
                        scoreSystem.GetComponent<TrickScore>().startTrickCounter(2);
                        timer = 0;
                    }
                    if (!skidSound.isPlaying)
                    {
                        skidSound.Play();
                    }
                }
                else
                {
                    wheel.wheelEffect.GetComponentInChildren<TrailRenderer>().emitting = false;
                    wheel.Smoke.SetActive(false);
                    if (skidSound.isPlaying)
                    {
                        skidSound.Stop();
                    }
                    scoreSystem.GetComponent<TrickScore>().stopTrickCounter();
                }
            }
            else
            {
                wheel.wheelEffect.GetComponentInChildren<TrailRenderer>().emitting = false;
                wheel.Smoke.SetActive(false);
                if (skidSound.isPlaying)
                {
                    skidSound.Stop();
                }
                timer = 0;
            }
        }
    }

    private void ResetCar()
    {
        // when the R key is pressed, reset the car to the starting position
        if (Input.GetKeyDown(KeyCode.R))
        {
            transform.position = new Vector3(0, 1, 0);
            transform.rotation = Quaternion.identity;
            carRb.linearVelocity = Vector3.zero;
            carRb.angularVelocity = Vector3.zero;
        }
    }

    private void Rotate()
    {
        if (Input.GetKey(KeyCode.T))
        {
            // Reset the car's rotation and move the position up a bit
            transform.rotation = Quaternion.identity;
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        }

    }

    void CheckIfAir()
    {
        float timer = 0;
        if (carRb.linearVelocity.y > 0.1f || carRb.linearVelocity.y < -0.1f)
        {
            scoreSystem.GetComponent<TrickScore>().startTrickCounter(1);
            timer = +Time.deltaTime;
            if (timer > 3)
            {
                scoreSystem.GetComponent<TrickScore>().startTrickCounter(3);
                timer = 0;
            }
        }
        else
        {
            scoreSystem.GetComponent<TrickScore>().stopTrickCounter();
        }


    }
}
