using System.Collections.Generic;
using UnityEngine;

public class Pizza : MonoBehaviour
{
    [Header("Movimento da esteira")]
    public float speed = 1.6f;
    public float exitX = 6.5f;
    public float spawnX = -6.5f;
    public float laneY = -2.5f;

    [Header("Comportamento")]
    public bool loopOnExit = true;
    public bool clearOnRespawn = true;

    private CircleCollider2D circle;
    private readonly Dictionary<string, int> counts = new();

    void Start()
    {
        circle = GetComponent<CircleCollider2D>();
        var p = transform.position;
        p.y = laneY;
        transform.position = p;
    }

    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);

        if (transform.position.x >= exitX)
        {
            if (loopOnExit)
            {
                if (clearOnRespawn) ResetPizza();
                transform.position = new Vector3(spawnX, laneY, transform.position.z);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    public bool ContainsPoint(Vector2 worldPoint)
    {
        if (circle == null) circle = GetComponent<CircleCollider2D>();
        return circle != null && circle.OverlapPoint(worldPoint);
    }

    public void AddIngredient(string name)
    {
        if (!counts.ContainsKey(name)) counts[name] = 0;
        counts[name] += 1;
    }

    public int GetCount(string name)
    {
        return counts.TryGetValue(name, out var v) ? v : 0;
    }

    public void ResetPizza()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);
        counts.Clear();
    }
}
