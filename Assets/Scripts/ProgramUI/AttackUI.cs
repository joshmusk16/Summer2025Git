using UnityEngine;

public class AttackUI : MonoBehaviour
{
    //This script handles all of the visual UI, the actual change to the ordering of the attackProgram List will
    //be hangled in another script with that list (remove and insert in new postion, etc.)
    public readonly int atkUICount = 5;

    private Vector2[] uiPositions = new Vector2[5];
    public GameObject[] attackPrograms = new GameObject[5];
    private GameObject heldProgram = null;

    public MouseTracker mouse;

    void Start()
    {
        SetStartingUIPositions();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && MouseDetected(ClosestAtkUIToMouse()))
        {
            heldProgram = ClosestAtkUIToMouse();
        }

        if (Input.GetKey(KeyCode.Mouse0) && heldProgram != null)
        {
            heldProgram.transform.position = Vector2.Lerp(heldProgram.transform.position, mouse.worldPosition, Time.deltaTime * 15f);
            UpdateAttackProgramUI();
        }

        if (Input.GetKeyUp(KeyCode.Mouse0) && heldProgram != null)
        {
            heldProgram = null;
            UpdateAttackProgramUI();
        }
    }

    void SetStartingUIPositions()
    {
        for (int i = 0; i < atkUICount; i++)
        {
            uiPositions[i] = attackPrograms[i].transform.position;
        }
    }

    void UpdateAttackProgramUI()
    {
        SortByYPosition();
        for (int i = 0; i < atkUICount; i++)
        {
            if (attackPrograms[i] != heldProgram)
            {
                attackPrograms[i].GetComponent<LerpUIHandler>().LocationLerp(uiPositions[i], 20f);
            }

            if (i == 0)
            {
                attackPrograms[i].GetComponent<LerpUIHandler>().ScaleLerp(new Vector2(2, 2), 20f);
            }
            else
            {
                attackPrograms[i].GetComponent<LerpUIHandler>().ScaleLerp(new Vector2(1, 1), 20f);
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
        float xBounds = obj.GetComponent<SpriteRenderer>().sprite.rect.width * obj.transform.localScale.x / 32f;
        float yBounds = obj.GetComponent<SpriteRenderer>().sprite.rect.height * obj.transform.localScale.y / 32f;

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

    GameObject ClosestAtkUIToMouse()
    {
        float distance = Vector2.Distance(attackPrograms[0].transform.position, mouse.worldPosition);
        GameObject closestObj = attackPrograms[0];
        foreach (GameObject attackProgram in attackPrograms)
        {
            if (Vector2.Distance(attackProgram.transform.position, mouse.worldPosition) < distance)
            {
                distance = Vector2.Distance(attackProgram.transform.position, mouse.worldPosition);
                closestObj = attackProgram;
            }
        }
        return closestObj;
    }
    


}
