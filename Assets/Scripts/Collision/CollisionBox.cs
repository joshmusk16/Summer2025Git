using UnityEngine;

public class CollisionBox : MonoBehaviour
{
    [Header("Box Properties")]
    public Vector2 offset = Vector2.zero;
    public Vector2 size = Vector2.one;
    public bool isActive = true;

    [Header("Visual Debug")]
    public bool showDebugGizmo = true;
    public Color gizmoColor = Color.white;

    // Calculate world bounds on each call
    public Rect WorldBounds
    {
        get
        {
            Vector2 worldPos = (Vector2)transform.position + offset;
            return new Rect(worldPos.x - size.x * 0.5f, worldPos.y - size.y * 0.5f, size.x, size.y);
        }
    }

    // Check if this box overlaps with another
    public bool Overlaps(CollisionBox other)
    {
        if (!isActive || !other.isActive) return false;
        return WorldBounds.Overlaps(other.WorldBounds);
    }

    protected virtual void OnDrawGizmos()
    {
        if (!showDebugGizmo) return;
        
        Gizmos.color = gizmoColor;
        Rect bounds = WorldBounds;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
        
        // Draw a slightly transparent filled version
        Color fillColor = gizmoColor;
        fillColor.a = 0.2f;
        Gizmos.color = fillColor;
        Gizmos.DrawCube(bounds.center, bounds.size);
    }

}
