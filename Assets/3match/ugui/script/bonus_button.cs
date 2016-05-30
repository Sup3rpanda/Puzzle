using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class bonus_button : MonoBehaviour {

	public bool player;

	public int slot_number;
	public Image full_image;
		Color base_color;

	public Button my_button;
	//public Toggle my_toggle;

	public int cost;
	Board_C board;


	// Use this for initialization
	void Start () {
		my_button.enabled = false;

		full_image.sprite =  GetComponent<Image>().sprite;
		base_color = full_image.color;
		board = Board_C.this_board.GetComponent<Board_C>();

		if (player)
			cost = board.bonus_cost[slot_number];
		else
			cost = board.enemy_bonus_cost[slot_number];

		Update_fill();

	}

	public void Update_fill()
	{
		if (player)
			{
			full_image.fillAmount = Mathf.Lerp(0,1, (float)board.filling_player_bonus[slot_number]/(float)cost);

			if (full_image.fillAmount == 1)
				my_button.enabled = true;
			else
				my_button.enabled = false;
			}
		else
			{
			full_image.fillAmount = Mathf.Lerp(0,1, (float)board.filling_enemy_bonus[slot_number]/(float)cost);

			}
		//full_image.rectTransform.sizeDelta = GetComponent<RectTransform>().sizeDelta;
		//full_image.rectTransform.pivot = new Vector2(0,0);

	}

	public void Click_me()
	{
		if (board.player_can_move)
			{
			if (board.bonus_select == Board_C.bonus.none) 
				Activate();
			else
				{
				if (board.slot_bonus_ico_selected == slot_number) //if you re-click on me, then deselect me
					Deselect();
				else //deselect previous slot, and activate this
					{
					board.gui_bonus_ico_array[board.slot_bonus_ico_selected].GetComponent<bonus_button>().Deselect();
					Activate();
					}
				}
			}
		else
			{
			if (board.bonus_slot[slot_number] == Board_C.bonus.give_more_time)//you can use time bonus even when gems are falling
				Activate();
			}
	}

	public void Deselect()
		{
		board.bonus_select = Board_C.bonus.none;

		if (player)
			{
			board.slot_bonus_ico_selected = -1;
			full_image.color = base_color;
			}
		else
			{
			full_image.color = base_color;
			}

		board.main_gem_selected_x = -10;
		board.main_gem_selected_y = -10;
		board.main_gem_color = -10;
		board.minor_gem_destination_to_x = -10;
		board.minor_gem_destination_to_y = -10;
		board.minor_gem_color = -10;
		}

	public void Activate()
	{
		if (board.player_turn)
			board.bonus_select = board.bonus_slot[slot_number];

		if (board.bonus_select == Board_C.bonus.give_more_time)
			{
			board.Add_time_bonus(board.add_time_bonus);
			Reset_fill();
			board.Play_bonus_sfx(8,true);
			}
		else if (board.bonus_select == Board_C.bonus.give_more_moves)
			{
			board.Gain_turns(board.add_moves_bonus);
			Reset_fill();
			board.Play_bonus_sfx(9,true);
			}
		else if (board.bonus_select == Board_C.bonus.heal_hp)
			{
			board.Heal_me();
			Reset_fill();
			board.Play_bonus_sfx(10,true);
			}
		else
			{
			board.slot_bonus_ico_selected = slot_number;
			full_image.color = Color.white;
			}
	}

	public void Reset_fill()
	{
		Deselect();

		if (player)
			{
			board.filling_player_bonus[slot_number] = 0;
			board.bonus_ready[slot_number] = false;
			}
		else
			{
			board.filling_enemy_bonus[slot_number] = 0;
			board.enemy_bonus_ready[slot_number] = false;
			}

		Update_fill();
	}
	

}
