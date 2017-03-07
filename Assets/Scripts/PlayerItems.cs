using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerItems : MonoBehaviour {

    private int spear;
    private int ladder;

    // Use this for initialization
    void Start () {
        //Get the current food point total stored in GameManager.instance between levels.
        //spear = GameManager.instance.playerFoodPoints;
        spear = 0;
        ladder = 0;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    //OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
    private void OnTriggerEnter2D(Collider2D other)
    {
        ////Check if the tag of the trigger collided with is Exit.
        //if (other.tag == "Exit")
        //{
        //    //Invoke the Restart function to start the next level with a delay of restartLevelDelay (default 1 second).
        //    Invoke("Restart", restartLevelDelay);

        //    //Disable the player object since level is over.
        //    enabled = false;
        //}

        //Check if the tag of the trigger collided with is Spear.
        if (other.tag == "Spear")
        {
            //Add spear to the players current spears total.
            spear++;
            print("Spear count:" + spear);

            //Disable the spear object the player collided with.
            other.gameObject.SetActive(false);
        }
        //Check if the tag of the trigger collided with is Ladder.
        else if (other.tag == "Ladder")
        {
            //Add Ladder to the players current Ladder total.
            ladder++;
            print("Ladder count:" + ladder);

            //Disable the Ladder object the player collided with.
            other.gameObject.SetActive(false);
        }
        //Check if the tag of the trigger collided with is Monster.
        else if (other.tag == "Monster")
        {
            if (spear <= 0)
            {
                //MonoBehaviour.Destroy(this);
                print("Game over...:");
                SceneManager.LoadScene(0);
            }
            else
            {
                //Removes ladder from the players current ladder total.
				spear--;
                print("Spear count:" + spear);

                //Disable the food object the player collided with.
                other.gameObject.SetActive(false);
            }
        }
        else if (other.tag == "Chest")
        {
            SceneManager.LoadScene(1);
        }
    }
}
