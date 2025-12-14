using System;
using System.Collections.Generic;
using UnityEngine;

public class ProgramUINew : MonoBehaviour
{
    //This script handles all of the visual UI, the actual change to the ordering of the ProgramLists will
    //be handled in the ProgramListData script (remove and insert in new postion, etc.)

    private const int MAX_UI_COUNT = 5;
    private const float PIXELS_PER_UNIT = 16f;

    //New totalYLength for determinging starting uiPositions with math rather than object reference
    public Vector2 startingMiddleVector;          //Starting vector for SetStartingUIPositions()
    private const float totalYLength = 5f;

    [Header("UI Configuration")]
    public ProgramType uiType = ProgramType.Attack;     //Assigned in inspector to differentiate attack v defense list
    public int programsUICount = MAX_UI_COUNT;

    //[Header("UI Elements")]
    //public GameObject[] uiPrograms = new GameObject[MAX_UI_COUNT];
    // private Vector2[] uiPositions = new Vector2[MAX_UI_COUNT];

    //convert arrays to lists for more flexible adding/removing
    [Header("UI Elements")]
    public GameObject emptyProgramPrefab;       //assigned in inspector, empty gameobjects with a spriteRenderer and a lerpHandler for Instantiation in SetStartingUIPositions()
    public int startingProgramUICount;          //assigned in inspector, value can be changed throughout rounds to increase / decrease size of starting hand  (implies need for add / remove functions to startingProgramUICount)
    private int displayedProgramUICount;         //The number of programs displayed at that moment in time, not necessarily at the start of a round
    public List<GameObject> uiPrograms = new();
    private List<Vector2> uiPositions = new();

    [Header("Dependencies")]
    public MouseTracker mouse;
    public ProgramListNew programsListData;
    private ProgramInputManager programInputManager;

    //UI State Management
    private int heldProgramFirstIndex;
    private GameObject heldProgram = null;

    private Dictionary<GameObject, bool> mouseHoverStates = new();

    void Start()
    {
        programInputManager = FindObjectOfType<ProgramInputManager>();

        SetupNewHand();

        // InstantiateEmptyUIPrograms();
        // SetStartingUIPositions();
        // SetUISprites();
        // InitializeMouseHoverStates();

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

                if (uiPrograms.IndexOf(heldProgram) == 0)
                {
                    heldProgram.GetComponent<LerpUIHandler>().ElasticScaleLerp(transform.localScale, new Vector3(2.5f, 2.5f), 0.5f);
                }
                else
                {
                    heldProgram.GetComponent<LerpUIHandler>().ElasticScaleLerp(transform.localScale, new Vector3(1.5f, 1.5f), 0.5f);
                }

                heldProgramFirstIndex = uiPrograms.IndexOf(heldProgram);
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
            int heldProgramLastIndex = uiPrograms.IndexOf(heldProgram);
            heldProgram.GetComponent<SpriteRenderer>().sortingOrder -= 1;
            heldProgram = null;
            UpdateProgramUI();

            programsListData.MoveProgram(heldProgramFirstIndex, heldProgramLastIndex);
        }
    }

    void SetupNewHand()
    {
        //First, determine the handsize
        displayedProgramUICount = startingProgramUICount;
        int handSize = programsListData.DetermineHandSize(displayedProgramUICount);

        //Second, draw a new hand based on that handsize
        programsListData.DrawNewHand(handSize);

        //Third, instantiate new empty UI programs based on that handsize
        InstantiateEmptyUIPrograms(handSize);

        //Fourth, set the EmptyUI Programs to their appropriate starting Positions
        SetStartingUIPositions(handSize);

        //Fifth, set the EmptyUI Programs to their appropriate Sprites
        SetUISprites(handSize);

    }

    void InstantiateEmptyUIPrograms(int handSize)
    {
        foreach(GameObject program in uiPrograms)
        {
            if(program != null)
            {
                Destroy(program);
            }
        }

        uiPrograms.Clear();

        for(int i = 0; i < handSize; i++)
        {
            GameObject newProgram = Instantiate(emptyProgramPrefab, gameObject.transform);
            uiPrograms.Add(newProgram);
        }   
    }

    void SetStartingUIPositions(int handSize)
    {
        if(handSize == 0) return;

        Vector2 spawningVector = startingMiddleVector + new Vector2(0, totalYLength / 2f);

        for (int i = 0; i < handSize; i++)
        {
            uiPrograms[i].transform.position = spawningVector;
            spawningVector -= new Vector2(0, totalYLength / handSize);
        }
    }

    void SetUISprites(int handSize)
    {
        if (programsListData == null || programsListData.drawnPrograms == null || 
        programsListData.drawnPrograms.Count == 0 || uiPrograms.Count == 0) return;

        for (int i = 0; i < handSize; i++)
        {
            uiPrograms[i].GetComponent<SpriteRenderer>().sprite = 
            programsListData.drawnPrograms[i].GetComponent<ProgramData>().uiSprite;
        }
    }

    public void ScrollProgramUI()
    {
        
    }

    int GetActiveUICount()
    {
        if (programsListData == null) return 0;

        int remaining = programsListData.totalProgramAmount - programsListData.currentProgramAmount;
        return Mathf.Min(displayedProgramUICount, remaining);
    }

    //Returns 0 vs 1 depending on whether or not the player is currently using an attack vs defense program
    //This method will probably be extended in the future to encompass all the programs that are currently in the que to deploy (when the queing system is added)
    //For example, if the first three attack programs have been added to the que, the method returns 3.
    int GetInitialIndex()
    {
        if (programInputManager == null) return 0;

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
        for (int i = 0; i < uiPrograms.Count; i++)
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

        if (uiPrograms.IndexOf(heldProgram) == 0)
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

        if (uiPrograms.IndexOf(heldProgram) == 0)
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

