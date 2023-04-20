using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputBuffer : MonoBehaviour
{
    
    //The input buffer
    List<ActionItem> inputBuffer = new List<ActionItem>();

    //set to true whenever we want to process actions from the input buffer, set to false when an action has to wait in the buffer
    //Notice I don't have any code here which sets actionAllowed to true, because that probably depends on states like in the middle of throwing a punch animation, or checking if a jump has finished, or whatever
    bool actionAllowed;  

    void Update()
    {
        checkInput();
        if (actionAllowed)
        {
            tryBufferedAction();
        }
    }

    //Check inputs here, and add them to the input buffer
    void checkInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            inputBuffer.Add(new ActionItem(ActionItem.InputAction.Jump, Time.time));
        }
        //Add checks for all other inputs here
    }

    //Call when we want to process the inputBuffer
    void tryBufferedAction()
    {
        if (inputBuffer.Count > 0)
        {
            foreach (ActionItem ai in inputBuffer.ToArray())  //Using ToArray so we iterate a copy of the list rather than the actual list, since we will be modifying the list in the loop
            {
                inputBuffer.Remove(ai);  //Remove it from the buffer
                if (ai.CheckIfValid())
                {
                    //Means the action is still within the allowed time, so we do the action and then break from processing more of the buffer
                    doAction(ai);
                    break;  //We probably only want to do 1 action at a time, so we just break here and don't process the rest of the inputBuffer
                }
            }
        }
    }

    void doAction(ActionItem ai)
    {
        //code to jump, punch, kick, etc

        if (ai.Action == ActionItem.InputAction.Jump)
        {

        }

        actionAllowed = false; 


        //Every action probably has some kind of wait period until the next action is allowed, so we set this to false here.
        //Some code somewhere else needs to be written to set it back to true
    }
}
