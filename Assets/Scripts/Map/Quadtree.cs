using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quadtree
{
    private const int MAX_CAPACITY = 4;
    public Rect boundary;
    public List<Vector2> points;
    public Quadtree[] children;

    public Quadtree(Rect boundary)
    {
        this.boundary = boundary;
        points = new List<Vector2>();
        children = null;
    }

    public void Insert(Vector2 point)
    {
        if (!boundary.Contains(point)) return;

        if (points.Count < MAX_CAPACITY)
        {
            points.Add(point);
        }
        else
        {
            if (children == null) Subdivide();

            foreach (Quadtree child in children) child.Insert(point);
        }
    }

    private void Subdivide()
    {
        float halfWidth = boundary.width / 2;
        float halfHeight = boundary.height / 2;
        Vector2 center = boundary.center;

        children = new Quadtree[4];
        children[0] = new Quadtree(new Rect(center.x - halfWidth, center.y - halfHeight, halfWidth, halfHeight));
        children[1] = new Quadtree(new Rect(center.x, center.y - halfHeight, halfWidth, halfHeight));
        children[2] = new Quadtree(new Rect(center.x - halfWidth, center.y, halfWidth, halfHeight));
        children[3] = new Quadtree(new Rect(center.x, center.y, halfWidth, halfHeight));
    }

    public List<Vector2> Query(Rect range) {
        List <Vector2> result = new List<Vector2>();

        if (!boundary.Overlaps(range)) return result;

        foreach (Vector2 point in points) {
            if (range.Contains(point)) result.Add(point);
        }

        if (children != null) {
            foreach(Quadtree child in children) result.AddRange(child.Query(range));
        }

        return result;
    }
}
