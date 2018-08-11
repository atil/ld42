using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform Head;
    public CircleCollider2D HeadCollider;
    public LineRenderer LineRenderer;

    private const float Speed = 1f;
    private bool _isCollecting;

    public List<Vector3> Points = new List<Vector3>();

    void Start()
    {
        Points.Add(transform.position);
        Points.Add(transform.position + Vector3.up);
    }

    public void Tick()
    {
        Vector3 firstDir = (Points[0] - Points[1]).normalized;
        Vector3 lastDir = (Points[Points.Count - 2] - Points[Points.Count - 1]).normalized;

        Points[0] += firstDir * Speed * Time.deltaTime;

        if (!_isCollecting)
        {
            Points[Points.Count - 1] += lastDir * Speed * Time.deltaTime;
        }

        Vector3 input;
        if (GetInput(firstDir, out input))
        {
            Points.Insert(0, Points[0] + input * 0.0001f);
        }

        if (Vector3.Distance(Points[Points.Count - 1], Points[Points.Count - 2]) < 0.01f)
        {
            Points.RemoveAt(Points.Count - 1);
        }

        LineRenderer.positionCount = Points.Count;
        LineRenderer.SetPositions(Points.ToArray());
        Head.transform.position = Points[0];
    }
   

    private bool GetInput(Vector3 firstDir, out Vector3 dir)
    {
        Vector3 input = Vector3.zero;
        dir = Vector3.zero;
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            input = Vector3.up;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            input = Vector3.down;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            input = Vector3.left;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            input = Vector3.right;
        }

        if (input.sqrMagnitude > 0 && Mathf.Abs(Vector3.Dot(firstDir, input)) < 0.01f)
        {
            dir = input;
            return true;
        }

        return false;
    }

    public void CollectFood()
    {
        StartCoroutine(CollectCoroutine());
    }

    private IEnumerator CollectCoroutine()
    {
        _isCollecting = true;
        yield return new WaitForSeconds(0.025f);
        _isCollecting = false;
    }
}
