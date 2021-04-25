using System.Linq;
using UnityEngine;

public struct Steer
{
    private ContextMap _old_context_map;
    private ContextMap _context_map;

    public Steer(int directions_count)
    {
        _context_map = new ContextMap(directions_count);
        _old_context_map = new ContextMap(directions_count);
    }

    public void ResetSteerData()
    {
        for (int i = 0; i < _context_map.DirectionsCount; i++)
            _context_map.Interests[i] = 0;
        for (int i = 0; i < _context_map.DirectionsCount; i++)
            _context_map.Dangers[i] = 0;
    }

    public void ApplyContextMap(ContextMap map)
    {
        for (int i = 0; i < _context_map.DirectionsCount; i++)
            _context_map.Interests[i] = Mathf.Max(map.Interests[i], _context_map.Interests[i]);
        for (int i = 0; i < _context_map.DirectionsCount; i++)
            _context_map.Dangers[i] = Mathf.Max(map.Dangers[i], _context_map.Dangers[i]);
    }

    public void SetNewMapUp()
    {
        _old_context_map = _context_map;
        ResetSteerData();
    }

    public void InterpolateWithOld(float time)
    {
        for (int i = 0; i < _context_map.DirectionsCount; i++)
            _context_map.Interests[i] = (_old_context_map.Interests[i] + time * _context_map.Interests[i]) / (time + 1);
        for (int i = 0; i < _context_map.DirectionsCount; i++)
            _context_map.Dangers[i] = (_old_context_map.Dangers[i] + _context_map.Dangers[i]) / 2;
    }
    public void VisualizeInterests(Vector2 position)
    {
        Gizmos.color = Color.green;
        for (int i = 0; i < _context_map.DirectionsCount; i++)
            //if (_dangers[i] <= 0)
                Gizmos.DrawLine(position, position + _context_map.Directions[i] * _context_map.Interests[i]);
    }

    public void VisualizeDangers(Vector2 position)
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < _context_map.DirectionsCount; i++)
            Gizmos.DrawLine(position, position + _context_map.Directions[i] * _context_map.Dangers[i]);
    }

    public void VisualizePickedDirection(Vector2 position)
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(position, position + GetTheBestDirection());
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(position, position + GetTheBestDirection(false));
    }

    public Vector2 GetTheBestDirection(bool use_gradients=true)
    {
        if (DirectionsCount == 0)
            return Vector2.zero;

        float minimal_danger = _context_map.Dangers.Min();

        int max_index = -1;
        float max_interest = 0;
        for (int i = 0; i < _context_map.DirectionsCount; i++)
        {
            if (_context_map.Interests[i] > max_interest && _context_map.Dangers[i] == minimal_danger)
            {
                max_interest = _context_map.Interests[i];
                max_index = i;
            }
        }

        if (max_index == -1) return Vector2.zero;

        Vector2 virtual_one = CalculateSubslotBestOption(max_index, minimal_danger);
        Vector2 actual_one = _context_map.Directions[max_index] * _context_map.Interests[max_index];

        if (virtual_one == Vector2.zero || !use_gradients) return actual_one;

        return actual_one.magnitude > virtual_one.magnitude ? actual_one   : virtual_one;
    }

    private Vector2 CalculateSubslotBestOption(int index, float min_danger)
    {
        int index_leftest = WrapIndexToNormal(index - 2);
        int index_left = WrapIndexToNormal(index - 1);
        int index_right = WrapIndexToNormal(index + 1);
        int index_rightest = WrapIndexToNormal(index + 2);
        Vector2 point0 = new Vector2(-2f, _context_map.Dangers[index_leftest] == min_danger ? _context_map.Interests[index_leftest] : 0);
        Vector2 point1 = new Vector2(-1f, _context_map.Dangers[index_left] == min_danger ? _context_map.Interests[index_left] : 0);
        Vector2 point2 = new Vector2(0f, _context_map.Dangers[index] == min_danger ? _context_map.Interests[index] : 0);
        Vector2 point3 = new Vector2(1f, _context_map.Dangers[index_right] == min_danger ? _context_map.Interests[index_right] : 0);
        Vector2 point4 = new Vector2(2f, _context_map.Dangers[index_rightest] == min_danger ? _context_map.Interests[index_rightest] : 0);

        //This is representation of line equation: y = ax + b, where a is line.x and b is line.y
        //right gradient
        Vector2 line1 = new Vector2(InclineOfLine(point1, point2), point1.y - InclineOfLine(point1, point2) * point1.x);
        Vector2 line2 = new Vector2(InclineOfLine(point3, point4), point3.y - InclineOfLine(point3, point4) * point3.x);

        Vector2 intersection_point = LinesIntersection(line1, line2);

        if (Mathf.Abs(intersection_point.x) > 2f || _context_map.Dangers[WrapIndexToNormal((int)intersection_point.x)] != min_danger)
            intersection_point.x = 0;

        Vector2 virtual_direction = Vector2.LerpUnclamped(_context_map.Directions[index], _context_map.Directions[index_right], intersection_point.x);

        Vector2 virtual_right = virtual_direction * intersection_point.y;

        //left gradient
        Vector2 line3 = new Vector2(InclineOfLine(point0, point1), point0.y - InclineOfLine(point0, point1) * point0.x);
        Vector2 line4 = new Vector2(InclineOfLine(point2, point3), point2.y - InclineOfLine(point2, point3) * point2.x);

        intersection_point = LinesIntersection(line3, line4);

        if (Mathf.Abs(intersection_point.x) > 2f || _context_map.Dangers[WrapIndexToNormal((int)intersection_point.x)] != min_danger)
            intersection_point.x = 0;

        virtual_direction = Vector2.LerpUnclamped(_context_map.Directions[index], _context_map.Directions[index_right], intersection_point.x);

        Vector2 virtual_left = virtual_direction * intersection_point.y;

        return virtual_left.magnitude > virtual_right.magnitude && virtual_left.magnitude < 3f ?
                                 virtual_left :
                                 (virtual_right.magnitude < 3f) ? virtual_right : Vector2.zero;

        float InclineOfLine(Vector2 p1, Vector2 p2)
        {
            return (p2.y - p1.y) / (p2.x - p1.x);
        }

        Vector2 LinesIntersection(Vector2 l1, Vector2 l2)
        {
            if (l1.x == l2.x) return Vector2.zero;

            float intersection = (l2.y - l1.y) / (l1.x - l2.x);

            return new Vector2(intersection, LineValue(l1, intersection));
        }

        float LineValue(Vector2 l, float x)
        {
            return x * l.x + l.y;
        }
    }

    private int WrapIndexToNormal(int index)
    {
        while (index > DirectionsCount - 1) index-=DirectionsCount;
        while (index < 0) index+=DirectionsCount;
        return index;
    }

    public Vector2 GetResultDirection()
    {
        if (DirectionsCount == 0)
            return Vector2.zero;

        Vector2 result = Vector2.zero;
        for (int i = 0; i < _context_map.DirectionsCount; i++)
            if (_context_map.Dangers[i] == 0)
                result += _context_map.Directions[i] * _context_map.Interests[i];

        return result;
    }

    public Vector2[] GetDirections() => _context_map.Directions;

    public int DirectionsCount => _context_map.DirectionsCount;
}
