using System;
using System.Collections.Generic;
using UnityEngine;

public class AttackUI : MonoBehaviour
{
    //This script handles all of the visual UI, the actual change to the ordering of the attackProgram List will
    //be hangled in the Attack Data script (remove and insert in new postion, etc.)

    private const int MAX_UI_COUNT = 5;
    private const float PIXELS_PER_UNIT = 16f;

    public int atkUICount = MAX_UI_COUNT;

    [Header("UI Configuration")]
    private Vector2[] uiPositions = new Vector2[MAX_UI_COUNT];
    public GameObject[] attackPrograms = new GameObject[MAX_UI_COUNT];

    [Header("Dependencies")]
    public MouseTracker mouse;
    public AttackProgramsData attackProgramsData;
    private ProgramInputManager programInputManager;

    //UI State Management
    private int heldProgramFirstIndex;
    private GameObject heldProgram = null;

    private Dictionary<GameObject, bool> mouseHoverStates = new();

    void Start()
    {
        programInputManager = FindObjectOfType<ProgramInputManager>();
        SetStartingUIPositions();
        SetUISprites();
        InitializeMouseHoverStates();

        if (programInputManager != null)
        {
            programInputManager.OnSlowModeExit += ResetMouseExitScales;
        }
    }

    void Update()
    {
        UpdateMouseHoverStates();

        if (programInputManager != null && programInputManager.inSlowTimeMode == true)
        {

            if (Input.GetKeyDown(KeyCode.Mouse0) && MouseDetected(ClosestAtkUIToMouse()))
            {
                heldProgram = ClosestAtkUIToMouse();
                heldProgram.GetComponent<SpriteRenderer>().sortingOrder += 1;

                if (Array.IndexOf(attackPrograms, heldProgram) == 0)
                {
                    heldProgram.GetComponent<LerpUIHandler>().ElasticScaleLerp(transform.localScale, new Vector3(2.5f, 2.5f), 0.5f);
                }
                else
                {
                    heldProgram.GetComponent<LerpUIHandler>().ElasticScaleLerp(transform.localScale, new Vector3(1.5f, 1.5f), 0.5f);
                }

                heldProgramFirstIndex = Array.IndexOf(attackPrograms, heldProgram);
            }

            if (Input.GetKey(KeyCode.Mouse0) && heldProgram != null)
            {
                heldProgram.GetComponent<LerpUIHandler>().LocationLerp(mouse.worldPosition, 25f);
                UpdateAttackProgramUI();
            }
        }

        if ((Input.GetKeyUp(KeyCode.Mouse0) && heldProgram != null) ||
        (programInputManager != null && programInputManager.inSlowTimeMode == false && heldProgram != null))
        {
            int heldProgramLastIndex = Array.IndexOf(attackPrograms, heldProgram);
            heldProgram.GetComponent<SpriteRenderer>().sortingOrder -= 1;
            heldProgram = null;
            UpdateAttackProgramUI();

            attackProgramsData.MoveAttackProgram(heldProgramFirstIndex, heldProgramLastIndex);
        }

        //Scroll input for debugging
        if (Input.GetKeyDown(KeyCode.U))
        {
            ScrollAttackUI();
        }
    }

    public void ScrollAttackUI()
    {
        attackProgramsData.IncreaseCurrentAtkCount();
        SetUISprites();
    }

    void SetStartingUIPositions()
    {
        for (int i = 0; i < atkUICount; i++)
        {
            uiPositions[i] = attackPrograms[i].transform.position;
        }
    }

