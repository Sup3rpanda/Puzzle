using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class back_to_home : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	public void Back_to_home()
	{
		
		SceneManager.LoadScene("home_demo");
	}
}
