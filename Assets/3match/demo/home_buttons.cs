using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class home_buttons : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	

	public void Alone_vs_timer_destroy_all_tiles()
	{
		SceneManager.LoadScene("solo_vs_time");
	}


	public void Alone_vs_moves_collect_gems()
	{
		SceneManager.LoadScene("solo_vs_moves");
	}


	public void HP_battle_against_enemy_AI()
	{
		SceneManager.LoadScene("HP_vs_AI");
	}

	public void Gems_battle_against_enemy_AI()
	{
		SceneManager.LoadScene("gems_vs_AI");
	}

	public void Reach_target_score()
	{
		SceneManager.LoadScene("target_score");
	}

	public void Destroy_all_blocks()
	{
		SceneManager.LoadScene("destroy_blocks");
	}

	public void Take_all_tokens()
	{
		SceneManager.LoadScene("take_all_tokens");
	}

	public void Diagonal()
	{
		SceneManager.LoadScene("diagonal");
	}

}
