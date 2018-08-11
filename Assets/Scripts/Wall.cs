using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public static float Speed = 0.02f;
    public Transform DirectionTransform;
    public LineRenderer LineRenderer;

    public Vector3 V1;
    public Vector3 V2;

    void Start()
    {
        UpdateInternal();
    }

    void Update ()
	{
	    UpdateInternal();
	}

    private void UpdateInternal()
    {
        Vector3 normal = (DirectionTransform.position - transform.position).normalized;
        transform.position += normal * Speed * Time.deltaTime;
        V1 = transform.position + Quaternion.Euler(0, 0, 90) * normal * 999f;
        V2 = transform.position + Quaternion.Euler(0, 0, -90) * normal * 999f;

        LineRenderer.SetPositions(new[] { V1, V2 });
    }
}
