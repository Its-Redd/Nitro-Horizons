using TMPro;
using UnityEngine;

public class Speedometer : MonoBehaviour
{
    private float speed = 0.0f;

    public TextMeshProUGUI speedometer;
    public Rigidbody carRb;

    void Update()
    {
        OnGUI();
    }

    void OnGUI()
    {
        speed = carRb.linearVelocity.magnitude * 3.6f;
        speedometer.text = speed.ToString("0") + " km/h";
    }


}
