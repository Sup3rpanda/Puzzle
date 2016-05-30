using UnityEngine;
using System.Collections;

public class tile_C : MonoBehaviour {

	public Board_C board;
	public explosion_score explosion_score_script;
	public int use_fx_big_explosion_here;

	//bonus fx limits
	int fx_end_r;
	int fx_end_l;
	int fx_end_up;
	int fx_end_down;

	public int after_explosion_I_will_become_this_bonus; // 0 = no bonus

	public int _x;
	public int _y;
	int move_direction;

	int[] two_last_colors_created = {0,1};//this avoid to create 3 gems with the same color
	int random_color;

	public GameObject my_gem;
	public GameObject my_padlock;

	/* if you have the menu kit, DELETE THIS LINE
	game_master my_game_master;
	// if you have the menu kit, DELETE THIS LINE*/

	void Start()
	{
		/* if you have the menu kit, DELETE THIS LINE
		if (game_master.game_master_obj)
			my_game_master = (game_master)game_master.game_master_obj.GetComponent("game_master");
		// if you have the menu kit, DELETE THIS LINE*/

		explosion_score_script = board.the_gui_score_of_this_move.GetChild(0).GetComponent<explosion_score>();
	}
	
	public void Debug_show_available_moves(int show_this_move)
		{
		if (show_this_move == 3)
			GetComponent<Renderer>().material.color = Color.gray;
		else if (show_this_move > 3)
			GetComponent<Renderer>().material.color = Color.black;
		else
			GetComponent<Renderer>().material.color = Color.white;
		/*
		if (board.board_array_master[_x,_y,0] >3) //if this gem can make an explosive move
			{
			renderer.material.color = Color.black;
			}
		else
			renderer.material.color = Color.white;*/
		}

	public void Debug_show_my_color()
	{
		if (board.board_array_master[_x,_y,1] == -99)
			GetComponent<Renderer>().material.color = Color.gray;
		else if(board.board_array_master[_x,_y,1] == 0)
			GetComponent<Renderer>().material.color = Color.red;
		else if(board.board_array_master[_x,_y,1] == 1)
			GetComponent<Renderer>().material.color = Color.cyan;
		else if(board.board_array_master[_x,_y,1] == 2)
			GetComponent<Renderer>().material.color = Color.magenta;
		else if(board.board_array_master[_x,_y,1] == 3)
			GetComponent<Renderer>().material.color = Color.yellow;
	}
	
	#region interaction
 	void OnMouseDown ()
	{

		board.touch_number++;
		if (board.touch_number == 1)
			{
			board.cursor.position = this.transform.position;
			/*
			Debug.Log("kind " + board.board_array_master[_x,_y,1]
			         + " bonus " + board.board_array_master[_x,_y,4]);
			*/
			/*
			Debug.Log(
				"tile hp " + board.board_array_master[_x,_y,0]
				+ "kind " + board.board_array_master[_x,_y,1]
				+ "block hp " + board.board_array_master[_x,_y,14]
				//"** 5 = " + board.board_array_master[_x,_y,5]
				//+ "** 6 up = " + board.board_array_master[_x,_y,6]
				//+ "** 7 down = " + board.board_array_master[_x,_y,7]
				//+ "** 8 right = " + board.board_array_master[_x,_y,8]
				//+ "** 9 left = " + board.board_array_master[_x,_y,9]
				);

			/*
				Board_C debug = (Board_C)boardthis_board.GetComponent("Board_C");
				if (debug.debug)//change gem color when click over it
				{
					if (boardarray_master[_x,_y,1] +1 == board.gem_length)
						boardarray_master[_x,_y,1]  = 0;
					else
						boardarray_master[_x,_y,1] ++;

					SpriteRenderer sprite_gem = my_gem.GetComponent<SpriteRenderer>();
						sprite_gem.sprite = board.gem_colors[boardarray_master[_x,_y,1]];


				}
				else
				{
				Debug.Log(_x + "," + _y 
				          + "** 1 = " + boardarray_master[_x,_y,1] 
				          + "** 2 = " + boardarray_master[_x,_y,2]
				          + "** 3 = " + boardarray_master[_x,_y,3]
				          + "** 4 = " + boardarray_master[_x,_y,4]
				          + "** 5 = " + boardarray_master[_x,_y,5]
				          + "** 6 = " + boardarray_master[_x,_y,6]
				          + "** 7 = " + boardarray_master[_x,_y,7]
				          + "** 8 = " + boardarray_master[_x,_y,8]
				          + "** 9 = " + boardarray_master[_x,_y,9]
				          + "** 10 = " + boardarray_master[_x,_y,10]
				          + "** 11 = " + boardarray_master[_x,_y,11]
				          + "** 12 = " + boardarray_master[_x,_y,12]
				          + "** 13 = " + boardarray_master[_x,_y,13]
				          + "** 14 = " + boardarray_master[_x,_y,14]
				          + "** 15 = " + boardarray_master[_x,_y,15]
				          + "** 16 = " + boardarray_master[_x,_y,16]
				          + "** 17 = " + boardarray_master[_x,_y,17]
				          );
				}*/


			if(board.player_turn && board.player_can_move) 
				//|| (board.player_can_move_when_gem_falling))
				{
				//Debug.Log("OnMouseDown " + board.player_can_move);
				//board.bonus_select = Board_C.bonus.destroy_horizontal_and_vertical;//DEBUG

				//if (board.player_can_move_when_gem_falling && board.board_array_master[_x,_y,11] != 0) //you can't interact with a gem in motion
					//return;

				if (board.bonus_select == Board_C.bonus.none)
					{
					if ( (board.board_array_master[_x,_y,1] >= 0) &&  (board.board_array_master[_x,_y,1] <= 9) //if this is a gem
					    	&& (board.board_array_master[_x,_y,3] == 0) ) //no padlock
					    	{
						if ( (board.trigger_by_select == Board_C.trigger_by.click) && (board.board_array_master[_x,_y,1] == 9) && (board.board_array_master[_x,_y,4] > 0) )// click on a clickable bonus
								{
								Debug.Log("click bonus");
								Cancell_old_selection();
								
								Trigger_bonus(true);

								return;
								}

							if (board.main_gem_selected_x < 0)//if none gem is selected
								{
								I_become_main_gem();
								}
							else if ( (board.main_gem_selected_x == _x) && (board.main_gem_selected_y ==_y) )
								{
								//you have click on the gem already selected
								}
							else if ( (board.main_gem_selected_x-1 == _x) &&  (board.main_gem_selected_y == _y))
								{
								//click on the gem next left of the main gem
								I_become_minor_gem(7);
								}
							else if ( (board.main_gem_selected_x+1 == _x) &&  (board.main_gem_selected_y == _y))
								{
								//click on the gem next right of main gem
								I_become_minor_gem(6);
								}
							else if ( (board.main_gem_selected_x == _x) &&  (board.main_gem_selected_y-1 == _y))
								{
								//click on the gem next up main gem
								I_become_minor_gem(5);
								}
							else if ( (board.main_gem_selected_x == _x) &&  (board.main_gem_selected_y+1 == _y))
								{
								//click on the gem next down main gem
								I_become_minor_gem(4);
								}
							else //click on a gem not adjacent the gem already selected, so this gem will be the new main gem
								{
								I_become_main_gem();
								}
							}	
						else
							Cancell_old_selection();
					}
				else
					{
					Cancell_old_selection();
					Try_to_use_bonus_on_this_tile();
					}
				}

			}
	}

	#region bonus
	public void Try_to_use_bonus_on_this_tile()
	{
		Debug.Log("Try_to_use_bonus_on_this_tile: " + _x+","+_y);
		if (board.board_array_master[_x,_y,1] >= 0) //click on a tile that have something in itself
		{
			switch(board.bonus_select)
			{
			case Board_C.bonus.destroy_one:
				Destroy_one(true);
				break;

			case Board_C.bonus.switch_gem_teleport:
				Gems_teleport();
				break;
				
			case Board_C.bonus.destroy_3x3:
				Destroy_3x3(true);
				break;
				
			case Board_C.bonus.destroy_horizontal:
				Destroy_horizontal(true);
				break;
				
			case Board_C.bonus.destroy_vertical:
				Destroy_vertical(true);
				break;
				
			case Board_C.bonus.destroy_horizontal_and_vertical:
				Destroy_horizontal_and_vertical(true);
				break;
				
			case Board_C.bonus.destroy_all_gem_with_this_color:
				Destroy_all_gems_with_this_color();
				break;
			}
			/*
			if (board.bonus_select != Board_C.bonus.switch_gem_teleport)
				{
				if (board.player_turn)
					board.gui_bonus_ico_array[board.slot_bonus_ico_selected].GetComponent<bonus_button>().Reset_fill();
				else
					board.gui_enemy_bonus_ico_array[board.enemy_chosen_bonus_slot].GetComponent<bonus_button>().Reset_fill();
				}*/
		}
	}

	public void Enemy_click_on_this_bonus()
		{
		Trigger_bonus(true);
		}

	public void Trigger_bonus(bool start_explosion)
	{
		//Debug.Log(_x +","+_y + " Trigger_bonus");
		if (board.board_array_master[_x,_y,4] == 1) //active hammer (need a click on a target gem to be activated)
		{
		}
		else if (board.board_array_master[_x,_y,4] == 2) //switch_gem_teleport
		{
		}
		else if (board.board_array_master[_x,_y,4] == 3) //explode bomb
		{
			Destroy_3x3(start_explosion);
		}
		else if (board.board_array_master[_x,_y,4] == 4) //destroy_horizontal
		{
			Destroy_horizontal(start_explosion);
		}
		else if (board.board_array_master[_x,_y,4] == 5) //destroy_vertical
		{
			Destroy_vertical(start_explosion);
		}
		else if (board.board_array_master[_x,_y,4] == 6) //destroy_horizontal_and_vertical
		{
			Destroy_horizontal_and_vertical(start_explosion);
		}
		else if (board.board_array_master[_x,_y,4] == 7) //destroy_all_gem_with_this_color (need a click on a target gem to be activated)
		{
		}
		else if (board.board_array_master[_x,_y,4] == 8) //give_more_time
		{
			Give_more_time();
		}
		else if (board.board_array_master[_x,_y,4] == 9) //give_more_moves
		{
			Give_more_moves();
		}
		else if (board.board_array_master[_x,_y,4] == 10) //heal
		{
			Heal_me();
		}
	}

