using System;
using System.Collections.Generic;
using UnityEngine;

public class ProgramUI : MonoBehaviour
{
    private const float PIXELS_PER_UNIT = 16f;
    private const float CURRENT_PROGRAM_SCALAR = 2f;

    public Vector2 startingMiddleVector;
    private const float TOTAL_Y_LENGTH = 15f;
    private const int SPACING_THRESHOLD = 5;   
    private const int LOWEST_SORTING_ORDER = 10;

    [Header("UI Configuration")]
    public ProgramType uiType = ProgramType.Attack;

    [Header("UI Elements")]
    public GameObject emptyProgramPrefab;
    public int startingHandSize;                 //assigned in inspector, value can be changed throughout rounds to increase / decrease size of starting hand  (implies need for add / remove functions to startingProgramUICount)
    private int displayedProgramUICount;         //The number of programs displayed at that moment in time, not necessarily at the start of a round
    [SerializeField] private List<GameObject> uiPrograms = new();
    [SerializeField] private List<Vector2> uiPositions = new();

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

        SetupNewHand();
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
                heldProgram.GetComponent<SpriteRenderer>().sortingOrder += uiPrograms.Count;

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
                LerpUIProgramsToPositions();
            }
        }

        if ((Input.GetKeyUp(KeyCode.Mouse0) && heldProgram != null) ||
        (programInputManager != null && programInputManager.inSlowTimeMode == false && heldProgram != null))
        {
            int heldProgramLastIndex = uiPrograms.IndexOf(heldProgram);
            heldProgram = null;
            LerpUIProgramsToPositions();
            UpdateSortingOrders();

            programsListData.MoveProgram(heldProgramFirstIndex, heldProgramLastIndex);
        }
    }

    //To be subscribed to animation completion in each program logic script
    public void ScrollOrSetupNewHand()
    {
        if(uiPrograms == null) return;

        if(GetProgramUICount() == 1)
        {
            SetupNewHand();
        }
        else
        {
            Debug.Log("Trying to Scroll");
            ScrollProgramUI();
        }
    }

    public void SetupNewHand()
    {
        //First, reset the hand size to starting hand size
        displayedProgramUICount = startingHandSize;
        int handSize = programsListData.DetermineHandSize(displayedProgramUICount);
        Debug.Log(handSize);

        //Second, draw a new hand based on that handsize
        programsListData.DrawNewHand(handSize);

        //Third, instantiate new empty UI programs based on that handsize
        InstantiateEmptyUIPrograms(handSize);

        //Fourth, set all new MouseHover states to false
        InitializeMouseHoverStates();

        //Fifth, set the EmptyUI Programs to their appropriate starting Positions
        SetUIPositions(handSize);

        //Sixth, assign those UI positions to each EmptyUI program
        AssignUIPositions();

        //Seventh, set the EmptyUI Programs to their appropriate Sprites
        SetUISprites(handSize);

        //Eighth, update the sorting order
        UpdateSortingOrders();
    }

    void UpdateSortingOrders()
    {
        if(uiPrograms.Count == 0) return;

        for(int i = 0; i < uiPrograms.Count; i++)
        {
            uiPrograms[i].GetComponent<SpriteRenderer>().sortingOrder = LOWEST_SORTING_ORDER + uiPrograms.Count - i;
        }
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

            if(i == 0)
            {
                newProgram.transform.localScale = new Vector2(2 ,2);
            }
        }   
    }

    //eventually seperate the creations of the UIPositions from the assignments of the objects to their positions

    void SetUIPositions(int handSize)
    {
        if(handSize == 0) return;

        int divider = handSize;

        if(handSize <= SPACING_THRESHOLD) 
        divider = SPACING_THRESHOLD;

        uiPositions.Clear();

        Vector2 spawningVector = startingMiddleVector + new Vector2(0, TOTAL_Y_LENGTH / 2f);
        Vector2 offsetVector = new(0, TOTAL_Y_LENGTH / divider);

        float currentProgramScalar = (CURRENT_PROGRAM_SCALAR - 1) / 2f;
        float xBounds = emptyProgramPrefab.GetComponent<SpriteRenderer>().sprite.rect.width / PIXELS_PER_UNIT;
        float yBounds = emptyProgramPrefab.GetComponent<SpriteRenderer>().sprite.rect.height / PIXELS_PER_UNIT;

        for (int i = 0; i < handSize; i++)
        {
            if(i == 0)
            {
                Vector2 currentProgramLocation = spawningVector;
                if(uiType == ProgramType.Attack)
                {
                    currentProgramLocation += new Vector2(xBounds, yBounds) * currentProgramScalar;
                }   
                else if(uiType == ProgramType.Defense)
                {
                    currentProgramLocation += new Vector2(-xBounds, yBounds) * currentProgramScalar;
                }

                uiPositions.Add(currentProgramLocation);
                spawningVector -= offsetVector;

                continue;      
            }

            uiPositions.Add(spawningVector);
            spawningVector -= offsetVector;
        }
    }

    void AssignUIPositions()
    {
        if(uiPositions.Count != uiPrograms.Count) return;

        for(int i = 0; i < uiPrograms.Count; i++)
        {
            uiPrograms[i].transform.position = uiPositions[i];
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

    void LerpUIProgramsToPositions()
    {
        if(uiPrograms.Count != uiPositions.Count) return;

        SortByYPosition();
        
        for (int i = GetInitialIndex(); i < GetProgramUICount(); i++)
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

    public void AddProgramsToHand(GameObject[] addPrograms, int[] indices)
    {
        if(addPrograms.Length != indices.Length) return;

        for(int i = 0; i < indices.Length; i++)
        {
            // If index is out of range, set it to insert at the end
            if(indices[i] < 0 || indices[i] > uiPrograms.Count + i)
            {
                indices[i] = uiPrograms.Count + i;
            }
        }

        programsListData.AddProgramsToHand(addPrograms, indices);

        for(int i = 0; i < addPrograms.Length; i++)
        {
            GameObject newProgram = Instantiate(emptyProgramPrefab, gameObject.transform);
            mouseHoverStates.Add(newProgram, false);
            uiPrograms.Insert(indices[i], newProgram);
        }

        int handSize  = uiPrograms.Count;

        SetUIPositions(handSize);

        for(int i = 0; i < addPrograms.Length; i++)
        {
            uiPrograms[indices[i]].transform.position = uiPositions[indices[i]];
        }

        LerpUIProgramsToPositions();
        UpdateSortingOrders();
        SetUISprites(handSize);
    }

    public void RemoveProgramsFromHand(int[] indices)
    {
        if(displayedProgramUICount - indices.Length < 1) return;

        for(int i = 0; i < indices.Length; i++)
        {
            if(indices[i] == 0) return;

            if(indices[i] < 0 || indices[i] >= uiPrograms.Count)
            {
                indices[i] = uiPrograms.Count - i;
            }
        }

        Array.Sort(indices);
        Array.Reverse(indices);

        programsListData.RemoveProgramsFromHand(indices);

        for(int i = 0; i < indices.Length; i++)
        {
            if(indices[i] < uiPrograms.Count)
            {
                Destroy(uiPrograms[indices[i]]);
                mouseHoverStates.Remove(uiPrograms[indices[i]]);
                uiPrograms.RemoveAt(indices[i]);
            }
        }

        int handSize = uiPrograms.Count;

        SetUIPositions(handSize);;
        LerpUIProgramsToPositions();
        UpdateSortingOrders();
        SetUISprites(handSize);
    }

    //In future, encorporate logic for adding or removing cards into this script most likely, 
    //between destroying the current program and moving
    public void ScrollProgramUI()
    {
        if(uiPrograms == null || uiPrograms.Count == 0) return;

        Destroy(uiPrograms[0]);
        mouseHoverStates.Remove(uiPrograms[0]);
        uiPrograms.RemoveAt(0);
        SetUIPositions(uiPrograms.Count);

        for(int i = 0; i < uiPrograms.Count; i++)
        {
            uiPrograms[i].GetComponent<LerpUIHandler>().LocationLerp(uiPositions[i], 25f);

            if(i == 0)
            {
                uiPrograms[i].GetComponent<LerpUIHandler>().ScaleLerp(new Vector2(2, 2), 25f);
            }
        }

        programsListData.ScrollCurrentProgram();
    }

    int GetProgramUICount()
    {
        if (programsListData == null) return 0;

        if(displayedProgramUICount != uiPrograms.Count)
        {
            displayedProgramUICount = uiPrograms.Count;
        }

        return displayedProgramUICount;
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

    void SortByYPosition()
    {
        int startIndex = GetInitialIndex();
        int endIndex = GetProgramUICount();

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
        if (uiPrograms.Count == 0) return null;
        
        int startIndex = GetInitialIndex();
        if (startIndex >= uiPrograms.Count) return null;
        
        GameObject closestObj = uiPrograms[startIndex];
        float minDistance = Vector2.Distance(closestObj.transform.position, mouse.GetUIMousePosition());
        
        for (int i = startIndex + 1; i < uiPrograms.Count; i++)
        {
            if (uiPrograms[i] == null) continue;
            
            float currentDistance = Vector2.Distance(uiPrograms[i].transform.position, mouse.GetUIMousePosition());
            
            if (currentDistance < minDistance)
            {
                minDistance = currentDistance;
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
        mouseHoverStates.Clear();

        if(uiPrograms.Count == 0) return;
 
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
        GameObject closest = ClosestUIToMouse();
        
        for (int i = GetInitialIndex(); i < GetProgramUICount(); i++)
        {
            if (uiPrograms[i] != null)
            {
                bool currentlyHovered = MouseDetected(uiPrograms[i]);
                bool isClosestAndHovered = currentlyHovered && (uiPrograms[i] == closest);
                bool previouslyClosestAndHovered = mouseHoverStates[uiPrograms[i]];

                mouseHoverStates[uiPrograms[i]] = isClosestAndHovered;

                // Mouse entered and is closest
                if (isClosestAndHovered && !previouslyClosestAndHovered)
                {
                    OnMouseEntering(uiPrograms[i]);
                }
                // Mouse exited or is no longer closest
                else if (!isClosestAndHovered && previouslyClosestAndHovered)
                {
                    OnMouseExiting(uiPrograms[i]);
                }
            }
        }
    }

    void OnMouseEntering(GameObject attackProgram)
    {
        if (attackProgram == heldProgram) return;

        attackProgram.GetComponent<SpriteRenderer>().sortingOrder = LOWEST_SORTING_ORDER + uiPrograms.Count + 1;

        if (uiPrograms.IndexOf(attackProgram) == 0)
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

        int index = uiPrograms.IndexOf(attackProgram);
        attackProgram.GetComponent<SpriteRenderer>().sortingOrder = LOWEST_SORTING_ORDER + uiPrograms.Count - index;

        if (index == 0)
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
        for (int i = GetInitialIndex(); i < GetProgramUICount(); i++)
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

