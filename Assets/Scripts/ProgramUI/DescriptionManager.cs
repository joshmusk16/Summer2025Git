using UnityEngine;

public class DescriptionManager : TextElement
{
    public const float OFFSET_FROM_PROGRAM = 2;

    public void DisplayDescription(GameObject uiObject, Program program, ProgramType programType)
    {
        headerInput = program.programName;
        textInput = program.programDescription;

        GenerateTextElement();

        gameObject.transform.position = uiObject.transform.position;

        Vector3 leftAnchorOffset = new((-BACKGROUND_BUFFER / 2f) - OFFSET_FROM_PROGRAM, -maxTextHeight / 2f);
        Vector3 rightAnchorOffset = new(maxTextWidth + (BACKGROUND_BUFFER / 2f) + OFFSET_FROM_PROGRAM, -maxTextHeight / 2f);

        if(programType == ProgramType.Attack)
        {
            gameObject.transform.position -= leftAnchorOffset;
        }
        else if(programType == ProgramType.Defense)
        {
            gameObject.transform.position -= rightAnchorOffset;
        }

    }

}
