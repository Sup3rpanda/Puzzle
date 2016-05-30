using UnityEngine;
using System.Collections;

public class FXController : MonoBehaviour {

    //Game Object vars
    AudioSource audioSource;
    public AudioClip aLosing;
    public AudioClip aLose;
    public AudioClip aWin;
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

    void Losing()
    {
        audioSource.PlayOneShot(aLosing);
    }

    void Lose()
    {
        audioSource.PlayOneShot(aLose);
    }

    void Win()
    {
        audioSource.PlayOneShot(aWin);
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
