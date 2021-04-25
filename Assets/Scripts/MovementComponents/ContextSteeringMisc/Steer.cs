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
        Gizmos.DrawLine(position, position + GetResultDirection());
    }

    public Vector2 GetTheBestDirection()
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

        Vector2 virtual_one = CalculateSubslotBestOption(max_index);
        Vector2 actual_one = _context_map.Directions[max_index] * _context_map.Interests[max_index];

        if (virtual_one == Vector2.zero) return actual_one;

        return actual_one.magnitude > virtual_one.magnitude ? actual_one   : virtual_one;
    }

    private Vector2 CalculateSubslotBestOption(int index)
    {
        int index_leftest = WrapIndexToNormal(index - 2);
        int index_left = WrapIndexToNormal(index - 1);
        int index_right = WrapIndexToNormal(index + 1);
        int index_rightest = WrapIndexToNormal(index + 2);
        Vector2 point0 = new Vector2(-2f, _context_map.Interests[index_leftest]);
        Vector2 point1 = new Vector2(-1f, _context_map.Interests[index_left]);
        Vector2 point2 = new Vector2(0f, _context_map.Interests[index]);
        Vector2 point3 = new Vector2(1f, _context_map.Interests[index_right]);
        Vector2 point4 = new Vector2(2f, _context_map.Interests[index_rightest]);

        //This is representation of line equation: y = ax + b, where a is line.x and b is line.y
        //right gradient
        Vector2 line1 = new Vector2(InclineOfLine(point1, point2), point1.y - InclineOfLine(point1, point2) * point1.x);
        Vector2 line2 = new Vector2(InclineOfLine(point3, point4), point3.y - InclineOfLine(point3, point4) * point3.x);

        Vector2 virtual_right = Vector2.zero;

        if (line1.x != line2.x)
        {
            float virtual_slot = (line2.y - line1.y) / (line1.x - line2.x);
            if (Mathf.Abs(virtual_slot) > 2f)
                virtual_slot = 0;

            Vector2 virtual_direction_right = Vector2.LerpUnclamped(_context_map.Directions[index], _context_map.Directions[index_right], virtual_slot);

            virtual_right = virtual_direction_right * (line1.x * virtual_slot + line1.y);
        }

        //left gradient
        Vector2 line3 = new Vector2(InclineOfLine(point0, point1), point0.y - InclineOfLine(point0, point1) * point0.x);
        Vector2 line4 = new Vector2(InclineOfLine(point2, point3), point2.y - InclineOfLine(point2, point3) * point2.x);

        Vector2 virtual_left = Vector2.zero;

        if (line3.x != line4.x)
        {
            float virtual_slot = (line4.y - line3.y) / (line3.x - line4.x);
            if (Mathf.Abs(virtual_slot) > 2f)
                virtual_slot = 0;

            Vector2 virtual_direction_left = Vector2.LerpUnclamped(_context_map.Directions[index], _context_map.Directions[index_right], virtual_slot);

            virtual_left = virtual_direction_left * (line3.x * virtual_slot + line3.y);
        }

        Vector2 virtual_result =virtual_left.magnitude > virtual_right.magnitude && virtual_left.magnitude < 3f ? 
                                virtual_left : 
                                (virtual_right.magnitude < 3f) ? virtual_right : Vector2.zero;

        return virtual_result;

        float InclineOfLine(Vector2 p1, Vector2 p2)
        {
            return (p2.y - p1.y) / (p2.x - p1.x);
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
