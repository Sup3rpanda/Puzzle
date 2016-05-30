using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class bonus_inventory : MonoBehaviour {

	public Board_C board;
	public GameObject inventory_button;

	public bool player;


	public GameObject[] bonus_list;
	Text[] bonus_quantity;

	// Use this for initialization
	void Start () {
	
		if (board.trigger_by_select == Board_C.trigger_by.inventory)
			{
			bonus_list = new GameObject[Enum.GetNames(typeof(Board_C.bonus)).Length];
			bonus_quantity = new Text[bonus_list.Length];

			for (int i = 1; i < bonus_list.Length; i++)
				{

				//create buttons
				bonus_list[i] = (GameObject)Instantiate(inventory_button);
				bonus_list[i].transform.SetParent(this.transform,false);
				bonus_list[i].name = ((Board_C.bonus)i).ToString();

				//set button image
				bonus_list[i].GetComponent<Image>().sprite = board.on_board_bonus_sprites[i];

				//active only player button
				if (player)
					bonus_list[i].GetComponent<Button>().interactable = true;
				else
					bonus_list[i].GetComponent<Button>().interactable = false;

				//set id
				inventory_bonus_button button_script = bonus_list[i].GetComponent<inventory_bonus_button>();
					button_script.my_id = i;
					button_script.my_inventory = this.gameObject.GetComponent<bonus_inventory>();

				bonus_quantity[i] = bonus_list[i].transform.GetChild(0).GetComponent<Text>();

				Update_bonus_count(i);

				}
		}
	}
	


	public void Update_bonus_count(int bonus_id)//call from bonus_inventory.Start(), Board_C.Update_inventory_bonus(int bonus_id, int quantity)
	{
		if (player)
		{
			bonus_quantity[bonus_id].text = board.player_bonus_inventory[bonus_id].ToString();
			
			if (board.player_bonus_inventory[bonus_id] > 0) //show only the bonuses avaibles
				{
				if (bonus_id == 8) //more time
					{
					if (board.lose_requirement_selected == Board_C.lose_requirement.timer)
						bonus_list[bonus_id].SetActive(true);
					else
						bonus_list[bonus_id].SetActive(false);
					}
				else if (bonus_id == 9) //more moves
					{
					if (board.lose_requirement_selected == Board_C.lose_requirement.player_have_zero_moves)
						bonus_list[bonus_id].SetActive(true);
					else
						bonus_list[bonus_id].SetActive(false);
					}
				else if (bonus_id == 10) //more HP
					{
					if (player)
						{
						if (board.lose_requirement_selected == Board_C.lose_requirement.player_hp_is_zero)
							bonus_list[bonus_id].SetActive(true);
						else
							bonus_list[bonus_id].SetActive(false);
						}
					else
						{
						if (board.win_requirement_selected == Board_C.win_requirement.enemy_hp_is_zero)
							bonus_list[bonus_id].SetActive(true);
						else
							bonus_list[bonus_id].SetActive(false);
						}
					}
				else
					bonus_list[bonus_id].SetActive(true);
				}
			else
				bonus_list[bonus_id].SetActive(false);
		}
		else
		{
			bonus_quantity[bonus_id].text = board.enemy_bonus_inventory[bonus_id].ToString();
			
			if (board.enemy_bonus_inventory[bonus_id] > 0) //show only the bonuses avaibles
				bonus_list[bonus_id].SetActive(true);
			else
				bonus_list[bonus_id].SetActive(false);
		}
	}
}
