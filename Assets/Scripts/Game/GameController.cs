using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public enum GameType { Endless, Arcade };
public enum GameMode { Classic, Plus };
public enum GameState { Loading, Paused, Playing, Countdown, Lost, Won }

public class GameController : MonoBehaviour {

    //Game Object vars
    public GameObject player;
    public GameObject fieldController;
    public GameObject fxController;
    public Text timeUI, scoreUI, hiScoreUI, roundUI, speedUI, difficultyUI, stopUI, readyUI;

    FieldController fieldScript;
    FXController fxControllerScript;

    //Level Settings
    public GameType gameType = GameType.Endless;
    public GameMode gameMode = GameMode.Classic;
    public GameState gameState = GameState.Loading;
    int score = 0;
    #region Vs Vars
    int difficulty = 1;
    int roundsBestOf = 3;
    int roundsPlayer = 0;
    int roundsOpponent = 0;
    #endregion
    #region Countdown Vars
    float countdownDelay = 1f;
    float countdownElapsed = 0f;
    #endregion
    //Game Setting vars
    public float matchDelay = 1f;
    public float comboGrace = .75f;
    float elapsedTime = 0f;
    #region Push Vars
    float nextPushTime;
    public float timeToPush = 5f;
    public float pushCooldown = .5f;
    float pushCooldownTime;
    #endregion
    #region Stop Vars
    float stop = 0f;
    float stopMax = 8f;
    #endregion
    #region Losing Vars
    public bool isLosingPlayer = false;
    public bool isLosingOpponent = false;
    float loseTime = 0f;
    public float timeToLose = 2f;
    #endregion

    // Use this for initialization
    void Start () {

        fieldScript = GameObject.FindGameObjectWithTag("Field").GetComponent<MonoBehaviour>() as FieldController;
        fieldScript.gameControllerScript = this;
        nextPushTime = timeToPush;

        fxControllerScript = GameObject.FindGameObjectWithTag("FX").GetComponent<MonoBehaviour>() as FXController;
    }

    // Update is called once per frame
    void Update () {

        if (gameState == GameState.Playing)
        {
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
                if (elapsedTime > nextPushTime)
                {
                    Push();
                }

                //Lose timing
                if (isLosingPlayer == true)
                {
                    if (elapsedTime > loseTime && loseTime > 0)
                    {
                        StateStartLost();
                    }
                }
            }
        }

        #region State Control
        //All other non playing states
        if (gameState == GameState.Loading && SceneManager.GetActiveScene().isLoaded)
        {
            StateStartCountdown();
        }
        if (gameState == GameState.Countdown)
        {
            countdownElapsed += Time.deltaTime;

            if (countdownElapsed > countdownDelay)
            {
                StateStartPlaying();
            }
        }
        if ( Input.GetButtonDown("Cancel") )
        {
            TogglePaused();
        }
    }

    public bool IsPlayable()
    {
        if ( gameState == GameState.Playing)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void TogglePaused()
    {
        if (gameState == GameState.Playing)
        {
            readyUI.text = "Paused, ESC to Continue";
            readyUI.enabled = true;
            gameState = GameState.Paused;
        }
        else if (gameState == GameState.Paused)
        {
            StateStartPlaying();
        }
    }

    void StateStartCountdown()
    {
        readyUI.enabled = true;
        gameState = GameState.Countdown;
    }

    void StateStartPlaying()
    {
        fxControllerScript.SendMessage("StartGame");
        readyUI.enabled = false;
        gameState = GameState.Playing;
    }

    void StateStartPaused()
    {
        gameState = GameState.Paused;
    }

    void StateStartLost()
    {
        fxController.SendMessage("Lose");
        gameState = GameState.Lost;
    }

    void StateStartWon()
    {
        fxController.SendMessage("Win");
        gameState = GameState.Won;
    }
    #endregion

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

            if ( stop > stopMax)
            {
                stop = stopMax;
            }

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
