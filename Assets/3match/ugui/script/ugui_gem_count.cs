using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ugui_gem_count : MonoBehaviour {

	public bool player;
	public Text[] my_text;
	Board_C board;

	// Use this for initialization
	void Start () {
		board = Board_C.this_board.GetComponent<Board_C>();
	}
	
	// Update is called once per frame
	void Update () {
		for (int n = 0; n < board.gem_length ; n++)
		{
			if (player)
				my_text[n].text = board.number_of_gems_collect_by_the_player[n] 
				+ " / " + board.number_of_gems_to_destroy_to_win[n];
			else
				my_text[n].text = board.number_of_gems_collect_by_the_enemy[n] 
				+ " / " + board.number_of_gems_to_destroy_to_win[n];
		}
	}
}
