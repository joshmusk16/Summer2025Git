using System;
using System.Collections.Generic;
using UnityEngine;

public class ProgramUI : MonoBehaviour
{
    //This script handles all of the visual UI, the actual change to the ordering of the ProgramLists will
    //be handled in the ProgramListData script (remove and insert in new postion, etc.)

    private const int MAX_UI_COUNT = 5;
    private const float PIXELS_PER_UNIT = 16f;

    [Header("UI Configuration")]
    public ProgramType uiType = ProgramType.Attack;
    public int programsUICount = MAX_UI_COUNT;

    [Header("UI Elements")]
    public GameObject[] uiPrograms = new GameObject[MAX_UI_COUNT];
    private Vector2[] uiPositions = new Vector2[MAX_UI_COUNT];

    [Header("Dependencies")]
    public MouseTracker mouse;
    public ProgramListData programsListData;
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

            if (Input.GetKeyDown(KeyCode.Mouse0) && MouseDetected(ClosestUIToMouse()))
            {
                heldProgram = ClosestUIToMouse();
                heldProgram.GetComponent<SpriteRenderer>().sortingOrder += 1;

                if (Array.IndexOf(uiPrograms, heldProgram) == 0)
                {
                    heldProgram.GetComponent<LerpUIHandler>().ElasticScaleLerp(transform.localScale, new Vector3(2.5f, 2.5f), 0.5f);
                }
                else
                {
                    heldProgram.GetComponent<LerpUIHandler>().ElasticScaleLerp(transform.localScale, new Vector3(1.5f, 1.5f), 0.5f);
                }

                heldProgramFirstIndex = Array.IndexOf(uiPrograms, heldProgram);
            }

