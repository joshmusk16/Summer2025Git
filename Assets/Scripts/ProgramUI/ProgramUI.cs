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

    [Header("Queue UI Elements")]
    public Vector2 queueUIOffsetVector;
    public GameObject queuePrefab;
    private Dictionary<GameObject, bool> queueUIStates = new();
    private List<GameObject> queueUIObjects = new();
    private List<Vector2> queueUIPositions = new();

    [Header("Dependencies")]
    public MouseTracker mouse;
    public ProgramListData programsListData;
    private ProgramInputManager programInputManager;
    private DescriptionManager descriptionManager;

    //UI State Management
    private int heldProgramFirstIndex;
    private GameObject heldProgram = null;

    private Dictionary<GameObject, bool> mouseHoverStates = new();

    void Start()
    {
        programInputManager = FindObjectOfType<ProgramInputManager>();
        descriptionManager = FindObjectOfType<DescriptionManager>();

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
            if (Input.GetKeyDown(KeyCode.Mouse0) && MouseDetected(ClosestUIToMouse(uiPrograms)))
            {
                heldProgram = ClosestUIToMouse(uiPrograms);
                heldProgram.GetComponent<SpriteRenderer>().sortingOrder += uiPrograms.Count;
                descriptionManager.DestroyTextElement();

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

            if (Input.GetKey(KeyCode.Mouse1) && MouseDetected(ClosestUIToMouse(queueUIObjects)))
            {
                GameObject closestQueueUI = ClosestUIToMouse(queueUIObjects);
                //Add method to make this state and all that come after inactive
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
            SetupNewHand(); //incorporate queue ui logic to setupnewhand
        }
        else
        {   
            ScrollProgramUI(); //incorporate queue ui logic to scrollProgramUI
        }
    }

    public void SetupNewHand()
    {
        //First, reset the hand size to starting hand size
        displayedProgramUICount = startingHandSize;
        int handSize = programsListData.DetermineHandSize(displayedProgramUICount);

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

        //Eighth, setup the queue UI
        SetupQueueUIOnStart();

        //Ninth, update all the sorting orders
        UpdateSortingOrders();
    }

    //To be called in ProgramInputManager
    public void UpdateQueueUIOnClick()
    {
        UpdateNextQueueUIActiveState();
        SetQueueUISprites();
    }

    void UpdateSortingOrders()
    {
        //if(uiPrograms.Count == 0 || uiPrograms.Count != queueUIObjects.Count) return;

        for(int i = 0; i < uiPrograms.Count; i++)
        {
            int sortingOrder = LOWEST_SORTING_ORDER + uiPrograms.Count - i;
            uiPrograms[i].GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;
            //queueUIObjects[i].GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;
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

    void SetupQueueUIOnStart()
    {
        InstantiateQueueUI();
        InitializeQueueUIStates();
        SetQueueUIPositions();
        AssignQueueUIPositions();
        UpdateSortingOrders();
    }

    void InstantiateQueueUI()
    {
        if(uiPrograms.Count == 0) return;

        foreach(GameObject queueUI in queueUIObjects)
        {
            if(queueUI != null)
            {
                Destroy(queueUI);
            }
        }

        queueUIObjects.Clear();

        for(int i = 0; i < uiPrograms.Count; i++)
        {
            GameObject newQueueUI = Instantiate(queuePrefab, gameObject.transform);
            queueUIObjects.Add(newQueueUI);
        }   
    }

    public void UpdateQueueUIEndofRound(ProgramType programType)
    {
        if(queueUIObjects.Count < 1) return; 

        if(programType == uiType)
        {
            for(int i = 1; i < queueUIObjects.Count; i++)
            {
                queueUIStates[queueUIObjects[i]] = false;
            }
        }
        else
        {
            for(int i = 0; i < queueUIObjects.Count; i++)
            {
                queueUIStates[queueUIObjects[i]] = false;
            }
        }

        SetQueueUISprites();
    }

#region Position Methods

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

    void SetQueueUIPositions()
    {
        if(uiPositions.Count == 0) return;

        queueUIPositions.Clear();

        float currentProgramScalar = (CURRENT_PROGRAM_SCALAR - 1) / 2f;
        float xBounds = emptyProgramPrefab.GetComponent<SpriteRenderer>().sprite.rect.width / PIXELS_PER_UNIT * currentProgramScalar;

        Vector2 offset = (uiType == ProgramType.Attack) ? -queueUIOffsetVector : queueUIOffsetVector;
        xBounds = (uiType == ProgramType.Attack) ? -xBounds : xBounds;

        for(int i = 0; i < uiPositions.Count; i++)
        {
            if(i == 0)
            {
            queueUIPositions.Add(uiPositions[i] + offset + new Vector2(xBounds, 0));
            }
            else
            {
            queueUIPositions.Add(uiPositions[i] + offset);   
            }
        }
    }

    void AssignQueueUIPositions()
    {
        if(queueUIObjects.Count != queueUIPositions.Count) return;

        for(int i = 0; i < queueUIObjects.Count; i++)
        {
            queueUIObjects[i].transform.position = queueUIPositions[i];
        }
    }

#endregion

#region Set Sprite Methods

    void SetUISprites(int handSize)
    {
        if (programsListData == null || programsListData.drawnPrograms == null || 
        programsListData.drawnPrograms.Count == 0 || uiPrograms.Count == 0) return;

        for (int i = 0; i < handSize; i++)
        {
            uiPrograms[i].GetComponent<SpriteRenderer>().sprite = 
            programsListData.drawnPrograms[i].GetComponent<Program>().uiSprite;
        }
    }

    void SetQueueUISprites()
    {
        if(queueUIObjects.Count == 0 || queueUIStates == null) return;

        foreach(GameObject queueUI in queueUIObjects)
        {
            if(queueUIStates[queueUI] == true)
            {
                queueUI.GetComponent<QueueUI>().SetActive(uiType);
            }
            else
            {
                queueUI.GetComponent<QueueUI>().SetInactive();
            }     
        }
    }

#endregion

#region Lerp UI Methods

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

    void LerpQueueUIToPositions()
    {
        if(queueUIObjects.Count != uiPrograms.Count ||
        queueUIObjects.Count != queueUIPositions.Count) return;

        for(int i = GetInitialIndex(); i < GetProgramUICount(); i++)
        {
            queueUIObjects[i].GetComponent<LerpUIHandler>().LocationLerp(queueUIPositions[i], 25f); 
        }
    }

#endregion

#region Add/Remove Methods

    public void AddProgramsToHand(GameObject[] addPrograms)
    {
        programsListData.AddProgramsToHand(addPrograms);

        for(int i = 0; i < addPrograms.Length; i++)
        {
            GameObject newProgram = Instantiate(emptyProgramPrefab, gameObject.transform);
            mouseHoverStates.Add(newProgram, false);
            uiPrograms.Add(newProgram);

            GameObject newQueueUI = Instantiate(queuePrefab, gameObject.transform);
            queueUIStates.Add(newQueueUI, false);
            queueUIObjects.Add(newQueueUI);
        }

        int handSize  = uiPrograms.Count;

        SetUIPositions(handSize);
        SetQueueUIPositions();

        for(int i = 0; i < addPrograms.Length; i++)
        {
            uiPrograms[handSize - i - 1].transform.position = uiPositions[handSize - i - 1];
            queueUIObjects[handSize - i - 1].transform.position = queueUIPositions[handSize - i - 1];
        }

        LerpUIProgramsToPositions();
        LerpQueueUIToPositions();
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

                Destroy(queueUIObjects[indices[i]]);
                queueUIStates.Remove(queueUIObjects[indices[i]]);
                queueUIObjects.RemoveAt(indices[i]);
            }
        }

        int handSize = uiPrograms.Count;

        SetUIPositions(handSize);
        SetQueueUIPositions();
        LerpUIProgramsToPositions();
        LerpQueueUIToPositions();
        UpdateSortingOrders();
        SetUISprites(handSize);
    }

    #endregion

    //In future, encorporate logic for adding or removing cards into this script most likely, 
    //between destroying the current program and moving
    public void ScrollProgramUI()
    {
        if(uiPrograms == null || uiPrograms.Count == 0) return;

        Destroy(uiPrograms[0]);
        mouseHoverStates.Remove(uiPrograms[0]);
        uiPrograms.RemoveAt(0);

        ScrollQueueUIStates();
        SetQueueUISprites();

        programsListData.DrawOneNewProgram();

        //Manually setup a single program...
        GameObject newProgram = Instantiate(emptyProgramPrefab, gameObject.transform);
        mouseHoverStates[newProgram] = false;
        newProgram.transform.position = uiPositions[^1];
        newProgram.GetComponent<LerpUIHandler>().ParabolicScaleLerp(new Vector3(1.1f, 1.1f), 0.25f, 2f);
        uiPrograms.Add(newProgram);

        SetUISprites(uiPrograms.Count);

        SetUIPositions(uiPrograms.Count);
        SetQueueUIPositions();

        for(int i = 0; i < uiPrograms.Count; i++)
        {
            uiPrograms[i].GetComponent<LerpUIHandler>().LocationLerp(uiPositions[i], 25f);

            if(i == 0)
            {
                uiPrograms[i].GetComponent<LerpUIHandler>().ScaleLerp(new Vector2(2, 2), 25f);
            }
        }

        LerpQueueUIToPositions();
    }

    void ScrollQueueUIStates()
    {
        if(queueUIObjects.Count == 0 || queueUIObjects.Count <= 1 || 
        queueUIStates == null) return;

        for(int i = 0; i < queueUIObjects.Count - 1; i++)
        {
            queueUIStates[queueUIObjects[i]] = queueUIStates[queueUIObjects[i + 1]];
        }

        queueUIStates[queueUIObjects[queueUIObjects.Count - 1]] = false;
    }


    //This method also needs to talk to the other ProgramUI script and deactivate all of its Inactive States
    //for the programs removed from the queue
    void UpdateQueueUIInactiveStates(GameObject selectedQueueUI)
    {
        if(queueUIObjects.Count == 0 || queueUIObjects.Count != uiPrograms.Count ||
        queueUIStates == null) return;

        int index = queueUIObjects.IndexOf(selectedQueueUI);

        for(int i = index; i < queueUIObjects.Count; i++)
        {
            queueUIObjects[i].GetComponent<QueueUI>().SetInactive();
            queueUIStates[queueUIObjects[i]] = false;
        }
    }

    void UpdateNextQueueUIActiveState()
    {
        if(queueUIObjects.Count == 0 || queueUIStates == null) return;

        queueUIStates[queueUIObjects[GetInitialIndexNew()]] = true;
    }

#region Index Offseting Methods

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

        return 0;   
    }

    int GetInitialIndexNew()
    {
        if(queueUIObjects.Count == 0 || queueUIStates == null) return 0;

        int offset = 0;
        
        for(int i = 0; i < queueUIObjects.Count; i++)
        {
            if(queueUIStates[queueUIObjects[i]] == true)
            {
                offset++;
            }
            else
            {
                break;
            }
        }

        return Mathf.Min(offset, queueUIObjects.Count - 1);
    }

