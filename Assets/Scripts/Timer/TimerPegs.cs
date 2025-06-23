using System.Collections.Generic;
using UnityEngine;

public class TimerPegs : MonoBehaviour
{

public GameObject peg;
public GameObject timer;
private List<GameObject> pegs = new List<GameObject>();
private TimerDisplay timerScript;

void Start()
{
    SpawnPegs(40);
    timerScript = timer.GetComponent<TimerDisplay>();
}

void Update()
{
    UpdatePegs();    
}


    void SpawnPegs(int amountOfPegs){
        if(amountOfPegs > 40 || amountOfPegs < 1){
            return;
        }
        else{
            for(int i = 0; i < amountOfPegs; i++){
                float x = (13 + (19 * i)) / 16f;
                GameObject tempPeg = Instantiate(peg, gameObject.transform.position + new Vector3 (x, 0), Quaternion.identity, gameObject.transform);
                tempPeg.name = "Peg " + i;
                pegs.Add(tempPeg);
            }
        }
    }

    void UpdatePegs(){
        int numOfActivePegs = Mathf.CeilToInt(timerScript.currentTime / timerScript.totalTime * 40f);
        for(int i = 0; i < 40; i++){
            if(i < numOfActivePegs){
                pegs[i].GetComponent<SpriteRenderer>().enabled = true;
            }
            else{
                pegs[i].GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }

}