            if (Input.GetKey(KeyCode.Mouse0) && heldProgram != null)
            {
                heldProgram.GetComponent<LerpUIHandler>().LocationLerp(mouse.GetUIMousePosition(), 25f);
                UpdateProgramUI();
            }
        }

        if ((Input.GetKeyUp(KeyCode.Mouse0) && heldProgram != null) ||
        (programInputManager != null && programInputManager.inSlowTimeMode == false && heldProgram != null))
        {
            int heldProgramLastIndex = Array.IndexOf(uiPrograms, heldProgram);
            heldProgram.GetComponent<SpriteRenderer>().sortingOrder -= 1;
            heldProgram = null;
            UpdateProgramUI();

            programsListData.MoveProgram(heldProgramFirstIndex, heldProgramLastIndex);
        }

        //Scroll input for debugging
        if (Input.GetKeyDown(KeyCode.U))
        {
            ScrollProgramUI();
        }
    }

    public void ScrollProgramUI()
    {
        programsListData.IncreaseCurrentCount();
        SetUISprites();
    }

    void SetStartingUIPositions()
    {
        for (int i = 0; i < programsUICount; i++)
        {
            uiPositions[i] = uiPrograms[i].transform.position;
        }
    }

    void SetUISprites()
    {
        if (programsListData == null || programsListData.programs == null || programsListData.programs.Count == 0)
            return;

        for (int i = 0; i < 5; i++)
        {
            if (i + programsListData.currentProgramAmount < programsListData.programs.Count)
            {
                uiPrograms[i].GetComponent<SpriteRenderer>().sprite =
                programsListData.programs[i + programsListData.currentProgramAmount].GetComponent<ProgramData>().uiSprite;

                Vector3 tempScale = uiPrograms[i].transform.localScale;
                uiPrograms[i].GetComponent<LerpUIHandler>().ParabolicScaleLerp(tempScale + new Vector3(0.35f, 0.35f), 0.2f, 2f);
            }
            else
            {
                uiPrograms[i].GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }

    int GetActiveUICount()
    {
        if (programsListData == null) return 0;

        int remaining = programsListData.totalProgramAmount - programsListData.currentProgramAmount;
        return Mathf.Min(5, remaining);
    }

    int GetInitialIndex()
    {
        if (programInputManager == null)
        {
            return 0;
        }

        //(AttackUI / Defense UI for isAttacking and IsDefending respectively)
        bool shouldOffset = false;

        if (uiType == ProgramType.Attack)
        {
            shouldOffset = programInputManager.isAttacking;
        }
        else if (uiType == ProgramType.Defense)
        {
            shouldOffset = programInputManager.isDefending;
        }

        if (shouldOffset)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    void UpdateProgramUI()
    {
        SortByYPosition();
        for (int i = GetInitialIndex(); i < GetActiveUICount(); i++)
        {
            if (uiPrograms[i] != heldProgram)
            {
                uiPrograms[i].GetComponent<LerpUIHandler>().LocationLerp(uiPositions[i], 25f);

                if (i == 0)
                {
                    uiPrograms[i].GetComponent<LerpUIHandler>().ScaleLerp(new Vector2(2, 2), 25f);
                }
                else
                {
                    uiPrograms[i].GetComponent<LerpUIHandler>().ScaleLerp(new Vector2(1, 1), 25f);
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
            if (uiPrograms[i] != null)
            {
                sortableItems.Add((uiPrograms[i], uiPrograms[i].transform.position.y));
            }
        }

        // Sort by Y position (descending)
        sortableItems.Sort((a, b) => b.yPos.CompareTo(a.yPos));

        // Update the array
        for (int i = 0; i < sortableItems.Count; i++)
        {
            uiPrograms[startIndex + i] = sortableItems[i].obj;
        }
    }

    private GameObject ClosestUIToMouse()
    {
        float distance = Vector2.Distance(uiPrograms[GetInitialIndex()].transform.position, mouse.GetUIMousePosition());
        GameObject closestObj = uiPrograms[GetInitialIndex()];
        for (int i = GetInitialIndex(); i < GetActiveUICount(); i++)
        {
            if (Vector2.Distance(uiPrograms[i].transform.position, mouse.GetUIMousePosition()) < distance)
            {
                distance = Vector2.Distance(uiPrograms[i].transform.position, mouse.GetUIMousePosition());
                closestObj = uiPrograms[i];
            }
        }
        return closestObj;
    }

    bool MouseDetected(GameObject obj)
    {
        float xBounds = obj.GetComponent<SpriteRenderer>().sprite.rect.width * obj.transform.localScale.x / (PIXELS_PER_UNIT * 2f);
        float yBounds = obj.GetComponent<SpriteRenderer>().sprite.rect.height * obj.transform.localScale.y / (PIXELS_PER_UNIT * 2f);

        if (mouse.GetUIMousePosition().x > obj.transform.position.x - xBounds
        && mouse.GetUIMousePosition().x < obj.transform.position.x + xBounds
        && mouse.GetUIMousePosition().y > obj.transform.position.y - yBounds
        && mouse.GetUIMousePosition().y < obj.transform.position.y + yBounds)
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
        for (int i = 0; i < uiPrograms.Length; i++)
        {
            if (uiPrograms[i] != null)
            {
                mouseHoverStates[uiPrograms[i]] = false;
            }
        }
    }

    void UpdateMouseHoverStates()
    {
        for (int i = GetInitialIndex(); i < GetActiveUICount(); i++)
        {
            if (uiPrograms[i] != null)
            {
                bool currentlyHovered = MouseDetected(uiPrograms[i]);
                bool previouslyHovered = mouseHoverStates[uiPrograms[i]];

                // Mouse entered the object
                if (currentlyHovered && !previouslyHovered)
                {
                    OnMouseEntering(uiPrograms[i]);
                }
                // Mouse exited the object
                else if (!currentlyHovered && previouslyHovered)
                {
                    OnMouseExiting(uiPrograms[i]);
                }

                // Update the state
                mouseHoverStates[uiPrograms[i]] = currentlyHovered;
            }
        }
    }

    void OnMouseEntering(GameObject attackProgram)
    {
        if (attackProgram == heldProgram) return;

        if (Array.IndexOf(uiPrograms, attackProgram) == 0)
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

        if (Array.IndexOf(uiPrograms, attackProgram) == 0)
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
                uiPrograms[i].GetComponent<LerpUIHandler>().ScaleLerp(new Vector2(2f, 2f), 25f);
            }
            else
            {
                uiPrograms[i].GetComponent<LerpUIHandler>().ScaleLerp(new Vector2(1f, 1f), 25f);
            }
        }
    }

    void OnDestroy()
    {
        programInputManager.OnSlowModeExit -= ResetMouseExitScales;
    }
}
