using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public AnimationCurve WallCollectCurve;
    public AnimationCurve FoodCollectCurve;

    public Wall[] Walls;
    public Player Player;
    public List<GameObject> Foods = new List<GameObject>();
    public GameObject FoodPrefab;

    public Text ScoreText;
    public GameObject GameOverRoot;

    private int _score;
    private bool _isRunning = true;

    void Start()
    {
        foreach (Wall wall in Walls)
        {
            wall.Init();
        }

        RefreshFood();
    }

    void Update()
	{
	    if (!_isRunning)
	    {
	        if (Input.anyKeyDown)
	        {
	            SceneManager.LoadScene("Game");
	        }
	        return;
	    }

        Player.Tick();
	    foreach (Wall wall in Walls)
	    {
	        wall.Tick();
	    }

        Rect r = GetRekt();

	    if (!r.Contains(Player.Points[0]))
	    {
            GameOver();
        }

        if (IsIntersectingWithItself())
	    {
            GameOver();
        }

        List<GameObject> collectedFood = new List<GameObject>();
	    foreach (GameObject food in Foods)
	    {
	        CircleCollider2D c = food.GetComponent<CircleCollider2D>();
	        if (Vector3.Distance(c.transform.position, Player.HeadCollider.transform.position) < c.radius + Player.HeadCollider.radius + 0.1f)
	        {
	            Player.CollectFood();
	            _score++;
	            ScoreText.text = _score.ToString();

                foreach (Wall wall in Walls)
	            {
	                wall.OnFoodCollected(WallCollectCurve);
	            }

                collectedFood.Add(food);
	        }
	    }

	    foreach (GameObject food in Foods)
	    {
	        if (!r.Contains(food.transform.position))
	        {
	            food.GetComponent<Food>().IsDestroyedByWall = true;
	            collectedFood.Add(food);
	        }
	    }

	    if (collectedFood.Count > 0)
	    {
	        RefreshFood();
	        foreach (GameObject food in collectedFood)
	        {
	            StartCoroutine(DestroyFoodCoroutine(food));
	            Foods.Remove(food);
	        }
        }
	}

    private IEnumerator DestroyFoodCoroutine(GameObject food)
    {
        const float duration = 0.5f;
        bool isDestroyedByWall = food.GetComponent<Food>().IsDestroyedByWall;
        Material m = food.GetComponent<Renderer>().material;
        Vector3 sourceScale = food.transform.localScale;
        Vector3 targetScale = food.transform.localScale * (isDestroyedByWall ? 0.5f : 2f);
        for (float f = 0f; f < duration; f += Time.deltaTime)
        {
            float t = FoodCollectCurve.Evaluate(f / duration);
            Color c = m.color;
            c.a = Mathf.Lerp(1f, 0f, t);
            m.color = c;

            food.transform.localScale = Vector3.Lerp(sourceScale, targetScale, t);

            yield return null;
        }

        Destroy(food);
    }

    private void RefreshFood()
    {
        Rect r = GetRekt();

        Vector3 randomPoint = Vector3.zero;
        int tryCount = 1000;
        do
        {
            if (tryCount-- < 0)
            {
                break;
            }

            randomPoint = new Vector3(Random.Range(r.xMin, r.xMax), Random.Range(r.yMin, r.yMax), 0);
        } while (r.size.sqrMagnitude > 0.001f && Vector3.Distance(randomPoint, Player.transform.position) < 1f);

        Foods.Add(Instantiate(FoodPrefab, randomPoint, Quaternion.identity));
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

    private void GameOver()
    {
        _isRunning = false;
        GameOverRoot.SetActive(true);
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
