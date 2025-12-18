using UnityEngine;

public class DebugAddAndRemove : MonoBehaviour
{

    public ProgramUI attackUI;
    public ProgramUI defenseUI;

    public GameObject[] addPrograms;
    public int[] addIndices;
    public int[] removeIndices;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            attackUI.AddProgramsToHand(addPrograms, addIndices);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            attackUI.RemoveProgramsFromHand(removeIndices);
        }
    }

}
