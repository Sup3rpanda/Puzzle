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
    public Text timeUI, scoreUI, hiScoreUI, roundUI, speedValueUI, difficultyUI, difficultyValueUI, stopUI, stopValueUI, readyUI, winLoseUI;
    public GameObject loadingScreen;

    FieldController fieldScript;
    FXController fxControllerScript;

    //Level Settings
    public GameType gameType = GameType.Endless;
    public GameMode gameMode = GameMode.Classic;
    public GameState gameState = GameState.Loading;

    #region General Gameplay Vars
    public float matchDelay = 1f;
    float elapsedTime = 0f;         //Total time game is in Playing mode
    int score = 0;                  //Score of matched blocks
    int speed = 1;                  //Current speed level
    int speedMax = 10;              //Maximum speed level
    float nextSpeedIncreaseTime;    //The elapsed time when a speed increase will happen
    float speedIncreaseInterval = 30f; //How long in between speed increases
    #endregion
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
    #region Push Vars
    float nextPushTime;         //The elapsed time when push will trigger again
    float pushInterval = 10f;    //How long between pushes
    float pushCooldown = .3f;   //How long after a manual push another can happen
    float pushCooldownTime;     //Brief time where push cant happen too quickly in succession
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

        //Initialize basic stuff
        fieldScript = GameObject.FindGameObjectWithTag("Field").GetComponent<MonoBehaviour>() as FieldController;
        fieldScript.gameControllerScript = this;
        nextSpeedIncreaseTime = speedIncreaseInterval;
        nextPushTime = pushInterval;
        fxControllerScript = GameObject.FindGameObjectWithTag("FX").GetComponent<MonoBehaviour>() as FXController;
        loadingScreen = GameObject.FindGameObjectWithTag("LoadingScreen");

        //Initialize Mode specific stuff
        if ( gameType == GameType.Endless)
        {
            roundUI.enabled = false;
            difficultyUI.enabled = false;
            difficultyValueUI.enabled = false;
        }
        else if ( gameType == GameType.Arcade)
        {
            scoreUI.enabled = false;
            hiScoreUI.enabled = false;
        }

    }

    // Update is called once per frame
    void Update () {

        if (gameState == GameState.Playing)
        {
            elapsedTime += Time.deltaTime;
            timeUI.text = elapsedTime.ToString();

            //Speed timing
            if ( elapsedTime > nextSpeedIncreaseTime )
            {
                SpeedIncrease();
            }

            //Stop timing
            if (stop > 0)
            {
                stop -= Time.deltaTime;
                stopValueUI.text = stop.ToString();
            }
            else
            {
                StopTimeReset();

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

    #region State Control

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
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(false);
        }
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
        fxControllerScript.SendMessage("Lose");
        winLoseUI.enabled = true;
        gameState = GameState.Lost;
    }

    void StateStartWon()
    {
        fxController.SendMessage("Win");
        gameState = GameState.Won;
    }
    #endregion

    public void SpeedIncrease()
    {
        if ( speed < speedMax)
        {
            speed++;
            speedValueUI.text = speed.ToString();
        }
        SpeedTimeReset();
    }

    void SpeedTimeReset()
    {
        nextSpeedIncreaseTime = elapsedTime + speedIncreaseInterval;
    }

    public void PushManual()
    {
        if ( pushCooldownTime < elapsedTime )
        {
            Push();
            pushCooldownTime = elapsedTime + pushCooldown;
        }
    }

    void Push()
    {
        fieldScript.Push(this);
        PushTimeReset();
        fxControllerScript.SendMessage("Push");
    }

    public void PushTimeReset()
    {
        nextPushTime = elapsedTime + (1 + pushInterval - speed);
        StopTimeReset();
    }

    public void StopTimeReset()
    {
        stop = 0f;
        stopValueUI.text = stop.ToString();
        stopUI.enabled = false;
        stopValueUI.enabled = false;
    }

    public void StartLose()
    {
        loseTime = elapsedTime + timeToLose;
        isLosingPlayer = true;
        fxControllerScript.SendMessage("Losing");
        //print("START LOSE: Elapsed: " + elapsedTime + "     Lose Time: " + loseTime);
    }

    public void StopLose()
    {
        loseTime = 0f;
        isLosingPlayer = false;
        //print("STOP LOSE: Elapsed: " + elapsedTime + "     Lose Time: " + loseTime);
    }

    void Stop(int matchSize = 0, int comboSize = 0, float timeStopOverride = 0)
    {
        if (timeStopOverride == 0)
        {
            float stopBonusTime = Mathf.RoundToInt(Mathf.Pow(2, matchSize)) * 0.03f;
            stop += stopBonusTime;

            if (stop > stopMax)
            {
                stop = stopMax;
            }

            nextPushTime += stopBonusTime;
            timeToLose += stopBonusTime;
            stopValueUI.text = stop.ToString();

            stopUI.enabled = true;
            stopValueUI.enabled = true;
        }
        else if (timeStopOverride > 0)
        {
            stop += timeStopOverride;
        }
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

}