    void SetUISprites()
    {
        for (int i = 0; i < 5; i++)
        {
            if (i + attackProgramsData.currentAttackProgramAmount < attackProgramsData.attackPrograms.Count)
            {
                attackPrograms[i].GetComponent<SpriteRenderer>().sprite =
                attackProgramsData.attackPrograms[i + attackProgramsData.currentAttackProgramAmount].GetComponent<AttackData>().uiSprite;

                Vector3 tempScale = attackPrograms[i].transform.localScale;
                attackPrograms[i].GetComponent<LerpUIHandler>().ParabolicScaleLerp(tempScale + new Vector3(0.35f, 0.35f), 0.2f, 2f);
            }
            else
            {
                attackPrograms[i].GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }

    int GetActiveUICount()
    {
        if (attackProgramsData == null) return 0;

        int remaining = attackProgramsData.totalAttackProgramAmount - attackProgramsData.currentAttackProgramAmount;
        return Mathf.Min(5, remaining);
    }

    int GetInitialIndex()
    {
        if (programInputManager == null)
        {
            return 0;
        }

        if (programInputManager.isAttacking == true)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    void UpdateAttackProgramUI()
    {
        SortByYPosition();
        for (int i = GetInitialIndex(); i < GetActiveUICount(); i++)
        {
            if (attackPrograms[i] != heldProgram)
            {
                attackPrograms[i].GetComponent<LerpUIHandler>().LocationLerp(uiPositions[i], 25f);

                if (i == 0)
                {
                    attackPrograms[i].GetComponent<LerpUIHandler>().ScaleLerp(new Vector2(2, 2), 25f);
                }
                else
                {
                    attackPrograms[i].GetComponent<LerpUIHandler>().ScaleLerp(new Vector2(1, 1), 25f);
                }
            }
        }
    }

    void SortByYPosition()
    {
        int startIndex = GetInitialIndex();
        int endIndex = GetActiveUICount();
        
        if (endIndex - startIndex <= 1) return;

        // Create a temporary array for sorting
        var sortableItems = new List<(GameObject obj, float yPos)>();
        
        for (int i = startIndex; i < endIndex; i++)
        {
            if (attackPrograms[i] != null)
            {
                sortableItems.Add((attackPrograms[i], attackPrograms[i].transform.position.y));
            }
        }

        // Sort by Y position (descending)
        sortableItems.Sort((a, b) => b.yPos.CompareTo(a.yPos));

        // Update the array
        for (int i = 0; i < sortableItems.Count; i++)
        {
            attackPrograms[startIndex + i] = sortableItems[i].obj;
        }
    }

    private GameObject ClosestAtkUIToMouse()
    {
        float distance = Vector2.Distance(attackPrograms[GetInitialIndex()].transform.position, mouse.worldPosition);
        GameObject closestObj = attackPrograms[GetInitialIndex()];
        for (int i = GetInitialIndex(); i < GetActiveUICount(); i++)
        {
            if (Vector2.Distance(attackPrograms[i].transform.position, mouse.worldPosition) < distance)
            {
                distance = Vector2.Distance(attackPrograms[i].transform.position, mouse.worldPosition);
                closestObj = attackPrograms[i];
            }
        }
        return closestObj;
    }

    bool MouseDetected(GameObject obj)
    {
        float xBounds = obj.GetComponent<SpriteRenderer>().sprite.rect.width * obj.transform.localScale.x / (PIXELS_PER_UNIT * 2f);
        float yBounds = obj.GetComponent<SpriteRenderer>().sprite.rect.height * obj.transform.localScale.y / (PIXELS_PER_UNIT * 2f);

        if (mouse.worldPosition.x > obj.transform.position.x - xBounds
        && mouse.worldPosition.x < obj.transform.position.x + xBounds
        && mouse.worldPosition.y > obj.transform.position.y - yBounds
        && mouse.worldPosition.y < obj.transform.position.y + yBounds)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //Mouse Hover Tracking Methods

    void InitializeMouseHoverStates()
    {
        for (int i = 0; i < attackPrograms.Length; i++)
        {
            if (attackPrograms[i] != null)
            {
                mouseHoverStates[attackPrograms[i]] = false;
            }
        }
    }

    void UpdateMouseHoverStates()
    {
        for (int i = GetInitialIndex(); i < GetActiveUICount(); i++)
        {
            if (attackPrograms[i] != null)
            {
                bool currentlyHovered = MouseDetected(attackPrograms[i]);
                bool previouslyHovered = mouseHoverStates[attackPrograms[i]];

                // Mouse entered the object
                if (currentlyHovered && !previouslyHovered)
                {
                    OnMouseEntering(attackPrograms[i]);
                }
                // Mouse exited the object
                else if (!currentlyHovered && previouslyHovered)
                {
                    OnMouseExiting(attackPrograms[i]);
                }

                // Update the state
                mouseHoverStates[attackPrograms[i]] = currentlyHovered;
            }
        }
    }

    void OnMouseEntering(GameObject attackProgram)
    {
        if (attackProgram == heldProgram) return;

        if (Array.IndexOf(attackPrograms, attackProgram) == 0)
        {
            attackProgram.GetComponent<LerpUIHandler>().ScaleLerp(new Vector2(2.3f, 2.3f), 25f);
        }
        else
        {
            attackProgram.GetComponent<LerpUIHandler>().ScaleLerp(new Vector2(1.3f, 1.3f), 25f);
        }
    }

    void OnMouseExiting(GameObject attackProgram)
    {
        if (attackProgram == heldProgram) return;

        if (Array.IndexOf(attackPrograms, attackProgram) == 0)
        {
            attackProgram.GetComponent<LerpUIHandler>().ScaleLerp(new Vector2(2f, 2f), 25f);
        }
        else
        {
            attackProgram.GetComponent<LerpUIHandler>().ScaleLerp(new Vector2(1f, 1f), 25f);
        }
    }

    void ResetMouseExitScales()
    {
        for (int i = GetInitialIndex(); i < GetActiveUICount(); i++)
        {
            if (i == 0)
            {
                attackPrograms[i].GetComponent<LerpUIHandler>().ScaleLerp(new Vector2(2f, 2f), 25f);
            }
            else
            {
                attackPrograms[i].GetComponent<LerpUIHandler>().ScaleLerp(new Vector2(1f, 1f), 25f);
            }
        }
    }

    void OnDestroy()
    {
        programInputManager.OnSlowModeExit -= ResetMouseExitScales;
    }
}
