using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public struct BehaviorInfo
{
    public string behaviorName;

    public Sprite[] animSprites;
    public float[] animFrames;
    public HitboxTiming[] hitboxTimings;

    [Tooltip("Must be a positive integer, 0 if behavior is not range dependent")]
    public int range;

    public int[] weights; //size must be equal to range, each member should be between 1-10, 10 most likely to occur, 1 least likely

    public int minTurnsToDeploy; //Once this behavior is selected based on its weights, a random turnUI amount will be selected from this min/max
    public int maxTurnsToDeploy;
}

public class EnemyAI : MonoBehaviour
{
private GameObject player;
private GameObject currentTile; //The tile this enemy is standing on

[Header("Dependencies")]
private TileGrid tileGrid;
private PlayerLogic playerLogic;
private CustomAnimator animator;

public GameObject turnUIPrefab;
private EnemyTurnUI turnUILogic;
private const float TURN_UI_VERTICAL_OFFSET = 2.75f;
private BehaviorInfo pendingBehavior; //The behavior that is set to deploy next

[Header("Range Targeting Parameters")]
public int tileTargetingRange; //Range to begin attacking behavior, i.e. once the play is within this range, AI begins
private int currentTileDistanceToPlayer;

[Header("Animation Data")]
public BehaviorInfo idleAnimation;
public List<BehaviorInfo> behaviors = new();

void Awake()
{
    tileGrid = FindObjectOfType<TileGrid>();
    playerLogic = FindObjectOfType<PlayerLogic>();
    animator = gameObject.GetComponent<CustomAnimator>();

    if(turnUIPrefab != null)
    {
        InstantiateTurnUI();    
    }

    if(playerLogic != null)
    {
        playerLogic.PlayerChangedPosition += CheckToStartAttackBehaviors;  
        player = playerLogic.gameObject;
    }

    CheckToStartAttackBehaviors();
}

private void InstantiateTurnUI()
{
    if(turnUILogic == null)
    {
        GameObject turnUIObject = Instantiate(turnUIPrefab, Vector2.zero, Quaternion.identity);
        turnUIObject.transform.SetParent(gameObject.transform);
        turnUIObject.transform.localPosition = new Vector3(0, TURN_UI_VERTICAL_OFFSET);

        turnUILogic = turnUIObject.GetComponent<EnemyTurnUI>();        
    }
}

private void UpdateCurrentTile()
{
    currentTile = tileGrid.FindNearestTileToGameObject(gameObject);
    currentTileDistanceToPlayer = tileGrid.GetTileDistanceBetweenObjects(currentTile, player);
}

private bool CheckForTargetInRange()
{
    if(player == null) return false;

    UpdateCurrentTile();

    if(currentTileDistanceToPlayer <= tileTargetingRange)
    {
        return true;
    }
    else
    {
        return false;
    }
}

private void CheckToStartAttackBehaviors()
{
    if (CheckForTargetInRange())
    {
        PickTurnsUntilBehaviorDeploys(PickWeightedRangeBasedBehavior(currentTileDistanceToPlayer));
        playerLogic.PlayerChangedPosition -= CheckToStartAttackBehaviors; 
    }
}

private void CheckOnAnimationComplete(ProgramType programType)
{
    playerLogic.PlayerChangedPosition += CheckToStartAttackBehaviors; 
    CheckToStartAttackBehaviors();

    animator.OnAnimationComplete -= CheckOnAnimationComplete;
}

private BehaviorInfo PickWeightedRangeBasedBehavior(int tileDistanceToPlayer)
{
    //First, fill the array with the proper weight values
    List<int> weights = new List<int>();
    List<BehaviorInfo> weightedBehaviors = new List<BehaviorInfo>();

    foreach (BehaviorInfo behavior in behaviors)
    {
        if (behavior.range != 0 && tileDistanceToPlayer <= behavior.range)
        {
            weights.Add(behavior.weights[tileDistanceToPlayer - 1]);
            weightedBehaviors.Add(behavior);
        }
    }

    if(weightedBehaviors.Count == 1) return weightedBehaviors[0]; //early return if theres only one behavior in range

    //Second, run the weighted roll and see which behavior wins
    int total = 0;
    foreach (int w in weights)
        total += w;

    int roll = UnityEngine.Random.Range(1, total + 1);

    int cumulative = 0;
    for (int i = 0; i < weights.Count; i++)
    {
        cumulative += weights[i];
        if (roll <= cumulative)
            return weightedBehaviors[i];
    }

    return weightedBehaviors[0];
}

private void PickTurnsUntilBehaviorDeploys(BehaviorInfo behavior)
{
    int turnsUntilAttack = UnityEngine.Random.Range(behavior.minTurnsToDeploy, behavior.maxTurnsToDeploy);
    pendingBehavior = behavior;
    
    InstantiateTurnUI(); //Fallback incase turnUI didnt instantiate in awake

    turnUILogic.InitializeTurnUI(turnsUntilAttack);

    QueueListData.OnProgramCompletion += DecreaseTurnUI;
}

private void DecreaseTurnUI()
{
    if(turnUILogic.turnUIObjects.Count > 1)
    {
        turnUILogic.RemoveTurnUI(1);   
    }
    else
    {
        turnUILogic.RemoveTurnUI(1);
        QueueListData.OnProgramCompletion -= DecreaseTurnUI;
        animator.PlayAnimation(pendingBehavior.animSprites, pendingBehavior.animFrames, ProgramType.Attack, false, true, pendingBehavior.hitboxTimings);
        animator.OnAnimationComplete += CheckOnAnimationComplete;
    }
}

private void OnDestroy()
{
    QueueListData.OnProgramCompletion -= DecreaseTurnUI;
    playerLogic.PlayerChangedPosition -= CheckToStartAttackBehaviors;
    animator.OnAnimationComplete -= CheckOnAnimationComplete;
}

}