	void Update_on_board_bonus_count()
	{
		if (board.number_of_bonus_on_board > 0)
			board.number_of_bonus_on_board--;

		Debug.Log("Update_on_board_bonus_count: " + board.number_of_bonus_on_board);
	}

	void Reset_charge_fill()
		{
		if (board.give_bonus_select == Board_C.give_bonus.after_charge)
			{
			if (board.player_turn)
				board.gui_bonus_ico_array[board.slot_bonus_ico_selected].GetComponent<bonus_button>().Reset_fill();
			else
				board.gui_enemy_bonus_ico_array[board.enemy_chosen_bonus_slot].GetComponent<bonus_button>().Reset_fill();
			}
		}

	void Gems_teleport()
	{
		if ( (board.board_array_master[_x,_y,1] >= 0) && (board.board_array_master[_x,_y,1] < 9)) //is a gem
			{
			if (board.board_array_master[_x,_y,3] == 0) //no padlock
				{
				if (board.main_gem_selected_x == -10) //select first gem
					{
					board.main_gem_selected_x = _x;
					board.main_gem_selected_y = _y;
					board.main_gem_color = board.board_array_master[_x,_y,1];
					//Debug.Log("teleport select first gem: " + _x + "," + _y);

					//If Enemy turn, set the second click:
						if (!board.player_turn)
							{
							//Debug.Log("secondary teleport gem: " + board.enemy_bonus_click_2_x + "," + board.enemy_bonus_click_2_y);
							tile_C script_main_gem = (tile_C)board.tiles_array[board.enemy_bonus_click_2_x,board.enemy_bonus_click_2_y].GetComponent("tile_C");
								script_main_gem.Invoke("Try_to_use_bonus_on_this_tile",board.enemy_move_delay);
							}
					}
				else //select second gem
					{
					if ( (board.main_gem_selected_x == _x) && (board.main_gem_selected_y == _y) )
						{
						//you have click on the same gem, so deselect it
						board.main_gem_selected_x = -10;
						board.main_gem_selected_y = -10;
						board.main_gem_color = -10;
						}
					else
						{
						//board.minor_gem_destination_to_x = _x;
						//board.minor_gem_destination_to_y = _y;
						board.minor_gem_color = board.board_array_master[_x,_y,1];
						Debug.Log("teleport select second gem: " + _x + "," + _y);

						//activate teleport
						board.number_of_gems_to_mix = 2;
						board.player_can_move = false;

						//change gems
						board.board_array_master[_x,_y,1] = board.main_gem_color;
						board.board_array_master[board.main_gem_selected_x,board.main_gem_selected_y,1] = board.minor_gem_color;

						board.Play_bonus_sfx(2);

						//update gem
						StartCoroutine(Shuffle_update());

						tile_C tile_script = (tile_C)board.tiles_array[board.main_gem_selected_x,board.main_gem_selected_y].GetComponent("tile_C");
							tile_script.StartCoroutine(tile_script.Shuffle_update());


						}
					}
				}
			}

	}

	void Heal_me()
	{
		board.player_can_move = false;
		board.board_array_master[_x,_y,4] = 0;
		board.Heal_me();

		board.Annotate_explosions(_x,_y);
		board.Play_bonus_sfx(10);
		board.Order_to_gems_to_explode();

		Update_on_board_bonus_count();
	}

	void Give_more_moves()
	{
		board.player_can_move = false;
		board.board_array_master[_x,_y,4] = 0;
		board.Gain_turns(board.add_moves_bonus);

		board.Annotate_explosions(_x,_y);
		board.Play_bonus_sfx(9);
		board.Order_to_gems_to_explode();

		Update_on_board_bonus_count();
	}

	void Give_more_time()
	{
		board.player_can_move = false;
		board.board_array_master[_x,_y,4] = 0;
		board.Add_time_bonus(board.add_time_bonus);

		board.Annotate_explosions(_x,_y);
		board.Play_bonus_sfx(8);
		board.Order_to_gems_to_explode();

		Reset_charge_fill();
		board.Update_inventory_bonus(8,-1);
		Update_on_board_bonus_count();
	}



	void Destroy_one(bool start_explosion)
	{
		if ( (board.board_array_master[_x,_y,1] >= 0) && (board.board_array_master[_x,_y,1] <= 9) && (board.board_array_master[_x,_y,4] >= -100) //it is a gem or junk
		    && (board.board_array_master[_x,_y,4] <= 0))//but not a bonus
			{

				board.player_can_move = false;
				board.cursor.gameObject.SetActive(false);

					Reset_charge_fill();
					board.Update_inventory_bonus(1,-1);

				if (board.bonus_have_explosion_fx)
				{
					if (board.garbage_recycle_bonus_one_fx.childCount> 0) //if you can recycle a previous fx
					{
						explosion_fx fx_temp = board.garbage_recycle_bonus_one_fx.GetChild(0).GetComponent<explosion_fx>();
						fx_temp.Show_me(new Vector3(_x,-_y,-1f));
						
					}
					else
					{
						Transform fx_temp = (Transform)Instantiate(board.destroy_one_fx,
						                                           new Vector3(_x,-_y,-1.5f),
						                                           Quaternion.identity);
						fx_temp.GetComponent<explosion_fx>().Activate_me(board.garbage_recycle_bonus_one_fx);
					}
				}

				board.Annotate_explosions(_x,_y);

				if (start_explosion)
					{
					board.Play_bonus_sfx(1);
					board.Order_to_gems_to_explode();
					}
	
			}
		else if ( (board.board_array_master[_x,_y,1] >= 41) && (board.board_array_master[_x,_y,1] < 60) ) //it is a block
			{
			board.player_can_move = false;
			board.cursor.gameObject.SetActive(false);

				Reset_charge_fill();
				board.Update_inventory_bonus(1,-1);

			if (board.bonus_have_explosion_fx)
			{
				if (board.garbage_recycle_bonus_one_fx.childCount> 0) //if you can recycle a previous fx
				{
					explosion_fx fx_temp = board.garbage_recycle_bonus_one_fx.GetChild(0).GetComponent<explosion_fx>();
					fx_temp.Show_me(new Vector3(_x,-_y,-1f));
					
				}
				else
				{
					Transform fx_temp = (Transform)Instantiate(board.destroy_one_fx,
					                                           new Vector3(_x,-_y,-1.5f),
					                                           Quaternion.identity);
					fx_temp.GetComponent<explosion_fx>().Activate_me(board.garbage_recycle_bonus_one_fx);
				}
			}

			board.number_of_elements_to_damage++;
			Damage_block();
			}

		Update_on_board_bonus_count();
	}
	
	void Destroy_3x3(bool start_explosion)
	{
		Debug.Log("Destroy_3x3");
		if ( ((board.board_array_master[_x,_y,1] >= 0) && (board.board_array_master[_x,_y,1] <= 9) && (board.board_array_master[_x,_y,4] >= -100))  //it is a gem or junk
		    ||  ((board.board_array_master[_x,_y,1] >= 41) && (board.board_array_master[_x,_y,1] < 60)) )//it is a block
			{
			if ((board.trigger_by_select == Board_C.trigger_by.inventory) && (board.board_array_master[_x,_y,4] > 0))
				return;

				board.board_array_master[_x,_y,4] = 0;
				if ((board.give_bonus_select != Board_C.give_bonus.after_charge) && (board.trigger_by_select != Board_C.trigger_by.inventory))
					Update_on_board_bonus_count();
				
				board.player_can_move = false;
				board.cursor.gameObject.SetActive(false);
				
				for (int y = (_y-1); y < ((_y-1)+3); y++)
				{
					if ((y >= 0) && (y < board._Y_tiles))
					{
						for (int x = (_x-1); x < ((_x-1)+3); x++)
						{
							if ((x >= 0) && (x < board._X_tiles))
							{
								if (board.board_array_master[x,y,1] >= 0) //if this tile have something
								{
									if  ( ( (board.board_array_master[x,y,1] >= 0) && (board.board_array_master[x,y,1] <= 9) && (board.board_array_master[x,y,4] >= -100) )  //it is a gem or junk 
										|| ( (board.board_array_master[x,y,1] >= 41) && (board.board_array_master[x,y,1] < 60) ) ) //it is a block
									{
									if (board.trigger_by_select != Board_C.trigger_by.inventory)
										board.Annotate_explosions(x,y);
									else
										{
										if (board.board_array_master[x,y,4] <= 0)
											board.Annotate_explosions(x,y);
										}
									}
								}
							}
						}
					}
				}

					Reset_charge_fill();
					board.Update_inventory_bonus(3,-1);


			if ( (board.number_of_elements_to_damage > 0) && (start_explosion))//if there is something to explode
				{

				if (board.bonus_have_explosion_fx)
					{
					if (board.destroy_3x3_fx.childCount> 0) //if you can recycle a previous fx
						{
						board.garbage_recycle_bonus_3x3_fx.GetChild(0).GetComponent<explosion_fx>().Show_me(new Vector3(_x,-_y,-1f));
						
						}
					else
						{
						Transform fx_temp = (Transform)Instantiate(board.destroy_3x3_fx,
						                                           new Vector3(_x,-_y,-1.5f),
						                                           Quaternion.identity);
						fx_temp.GetComponent<explosion_fx>().Activate_me(board.garbage_recycle_bonus_3x3_fx);
						}
					}

					board.Play_bonus_sfx(3);
					board.Order_to_gems_to_explode();
				}

			}
	}
	
