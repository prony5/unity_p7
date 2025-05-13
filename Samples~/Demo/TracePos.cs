using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using P7;

public class TracePos : MonoBehaviour
{
    // Start is called before the first frame update
    public Telemetry telemetry;

    [Header("Demo movement")]
    public float moveSpeed = 2f;
    public float rotationSpeed = 5f;
    public float sphereRadius = 5f;

    private Vector3 targetPosition;
    private Quaternion targetRotation;

    void Start()
    {
        GenerateNewTargetPosition();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        if (transform.position != targetPosition)
        {
            targetRotation = Quaternion.LookRotation(targetPosition - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            GenerateNewTargetPosition();
        }

        if (telemetry != null && telemetry.Count > 2)
        {
            telemetry[0].Add(transform.position.x);
            telemetry[1].Add(transform.position.y);
            telemetry[2].Add(transform.position.z);
        }
    }

    void GenerateNewTargetPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere.normalized;
        float randomDistance = Random.Range(0f, sphereRadius);
        targetPosition = randomDirection * randomDistance;

        Debug.DrawLine(transform.position, targetPosition, Color.green, 2f);
    }
}
