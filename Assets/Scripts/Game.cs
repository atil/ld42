using System.Linq;
using UnityEngine;

public class Game : MonoBehaviour
{
    public Wall[] Walls;
    public Player Player;

	void Update ()
	{
        Rect r = GetRekt();

	    if (Player.Points.Exists(p => !r.Contains(p)))
	    {
            // game over
	        //Debug.Log("gameover 1");
        }

        if (IsIntersectingWithItself())
	    {
            // game over
            Debug.Log("gameover 2");
        }

	}

    public bool IsIntersectingWithItself()
    {
        Vector3 p0 = Player.Points[0], p1 = Player.Points[1];
        for (int i = 1; i < Player.Points.Count - 1; i++)
        {
            Vector2 temp;
            if (LineSegmentsIntersection(p0, p1, Player.Points[i], Player.Points[i + 1], out temp))
            {
                return true;
            }
        }

        return false;
    }

    public Rect GetRekt()
    {
        Vector3[] points = new Vector3[4];
        Vector3 intersection;
        for (int i = 0; i < 3; i++)
        {
            bool succ = LineIntersection(Walls[i].V1, Walls[i].V2, Walls[i + 1].V1, Walls[i + 1].V2, out intersection);

            points[i] = intersection;
        }

        LineIntersection(Walls[3].V1, Walls[3].V2, Walls[0].V1, Walls[0].V2, out intersection);
        points[3] = intersection;

        float xmax = points.Max(p => p.x);
        float xmin = points.Min(p => p.x);
        float ymax = points.Max(p => p.y);
        float ymin = points.Min(p => p.y);

        return new Rect()
        {
            xMax = xmax,
            xMin = xmin,
            yMax = ymax,
            yMin = ymin,
        };
    }

    // https://stackoverflow.com/questions/4543506/algorithm-for-intersection-of-2-lines
    private static bool LineIntersection(Vector3 line1V1, Vector3 line1V2, Vector3 line2V1, Vector3 line2V2, out Vector3 intersection)
    {
        //Line1
        float a1 = line1V2.y - line1V1.y;
        float b1 = line1V2.x - line1V1.x;
        float c1 = a1 * line1V1.x + b1 * line1V1.y;

        //Line2
        float a2 = line2V2.y - line2V1.y;
        float b2 = line2V2.x - line2V1.x;
        float c2 = a2 * line2V1.x + b2 * line2V1.y;

        float det = a1 * b2 - a2 * b1;
        if (det == 0)
        {
            intersection = Vector3.zero;
            return false;//parallel lines
        }

        float x = (b2 * c1 - b1 * c2) / det;
        float y = (a1 * c2 - a2 * c1) / det;

        intersection = new Vector3(x, y, 0);
        return true;
    }

    // https://github.com/setchi/Unity-LineSegmentsIntersection/blob/master/Assets/LineSegmentIntersection/Scripts/Math2d.cs
    public static bool LineSegmentsIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector3 p4, out Vector2 intersection)
    {
        intersection = Vector2.zero;

        var d = (p2.x - p1.x) * (p4.y - p3.y) - (p2.y - p1.y) * (p4.x - p3.x);

        if (d == 0.0f)
        {
            return false;
        }

        var u = ((p3.x - p1.x) * (p4.y - p3.y) - (p3.y - p1.y) * (p4.x - p3.x)) / d;
        var v = ((p3.x - p1.x) * (p2.y - p1.y) - (p3.y - p1.y) * (p2.x - p1.x)) / d;

        if (u < 0.01f || u > 0.99f || v < 0.01f || v > 0.99f)
        {
            return false;
        }

        intersection.x = p1.x + u * (p2.x - p1.x);
        intersection.y = p1.y + u * (p2.y - p1.y);

        return true;
    }
}
