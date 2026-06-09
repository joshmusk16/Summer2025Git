using System;
using UnityEngine;

[Serializable]
public struct AnimationInfo
{
    public Sprite[] animSprites;
    public float[] animFrames;
    public HitboxTiming[] hitboxTimings;
}

public class EnemyAI : MonoBehaviour
{
public GameObject player;
private GameObject currentTile; //The tile this enemy is standing on

[Header("Dependencies")]
public TileGrid tileGrid;
private Animator animator;
private PlayerLogic playerLogic;

[Header("Range Targeting Parameters")]
public int tileTargetingRange;

[Header("Animation Data")]
public AnimationInfo idleAnimation;
public AnimationInfo attackAnimation;

void Awake()
{
    tileGrid = FindObjectOfType<TileGrid>();
    playerLogic = FindObjectOfType<PlayerLogic>();
    animator = gameObject.GetComponent<Animator>();

    if(playerLogic != null)
    {
        playerLogic.PlayerChangedPosition += PlayerInRangeBehavior;
    }
}


    private void UpdateCurrentTile()
{
    currentTile = tileGrid.FindNearestTileToGameObject(gameObject);
}

private bool CheckForTargetInRange()
{
    if(player == null) return false;

    if(currentTile == null) UpdateCurrentTile();

    if(tileGrid.GetTileDistanceBetweenObjects(currentTile, player) <= tileTargetingRange)
    {
        return true;
    }
    else
    {
        return false;
    }
}

private void PlayerInRangeBehavior(Vector2 vector)
{
    //Need to go back to playerLogic and create a new variable for lastTilePosition, if lastTilePositon == newOne, dont run the event
}

}
