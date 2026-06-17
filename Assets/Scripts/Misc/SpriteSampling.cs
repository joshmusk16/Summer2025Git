using UnityEngine;

public class SpriteSampling : MonoBehaviour
{

public float GetTightWidth(Sprite sprite)
{
    Texture2D tex = sprite.texture;
    RectInt rect = new RectInt(
        (int)sprite.textureRect.x, 
        (int)sprite.textureRect.y,
        (int)sprite.textureRect.width, 
        (int)sprite.textureRect.height
    );

    int minX = rect.xMax;
    int maxX = rect.xMin;

    for (int x = rect.xMin; x < rect.xMax; x++)
        for (int y = rect.yMin; y < rect.yMax; y++)
            if (tex.GetPixel(x, y).a > 0.01f)
            {
                minX = Mathf.Min(minX, x);
                maxX = Mathf.Max(maxX, x);
            }

    if (maxX < minX)    
    {
        Debug.LogWarning($"GetTightWidth fallback triggered for: {sprite.name}, bounds.size.x: {sprite.bounds.size.x}");
        return sprite.bounds.size.x;
    }

    float tightWidthPixels = maxX - minX + 1;
    return tightWidthPixels / sprite.pixelsPerUnit;
}

public float GetTightHeight(Sprite sprite)
{
    Texture2D tex = sprite.texture;
    RectInt rect = new RectInt(
        (int)sprite.textureRect.x, 
        (int)sprite.textureRect.y,
        (int)sprite.textureRect.width, 
        (int)sprite.textureRect.height
    );

    int minY = rect.yMax;
    int maxY = rect.yMin;

    for (int x = rect.xMin; x < rect.xMax; x++)
        for (int y = rect.yMin; y < rect.yMax; y++)
            if (tex.GetPixel(x, y).a > 0.01f)
            {
                minY = Mathf.Min(minY, y);
                maxY = Mathf.Max(maxY, y);
            }

    if (maxY < minY) return sprite.bounds.size.y; //fallback if fully transparent

    float tightHeightPixels = maxY - minY + 1;
    return tightHeightPixels / sprite.pixelsPerUnit;
}

public Vector2 GetTightTopLeft(Sprite sprite, Transform spriteTransform)
{
    Texture2D tex = sprite.texture;
    RectInt rect = new RectInt(
        (int)sprite.textureRect.x,
        (int)sprite.textureRect.y,
        (int)sprite.textureRect.width,
        (int)sprite.textureRect.height
    );

    int minX = rect.xMax;
    int maxY = rect.yMin;

    for (int x = rect.xMin; x < rect.xMax; x++)
        for (int y = rect.yMin; y < rect.yMax; y++)
            if (tex.GetPixel(x, y).a > 0.01f)
            {
                minX = Mathf.Min(minX, x);
                maxY = Mathf.Max(maxY, y);
            }

    //Fallback if fully transparent
    if (minX > rect.xMax - 1 || maxY < rect.yMin)
    {
        Debug.LogWarning($"GetTightTopLeft fallback triggered for: {sprite.name}");
        Bounds b = sprite.bounds;
        Vector3 worldPos = spriteTransform.TransformPoint(new Vector3(b.min.x, b.max.y, 0));
        return new Vector2(worldPos.x, worldPos.y);
    }

    float localX = (minX - sprite.textureRect.x - sprite.pivot.x) / sprite.pixelsPerUnit;
    float localY = (maxY - sprite.textureRect.y - sprite.pivot.y + 1) / sprite.pixelsPerUnit;

    //Transform local sprite space to world space
    Vector3 worldPoint = spriteTransform.TransformPoint(new Vector3(localX, localY, 0));
    return new Vector2(worldPoint.x, worldPoint.y);
}

public Vector2 GetTightBottomLeft(Sprite sprite, Transform spriteTransform)
{
    Texture2D tex = sprite.texture;
    RectInt rect = new RectInt(
        (int)sprite.textureRect.x,
        (int)sprite.textureRect.y,
        (int)sprite.textureRect.width,
        (int)sprite.textureRect.height
    );

    int minX = rect.xMax;
    int minY = rect.yMax;

    for (int x = rect.xMin; x < rect.xMax; x++)
        for (int y = rect.yMin; y < rect.yMax; y++)
            if (tex.GetPixel(x, y).a > 0.01f)
            {
                minX = Mathf.Min(minX, x);
                minY = Mathf.Min(minY, y);
            }

    //Fallback if fully transparent
    if (minX > rect.xMax - 1 || minY > rect.yMax - 1)
    {
        Debug.LogWarning($"GetTightBottomLeft fallback triggered for: {sprite.name}");
        Bounds b = sprite.bounds;
        Vector3 worldPos = spriteTransform.TransformPoint(new Vector3(b.min.x, b.min.y, 0));
        return new Vector2(worldPos.x, worldPos.y);
    }

    float localX = (minX - sprite.textureRect.x - sprite.pivot.x) / sprite.pixelsPerUnit;
    float localY = (minY - sprite.textureRect.y - sprite.pivot.y) / sprite.pixelsPerUnit;

    //Transform local sprite space to world space
    Vector3 worldPoint = spriteTransform.TransformPoint(new Vector3(localX, localY, 0));
    return new Vector2(worldPoint.x, worldPoint.y);
}

public Vector2 GetTightBottomLeftOffset(Sprite sprite)
{
    Texture2D tex = sprite.texture;
    RectInt rect = new RectInt(
        (int)sprite.textureRect.x,
        (int)sprite.textureRect.y,
        (int)sprite.textureRect.width,
        (int)sprite.textureRect.height
    );

    int minX = rect.xMax;
    int minY = rect.yMax;

    for (int x = rect.xMin; x < rect.xMax; x++)
        for (int y = rect.yMin; y < rect.yMax; y++)
            if (tex.GetPixel(x, y).a > 0.01f)
            {
                minX = Mathf.Min(minX, x);
                minY = Mathf.Min(minY, y);
            }

    // Fallback if fully transparent
    if (minX > rect.xMax - 1 || minY > rect.yMax - 1)
    {
        Debug.LogWarning($"GetTightBottomLeftOffset fallback triggered for: {sprite.name}");
        return Vector2.zero;
    }

    float tightLocalX = (minX - sprite.textureRect.x - sprite.pivot.x) / sprite.pixelsPerUnit;
    float tightLocalY = (minY - sprite.textureRect.y - sprite.pivot.y) / sprite.pixelsPerUnit;

    float boundsLocalX = sprite.bounds.min.x;
    float boundsLocalY = sprite.bounds.min.y;

    return new Vector2(tightLocalX - boundsLocalX, tightLocalY - boundsLocalY);
}

public Vector2 GetTightBottomRight(Sprite sprite, Transform spriteTransform)
{
    Texture2D tex = sprite.texture;
    RectInt rect = new RectInt(
        (int)sprite.textureRect.x,
        (int)sprite.textureRect.y,
        (int)sprite.textureRect.width,
        (int)sprite.textureRect.height
    );

    int maxX = rect.xMin;
    int minY = rect.yMax;

    for (int x = rect.xMin; x < rect.xMax; x++)
        for (int y = rect.yMin; y < rect.yMax; y++)
            if (tex.GetPixel(x, y).a > 0.01f)
            {
                maxX = Mathf.Max(maxX, x);
                minY = Mathf.Min(minY, y);
            }

    //Fallback if fully transparent
    if (maxX < rect.xMin - 1 || minY > rect.yMax - 1)
    {
        Debug.LogWarning($"GetTightBottomRight fallback triggered for: {sprite.name}");
        Bounds b = sprite.bounds;
        Vector3 worldPos = spriteTransform.TransformPoint(new Vector3(b.max.x, b.min.y, 0));
        return new Vector2(worldPos.x, worldPos.y);
    }

    float localX = (maxX - sprite.textureRect.x - sprite.pivot.x + 1) / sprite.pixelsPerUnit;
    float localY = (minY - sprite.textureRect.y - sprite.pivot.y) / sprite.pixelsPerUnit;

    //Transform local sprite space to world space
    Vector3 worldPoint = spriteTransform.TransformPoint(new Vector3(localX, localY, 0));
    return new Vector2(worldPoint.x, worldPoint.y);
}

}
