using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameController : MonoBehaviour {

    //Game Object vars
    public GameObject player;
    public GameObject fieldController;
    public GameObject fxController;
    public Text timeUI, scoreUI, hiScoreUI, roundUI, speedUI, difficultyUI, stopUI;

    FieldController fieldScript;
    BlockScript blockScript;
    FXController fxControllerScript;


    //Level Settings
    string gameType = "Endless";
    int difficulty = 1;
    int roundsBestOf = 3;
    int roundsPlayer = 0;
    int roundsOpponent = 0;

    //Game Setting vars
    public float matchDelay = 1f;
    public float comboGrace = .75f;
    float elapsedTime = 0f;
    float nextPushTime;
    public float timeToPush = 3f;
    public float pushCooldown = .5f;
    float pushCooldownTime;

    float stop = 0f;
    float stopMax = 8f;

    public bool isLosingPlayer = false;
    public bool isLosingOpponent = false;
    float loseTime = 0f;
    public float timeToLose = 2f;

    int score = 0;

    // Use this for initialization
    void Start () {

        fieldScript = GameObject.FindGameObjectWithTag("Field").GetComponent<MonoBehaviour>() as FieldController;
        fieldScript.gameControllerScript = this;
        nextPushTime = timeToPush;

        fxControllerScript = GameObject.FindGameObjectWithTag("FX").GetComponent<MonoBehaviour>() as FXController;
    }

    // Update is called once per frame
    void Update () {
        elapsedTime += Time.deltaTime;
        timeUI.text = elapsedTime.ToString();

        //Stop timing
        if (stop > 0)
        {
            stop -= Time.deltaTime;
            stopUI.text = stop.ToString();
        }
        else
        {
            stop = 0f;
            stopUI.text = stop.ToString();

            //Push timing
            if (elapsedTime > nextPushTime )
            {
                Push();
            }

            //Lose timing
            if (isLosingPlayer == true)
            {

                if (elapsedTime > loseTime && loseTime > 0)
                {
                    print("LOSE");
                    fxController.SendMessage("Lose");
                    Time.timeScale = 0f;
                }
            }
        }
    }

    public void PushManual()
    {
        if ( pushCooldownTime < elapsedTime )
        {
            Push();
            pushCooldownTime = elapsedTime + pushCooldown;
        }
    }

    public void Push()
    {
        fieldScript.Push(this);
        PushTimeReset();
    }

    public void PushTimeReset()
    {
        nextPushTime = elapsedTime + timeToPush;
        StopTimeReset();
    }

    public void StopTimeReset()
    {
        stop = 0;
    }

    public void StartLose()
    {
        loseTime = elapsedTime + timeToLose;
        isLosingPlayer = true;
        fxController.SendMessage("Losing");
        //print("START LOSE: Elapsed: " + elapsedTime + "     Lose Time: " + loseTime);

    }

    public void StopLose()
    {
        loseTime = 0f;
        isLosingPlayer = false;
        //print("STOP LOSE: Elapsed: " + elapsedTime + "     Lose Time: " + loseTime);
    }

    public void MatchScore(int matchSize, int comboSize = 0)
    {
        if (comboSize > 0)
        {
            score += 2 ^ comboSize * 40;
            score += 2 ^ matchSize * 15;

            scoreUI.text = score.ToString();
        }
        else if (matchSize > 0)
        {
            score += Mathf.RoundToInt(Mathf.Pow(2, matchSize)) * 15;
            scoreUI.text = score.ToString();

        }

        Stop(matchSize, comboSize);
    }

    public void Stop(int matchSize = 0, int comboSize = 0, float timeStopOverride = 0)
    {
        if (timeStopOverride == 0)
        {
            float stopBonusTime = Mathf.RoundToInt(Mathf.Pow(2, matchSize)) * 0.03f;
            stop += stopBonusTime;
            nextPushTime += stopBonusTime;
            timeToLose += stopBonusTime;
            stopUI.text = stop.ToString();
        }
        else if (timeStopOverride > 0)
        {
            stop += timeStopOverride;
        }
    }
}