	void Destroy_horizontal(bool start_explosion)
	{
		if  ( ((board.board_array_master[_x,_y,1] >= 0) && (board.board_array_master[_x,_y,1] <= 9) && (board.board_array_master[_x,_y,4] >= -100))  //is a gem or junk
			||  ((board.board_array_master[_x,_y,1] >= 41) && (board.board_array_master[_x,_y,1] < 60)) )//it is a block
			{
			if ((board.trigger_by_select == Board_C.trigger_by.inventory) && (board.board_array_master[_x,_y,4] > 0))
				return;

			board.board_array_master[_x,_y,4] = 0;
			if ((board.give_bonus_select != Board_C.give_bonus.after_charge) && (board.trigger_by_select != Board_C.trigger_by.inventory))
				Update_on_board_bonus_count();

			board.player_can_move = false;
			board.cursor.gameObject.SetActive(false);


			if ((board.give_bonus_select == Board_C.give_bonus.after_charge) || (board.trigger_by_select == Board_C.trigger_by.inventory) )
				board.elements_to_damage_array = new GameObject[board._X_tiles];
			else if ((board.trigger_by_select == Board_C.trigger_by.free_switch) && (board.elements_to_damage_array.Length < board.total_tiles))
				board.elements_to_damage_array = new GameObject[board.total_tiles];

				board.Annotate_explosions(_x,_y);

			Destroy_on_the_right_side();
			Destroy_on_the_left_side();

				Reset_charge_fill();
				board.Update_inventory_bonus(4,-1);

			if ((board.number_of_elements_to_damage > 0) && (start_explosion))//if there is something to explode
				{
				if (board.bonus_have_explosion_fx)
					{
					if (board.garbage_recycle_bonus_horizontal_fx.childCount> 0) //if you can recycle a previous fx
						{
						explosion_fx fx_temp = board.garbage_recycle_bonus_horizontal_fx.GetChild(0).GetComponent<explosion_fx>();
						fx_temp.Setup_horizontal(fx_end_r,fx_end_l);
						fx_temp.Show_me(new Vector3(_x,-_y,-1f));
						
						}
					else
						{
						Transform fx_temp = (Transform)Instantiate(board.destroy_horizontal_fx,
						                                           new Vector3(_x,-_y,-1.5f),
						                                           Quaternion.identity);
						fx_temp.GetComponent<explosion_fx>().Setup_horizontal(fx_end_r,fx_end_l);
						fx_temp.GetComponent<explosion_fx>().Activate_me(board.garbage_recycle_bonus_horizontal_fx);
						}
					}

				board.Play_bonus_sfx(4);
				board.Order_to_gems_to_explode();
				//Update_on_board_bonus_count();
				}



			}
	}

	void Destroy_on_the_right_side()
	{
		fx_end_r = 0;

		//decide if stop when it an empty tile or not
		int tile_target = 0;
		if (!board.linear_explosion_stop_against_empty_space)
			tile_target = -1;

		for (int x = (_x+1); x < board._X_tiles; x++)
			{
			if (board.board_array_master[x,_y,0] >= tile_target)
				{
				if (board.board_array_master[x,_y,1] > -99) //if there is something in this tile
					{
					if ((board.board_array_master[x,_y,1] >= 0) && (board.board_array_master[x,_y,1] <= 9))//if gem or special gem
						{
						if (board.board_array_master[x,_y,4] == 0) //it is a normal gem
							{
							board.Annotate_explosions(x,_y);
							fx_end_r++;
							}
						else if (board.board_array_master[x,_y,4] > 0) //it is a bonus
							{
							if (board.linear_explosion_stop_against_bonus)
								break;
							else
								{
								if (board.trigger_by_select != Board_C.trigger_by.inventory)
									{
									board.Annotate_explosions(x,_y);
									fx_end_r++;
									}
								else
									{
									fx_end_r++;
									}
								}
							}
						else //is a special gem
							{
							if (board.board_array_master[x,_y,4] == -200) //token
								{
								if (board.linear_explosion_stop_against_token)
									break;
								else
									fx_end_r++;
								}
							else if (board.board_array_master[x,_y,4] == -100) //junk
								{
								board.Annotate_explosions(x,_y);
								fx_end_r++;
								}
							}
						}
					else if ((board.board_array_master[x,_y,1] > 40) && (board.board_array_master[x,_y,1] < 60)) //it is a block
						{
						if (board.linear_explosion_stop_against_block)
							break;
						else
							{
							board.Annotate_explosions(x,_y);
							fx_end_r++;
							}
						}
					else
						{
						Debug.LogWarning("hit mysterious stuff at " + x + "," +_y + " = " + board.board_array_master[x,_y,1]);
						}
					}
				else //this tile is empty
					{
					if (board.board_array_master[x,_y,0] == -1) //no tile here
						{
						if (board.linear_explosion_stop_against_empty_space)
							break;
						else
							fx_end_r++;
						}
					else
						fx_end_r++;
					}
				}
			else
				break;
			
			}
	}

	void Destroy_on_the_left_side()
	{

		fx_end_l = 0;
		
		//decide if stop when it an empty tile or not
		int tile_target = 0;
		if (!board.linear_explosion_stop_against_empty_space)
			tile_target = -1;
		
		for (int x = (_x-1); x >= 0; x--)
		{
			if (board.board_array_master[x,_y,0] >= tile_target)
			{
				if (board.board_array_master[x,_y,1] > -99) //if there is something in this tile
				{
					if ((board.board_array_master[x,_y,1] >= 0) && (board.board_array_master[x,_y,1] <= 9))//if gem or special gem
					{
						if (board.board_array_master[x,_y,4] == 0) //it is a normal gem
						{
							board.Annotate_explosions(x,_y);
							fx_end_l++;
						}
						else if (board.board_array_master[x,_y,4] > 0) //it is a bonus
						{
							if (board.linear_explosion_stop_against_bonus)
								break;
							else
							{
								if (board.trigger_by_select != Board_C.trigger_by.inventory)
								{
									board.Annotate_explosions(x,_y);
									fx_end_l++;
								}
								else
								{
									fx_end_l++;
								}
							}
						}
						else //is a special gem
						{
							if (board.board_array_master[x,_y,4] == -200) //token
							{
								if (board.linear_explosion_stop_against_token)
									break;
								else
									fx_end_l++;
							}
							else if (board.board_array_master[x,_y,4] == -100) //junk
							{
								board.Annotate_explosions(x,_y);
								fx_end_l++;
							}
						}
					}
					else if ((board.board_array_master[x,_y,1] > 40) && (board.board_array_master[x,_y,1] < 60)) //it is a block
					{
						if (board.linear_explosion_stop_against_block)
							break;
						else
						{
							board.Annotate_explosions(x,_y);
							fx_end_l++;
						}
					}
					else
					{
						Debug.LogWarning("hit mysterious stuff at " + x + "," +_y + " = " + board.board_array_master[x,_y,1]);
					}
				}
				else //this tile is empty
				{
					if (board.board_array_master[x,_y,0] == -1) //no tile here
					{
						if (board.linear_explosion_stop_against_empty_space)
							break;
						else
							fx_end_l++;
					}
					else
						fx_end_l++;
				}
			}
			else
				break;
			
		}

	}

	void Destroy_above()
	{

		fx_end_up = 0;
		
		//decide if stop when it an empty tile or not
		int tile_target = 0;
		if (!board.linear_explosion_stop_against_empty_space)
			tile_target = -1;
		
		for (int y = (_y-1); y >= 0; y--)
		{
			if (board.board_array_master[_x,y,0] >= tile_target)
			{
				if (board.board_array_master[_x,y,1] > -99) //if there is something in this tile
				{
					if ((board.board_array_master[_x,y,1] >= 0) && (board.board_array_master[_x,y,1] <= 9))//if gem or special gem
					{
						if (board.board_array_master[_x,y,4] == 0) //it is a normal gem
						{
							board.Annotate_explosions(_x,y);
							fx_end_up++;
						}
						else if (board.board_array_master[_x,y,4] > 0) //it is a bonus
						{
							if (board.linear_explosion_stop_against_bonus)
								break;
							else
							{
								if (board.trigger_by_select != Board_C.trigger_by.inventory)
								{
									board.Annotate_explosions(_x,y);
									fx_end_up++;
								}
								else
								{
									fx_end_up++;
								}
							}
						}
						else //is a special gem
						{
							if (board.board_array_master[_x,y,4] == -200) //token
							{
								if (board.linear_explosion_stop_against_token)
									break;
								else
									fx_end_up++;
							}
							else if (board.board_array_master[_x,y,4] == -100) //junk
							{
								board.Annotate_explosions(_x,y);
								fx_end_up++;
							}
						}
					}
					else if ((board.board_array_master[_x,y,1] > 40) && (board.board_array_master[_x,y,1] < 60)) //it is a block
					{
						if (board.linear_explosion_stop_against_block)
							break;
						else
						{
							board.Annotate_explosions(_x,y);
							fx_end_up++;
						}
					}
					else
					{
						Debug.LogWarning("hit mysterious stuff at " + _x + "," +y + " = " + board.board_array_master[_x,y,1]);
					}
				}
				else //this tile is empty
				{
					if (board.board_array_master[_x,y,0] == -1) //no tile here
					{
						if (board.linear_explosion_stop_against_empty_space)
							break;
						else
							fx_end_up++;
					}
					else
						fx_end_up++;
				}
			}
			else
				break;
			
		}

	}

	void Destroy_beneath()
	{

		fx_end_down = 0;
		
		//decide if stop when it an empty tile or not
		int tile_target = 0;
		if (!board.linear_explosion_stop_against_empty_space)
			tile_target = -1;

		for (int y = (_y+1); y < board._Y_tiles; y++)
		{
			if (board.board_array_master[_x,y,0] >= tile_target)
			{
				if (board.board_array_master[_x,y,1] > -99) //if there is something in this tile
				{
					if ((board.board_array_master[_x,y,1] >= 0) && (board.board_array_master[_x,y,1] <= 9))//if gem or special gem
					{
						if (board.board_array_master[_x,y,4] == 0) //it is a normal gem
						{
							board.Annotate_explosions(_x,y);
							fx_end_down++;
						}
						else if (board.board_array_master[_x,y,4] > 0) //it is a bonus
						{
							if (board.linear_explosion_stop_against_bonus)
								break;
							else
							{
								if (board.trigger_by_select != Board_C.trigger_by.inventory)
								{
									board.Annotate_explosions(_x,y);
									fx_end_down++;
								}
								else
								{
									fx_end_down++;
								}
							}
						}
						else //is a special gem
						{
							if (board.board_array_master[_x,y,4] == -200) //token
							{
								if (board.linear_explosion_stop_against_token)
									break;
								else
									fx_end_down++;
							}
							else if (board.board_array_master[_x,y,4] == -100) //junk
							{
								board.Annotate_explosions(_x,y);
								fx_end_down++;
							}
						}
					}
					else if ((board.board_array_master[_x,y,1] > 40) && (board.board_array_master[_x,y,1] < 60)) //it is a block
					{
						if (board.linear_explosion_stop_against_block)
							break;
						else
						{
							board.Annotate_explosions(_x,y);
							fx_end_down++;
						}
					}
					else
					{
						Debug.LogWarning("hit mysterious stuff at " + _x + "," +y + " = " + board.board_array_master[_x,y,1]);
					}
				}
				else //this tile is empty
				{
					if (board.board_array_master[_x,y,0] == -1) //no tile here
					{
						if (board.linear_explosion_stop_against_empty_space)
							break;
						else
							fx_end_down++;
					}
					else
						fx_end_down++;
				}
			}
			else
				break;
			
		}

	}
	
