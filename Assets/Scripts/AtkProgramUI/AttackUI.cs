using UnityEngine;

public class AttackUI : MonoBehaviour
{
    //This script handles all of the visual UI, the actual change to the ordering of the attackProgram List will
    //be hangled in the Attack Data script (remove and insert in new postion, etc.)

    public int atkUICount = 5;

    private Vector2[] uiPositions = new Vector2[5];
    public GameObject[] attackPrograms = new GameObject[5];
    private GameObject heldProgram = null;

    public MouseTracker mouse;
    public AttackProgramsData attackProgramsData;

    private int heldProgramFirstIndex;

    void Start()
    {
        SetStartingUIPositions();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && MouseDetected(ClosestAtkUIToMouse()))
        {
            heldProgram = ClosestAtkUIToMouse();
            heldProgramFirstIndex = System.Array.IndexOf(attackPrograms, heldProgram);
        }

        if (Input.GetKey(KeyCode.Mouse0) && heldProgram != null)
        {
            heldProgram.GetComponent<LerpUIHandler>().LocationLerp(mouse.worldPosition, 25f);
            UpdateAttackProgramUI();
        }

        if (Input.GetKeyUp(KeyCode.Mouse0) && heldProgram != null)
        {
            int heldProgramLastIndex = System.Array.IndexOf(attackPrograms, heldProgram);

            heldProgram = null;
            UpdateAttackProgramUI();

            attackProgramsData.MoveAttackProgram(heldProgramFirstIndex, heldProgramLastIndex);
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            attackProgramsData.IncreaseCurrentAtkCount();
            SetUISprites();
        }
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
            if (i + attackProgramsData.currentAttackProgramAmount < attackProgramsData.totalAttackProgramAmount)
            {
                attackPrograms[i].GetComponent<SpriteRenderer>().sprite =
                attackProgramsData.attackPrograms[i + attackProgramsData.currentAttackProgramAmount].GetComponent<AttackData>().uiSprite;
                if (i != 0)
                {
                    attackPrograms[i].GetComponent<LerpUIHandler>().ParabolicScaleLerp(new Vector2(1.1f, 1.1f), 0.2f, 2f);
                }
                else
                {
                    attackPrograms[i].GetComponent<LerpUIHandler>().ParabolicScaleLerp(new Vector2(2.1f, 2.1f), 0.2f, 2f);
                }
            }
            else
            {
                attackPrograms[i].GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }

    int SetAtkCount()
    {
        if (attackProgramsData.totalAttackProgramAmount - attackProgramsData.currentAttackProgramAmount > 5)
        {
            return 5;
        }
        else
        {
            return attackProgramsData.totalAttackProgramAmount - attackProgramsData.currentAttackProgramAmount;
        }
    }

    void UpdateAttackProgramUI()
    {
        SortByYPosition();
        for (int i = 0; i < SetAtkCount(); i++)
        {
            if (attackPrograms[i] != heldProgram)
            {
                attackPrograms[i].GetComponent<LerpUIHandler>().LocationLerp(uiPositions[i], 25f);
            }

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

    void SortByYPosition()
    {
        int n = SetAtkCount();
        
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
        for(int i = 0; i < SetAtkCount(); i++)
        {
            if (Vector2.Distance(attackPrograms[i].transform.position, mouse.worldPosition) < distance)
            {
                distance = Vector2.Distance(attackPrograms[i].transform.position, mouse.worldPosition);
                closestObj = attackPrograms[i];
            }
        }
        return closestObj;
    }
    


}
