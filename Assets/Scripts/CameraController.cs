using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSmoothness = 10.0f;
    public float rotateSmoothness = 10.0f;

    public Vector3 moveOffset;
    public Vector3 rotateOffset;

    public Transform target;

    void FixedUpdate()
    {
        Follow();
    }

    void Follow()
    {
        HandleMovement();
        HandleRotation();

    }

    void HandleMovement()
    {
        Vector3 targetPosition = target.position + target.TransformDirection(moveOffset);
        transform.position = Vector3.Lerp(transform.position, targetPosition, moveSmoothness * Time.deltaTime);
    }

    void HandleRotation()
    {
        Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position, target.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSmoothness * Time.deltaTime);
    }


}