	void Destroy_vertical(bool start_explosion)
	{
		if  ( ((board.board_array_master[_x,_y,1] >= 0) && (board.board_array_master[_x,_y,1] <= 9) && (board.board_array_master[_x,_y,4] >= -100))  //it is a gem or junk
			||  ((board.board_array_master[_x,_y,1] >= 41) && (board.board_array_master[_x,_y,1] < 60)) )//it is a block
		{
			if ((board.trigger_by_select == Board_C.trigger_by.inventory) && (board.board_array_master[_x,_y,4] > 0))
				return;
			
			board.board_array_master[_x,_y,4] = 0;
			if ((board.give_bonus_select != Board_C.give_bonus.after_charge) && (board.trigger_by_select != Board_C.trigger_by.inventory))
				Update_on_board_bonus_count();

			board.player_can_move = false;
			board.cursor.gameObject.SetActive(false);


			if ((board.give_bonus_select == Board_C.give_bonus.after_charge) || (board.trigger_by_select == Board_C.trigger_by.inventory) )
				board.elements_to_damage_array = new GameObject[board._Y_tiles];
			else if ((board.trigger_by_select == Board_C.trigger_by.free_switch) && (board.elements_to_damage_array.Length < board.total_tiles))
				board.elements_to_damage_array = new GameObject[board.total_tiles];

			board.Annotate_explosions(_x,_y);

			Destroy_beneath();
			Destroy_above();

				Reset_charge_fill();
				board.Update_inventory_bonus(5,-1);

			if ( (board.number_of_elements_to_damage > 0) && (start_explosion))//if there is something to explode
				{
				if (board.bonus_have_explosion_fx)
					{
					if (board.garbage_recycle_bonus_vertical_fx.childCount> 0) //if you can recycle a previous fx
						{
						explosion_fx fx_temp = board.garbage_recycle_bonus_vertical_fx.GetChild(0).GetComponent<explosion_fx>();
						fx_temp.Setup_vertical(fx_end_up,fx_end_down);
						fx_temp.Show_me(new Vector3(_x,-_y,-1f));
						
						}
					else
						{
						Transform fx_temp = (Transform)Instantiate(board.destroy_vertical_fx,
						                                           new Vector3(_x,-_y,-1.5f),
						                                           Quaternion.identity);
						fx_temp.GetComponent<explosion_fx>().Setup_vertical(fx_end_up,fx_end_down);
						fx_temp.GetComponent<explosion_fx>().Activate_me(board.garbage_recycle_bonus_vertical_fx);
						}
					}

				board.Play_bonus_sfx(5);
				board.Order_to_gems_to_explode();

				//Update_on_board_bonus_count();
				}

		}

	}
	
	void Destroy_horizontal_and_vertical(bool start_explosion)
	{
		Debug.Log("Destroy_horizontal_and_vertical");
		if  ( ((board.board_array_master[_x,_y,1] >= 0) && (board.board_array_master[_x,_y,1] <= 9) && (board.board_array_master[_x,_y,4] >= -100))  //is a gem or junk
			||  ((board.board_array_master[_x,_y,1] >= 41) && (board.board_array_master[_x,_y,1] < 60)) )//it is a block
		{
			if ((board.trigger_by_select == Board_C.trigger_by.inventory) && (board.board_array_master[_x,_y,4] > 0))
				return;

			board.board_array_master[_x,_y,4] = 0;
			if ((board.give_bonus_select != Board_C.give_bonus.after_charge) && (board.trigger_by_select != Board_C.trigger_by.inventory))
				Update_on_board_bonus_count();

			board.player_can_move = false;
			board.cursor.gameObject.SetActive(false);


			if ((board.give_bonus_select == Board_C.give_bonus.after_charge) || (board.trigger_by_select == Board_C.trigger_by.inventory) )
				board.elements_to_damage_array = new GameObject[board._X_tiles+board._Y_tiles];
			else if ((board.trigger_by_select == Board_C.trigger_by.free_switch) && (board.elements_to_damage_array.Length < board.total_tiles))
				board.elements_to_damage_array = new GameObject[board.total_tiles];

			board.Annotate_explosions(_x,_y);

			Destroy_on_the_right_side();
			Destroy_on_the_left_side();

			Destroy_beneath();
			Destroy_above();

				Reset_charge_fill();
				board.Update_inventory_bonus(6,-1);

			Debug.Log("board.number_of_elements_to_damage: " + board.number_of_elements_to_damage + " start_explosion: " + start_explosion);
			if ((board.number_of_elements_to_damage > 0) && (start_explosion))//if there is something to explode
				{
				Debug.Log("if there is something to explode");
				if (board.bonus_have_explosion_fx)
					{
					if (board.garbage_recycle_bonus_horizontal_and_vertical_fx.childCount> 0) //if you can recycle a previous fx
						{
						explosion_fx fx_temp = board.garbage_recycle_bonus_horizontal_and_vertical_fx.GetChild(0).GetComponent<explosion_fx>();
						fx_temp.Setup_horizontal_and_vertical(fx_end_up,fx_end_down,fx_end_r,fx_end_l);
						fx_temp.Show_me(new Vector3(_x,-_y,-1f));
						
						}
					else
						{
						Transform fx_temp = (Transform)Instantiate(board.destroy_horizontal_and_vertical_fx,
						                                           new Vector3(_x,-_y,-1.5f),
						                                           Quaternion.identity);
						fx_temp.GetComponent<explosion_fx>().Setup_horizontal_and_vertical(fx_end_up,fx_end_down,fx_end_r,fx_end_l);
						fx_temp.GetComponent<explosion_fx>().Activate_me(board.garbage_recycle_bonus_horizontal_and_vertical_fx);
						}
					}

				board.Play_bonus_sfx(6);
				board.Order_to_gems_to_explode();
				//Update_on_board_bonus_count();
				}
		}

	}

	void Destroy_all_gems_with_this_color()
	{
		if (board.board_array_master[_x,_y,4] != 0)
			return;

		Debug.Log("Destroy_all_gems_with_this_color");
		board.player_can_move = false;
		board.cursor.gameObject.SetActive(false);


		board.elements_to_damage_array = new GameObject[board._X_tiles*board._Y_tiles];

		for (int y = 0; y < board._Y_tiles; y++)
		{
			for (int x = 0; x < board._X_tiles; x++)
			{
				//Debug.Log("check " + x +","+y);
				if (board.board_array_master[x,y,1] == board.board_array_master[_x,_y,1]) //if this gem have my same color
					{
					board.Annotate_explosions(x,y);
					}
			}
		}

			Reset_charge_fill();
			board.Update_inventory_bonus(7,-1);

		board.Play_bonus_sfx(7);
		board.Order_to_gems_to_explode();
		Update_on_board_bonus_count();
	}
	#endregion
	
	void Cancell_old_selection()
	{
		if (board.bonus_select != Board_C.bonus.switch_gem_teleport)
			{
			board.main_gem_selected_x = -10;
			board.main_gem_selected_y = -10;
			board.main_gem_color = -10;
			board.minor_gem_destination_to_x = -10;
			board.minor_gem_destination_to_y = -10;
			board.minor_gem_color = -10;
			}
	}


	void OnMouseUp()
    {
		board.touch_number--;

    }

    void OnMouseEnter()
    {
		if (board.player_turn && board.player_can_move)
			//|| board.player_can_move_when_gem_falling)
			{
			if (board.touch_number == 0)
				{
				board.cursor.position = this.transform.position;
				board.cursor.gameObject.SetActive(true);
				}

			}

		if (board.touch_number == 1)
			Gem_drag();

    }

	void OnMouseExit()
	{
		if (board.player_turn && board.player_can_move)
			//|| board.player_can_move_when_gem_falling)
		{
			if (board.touch_number == 0)
				board.cursor.gameObject.SetActive(false);
		}
			
	}

	void Gem_drag()
	{ 
		if (board.touch_number == 1 && (board.bonus_select == Board_C.bonus.none))
		{
		if (board.avatar_main_gem && board.player_can_move)
			{
			//Debug.Log("Gem_drag() " + board.player_can_move);
			if ( (board.board_array_master[_x,_y,1] >= 0) && (board.board_array_master[_x,_y,1] <= 9) //it is a gem
			    && (board.board_array_master[_x,_y,3] == 0) ) //and it is free
				{ 
				if ( (board.main_gem_selected_x == _x+1) && (board.main_gem_selected_y == _y) ) //move left
					I_become_minor_gem(7);
				else if ( (board.main_gem_selected_x == _x-1) && (board.main_gem_selected_y == _y) ) //more right
					I_become_minor_gem(6); 
				else if ( (board.main_gem_selected_y == _y+1) && (board.main_gem_selected_x == _x) ) //move up
					I_become_minor_gem(5); 
				else if ( (board.main_gem_selected_y == _y-1) && (board.main_gem_selected_x == _x) ) //move down
					I_become_minor_gem(4); 
				}
			else
				{ 
				Debug.Log("invalid drag");
				}
			}
		}
	}
	

	public void I_become_main_gem()
		{
		//Debug.Log("I_become_main_gem() " + _x+","+_y);
		board.avatar_main_gem = my_gem;
		board.main_gem_selected_x = _x;
		board.main_gem_selected_y = _y;
		board.main_gem_color = board.board_array_master[_x,_y,1];

		if(!board.player_turn)
			board.cursor.gameObject.SetActive(true);

		//empty old selection
		board.minor_gem_destination_to_x = -10;
		board.minor_gem_destination_to_y = -10;
		board.minor_gem_color = -10;
		board.avatar_minor_gem = null;
		}
	
