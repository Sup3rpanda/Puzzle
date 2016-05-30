using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {

	public void LoadGame ()	{
        //Application.LoadLevel ("Game");
        SceneManager.LoadScene("Arcade");
	}
}
