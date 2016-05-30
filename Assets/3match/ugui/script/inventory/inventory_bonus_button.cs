using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class inventory_bonus_button : MonoBehaviour {


	public int my_id;
	public bonus_inventory my_inventory;
	Image my_image;
	Color base_color;


	void Start()
		{
		my_image = GetComponent<Image>();
		base_color = my_image.color;
		}

	public void Click_me()
		{
		if (my_inventory.board.player_can_move)
			{
			if (my_inventory.board.bonus_select == Board_C.bonus.none) 
				Activate();
			else
				{
				if (my_inventory.board.bonus_select == (Board_C.bonus)my_id) //if you re-click on me, then deselect me
					Deselect();
				else //deselect previous slot, and activate this
					{
					my_inventory.bonus_list[(int)my_inventory.board.bonus_select].GetComponent<inventory_bonus_button>().Deselect();
					Activate();
					}
				}
			}
		}

	public void Activate()
		{
			my_inventory.board.bonus_select = (Board_C.bonus)my_id;

		if (my_inventory.board.bonus_select == Board_C.bonus.give_more_time)
			{
			my_inventory.board.Add_time_bonus(my_inventory.board.add_time_bonus);
			my_inventory.board.player_bonus_inventory[my_id]--;
			my_inventory.Update_bonus_count(my_id);
			my_inventory.board.bonus_select = Board_C.bonus.none;
			my_inventory.board.Play_bonus_sfx(8,true);
			}
		else if (my_inventory.board.bonus_select == Board_C.bonus.give_more_moves)
			{
			my_inventory.board.Gain_turns(my_inventory.board.add_moves_bonus);

			my_inventory.board.player_bonus_inventory[my_id]--;
			my_inventory.Update_bonus_count(my_id);
			my_inventory.board.bonus_select = Board_C.bonus.none;

			my_inventory.board.Play_bonus_sfx(9,true);
			}
		else if (my_inventory.board.bonus_select == Board_C.bonus.heal_hp)
			{
			if (my_inventory.player)
				{
				if ((my_inventory.board.current_player_hp + my_inventory.board.heal_hp_player_bonus) <= my_inventory.board.max_player_hp)
					my_inventory.board.current_player_hp += my_inventory.board.heal_hp_player_bonus;
				else
					my_inventory.board.current_player_hp = my_inventory.board.max_player_hp;

				my_inventory.board.player_bonus_inventory[my_id]--;
				}
			else
				{
				if ((my_inventory.board.current_enemy_hp + my_inventory.board.heal_hp_enemy_bonus) <= my_inventory.board.max_enemy_hp)
					my_inventory.board.current_enemy_hp += my_inventory.board.heal_hp_enemy_bonus;
				else
					my_inventory.board.current_enemy_hp = my_inventory.board.max_enemy_hp;

				my_inventory.board.enemy_bonus_inventory[my_id]--;
				}
			
			my_inventory.board.Update_hp();
			
			my_inventory.Update_bonus_count(my_id);
			my_inventory.board.bonus_select = Board_C.bonus.none;
			my_inventory.board.Check_secondary_explosions();

			my_inventory.board.Play_bonus_sfx(10,true);
			}
		else
			{
			my_image.color = Color.white;
			}
		}

	public void Deselect()
		{
		my_inventory.board.bonus_select = Board_C.bonus.none;
		

		my_image.color = base_color;

		
		my_inventory.board.main_gem_selected_x = -10;
		my_inventory.board.main_gem_selected_y = -10;
		my_inventory.board.main_gem_color = -10;
		my_inventory.board.minor_gem_destination_to_x = -10;
		my_inventory.board.minor_gem_destination_to_y = -10;
		my_inventory.board.minor_gem_color = -10;
		}
}