	public void I_become_minor_gem(int direction)
		{
		//Debug.Log("I_become_minor_gem() " + _x+","+_y);
		board.avatar_minor_gem = my_gem;
		board.minor_gem_destination_to_x = _x;
		board.minor_gem_destination_to_y = _y;
		board.minor_gem_color = board.board_array_master[_x,_y,1];

		if(board.player_turn)
			{
			board.cursor.position = this.transform.position;

			board.Switch_gem(direction);
			}
		else
			{
			move_direction = direction;
			Invoke("Delay_switch",board.enemy_move_delay);
			}
		}

	void Delay_switch()
	{
		board.cursor.position = this.transform.position;
		board.Switch_gem(move_direction);
	}
	
	#endregion

	
	#region destroy gem

	public void Check_the_content_of_this_tile()
		{
		if (board.board_array_master[_x,_y,4] == -100)
			{
			//Debug.Log("junk in " + _x + "," +_y);
			board.Annotate_explosions(_x,_y);
			}
		else if (board.board_array_master[_x,_y,4] == -200)
			{
			//Debug.Log("token in " + _x + "," +_y);
			board.Annotate_explosions(_x,_y);
			board.number_of_token_collected++;
			board.Update_token_count();
			}
		else if ((board.board_array_master[_x,_y,4] > 0) && (board.trigger_by_select == Board_C.trigger_by.inventory))
			{
			//Debug.Log("bonus in " + _x + "," +_y);

			//add bonus to inventory
			board.Update_inventory_bonus(board.board_array_master[_x,_y,4], +1);

			board.board_array_master[_x,_y,4] = 0;//deactivate bonus to avoid trigger when explode
			Update_on_board_bonus_count();
			after_explosion_I_will_become_this_bonus = 0;

			board.Annotate_explosions(_x,_y);

			}
		}


	public void Explosion()
		{
		//Debug.Log(_x+","+_y + " = " + board.board_array_master[_x,_y,1]);
		if (board.board_array_master[_x,_y,1] >= 0) //if there is something here
			{
			if (board.board_array_master[_x,_y,1] < board.gem_length)//if this is a normal gem
				{
				if (board.board_array_master[_x,_y,3] > 0 )//if there is a padlock
					{
					board.board_array_master[_x,_y,11] = 0;//reset explosion
					
					board.board_array_master[_x,_y,3] --;
					board.number_of_elements_to_damage--;
					board.number_of_padlocks_involved_in_explosion--;
					
					//check padlock hp
					if(board.board_array_master[_x,_y,3]> 0)
						{
						SpriteRenderer sprite_lock = my_padlock.GetComponent<SpriteRenderer>();
						sprite_lock.sprite = board.lock_gem_hp[board.board_array_master[_x,_y,3]-1];
						}
					else //the padlock hp is 0, so destroy the padlock
						{
						Destroy(my_padlock);
						board.board_array_master[_x,_y,10] = 1;//now can fall
						
						board.padlock_count--;
						if ((board.win_requirement_selected == Board_C.win_requirement.destroy_all_padlocks) && (board.padlock_count == 0))
							board.Player_win();
						}
					If_all_explosion_are_completed();
					}
				else//no padlock
					{
					if (board.player_turn)
						board.total_number_of_gems_destroyed_by_the_player[board.board_array_master[_x,_y,1]] ++;
					else
						board.total_number_of_gems_destroyed_by_the_enemy[board.board_array_master[_x,_y,1]] ++;
					
					Destroy_gem_avatar();
					}
				}
			else if ( (board.board_array_master[_x,_y,1] >= 41) && (board.board_array_master[_x,_y,1] < 60) ) //it is a block
				{
				Damage_block();
				}
			else if (board.board_array_master[_x,_y,1] == 9)//this was a bonus
				Destroy_gem_avatar();
			}
		else//this tile is empty, so...
			board.number_of_elements_to_damage--;//strike off this explosion
		}
	

	void Update_tile_hp()
	{
		if (board.board_array_master[_x,_y,0] > 0)//if i can lose hp
		{
			board.board_array_master[_x,_y,0]--;

			//update score
			if (board.player_turn)
				board.player_score += board.score_reward_for_damaging_tiles;
			else
				board.enemy_score += board.score_reward_for_damaging_tiles;

			board.Update_score();

				//update tile sprite
				SpriteRenderer sprite_hp = GetComponent<SpriteRenderer>();
				sprite_hp.sprite = board.tile_hp[board.board_array_master[_x,_y,0]];
				
				board.Update_board_hp();

				if (board.tile_give == Board_C.tile_destroyed_give.more_hp)
					{
					if (board.player_turn)
						board.current_player_hp += board.tile_gift_int;
					else
						board.current_enemy_hp += board.tile_gift_int;

					board.Update_hp();
					}
				else if (board.tile_give == Board_C.tile_destroyed_give.more_time)
					{
					board.Add_time_bonus(board.tile_gift_float);
					}
				else if (board.tile_give == Board_C.tile_destroyed_give.more_moves)
					{
					board.Gain_turns(board.tile_gift_int);
					}

				if (board.HP_board <= 0)
					{
					if (board.win_requirement_selected == Board_C.win_requirement.destroy_all_tiles)
						board.Player_win();
					else if ((board.win_requirement_selected == Board_C.win_requirement.take_all_tokens) && board.show_token_after_all_tiles_are_destroyed)
						board.Show_all_token_on_board();
					}

		}
	}
		
	void Destroy_gem_avatar()
		{
		board.Add_time_bonus(board.time_bonus_for_gem_explosion);

		if (board.explosions_damages_tiles)
			Update_tile_hp();

		//damage adjacent blocks
		if ( (_y+1 < board._Y_tiles) && (board.board_array_master[_x,_y+1,14] > 0) )
		{
			board.number_of_elements_to_damage++;

			tile_C script_target_block = (tile_C)board.tiles_array[_x,_y+1].GetComponent("tile_C");
				script_target_block.Damage_block();

		}
		if ( (_y-1 >=0) && (board.board_array_master[_x,_y-1,14] > 0) )
		{
			board.number_of_elements_to_damage++;

			tile_C script_target_block = (tile_C)board.tiles_array[_x,_y-1].GetComponent("tile_C");
				script_target_block.Damage_block();

		}
		if ( (_x+1 < board._X_tiles) && (board.board_array_master[_x+1,_y,14] > 0) )
		{
			board.number_of_elements_to_damage++;
			
			tile_C script_target_block = (tile_C)board.tiles_array[_x+1,_y].GetComponent("tile_C");
				script_target_block.Damage_block();

		}
		if ( (_x-1 >=0) && (board.board_array_master[_x-1,_y,14] > 0) )
		{
			board.number_of_elements_to_damage++;
			
			tile_C script_target_block = (tile_C)board.tiles_array[_x-1,_y].GetComponent("tile_C");
				script_target_block.Damage_block();

		}

		//explosion
		if (board.show_score_of_this_move && board.score_of_this_turn_move > 0)
			{
			board.the_gui_score_of_this_move.position = new Vector3(_x,-_y,-1f);
			explosion_score_script.Show_score(board.score_of_this_turn_move);
			board.score_of_this_turn_move = 0;
			}

		if (board.gem_explosion_fx_rule_selected == Board_C.gem_explosion_fx_rule.for_each_gem)
			{
			if (board.board_array_master[_x,_y,1] < board.gem_length)//if is a gem
				{
				if (board.garbage_recycle_gem_explosion_fx[board.board_array_master[_x,_y,1]].childCount > 0) //if you can recycle a previous fx
					{
					board.garbage_recycle_gem_explosion_fx[board.board_array_master[_x,_y,1]].GetChild(0).GetComponent<explosion_fx>().Show_me(new Vector3(_x,-_y,-1f));
					}
				else //create a new fx
					{
					Transform fx_temp = (Transform)Instantiate(board.gem_explosion_fx[board.board_array_master[_x,_y,1]],
					           	 		new Vector3(_x,-_y,-1f),
					            		Quaternion.identity);
					fx_temp.GetComponent<explosion_fx>().Activate_me( board.garbage_recycle_gem_explosion_fx[board.board_array_master[_x,_y,1]]);
					}
				}
			}
		else if (board.gem_explosion_fx_rule_selected == Board_C.gem_explosion_fx_rule.only_for_big_explosion)
			{
				if (use_fx_big_explosion_here > 0)
					{
					if (board.garbage_recycle_big_explosion_fx[use_fx_big_explosion_here-4].childCount > 0) //if you can recycle a previous fx
							{
							board.garbage_recycle_big_explosion_fx[use_fx_big_explosion_here-4].GetChild(0).GetComponent<explosion_fx>().Show_me(new Vector3(_x,-_y,-1f));
							}
						else //create a new fx
							{
							Transform fx_temp = (Transform)Instantiate(board.gem_big_explosion_fx[use_fx_big_explosion_here-4],
							                                           new Vector3(_x,-_y,-1f),
							                                           Quaternion.identity);
							fx_temp.GetComponent<explosion_fx>().Activate_me(board.garbage_recycle_big_explosion_fx[use_fx_big_explosion_here-4]);
							}

						use_fx_big_explosion_here = 0;
					}
			}

			

			my_gem.transform.localScale = new Vector3(1.3f,1.3f,1.3f);
			StartCoroutine(Destroy_animation());
			
			if (board.play_this_bonus_sfx == -1)//you don't play sound explosion fx if you must play a bonus sfx
				board.Play_sfx(board.explosion_sfx);

		}

	IEnumerator Destroy_animation()
	{
		yield return new WaitForSeconds(0.03f);

		if (my_gem.transform.localScale.x > 0)
			{
			my_gem.transform.localScale -= new Vector3(0.1f,0.1f,0.1f);
			StartCoroutine(Destroy_animation());
			}
		else
			{
			my_gem.transform.localScale = Vector3.zero;
			if (after_explosion_I_will_become_this_bonus == 0)
				Destroy_gem();
			else
				Change_gem_in_bonus();
			}
	
	}

