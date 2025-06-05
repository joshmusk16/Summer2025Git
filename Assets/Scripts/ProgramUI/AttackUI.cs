using UnityEngine;

public class AttackUI : MonoBehaviour
{
    //This script handles all of the visual UI, the actual change to the ordering of the attackProgram List will
    //be hangled in another script with that list (remove and insert in new postion, etc.)

    private Vector2[] uiPositions = new Vector2[4];
    public GameObject[] attackPrograms = new GameObject[4];
    private GameObject heldProgram = null;

    private Vector2 mouseWorldPosition;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        SetStartingUIPositions();
    }

    void Update()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;

        mouseWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(
                mouseScreenPosition.x,
                mouseScreenPosition.y,
                Mathf.Abs(mainCamera.transform.position.z)));

        if (Input.GetKeyDown(KeyCode.Mouse0) && MouseDetected(ClosestAtkUIToMouse()))
        {
            heldProgram = ClosestAtkUIToMouse();
        }

        if (Input.GetKey(KeyCode.Mouse0) && heldProgram != null)
        {
            heldProgram.transform.position = Vector2.Lerp(heldProgram.transform.position, mouseWorldPosition, Time.deltaTime * 15f);
        }

        if (Input.GetKeyUp(KeyCode.Mouse0) && heldProgram != null)
        {
            heldProgram = null;
        }

        UpdateAttackProgramUI();
    }

    void SetStartingUIPositions()
    {
        for (int i = 0; i < 4; i++)
        {
            uiPositions[i] = attackPrograms[i].transform.position;
        }
    }

    void UpdateAttackProgramUI()
    {
        SortByYPosition();
        for (int i = 0; i < 4; i++)
        {
            if (attackPrograms[i] != heldProgram)
            {
                attackPrograms[i].transform.position = Vector2.Lerp(attackPrograms[i].transform.position, uiPositions[i], Time.deltaTime * 15f);
            }
        }
    }

    void SortByYPosition()
    {
        if (heldProgram != null)
        {
        int n = attackPrograms.Length;

            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - i - 1; j++)
                {
                    if (attackPrograms[j].transform.position.y < attackPrograms[j + 1].transform.position.y)
                    {
                        GameObject temp = attackPrograms[j];
                        attackPrograms[j] = attackPrograms[j + 1];
                        attackPrograms[j + 1] = temp;
                    }
                }
            }
        }
    }

    bool MouseDetected(GameObject obj)
    {
        float xBounds = obj.GetComponent<SpriteRenderer>().sprite.rect.width / 32f;
        float yBounds = obj.GetComponent<SpriteRenderer>().sprite.rect.height / 32f;

        if (mouseWorldPosition.x > obj.transform.position.x - xBounds
        && mouseWorldPosition.x < obj.transform.position.x + xBounds
        && mouseWorldPosition.y > obj.transform.position.y - yBounds
        && mouseWorldPosition.y < obj.transform.position.y + yBounds)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    GameObject ClosestAtkUIToMouse()
    {
        float distance = Vector2.Distance(attackPrograms[0].transform.position, mouseWorldPosition);
        GameObject closestObj = attackPrograms[0];
        foreach (GameObject attackProgram in attackPrograms)
        {
            if (Vector2.Distance(attackProgram.transform.position, mouseWorldPosition) < distance)
            {
                distance = Vector2.Distance(attackProgram.transform.position, mouseWorldPosition);
                closestObj = attackProgram;
            }
        }
        return closestObj;
    }
    


}
