using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Praise : MonoBehaviour {

	public GameObject my_canvas;
	public Text my_text; 


	public bool show_count;

	public string[] big_explosion_praise;
		public AudioClip[] big_explosion_praise_sfx; //this must have Length = big_explosion_praise.Length or be 0
	public string[] combo_secondary_explosion_praise;
		public AudioClip[] combo_secondary_explosion_praise_sfx;  //this must have Length = combo_secondary_explosion_praise.Length or be 0
	public string[] combo_color_praise;
		public AudioClip[] combo_color_praise_sfx; //this must have Length = combo_color_praise.Length or be 0


	//these are used when board_C.continue_to_play_after_win_until_lose_happen is true, in order to tell to player that he/she have reach a win condition
	public string[] get_a_star; //this array length must be 3
	public AudioClip[] get_a_star_sfx; //this array length must be 3
	[HideInInspector]

	public Board_C board;
	/* if you have the menu kit, DELETE THIS LINE
	game_master my_game_master;
	// if you have the menu kit, DELETE THIS LINE*/

	public Text info_text;
	public float info_timer;
	public AudioClip gain_a_turn_sfx;


	void Start()
	{
		/* if you have the menu kit, DELETE THIS LINE
		if (game_master.game_master_obj)
			my_game_master = (game_master)game_master.game_master_obj.GetComponent("game_master");
		// if you have the menu kit, DELETE THIS LINE*/
	}

	public void Big_explosion(int explosion_magnitude)
	{

		string count = "";
		int adjust_count = 3;

		if (show_count)
			{
			count = "x"+explosion_magnitude.ToString()+" ";
			}

		if ((explosion_magnitude-adjust_count) > big_explosion_praise.Length)
			my_text.text = count+big_explosion_praise[big_explosion_praise.Length-1];
		else
			my_text.text = count+big_explosion_praise[explosion_magnitude-adjust_count-1];


		if (big_explosion_praise_sfx.Length > 0)
		{

			if ((explosion_magnitude-adjust_count) > big_explosion_praise_sfx.Length)
				board.Play_sfx(big_explosion_praise_sfx[big_explosion_praise.Length-1]);
			else
				board.Play_sfx(big_explosion_praise_sfx[explosion_magnitude-adjust_count-1]);
		}

		if(!my_canvas.GetComponent<Animation>().isPlaying)
			my_canvas.GetComponent<Animation>().Play("score_anim");
		
	}

	public void Combo_secondary_explosion(int combo_length)
	{

		string count = "";
		if (show_count && combo_length > 1)
			{
			count = "x"+combo_length.ToString()+" ";
			}

		if (combo_length > combo_secondary_explosion_praise.Length)
			my_text.text = count+combo_secondary_explosion_praise[combo_secondary_explosion_praise.Length-1];
		else
			my_text.text = count+combo_secondary_explosion_praise[combo_length-1];

		if (combo_secondary_explosion_praise_sfx.Length > 0)
		{
			if ((combo_secondary_explosion_praise.Length) > combo_secondary_explosion_praise_sfx.Length)
				board.Play_sfx(combo_secondary_explosion_praise_sfx[combo_secondary_explosion_praise_sfx.Length-1]);
			else
				board.Play_sfx(combo_secondary_explosion_praise_sfx[combo_length-1]);
		}

		if(!my_canvas.GetComponent<Animation>().isPlaying)
			my_canvas.GetComponent<Animation>().Play("score_anim");

	}

	public void Combo_color(int combo_length)
	{
		string count = "";
		if (show_count && combo_length > 1)
			{
			count = "x"+combo_length.ToString()+" ";
			}

		if (combo_length > combo_color_praise.Length)
			my_text.text = count+combo_color_praise[combo_color_praise.Length-1];
		else
			my_text.text = count+combo_color_praise[combo_length-1];

		if (combo_color_praise_sfx.Length > 0)
		{
			if ((combo_color_praise.Length) > combo_color_praise_sfx.Length)
				board.Play_sfx(combo_color_praise_sfx[combo_color_praise.Length-1]);
			else
				board.Play_sfx(combo_color_praise_sfx[combo_length-1]);
		}


		if(!my_canvas.GetComponent<Animation>().isPlaying)
			my_canvas.GetComponent<Animation>().Play("score_anim");
	}

	public void Win_and_continue_to_play(int n_star)
	{
		Debug.Log ("Win_and_continue_to_play "+ n_star);
		if (board.for_gain_a_star)
			{
			my_text.text = get_a_star[n_star];
			if (get_a_star_sfx.Length > 0)
				{
				if ((n_star) > get_a_star_sfx.Length)
					board.Play_sfx(get_a_star_sfx[get_a_star_sfx.Length-1]);
				else
					board.Play_sfx(get_a_star_sfx[n_star]);
				}

			if(my_canvas.GetComponent<Animation>().isPlaying)
				my_canvas.GetComponent<Animation>().Stop();

			my_canvas.GetComponent<Animation>().Play("score_anim");
			}

		/* if you have the menu kit, DELETE THIS LINE
		board.my_game_uGUI.New_star_score (n_star+1);
		// if you have the menu kit, DELETE THIS LINE */
			
	}

	public void Gain_a_turn(string name, int quantity)
	{



		string temp_text = name + " gain ";
		if (quantity > 1)
			temp_text += quantity.ToString () + " turns";
		else
			temp_text += "one turn";

		info_text.text = temp_text;
		info_text.gameObject.SetActive (true);
		Invoke ("Close_info", info_timer);

		if (gain_a_turn_sfx)
			board.Play_sfx(gain_a_turn_sfx);

	}

	void Close_info()
	{
		info_text.gameObject.SetActive (false);
	}


}