	public void Change_gem_in_bonus()
	{
		//Debug.Log("Change_gem_in_bonus " + _x + "," + _y + " my color: " + board.board_array_master[_x,_y,1] + "bonus: " + after_explosion_I_will_become_this_bonus);
		if (board.board_array_master[_x,_y,1] <= 4)//if this is a normal gem
			Update_gems_score();

		//if (board.trigger_by_select != Board_C.trigger_by.color) //if don't use color to trigger, set bonus color to neutre good color (9)
			board.board_array_master[_x,_y,1] = 9;

		//create bonus
		board.board_array_master[_x,_y,4] = after_explosion_I_will_become_this_bonus;
		SpriteRenderer sprite_gem = my_gem.GetComponent<SpriteRenderer>();
			sprite_gem.sprite = board.on_board_bonus_sprites[after_explosion_I_will_become_this_bonus];

		StartCoroutine("Return_to_normal_size");
		after_explosion_I_will_become_this_bonus = 0;

		board.number_of_bonus_on_board++;

	}

	IEnumerator Return_to_normal_size()
	{
		while (my_gem.transform.localScale.x < 1.2f)
			{
			my_gem.transform.localScale += new Vector3(0.2f,0.2f,0.2f);
			yield return new WaitForSeconds(0.03f);
			}
	
		if (my_gem.transform.localScale.x >= 1.2f)
			{
			my_gem.transform.localScale = Vector3.one;

			board.number_of_elements_to_damage--;
			board.board_array_master[_x,_y,11] = 0;//destruction is over
		
			If_all_explosion_are_completed();
			}
	}

	
	void Damage_block()
		{
		//Debug.Log("Damage_block");
			board.board_array_master[_x,_y,14]--;
			
			if (board.board_array_master[_x,_y,14] <= 0)//if the hit had destroy the block
				{
				Destroy(my_gem);

				board.block_count--;
				if ((board.win_requirement_selected == Board_C.win_requirement.destroy_all_blocks) && (board.block_count == 0))
					board.Player_win();

				board.board_array_master[_x,_y,14] = 0;
				//now this tile is empty
					//my_gem = null;
					board.board_array_master[_x,_y,1] = -99;

				}
			else //update block sprite
				{
				SpriteRenderer sprite_block = my_gem.GetComponent<SpriteRenderer>();
				if ((board.board_array_master[_x,_y,1] > 40) && (board.board_array_master[_x,_y,1] < 50)) //normal block
					sprite_block.sprite = board.block_hp[board.board_array_master[_x,_y,14]-1];
				else if ((board.board_array_master[_x,_y,1] > 50) && (board.board_array_master[_x,_y,1] < 60)) //falling block
					sprite_block.sprite = board.falling_block_hp[board.board_array_master[_x,_y,14]-1];
				}

		board.number_of_elements_to_damage--;
		board.board_array_master[_x,_y,11] = 0;//reset explosion
		If_all_explosion_are_completed();
		}

	void Put_in_garbage(GameObject target)
	{
		target.transform.parent = board.garbage_recycle;
	}
	
	public void Destroy_gem()
		{
		if (board.board_array_master[_x,_y,1] < 9)//if this is a normal gem
			Update_gems_score();

		if (board.board_array_master[_x,_y,4] > 0)
		{
			Debug.Log("DESTROY BONUS");
		}

		board.board_array_master[_x,_y,11] = 0;//destruction is over
		board.board_array_master[_x,_y,1] = -99;//now this tile is empty (so it don't have color)
		board.board_array_master[_x,_y,4] = 0; //no special
		board.board_array_master[_x,_y,10] = 0; //can't fall
		Put_in_garbage(my_gem);
		my_gem = null;

		board.number_of_elements_to_damage--;

		If_all_explosion_are_completed();


		}

	void If_all_explosion_are_completed()
	{
		//Debug.Log(_x + "," + _y + "elements: " + board.number_of_elements_to_damage + "padlocks: " + board.number_of_padlocks_involved_in_explosion);
		//if ( (board.number_of_elements_to_damage - board.number_of_padlocks_involved_in_explosion ) == 0)
		if ( board.number_of_elements_to_damage == 0 && board.number_of_padlocks_involved_in_explosion == 0)
			{
			//Debug.Log("all explosions are completed");
			board.Start_update_board();
			}
	}

	void Update_gems_score()
	{
		//fill bonus
		if (board.give_bonus_select == Board_C.give_bonus.after_charge)
			{
			board.Update_bonus_fill(_x,_y,board.board_array_master[_x,_y,1]);
			}

		if (board.player_turn)
			{			
			board.total_number_of_gems_destroyed_by_the_player[board.board_array_master[_x,_y,1]]++;
	
			if (board.number_of_gems_collect_by_the_player[board.board_array_master[_x,_y,1]] 
			    < board.number_of_gems_to_destroy_to_win[board.board_array_master[_x,_y,1]])
				{
				board.number_of_gems_collect_by_the_player[board.board_array_master[_x,_y,1]]++;
				if (!board.player_win)
					{
					board.total_number_of_gems_remaining_for_the_player--;
					board.total_number_of_gems_required_colletted++;
					}
				if (board.total_number_of_gems_remaining_for_the_player <= 0 
				    && board.win_requirement_selected == Board_C.win_requirement.collect_gems)
						{
						board.This_gem_color_is_collected(board.board_array_master[_x,_y,1]);
						board.Player_win();
						}
				}
			else
				{
				board.This_gem_color_is_collected(board.board_array_master[_x,_y,1]);	
				board.number_of_gems_collect_by_the_player[board.board_array_master[_x,_y,1]]++;
				}

			if (board.continue_to_play_after_win_until_lose_happen)
			{
				if (board.win_requirement_selected == Board_C.win_requirement.collect_gems)
				{
					if (board.player_win)
						{
						board.additional_gems_collected_by_the_player++;

						if ((board.current_star_score < 3) && (board.player_score >= board.three_stars_target_score) && (board.three_stars_target_additional_gems_collected <= board.additional_gems_collected_by_the_player))

							{
							//if ((board.player_score >= board.three_stars_target_score) && (board.three_stars_target_additional_gems_collected <= board.additional_gems_collected_by_the_player))
								//{
								board.current_star_score = 3;
								board.praise_script.Win_and_continue_to_play(board.current_star_score-1);
								//}
							}
						else if ((board.current_star_score < 2) && (board.player_score >= board.two_stars_target_score) && (board.two_stars_target_additional_gems_collected <= board.additional_gems_collected_by_the_player))

							{
						//	if ((board.player_score >= board.two_stars_target_score) && (board.two_stars_target_additional_gems_collected <= board.additional_gems_collected_by_the_player))
								//{
								board.current_star_score = 2;
								board.praise_script.Win_and_continue_to_play(board.current_star_score-1);
								//}
							}
						} 

					/* if you have the menu kit, DELETE THIS LINE
					if (board.use_star_progress_bar)
						board.my_game_uGUI.my_progress_bar.Update_fill(board.total_number_of_gems_required_colletted+board.additional_gems_collected_by_the_player);

					// if you have the menu kit, DELETE THIS LINE*/
				} 
			}

			if (board.use_armor)//damage the enemy according to his armor vulnerability 
				{
				if (board.current_enemy_hp > 0)
					{
					switch(board.enemy_armor[board.board_array_master[_x,_y,1]])
						{
						case Board_C.armor.weak:
							board.current_enemy_hp -= board.gem_damage_value*2;
						break;


						case Board_C.armor.normal:
							board.current_enemy_hp -= board.gem_damage_value;
						break;


						case Board_C.armor.strong:
							board.current_enemy_hp -= (int)(board.gem_damage_value*0.5f);
						break;


						case Board_C.armor.immune:
						
						break;

						case Board_C.armor.absorb:
							if ( (board.current_enemy_hp + board.gem_damage_value) > board.max_enemy_hp )
								board.current_enemy_hp = board.max_enemy_hp;
							else
								board.current_enemy_hp += board.gem_damage_value;
						break;

						case Board_C.armor.repel:
							if ((board.current_player_hp - board.gem_damage_value) < 0)
								board.current_player_hp = 0;
							else
								board.current_player_hp -= board.gem_damage_value;
						break;
						}
					}

				}


			if (board.current_enemy_hp < 0)
				board.current_enemy_hp = 0;

			board.Update_hp();
			
			if (board.current_enemy_hp == 0 
			    && board.win_requirement_selected == Board_C.win_requirement.enemy_hp_is_zero)
					board.Player_win();

			}
		else //is the enemy turn
			{
			board.total_number_of_gems_destroyed_by_the_enemy[board.board_array_master[_x,_y,1]]++;
			if (board.number_of_gems_collect_by_the_enemy[board.board_array_master[_x,_y,1]] 
			    < board.number_of_gems_to_destroy_to_win[board.board_array_master[_x,_y,1]])
				{
				board.number_of_gems_collect_by_the_enemy[board.board_array_master[_x,_y,1]]++;

				board.total_number_of_gems_remaining_for_the_enemy--;

				if (board.total_number_of_gems_remaining_for_the_enemy <= 0 
				   && board.lose_requirement_selected == Board_C.lose_requirement.enemy_collect_gems)
					board.Player_lose();
				}
			else
				{
				board.This_gem_color_is_collected(board.board_array_master[_x,_y,1]);
				board.number_of_gems_collect_by_the_enemy[board.board_array_master[_x,_y,1]]++;
				}

			if (board.use_armor)//damage the player according to his armor vulnerability
				{
				if (board.current_player_hp > 0)
					{
					switch(board.player_armor[board.board_array_master[_x,_y,1]])
						{
						case Board_C.armor.weak:
							board.current_player_hp -= board.gem_damage_value*2;
							break;
							
							
						case Board_C.armor.normal:
							board.current_player_hp -= board.gem_damage_value;
							break;
							
							
						case Board_C.armor.strong:
							board.current_player_hp -= (int)(board.gem_damage_value*0.5f);
							break;
							
							
						case Board_C.armor.immune:
							
							break;
							
						case Board_C.armor.absorb:
							if ( (board.current_player_hp + board.gem_damage_value) > board.max_player_hp )
								board.current_player_hp = board.max_player_hp;
							else
								board.current_player_hp += board.gem_damage_value;
							break;
							
						case Board_C.armor.repel:
							if ((board.current_enemy_hp - board.gem_damage_value) < 0)
								board.current_enemy_hp = 0;
							else
								board.current_enemy_hp -= board.gem_damage_value;
							break;
						}
					}
				}


			if (board.current_player_hp < 0)
				board.current_player_hp = 0;

			board.Update_hp();
			
			if (board.current_player_hp == 0 
			    && board.lose_requirement_selected == Board_C.lose_requirement.player_hp_is_zero)
				board.Player_lose();

			}
	}
	#endregion


