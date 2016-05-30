using UnityEngine;
using System.Collections;
using  UnityEngine.UI;

public class ugui_armor : MonoBehaviour {

	public bool player;
	public Text[] my_text;
	Board_C board;


	// Use this for initialization
	void Start () {
		board = Board_C.this_board.GetComponent<Board_C>();


		for (int n = 0; n < board.gem_length ; n++)
		{
			if (player)
				my_text[n].text = Board_C.this_board.GetComponent<Board_C>().player_armor[n].ToString() ;
			else
				my_text[n].text = Board_C.this_board.GetComponent<Board_C>().enemy_armor[n].ToString();
		}

	}
	

}