#endregion

    void SortByYPosition()
    {
        int startIndex = GetInitialIndexNew();
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

    private GameObject ClosestUIToMouse(List<GameObject> list)
    {
        if (list.Count == 0) return null;
        
        int startIndex = GetInitialIndexNew();
        if (startIndex >= list.Count) return null;
        
        GameObject closestObj = list[startIndex];
        float minDistance = Vector2.Distance(closestObj.transform.position, mouse.GetUIMousePosition());
        
        for (int i = startIndex + 1; i < list.Count; i++)
        {
            if (list[i] == null) continue;
            
            float currentDistance = Vector2.Distance(list[i].transform.position, mouse.GetUIMousePosition());
            
            if (currentDistance < minDistance)
            {
                minDistance = currentDistance;
                closestObj = list[i];
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

#region Initialize Dictionaries

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

    void InitializeQueueUIStates()
    {
        queueUIStates.Clear();

        if(queueUIObjects.Count == 0) return;
 
        for (int i = 0; i < queueUIObjects.Count; i++)
        {
            if (queueUIObjects[i] != null)
            {
                queueUIStates[queueUIObjects[i]] = false;
            }
        }
    }

#endregion

    void UpdateMouseHoverStates()
    {
        GameObject closest = ClosestUIToMouse(uiPrograms);

        List<GameObject> toEnter = new();
        List<GameObject> toExit = new();

        for (int i = GetInitialIndexNew(); i < GetProgramUICount(); i++)
        {
            if (uiPrograms[i] != null)
            {
                bool currentlyHovered = MouseDetected(uiPrograms[i]);
                bool isClosestAndHovered = currentlyHovered && (uiPrograms[i] == closest);
                bool previouslyClosestAndHovered = mouseHoverStates[uiPrograms[i]];

                mouseHoverStates[uiPrograms[i]] = isClosestAndHovered;

                if (isClosestAndHovered && !previouslyClosestAndHovered)
                    toEnter.Add(uiPrograms[i]);
                else if (!isClosestAndHovered && previouslyClosestAndHovered)
                    toExit.Add(uiPrograms[i]);
            }
        }

        // Always process exits first, then enters
        foreach (GameObject program in toExit)
            OnMouseExiting(program);

        foreach (GameObject program in toEnter)
            OnMouseEntering(program);
    }

    void OnMouseEntering(GameObject attackProgram)
    {
        if (attackProgram == heldProgram) return;

        attackProgram.GetComponent<SpriteRenderer>().sortingOrder = LOWEST_SORTING_ORDER + uiPrograms.Count + 1;
        descriptionManager.DisplayDescription(attackProgram, 
        programsListData.drawnPrograms[uiPrograms.IndexOf(attackProgram)].GetComponent<Program>(), uiType);   

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
        descriptionManager.DestroyTextElement();

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
        for (int i = GetInitialIndexNew(); i < GetProgramUICount(); i++)
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

