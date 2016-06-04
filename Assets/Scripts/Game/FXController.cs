using UnityEngine;
using System.Collections;

public class FXController : MonoBehaviour {

    //Game Object vars
    AudioSource audioSource;
    public AudioClip aStartGame;
    public AudioClip aLosing;
    public AudioClip aLose;
    public AudioClip aWon;
    public AudioClip aBlockSwap;
    public AudioClip aCombo;
    public AudioClip aMatch;
    public AudioClip aBlockMatch;


    // Use this for initialization
    void Start () {
        audioSource = GameObject.FindObjectOfType<AudioSource>() as AudioSource;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void StartGame()
    {
        audioSource.PlayOneShot(aStartGame);
    }

    void Losing()
    {
        audioSource.PlayOneShot(aLosing);
    }

    void Lose()
    {
        audioSource.PlayOneShot(aLose);
    }

    void Won()
    {
        audioSource.PlayOneShot(aWon);
    }

    void BlockSwap()
    {
        audioSource.PlayOneShot(aBlockSwap);
    }

    void Combo()
    {
        audioSource.PlayOneShot(aCombo);
    }

    void Match()
    {
        audioSource.PlayOneShot(aMatch);
    }

    void BlockMatch()
    {
        audioSource.PlayOneShot(aBlockMatch);
        
    }
}