	public void Count_how_much_gem_there_are_to_move_over_me()
	{


		int empty_tiles = 0;
		for (int n_tiles = 0; (_y-n_tiles >=0); n_tiles++)
		{
			//interrupt the count if
			if ( (board.board_array_master[_x,_y-n_tiles,0] == -1)// no tile 
			    || ((board.board_array_master[_x,_y-n_tiles,1] >= 0) && (board.board_array_master[_x,_y-n_tiles,10]==0)) )//or this thing can't fall
				break;


			//annotate this tile as checked
			board.board_array_master[_x,_y-n_tiles,13] --;


			if (board.board_array_master[_x,_y-n_tiles,10] == 1) //if there is something to move
				{
				//this gem must be fall
				board.number_of_gems_to_move++;
				}
			else if (board.board_array_master[_x,_y-n_tiles,1] == -99) //this tile is empty
				{
				empty_tiles++;
				}

			if (board.board_array_master[_x,_y-n_tiles,12] > 0) //I reach the leader-tile
				board.number_of_new_gems_to_create += empty_tiles;
		}

	}
	

	public void Make_fall_all_free_gems_over_this_empty_tile()
	{


		bool leader_tile_become_empty = false;
		bool leader_tile_become_active = false;
		int number_of_gems_that_fall_in_this_column = 0;

		for (int yy = _y; yy >= 0 ; yy--)//look all tiles over me
		{

			//interrupt the count if
			if ((board.board_array_master[_x,yy,0] == -1)// no tile 
				|| (board.board_array_master[_x,yy,1] >= 0) && (board.board_array_master[_x,yy,10]==0)) //or this thing can't fall
				break;
			else //vertical falling
			{
				board.board_array_master[_x,yy,13] = 1; //annotate this tile as checked

				if (board.board_array_master[_x,yy,10] == 1) //if there is something that can fall
				{

					//update gem position
						//gem color go in this tile
						board.board_array_master[_x,_y-number_of_gems_that_fall_in_this_column,1] = board.board_array_master[_x,yy,1];
						//cancell gem color from old starting tile
						board.board_array_master[_x,yy,1] = -99;
						//pass special 
						board.board_array_master[_x,_y-number_of_gems_that_fall_in_this_column,4] = board.board_array_master[_x,yy,4];
						//cancel special 
						board.board_array_master[_x,yy,4] = 0;
						//new gem position
						board.board_array_master[_x,_y-number_of_gems_that_fall_in_this_column,10] = 1;
						//cancell gem from old starting tile
						board.board_array_master[_x,yy,10] = 0;
						//update falling block hp
						board.board_array_master[_x,_y-number_of_gems_that_fall_in_this_column,14] = board.board_array_master[_x,yy,14]; 
						board.board_array_master[_x,yy,14] = 0;


					//now show the gem avatar fall
					tile_C script_destination_tile = (tile_C)board.tiles_array[_x,(_y-number_of_gems_that_fall_in_this_column)].GetComponent("tile_C");
					tile_C script_origin_tile = (tile_C)board.tiles_array[_x,yy].GetComponent("tile_C");
						//link the avatar to new tile 
						script_destination_tile.my_gem = script_origin_tile.my_gem;
						//unlik the avater from the old tile
						script_origin_tile.my_gem = null;
						//start the animation
						script_destination_tile.Update_gem_position();


					//update gem moved count
					number_of_gems_that_fall_in_this_column++;

						//if a leader tile become empty, create a new gem
						if (board.board_array_master[_x,yy,12] == 1)//if this i a leader tile
							{	
							leader_tile_become_empty = true;
							script_origin_tile.StartCoroutine("Create_falling_gem");
							}
						

				}

				else if (!leader_tile_become_empty && !leader_tile_become_active) 
				{
					if ( (board.board_array_master[_x,yy,1] == -99)  //this tile is empty
					    && (board.board_array_master[_x,yy,12] == 1) )//and is a leader tile
					{	
						
						tile_C script_native_tile = (tile_C)board.tiles_array[_x,yy].GetComponent("tile_C");
							script_native_tile.StartCoroutine("Create_falling_gem");
						leader_tile_become_active = true;

					}
					
				}
			}
		}
	}

	void Generate_a_new_gem_in_this_tile()
	{
		//create a new gem in this tile
		random_color = (UnityEngine.Random.Range(0,board.gem_length));
	
		//Don't create 3 gem with the same color subsequently
		if (two_last_colors_created[0] == two_last_colors_created[1])
		{
			if (random_color == two_last_colors_created[0])
			{
				//update the color choiced				
				if (random_color+1 < board.gem_length)
					random_color++;
				else
					random_color = 0;
			}
		}
		
		//avoid 3 gem with the same color on x axis too
		if ( (_x >= 2) && (board.board_array_master[_x-1,_y,12] == 1) && (board.board_array_master[_x-2,_y,12] == 1) )
		{	//if this gem not will fall	
			if (!( (_y < board._Y_tiles-1) && (board.board_array_master[_x,_y+1,1] >= 0) )) //there are no empty tile under me
			{
				if (random_color == board.board_array_master[_x-1,_y,1])
				{
					//update the color choiced
					if (random_color+1 < board.gem_length)
						random_color++;
					else
						random_color = 0;
					
				}
				
			}
		}
		
		board.board_array_master[_x,_y,1] = random_color;
		//update list of color choiced by this leader-tile
		two_last_colors_created[0] = two_last_colors_created[1];
		two_last_colors_created[1] = random_color;
		
		board.board_array_master[_x,_y,10] = 1;//now this tile have a gem 
		
		//create gem avatar
		Take_from_garbage();

	}

	public IEnumerator Create_falling_gem()
	{
		yield return new WaitForSeconds(0.03f);//= wait that the previous gem is fall before create a new gem

		Generate_a_new_gem_in_this_tile ();
		//show animation
		my_gem.transform.localScale = new Vector3(0.0f,0.0f,0.0f);
			while (my_gem.transform.localScale.x < 1.2f)
				{
				my_gem.transform.localScale += new Vector3(0.3f,0.3f,0.3f);
				yield return new WaitForSeconds(0.03f);
				}

			if (my_gem.transform.localScale.x >= 1.2f)
				my_gem.transform.localScale = Vector3.one;

		board.number_of_new_gems_to_create--;

		//look at the tile under me to see if this gem must fall
		if ( (_y < board._Y_tiles-1) && (board.board_array_master[_x,_y+1,0] > -1) //there is a tile under me
		    && (board.board_array_master[_x,_y+1,1] == -99))  //and it is empty
			{

					board.number_of_gems_to_move++;
					board.gem_falling_ongoing = true;
					Search_last_empty_tile_under_me();
					if (board.board_array_master[_x,_y,1] == -99)//if this leader-tile is empty again
					{
						//repeat procedure
						StartCoroutine("Create_falling_gem");
					}
				
			
			}
			else //this gem don't fall
			{
			if (board.number_of_new_gems_to_create == 0) 
				{				
				Check_if_gem_movements_are_all_done();
				}
			}

	}

	public void Fall_by_one_step(int fall_direction)//0 down, 1 down R, 2 down L
	{
		if (fall_direction == 0)
		{
			//move the gem on the tile next up the last tile scanned
			board.board_array_master[_x,_y+1,1] = board.board_array_master[_x,_y,1];
			//color from old tile position
			board.board_array_master[_x,_y,1] = -99;
			
			//special
			board.board_array_master[_x,_y+1,4] = board.board_array_master[_x,_y,4];
			board.board_array_master[_x,_y,4] = 0;
			
			//the gem go in the new tile position
			board.board_array_master[_x,_y+1,10] = 1;
			//empty the old tile position
			board.board_array_master[_x,_y,10] = 0;
			
			//show the falling animation
			tile_C tile_script = (tile_C)board.tiles_array[_x,_y+1].GetComponent("tile_C");
			tile_script.my_gem = my_gem;
			my_gem = null;

			board.board_array_master[_x,_y+1,11] = board.board_array_master [_x, _y, 11];
			board.board_array_master [_x, _y, 11] = 0;

			tile_script.Update_gem_position();

		}
		else if (fall_direction == 1)
		{
			//move the gem on the tile next up the last tile scanned
			board.board_array_master[_x+1,_y+1,1] = board.board_array_master[_x,_y,1];
			//color from old tile position
			board.board_array_master[_x,_y,1] = -99;
			
			//special
			board.board_array_master[_x+1,_y+1,4] = board.board_array_master[_x,_y,4];
			board.board_array_master[_x,_y,4] = 0;
			
			//the gem go in the new tile position
			board.board_array_master[_x+1,_y+1,10] = 1;
			//empty the old tile position
			board.board_array_master[_x,_y,10] = 0;
			
			//show the falling animation
			tile_C tile_script = (tile_C)board.tiles_array[_x+1,_y+1].GetComponent("tile_C");
			tile_script.my_gem = my_gem;
			my_gem = null;

			board.board_array_master[_x+1,_y+1,11] = board.board_array_master [_x, _y, 11];
			board.board_array_master [_x, _y, 11] = 0;

			tile_script.Update_gem_position();
		}
		else if (fall_direction == 2)
		{
			//move the gem on the tile next up the last tile scanned
			board.board_array_master[_x-1,_y+1,1] = board.board_array_master[_x,_y,1];
			//color from old tile position
			board.board_array_master[_x,_y,1] = -99;
			
			//special
			board.board_array_master[_x-1,_y+1,4] = board.board_array_master[_x,_y,4];
			board.board_array_master[_x,_y,4] = 0;
			
			//the gem go in the new tile position
			board.board_array_master[_x-1,_y+1,10] = 1;
			//empty the old tile position
			board.board_array_master[_x,_y,10] = 0;
			
			//show the falling animation
			tile_C tile_script = (tile_C)board.tiles_array[_x-1,_y+1].GetComponent("tile_C");
			tile_script.my_gem = my_gem;
			my_gem = null;

			board.board_array_master[_x-1,_y+1,11] = board.board_array_master [_x, _y, 11];
			board.board_array_master [_x, _y, 11] = 0;

			tile_script.Update_gem_position();
			
		}

		//board.number_of_gems_to_move--;
	}

