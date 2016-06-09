using UnityEngine;
using System.Collections;

public class FXController : MonoBehaviour {

    //Game Object vars
    AudioSource audioSource;

    // Use this for initialization
    void Start () {
        audioSource = GameObject.FindObjectOfType<AudioSource>() as AudioSource;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    #region Game States and Actions
    public AudioClip aStartGame;
    public AudioClip aLosing;
    public AudioClip aLose;
    public AudioClip aWon;
    public AudioClip aPush;

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

    void Push()
    {
        audioSource.PlayOneShot(aPush);
    }
    #endregion

    #region Blocks and Matching
    public AudioClip aBlockSwap;
    public AudioClip aBlockDrop;
    public AudioClip aCombo;
    public AudioClip aMatch;
    public AudioClip aMatchBlock;

    void BlockSwap()
    {
        audioSource.PlayOneShot(aBlockSwap);
    }

    void BlockDrop()
    {
        audioSource.PlayOneShot(aBlockDrop);
    }

    void Combo()
    {
        audioSource.PlayOneShot(aCombo);
    }

    void Match()
    {
        audioSource.PlayOneShot(aMatch);
    }

    public void MatchBlock(BlockScript block, float delay)
    {
        block.state = BlockState.Match;
        block.blockRenderer.material.color = new Color(.75f, .75f, .75f, .5f);

        this.Invoke("MatchBlockResolve", delay);
        block.Invoke("MatchBlockResolve", delay);
    }

    void MatchBlockResolve()
    {
        audioSource.PlayOneShot(aMatchBlock);
    }
    #endregion

}
