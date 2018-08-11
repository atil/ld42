using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public Camera Camera;
    public Transform DirectionTransform;
    public LineRenderer LineRenderer;

    public Vector3 V1;
    public Vector3 V2;

    private float _defaultSpeed = 0.02f;
    private float _pushSpeed = -0.5f;
    private float _speed;

    public void Init()
    {
        _speed = _defaultSpeed;
        UpdateInternal();
    }

    public void Tick ()
	{
	    UpdateInternal();
	}

    public void OnFoodCollected(AnimationCurve curve)
    {
        StartCoroutine(OnFoodCollectedCoroutine(curve));
    }

    private IEnumerator OnFoodCollectedCoroutine(AnimationCurve curve)
    {
        const float duration = 1f;
        for (float f = 0f; f < duration; f += Time.deltaTime)
        {
            float t = curve.Evaluate(f / duration);

            _speed = Mathf.Lerp(_pushSpeed, _defaultSpeed, t);
            yield return null;
        }

        _defaultSpeed += 0.005f;
        _pushSpeed += -0.01f;
    }

    private void UpdateInternal()
    {
        List<Plane> planes = new List<Plane>(GeometryUtility.CalculateFrustumPlanes(Camera));GeometryUtility.CalculateFrustumPlanes(Camera);

        Vector3 normal = (DirectionTransform.position - transform.position).normalized;
        Vector3 nextPos = transform.position + normal * _speed * Time.deltaTime;

        if (planes.TrueForAll(p => p.GetSide(nextPos)))
        {
            transform.position = nextPos;
        }

        V1 = transform.position + Quaternion.Euler(0, 0, 90) * normal * 999f;
        V2 = transform.position + Quaternion.Euler(0, 0, -90) * normal * 999f;

        LineRenderer.SetPositions(new[] { V1, V2 });
    }
}