	public IEnumerator Create_gem()
	{
		//Debug.Log(_x + "," + _y + "Create_gem()");
		Generate_a_new_gem_in_this_tile ();
		my_gem.transform.localScale = new Vector3(0.0f,0.0f,0.0f);

		while (my_gem.transform.localScale.x < 1.2f)
		{
			my_gem.transform.localScale += new Vector3(0.3f,0.3f,0.3f);
			yield return new WaitForSeconds(0.03f);
		}
		
		if (my_gem.transform.localScale.x >= 1.2f)
			my_gem.transform.localScale = Vector3.one;
		
		board.number_of_new_gems_to_create--;
		board.board_array_master [_x, _y, 11] = 0;//xxxx 

		Check_if_gem_movements_are_all_done ();
	}


	void Search_last_empty_tile_under_me()
		{

			for (int yy = 1; (_y+yy)<= board._Y_tiles-1 ; yy++)//scan tiles under me
				{
				if((board.board_array_master[_x,(_y+yy),0] == -1) //if I find no tile
			   		|| (board.board_array_master[_x,_y+yy,1] >= 0) ) //or a tile with something
						{

								//move the gem on the tile next up the last tile scanned
								board.board_array_master[_x,_y+yy-1,1] = board.board_array_master[_x,_y,1];
								//color from old tile position
								board.board_array_master[_x,_y,1] = -99;

								//special
								board.board_array_master[_x,_y+yy-1,4] = board.board_array_master[_x,_y,4];
								board.board_array_master[_x,_y,4] = 0;

								//the gem go in the new tile position
								board.board_array_master[_x,_y+yy-1,10] = 1;
								//empty the old tile position
								board.board_array_master[_x,_y,10] = 0;

								//show the falling animation
								tile_C tile_script = (tile_C)board.tiles_array[_x,(_y+yy-1)].GetComponent("tile_C");
											tile_script.my_gem = my_gem;
											tile_script.Update_gem_position();
											my_gem = null;

								break;
						}
					else if ((_y+yy) == board._Y_tiles-1) //if I'm on the last row
						{

						//move gem color on last tile
						board.board_array_master[_x,_y+yy,1] = board.board_array_master[_x,_y,1];
						//remove color from old tile
						board.board_array_master[_x,_y,1] = -99;

						//special 
						board.board_array_master[_x,_y+yy-1,4] = board.board_array_master[_x,_y,4];
						board.board_array_master[_x,_y,4] = 0;
						
						//gem go in the new position
						board.board_array_master[_x,_y+yy,10] = 1;
						//empty the old tile
						board.board_array_master[_x,_y,10] = 0;


						//show falling animation
						tile_C tile_script = (tile_C)board.tiles_array[_x,(_y+yy)].GetComponent("tile_C");
						tile_script.my_gem = my_gem;
						my_gem = null;
							tile_script.my_gem.transform.parent = tile_script.transform;
						tile_script.Update_gem_position();
					
						
						break;
						}
				}

		}

	#region replace gem during shuffle
	public IEnumerator Shuffle_update()//call from Board_C.Shuffle(), Gems_teleport()
	{
		//minimize gem
		while (my_gem.transform.localScale.x > 0)
		{
		my_gem.transform.localScale -= new Vector3(0.2f,0.2f,0.2f);
		yield return new WaitForSeconds(0.03f);
		}
	
		if (my_gem.transform.localScale.x <= 0)
			my_gem.transform.localScale = Vector3.zero;

		//update gem
		SpriteRenderer sprite_gem = my_gem.GetComponent<SpriteRenderer>();
			sprite_gem.sprite = board.gem_colors[board.board_array_master[_x,_y,1]];

		//return to normal size
		while (my_gem.transform.localScale.x < 1.2f)
		{
			my_gem.transform.localScale += new Vector3(0.2f,0.2f,0.2f);
			yield return new WaitForSeconds(0.03f);
		}
		
		if (my_gem.transform.localScale.x >= 1.2f)
			my_gem.transform.localScale = Vector3.one;

		Check_if_shuffle_is_done();
	}

	void Check_if_shuffle_is_done()
		{
		board.number_of_gems_to_mix--;
		if (board.number_of_gems_to_mix == 0)
			{
			if (board.bonus_select != Board_C.bonus.switch_gem_teleport)
				board.Check_ALL_possible_moves();
			else
				{
				Reset_charge_fill();
				board.Update_inventory_bonus(2,-1);
				board.Check_secondary_explosions();
				}
			}
		}

	public IEnumerator Show_token()
	{
		//minimize gem
		while (my_gem.transform.localScale.x > 0)
		{
			my_gem.transform.localScale -= new Vector3(0.2f,0.2f,0.2f);
			yield return new WaitForSeconds(0.03f);
		}
		
		if (my_gem.transform.localScale.x <= 0)
			my_gem.transform.localScale = Vector3.zero;
		
		//update gem
		SpriteRenderer sprite_gem = my_gem.GetComponent<SpriteRenderer>();
		sprite_gem.sprite = board.token;
		
		//return to normal size
		while (my_gem.transform.localScale.x < 1.2f)
		{
			my_gem.transform.localScale += new Vector3(0.2f,0.2f,0.2f);
			yield return new WaitForSeconds(0.03f);
		}
		
		if (my_gem.transform.localScale.x >= 1.2f)
			my_gem.transform.localScale = Vector3.one;

	}

	#endregion

	
	#region manage gem falling and creation
	
	public void Update_gem_position()
		{
		board.gem_falling_ongoing = true;
		StartCoroutine(Move_gem_to_this_tile());
		}
	

	IEnumerator Move_gem_to_this_tile()
	{


		while (Vector3.Distance(transform.position, my_gem.transform.position) > board.accuracy) 
			{
			yield return new WaitForSeconds (0.015f);

			if (my_gem) 
				my_gem.transform.Translate (((transform.position - my_gem.transform.position).normalized) * board.falling_speed * Time.deltaTime, Space.World);
			else
				Debug.LogWarning ("no gem in " + _x + "," + _y);


			}

		if (Vector3.Distance (transform.position, my_gem.transform.position) <= board.accuracy) 
			{
			if (Time.timeSinceLevelLoad > board.latest_sfx_time + 0.01f) 
				{
				Play_sfx (board.end_fall_sfx);
				board.latest_sfx_time = Time.timeSinceLevelLoad;
				}

			if (Vector3.Distance (transform.position, my_gem.transform.position) != 0)
				my_gem.transform.position = transform.position;


			if (!board.diagonal_falling)
				StartCoroutine (End_fall_animation ());
				

				End_falling_gems ();
			}


	}

	IEnumerator End_fall_animation()
	{

		Vector3 temp_position = new Vector3(transform.position.x, transform.position.y-0.25f , transform.position.z);

		//go down
		while(Vector3.Distance(temp_position, my_gem.transform.position) > board.accuracy)
		{
			yield return new WaitForSeconds(0.02f);
			
			my_gem.transform.Translate(((temp_position - my_gem.transform.position).normalized) * (board.falling_speed*0.25f)* Time.deltaTime, Space.World);
			
		}

		//return up
		while(Vector3.Distance(transform.position, my_gem.transform.position) > board.accuracy)
		{
			yield return new WaitForSeconds(0.03f);
			
			my_gem.transform.Translate(((transform.position - my_gem.transform.position).normalized) * (board.falling_speed*0.25f) * Time.deltaTime, Space.World);
			
		}


		if (Vector3.Distance(transform.position, my_gem.transform.position) <= board.accuracy)
		{
			if(Vector3.Distance(transform.position, my_gem.transform.position) != 0)
				my_gem.transform.position = transform.position;
		}


	}


	public IEnumerator Switch_gem_animation()
	{

		while(Vector3.Distance(transform.position, my_gem.transform.position) > board.accuracy)
		{
			yield return new WaitForSeconds(0.015f);
			my_gem.transform.Translate(((transform.position - my_gem.transform.position).normalized) * board.switch_speed * Time.deltaTime, Space.World);

			if  (Vector3.Distance(transform.position, my_gem.transform.position) <= board.accuracy)
			{
				if(Vector3.Distance(transform.position, my_gem.transform.position) != 0)
					my_gem.transform.position = transform.position;
			}
		}


		
	}
	
	void End_falling_gems()
		{

		//Debug.Log (_x + "," + _y + " = " + board.board_array_master [_x, _y, 1]);
		board.board_array_master [_x, _y, 11] = 0;//xxxx 

			board.number_of_gems_to_move--;
			Check_if_gem_movements_are_all_done();
		}
	

	void Play_sfx(AudioClip clip)
	{
		if (board.Stage_uGUI_obj)
			{
			/* if you have the menu kit, DELETE THIS LINE 
			if (my_game_master && clip)
				my_game_master.Gui_sfx(clip);
			//if you have the menu kit, DELETE THIS LINE */
			}
		else
			{
			GetComponent<AudioSource>().clip = clip;
			GetComponent<AudioSource>().Play();
			}
		
	}

	void Check_if_gem_movements_are_all_done()
		{
		if (board.number_of_gems_to_move + board.number_of_new_gems_to_create == 0)
			{
			board.gem_falling_ongoing = false;

			if (board.diagonal_falling)
				{
				//board.Debug_Board();
				board.Start_update_board ();
				}
			else
				board.Check_secondary_explosions();
				
			}

		}

	void Take_from_garbage()
	{
		my_gem = board.garbage_recycle.transform.GetChild(0).gameObject;
		my_gem.transform.position = new Vector3(_x,-_y,0)+board.pivot_board.position;
		my_gem.transform.parent = board.pivot_board.transform;

		SpriteRenderer sprite_gem = my_gem.GetComponent<SpriteRenderer>();
			sprite_gem.sprite = board.gem_colors[board.board_array_master[_x,_y,1]];

	}





	#endregion


}
