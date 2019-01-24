using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{

    public Text movesText, scoreText;
    private int moveCounter;
    private int score;

    public int Score
    {
        get
        {
            return score;
        }

        set
        {
            score = value;
            scoreText.text = "Score: " + score;
        }
    }

    public int MoveCounter
    {
        get { return moveCounter; }
        set
        {
            moveCounter = value;
            movesText.text = "Moves: " + moveCounter;
        }
    }

    public static GUIManager sharedInstance;
    // Start is called before the first frame update
    void Start()
    {

        if (sharedInstance == null)
        {
            sharedInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        score = 0;
        moveCounter = 30;
        movesText.text = "Moves: " + moveCounter;
        scoreText.text = "Score: " + score;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
