
using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using UnityEditor;

using System.IO;
using System;

[CustomEditor(typeof(Board_C))]
internal class match_controller : Editor {
	
	
	
	
	int total_gem_to_collect;
	
	#region temp AI
	public enum enemy_AI_gem_collect
	{
		random,
		collect_gems_from_less_to_more, 
		collect_gems_from_more_to_less, 
		by_hand_setup
	}
	public enemy_AI_gem_collect enemy_AI_gem_collect_select;
	
	public enum enemy_AI_battle
	{
		random,
		dynamic_battle, 
		by_hand_setup
	}
	public enemy_AI_battle enemy_AI_battle_select;
	
	public enum enemy_AI_target_score
	{
		random,
		by_hand_setup
	}
	public enemy_AI_target_score enemy_AI_target_score_select;
	#endregion
	
	#region temp bonus
	public enum charge_bonus
	{
		none = 0,	
		destroy_one = 1, 
		switch_gem_teleport = 2,
		destroy_3x3 = 3, 
		destroy_horizontal = 4,	
		destroy_vertical = 5,
		destroy_horizontal_and_vertical = 6,
		destroy_all_gem_with_this_color = 7,
		give_more_time = 8,
		give_more_moves = 9,
		heal_hp = 10
	}
	public charge_bonus[] charge_bonus_select;
	public charge_bonus[] enemy_charge_bonus_select;
	
	public enum on_board_bonus
	{
		none = 0,	
		//destroy_one = 1, 
		//switch_gem_teleport = 2,
		destroy_3x3 = 3, 
		destroy_horizontal = 4,	
		destroy_vertical = 5,
		destroy_horizontal_and_vertical = 6,
		//destroy_all_gem_with_this_color = 7,
		give_more_time = 8,
		give_more_moves = 9,
		heal_hp = 10
	}
	public on_board_bonus[] on_board_bonus_select;


	#endregion
	
	
	void OnEnable()
	{
		Board_C my_target = (Board_C)target;
		on_board_bonus_select = new on_board_bonus[my_target.bonus_slot.Length];
		charge_bonus_select = new charge_bonus[my_target.bonus_slot.Length];
		enemy_charge_bonus_select = new charge_bonus[my_target.enemy_bonus_slot.Length];
	}
	
	public override void OnInspectorGUI()
	{
		
		Stage();
		Rules();
		Camera_setup();
		Assign_sprites();
		Audio_sfx();
		Visual_enhancements();
		Advanced();
		if (!EditorApplication.isPlaying)
			Show_gui();
		
	}
	
	void Stage()
	{
		Board_C my_target = (Board_C)target;
		EditorGUI.BeginChangeCheck ();
        Undo.RecordObject(my_target, "edit_stage");

        if (my_target.Stage_uGUI_obj == null)
			my_target.current_level = EditorGUILayout.IntField("Stage n",my_target.current_level);
		
		if (!my_target.file_txt_asset)
			GUI.color = Color.red;
		else
			GUI.color = Color.white;
		my_target.file_txt_asset = EditorGUILayout.ObjectField("Stage file", my_target.file_txt_asset, typeof(TextAsset), true) as TextAsset;
		GUI.color = Color.white;
		
		my_target.start_after_selected = (Board_C.start_after)EditorGUILayout.EnumPopup("Start condition",my_target.start_after_selected);
		if (my_target.start_after_selected == Board_C.start_after.time)
		{
			EditorGUI.indentLevel++;
			if (my_target.start_after_n_seconds < 0)
				GUI.color = Color.red;
			else
				GUI.color = Color.white;
			
			my_target.start_after_n_seconds = EditorGUILayout.FloatField("start after n seconds",my_target.start_after_n_seconds);
			
			GUI.color = Color.white;
			EditorGUI.indentLevel--;
		}

		if (EditorGUI.EndChangeCheck ())
			EditorUtility.SetDirty(my_target);
	}
	
	void Rules()
	{
		Board_C my_target = (Board_C)target;
		EditorGUI.BeginChangeCheck ();
        Undo.RecordObject(my_target, "edit_rules");

        my_target.show_rules = EditorGUILayout.Foldout(my_target.show_rules, "Rules");
		if (my_target.show_rules)
		{
			EditorGUI.indentLevel = 1;
			
			my_target.win_requirement_selected = (Board_C.win_requirement)EditorGUILayout.EnumPopup("Win requirement",my_target.win_requirement_selected);
			EditorGUI.indentLevel++;
			if (my_target.win_requirement_selected == Board_C.win_requirement.collect_gems)
			{
				total_gem_to_collect = 0;
				for (int i = 0; i < my_target.gem_length ; i ++)
				{
					if (my_target.number_of_gems_to_destroy_to_win[i] < 0)
						GUI.color = Color.red;
					else
						GUI.color = Color.white;
					my_target.number_of_gems_to_destroy_to_win[i] = EditorGUILayout.IntField("gem " + i + " needful", my_target.number_of_gems_to_destroy_to_win[i]);
					total_gem_to_collect += my_target.number_of_gems_to_destroy_to_win[i];
				}
				GUI.color = Color.white;
				if (total_gem_to_collect == 0)
					EditorGUILayout.LabelField("WARNING! The total number of gem to collect can't be zero!");
				
				
			}
			else if (my_target.win_requirement_selected == Board_C.win_requirement.reach_target_score)
			{ 
				my_target.target_score = EditorGUILayout.IntField("target score", my_target.target_score);
				
			}
			else if (my_target.win_requirement_selected == Board_C.win_requirement.take_all_tokens)
				{ 
				EditorGUI.indentLevel++;
					my_target.show_token_after_all_tiles_are_destroyed = EditorGUILayout.Toggle("Show token only after all tiles are destroyed", my_target.show_token_after_all_tiles_are_destroyed);
				EditorGUI.indentLevel--;
				}

			if (my_target.win_requirement_selected != Board_C.win_requirement.play_until_lose && my_target.win_requirement_selected != Board_C.win_requirement.enemy_hp_is_zero)
				my_target.continue_to_play_after_win_until_lose_happen = EditorGUILayout.Toggle("continue to play after win",my_target.continue_to_play_after_win_until_lose_happen);
			else
				my_target.continue_to_play_after_win_until_lose_happen = false;

			GUI.color = Color.white;
			EditorGUI.indentLevel--;
			
			my_target.lose_requirement_selected = (Board_C.lose_requirement)EditorGUILayout.EnumPopup("Lose requirement",my_target.lose_requirement_selected);
			EditorGUI.indentLevel++;
			if (my_target.lose_requirement_selected == Board_C.lose_requirement.timer)
			{
				if (my_target.timer <= 0)
					GUI.color = Color.red;
				else
					GUI.color = Color.white;
				my_target.timer = EditorGUILayout.FloatField("Time in seconds",my_target.timer);

				EditorGUILayout.LabelField("time bonus for:");
				EditorGUI.indentLevel++;
				if (my_target.time_bonus_for_gem_explosion < 0)
					GUI.color = Color.red;
				else
					GUI.color = Color.white;
				my_target.time_bonus_for_gem_explosion = EditorGUILayout.FloatField("gem explosion",my_target.time_bonus_for_gem_explosion);
				
				if (my_target.time_bonus_for_secondary_explosion < 0)
					GUI.color = Color.red;
				else
					GUI.color = Color.white;
				my_target.time_bonus_for_secondary_explosion = EditorGUILayout.FloatField("secondary explosion",my_target.time_bonus_for_secondary_explosion);
				EditorGUI.indentLevel--;
				
			}
			else if (my_target.lose_requirement_selected == Board_C.lose_requirement.player_have_zero_moves)
			{
				my_target.max_moves = EditorGUILayout.IntField("Moves",my_target.max_moves);
				EditorGUI.indentLevel++;
				my_target.show_move_reward_system = EditorGUILayout.Foldout(my_target.show_move_reward_system, "move rewards");
				if (my_target.show_move_reward_system)
				{
					my_target.lose_turn_if_bad_move = EditorGUILayout.Toggle("lost a move if bad move",my_target.lose_turn_if_bad_move);
					EditorGUILayout.LabelField("gain moves if explode:");
					EditorGUI.indentLevel++;

					my_target.gain_turn_if_secondary_explosion  = EditorGUILayout.Toggle("secondary explosion",my_target.gain_turn_if_secondary_explosion);
					if (my_target.gain_turn_if_secondary_explosion)
					{
						EditorGUI.indentLevel++;
						if (my_target.seconday_explosion_maginiture_needed_to_gain_a_turn < 3)
							GUI.color = Color.red;
						else
							GUI.color = Color.white;
						my_target.seconday_explosion_maginiture_needed_to_gain_a_turn = EditorGUILayout.IntField("minimum magnitude requested", my_target.seconday_explosion_maginiture_needed_to_gain_a_turn);
						GUI.color = Color.white;

						if (my_target.combo_lenght_needed_to_gain_a_turn < 0)
							GUI.color = Color.red;
						else
							GUI.color = Color.white;
						my_target.combo_lenght_needed_to_gain_a_turn = EditorGUILayout.IntField("minimum combo lenght requested", my_target.combo_lenght_needed_to_gain_a_turn);
						GUI.color = Color.white;
						EditorGUI.indentLevel--;
					}

					my_target.gain_turn_if_explode_same_color_of_previous_move = EditorGUILayout.Toggle("same color of your previous move",my_target.gain_turn_if_explode_same_color_of_previous_move);
					if (my_target.gain_turn_if_explode_same_color_of_previous_move)
					{
						EditorGUI.indentLevel++;
						if (my_target.move_gained_for_explode_same_color_in_two_adjacent_turn < 0)
							GUI.color = Color.red;
						else
							GUI.color = Color.white;
						my_target.move_gained_for_explode_same_color_in_two_adjacent_turn = EditorGUILayout.IntField("give n moves:",my_target.move_gained_for_explode_same_color_in_two_adjacent_turn);
						GUI.color = Color.white;
						EditorGUI.indentLevel--;
					}
					my_target.gain_turn_if_explode_more_than_3_gems  = EditorGUILayout.Toggle("more than 3 gems",my_target.gain_turn_if_explode_more_than_3_gems);
					if (my_target.gain_turn_if_explode_more_than_3_gems)
					{
						EditorGUI.indentLevel++;
						for (int i = 0; i < 4 ; i ++)
						{
							if (my_target.move_gained_when_explode_more_than_3_gems[i] < 0)
								GUI.color = Color.red;
							else
								GUI.color = Color.white;
							my_target.move_gained_when_explode_more_than_3_gems[i] = EditorGUILayout.IntField("explode " + (4+i) + " gems give " + my_target.move_gained_when_explode_more_than_3_gems[i] + " moves", my_target.move_gained_when_explode_more_than_3_gems[i]);
						}
						GUI.color = Color.white;
						EditorGUI.indentLevel--;
					}
					
					if ((my_target.gain_turn_if_explode_same_color_of_previous_move) || (my_target.gain_turn_if_explode_more_than_3_gems))
					{
						EditorGUILayout.BeginHorizontal();
						my_target.chain_turns_limit = EditorGUILayout.Toggle("chain limit",my_target.chain_turns_limit);
						if (my_target.chain_turns_limit)
						{
							if (my_target.max_chain_turns <= 0)
								GUI.color = Color.red;
							else
								GUI.color = Color.white;
							my_target.max_chain_turns = EditorGUILayout.IntField("max",my_target.max_chain_turns);
							GUI.color = Color.white;
						}
						EditorGUILayout.EndHorizontal();
					}
					EditorGUI.indentLevel--;
					
				}
				EditorGUI.indentLevel--;
			}
			
			GUI.color = Color.white;
			EditorGUI.indentLevel--;

			if (my_target.win_requirement_selected == Board_C.win_requirement.destroy_all_tiles)
				my_target.explosions_damages_tiles = true;
			else if ((my_target.win_requirement_selected == Board_C.win_requirement.take_all_tokens) && my_target.show_token_after_all_tiles_are_destroyed)
				my_target.explosions_damages_tiles = true;
			else
				my_target.explosions_damages_tiles = EditorGUILayout.Toggle("Explosion damages tiles", my_target.explosions_damages_tiles);


			EditorGUI.indentLevel++;
			if (my_target.explosions_damages_tiles)
			{
				my_target.show_chess_board_decoration = false;
				my_target.tile_give = (Board_C.tile_destroyed_give)EditorGUILayout.EnumPopup("Damage a tile give",my_target.tile_give);
				EditorGUI.indentLevel++;
				if (my_target.tile_give == Board_C.tile_destroyed_give.more_hp)
				{
					if (my_target.tile_gift_int <= 0)
						GUI.color = Color.red;
					else
						GUI.color = Color.white;
					my_target.tile_gift_int = EditorGUILayout.IntField("How much HP?",my_target.tile_gift_int);
				}
				else if (my_target.tile_give == Board_C.tile_destroyed_give.more_time)
				{
					if (my_target.tile_gift_float <= 0)
						GUI.color = Color.red;
					else
						GUI.color = Color.white;
					my_target.tile_gift_float = EditorGUILayout.FloatField("How much seconds?",my_target.tile_gift_float);
				}
				else if (my_target.tile_give == Board_C.tile_destroyed_give.more_moves)
				{
					if (my_target.tile_gift_int <= 0)
						GUI.color = Color.red;
					else
						GUI.color = Color.white;
					my_target.tile_gift_int = EditorGUILayout.IntField("How much moves?",my_target.tile_gift_int);
				}
				GUI.color = Color.white;
				EditorGUI.indentLevel--;


			}
			EditorGUI.indentLevel--;
			
			EditorGUILayout.BeginHorizontal();
			my_target.show_hint = EditorGUILayout.Toggle("Show hint", my_target.show_hint);
			if (my_target.show_hint)
				my_target.show_hint_after_n_seconds = EditorGUILayout.FloatField("after n seconds",my_target.show_hint_after_n_seconds);
			EditorGUILayout.EndHorizontal();
			
			my_target.show_bonus = EditorGUILayout.Foldout(my_target.show_bonus, "Bonus");
			if (my_target.show_bonus)
			{
				
				my_target.give_bonus_select = (Board_C.give_bonus)EditorGUILayout.EnumPopup("give bonus" ,my_target.give_bonus_select);
				EditorGUI.indentLevel++;

				if (my_target.give_bonus_select == Board_C.give_bonus.no)
					my_target.trigger_by_select = Board_C.trigger_by.OFF;
				else if (my_target.give_bonus_select == Board_C.give_bonus.after_charge)
				{
					my_target.trigger_by_select = Board_C.trigger_by.OFF;
					my_target.show_player_charge_bonus = EditorGUILayout.Foldout(my_target.show_player_charge_bonus, "player slot bonus");
					if (my_target.show_player_charge_bonus)
					{
						my_target.bonus_slot_availables = EditorGUILayout.IntSlider("n. slot availables",my_target.bonus_slot_availables,0,my_target.gem_length);
						
						for (int i = 0; i < my_target.bonus_slot_availables ; i ++)
						{
							//copy from target to temp
							switch(my_target.bonus_slot[i])
							{
							case Board_C.bonus.none:
								charge_bonus_select[i] = charge_bonus.none;
								break;
								
							case Board_C.bonus.destroy_one:
								charge_bonus_select[i] = charge_bonus.destroy_one;
								break;
								
							case Board_C.bonus.switch_gem_teleport:
								charge_bonus_select[i] = charge_bonus.switch_gem_teleport;
								break;
								
							case Board_C.bonus.destroy_3x3:
								charge_bonus_select[i] = charge_bonus.destroy_3x3;
								break;
								
							case Board_C.bonus.destroy_horizontal:
								charge_bonus_select[i] = charge_bonus.destroy_horizontal;
								break;
								
							case Board_C.bonus.destroy_vertical:
								charge_bonus_select[i] = charge_bonus.destroy_vertical;
								break;
								
							case Board_C.bonus.destroy_horizontal_and_vertical:
								charge_bonus_select[i] = charge_bonus.destroy_horizontal_and_vertical;
								break;
								
							case Board_C.bonus.destroy_all_gem_with_this_color:
								charge_bonus_select[i] = charge_bonus.destroy_all_gem_with_this_color;
								break;
								
							case Board_C.bonus.give_more_time:
								charge_bonus_select[i] = charge_bonus.give_more_time;
								break;

							case Board_C.bonus.give_more_moves:
								charge_bonus_select[i] = charge_bonus.give_more_moves;
								break;

							case Board_C.bonus.heal_hp:
								charge_bonus_select[i] = charge_bonus.heal_hp;
								break;
							}
							EditorGUILayout.BeginHorizontal();
							charge_bonus_select[i] = (charge_bonus)EditorGUILayout.EnumPopup("B. " + i,charge_bonus_select[i]);
							//my_target.bonus_slot[i] = (Board_C.bonus)charge_bonus_select[i];
							my_target.bonus_cost[i] = EditorGUILayout.IntField(i + " gems cost",my_target.bonus_cost[i]);
							EditorGUILayout.EndHorizontal();
							
							//copy from temp to target
							switch(charge_bonus_select[i])
							{
							case charge_bonus.none:
								my_target.bonus_slot[i] = Board_C.bonus.none;
								break;
								
							case charge_bonus.destroy_one:
								my_target.bonus_slot[i] = Board_C.bonus.destroy_one;
								break;
								
							case charge_bonus.switch_gem_teleport:
								my_target.bonus_slot[i] = Board_C.bonus.switch_gem_teleport;
								break;
								
							case charge_bonus.destroy_3x3:
								my_target.bonus_slot[i] = Board_C.bonus.destroy_3x3;
								break;
								
							case charge_bonus.destroy_horizontal:
								my_target.bonus_slot[i] = Board_C.bonus.destroy_horizontal;
								break;
								
							case charge_bonus.destroy_vertical:
								my_target.bonus_slot[i] = Board_C.bonus.destroy_vertical;
								break;
								
							case charge_bonus.destroy_horizontal_and_vertical:
								my_target.bonus_slot[i] = Board_C.bonus.destroy_horizontal_and_vertical;
								break;
								
							case charge_bonus.destroy_all_gem_with_this_color:
								my_target.bonus_slot[i] = Board_C.bonus.destroy_all_gem_with_this_color;
								break;
								
							case charge_bonus.give_more_time:
								my_target.bonus_slot[i] = Board_C.bonus.give_more_time;
								break;

							case charge_bonus.give_more_moves:
								my_target.bonus_slot[i] = Board_C.bonus.give_more_moves;
								break;

							case charge_bonus.heal_hp:
								my_target.bonus_slot[i] = Board_C.bonus.heal_hp;
								break;
							}
							
							/*
							EditorGUILayout.BeginHorizontal();
							my_target.bonus_slot[i] = (Board_C.bonus)EditorGUILayout.EnumPopup("B. " + i,my_target.bonus_slot[i]);
							my_target.bonus_cost[i] = EditorGUILayout.IntField(i + " gems cost",my_target.bonus_cost[i]);
							EditorGUILayout.EndHorizontal();
							*/
							if (my_target.bonus_slot[i] == Board_C.bonus.give_more_time)
							{
								EditorGUI.indentLevel++;
								my_target.add_time_bonus = EditorGUILayout.FloatField("How much seconds?",my_target.add_time_bonus);
								EditorGUI.indentLevel--;
							}
							else if (my_target.bonus_slot[i] == Board_C.bonus.heal_hp)
							{
								EditorGUI.indentLevel++;
								my_target.heal_hp_player_bonus = EditorGUILayout.IntField("How much heal?",my_target.heal_hp_player_bonus);
								EditorGUI.indentLevel--;
							}
						}
					}

					if ( //(my_target.lose_requirement_selected == Board_C.lose_requirement.player_hp_is_zero) ||
					    (my_target.win_requirement_selected == Board_C.win_requirement.enemy_hp_is_zero)
					    || (my_target.lose_requirement_selected == Board_C.lose_requirement.enemy_collect_gems)
					    || (my_target.lose_requirement_selected == Board_C.lose_requirement.enemy_reach_target_score))
						{
						my_target.show_enemy_charge_bonus = EditorGUILayout.Foldout(my_target.show_enemy_charge_bonus, "enemy slot bonus");
						if (my_target.show_enemy_charge_bonus)
						{
							my_target.enemey_bonus_slot_availables = EditorGUILayout.IntSlider("n. slot availables",my_target.enemey_bonus_slot_availables,0,my_target.gem_length);
							
							for (int i = 0; i < my_target.enemey_bonus_slot_availables ; i ++)
							{
								//copy from target to temp
								switch(my_target.enemy_bonus_slot[i])
								{
								case Board_C.bonus.none:
									enemy_charge_bonus_select[i] = charge_bonus.none;
									break;
									
								case Board_C.bonus.destroy_one:
									enemy_charge_bonus_select[i] = charge_bonus.destroy_one;
									break;
									
								case Board_C.bonus.switch_gem_teleport:
									enemy_charge_bonus_select[i] = charge_bonus.switch_gem_teleport;
									break;
									
								case Board_C.bonus.destroy_3x3:
									enemy_charge_bonus_select[i] = charge_bonus.destroy_3x3;
									break;
									
								case Board_C.bonus.destroy_horizontal:
									enemy_charge_bonus_select[i] = charge_bonus.destroy_horizontal;
									break;
									
								case Board_C.bonus.destroy_vertical:
									enemy_charge_bonus_select[i] = charge_bonus.destroy_vertical;
									break;
									
								case Board_C.bonus.destroy_horizontal_and_vertical:
									enemy_charge_bonus_select[i] = charge_bonus.destroy_horizontal_and_vertical;
									break;
									
								case Board_C.bonus.destroy_all_gem_with_this_color:
									enemy_charge_bonus_select[i] = charge_bonus.destroy_all_gem_with_this_color;
									break;
									
								case Board_C.bonus.give_more_time:
									enemy_charge_bonus_select[i] = charge_bonus.give_more_time;
									break;

								case Board_C.bonus.give_more_moves:
									enemy_charge_bonus_select[i] = charge_bonus.give_more_moves;
									break;

								case Board_C.bonus.heal_hp:
									enemy_charge_bonus_select[i] = charge_bonus.heal_hp;
									break;
								}
								EditorGUILayout.BeginHorizontal();
								enemy_charge_bonus_select[i] = (charge_bonus)EditorGUILayout.EnumPopup("B. " + i,enemy_charge_bonus_select[i]);
								//my_target.bonus_slot[i] = (Board_C.bonus)charge_bonus_select[i];
								my_target.enemy_bonus_cost[i] = EditorGUILayout.IntField(i + " gems cost",my_target.enemy_bonus_cost[i]);
								EditorGUILayout.EndHorizontal();
								
								//copy from temp to target
								switch(enemy_charge_bonus_select[i])
								{
								case charge_bonus.none:
									my_target.enemy_bonus_slot[i] = Board_C.bonus.none;
									break;
									
								case charge_bonus.destroy_one:
									my_target.enemy_bonus_slot[i] = Board_C.bonus.destroy_one;
									break;
									
								case charge_bonus.switch_gem_teleport:
									my_target.enemy_bonus_slot[i] = Board_C.bonus.switch_gem_teleport;
									break;
									
								case charge_bonus.destroy_3x3:
									my_target.enemy_bonus_slot[i] = Board_C.bonus.destroy_3x3;
									break;
									
								case charge_bonus.destroy_horizontal:
									my_target.enemy_bonus_slot[i] = Board_C.bonus.destroy_horizontal;
									break;
									
								case charge_bonus.destroy_vertical:
									my_target.enemy_bonus_slot[i] = Board_C.bonus.destroy_vertical;
									break;
									
								case charge_bonus.destroy_horizontal_and_vertical:
									my_target.enemy_bonus_slot[i] = Board_C.bonus.destroy_horizontal_and_vertical;
									break;
									
								case charge_bonus.destroy_all_gem_with_this_color:
									my_target.enemy_bonus_slot[i] = Board_C.bonus.destroy_all_gem_with_this_color;
									break;
									
								case charge_bonus.give_more_time:
									my_target.enemy_bonus_slot[i] = Board_C.bonus.give_more_time;
									break;

								case charge_bonus.give_more_moves:
									my_target.enemy_bonus_slot[i] = Board_C.bonus.give_more_moves;
									break;

								case charge_bonus.heal_hp:
									my_target.enemy_bonus_slot[i] = Board_C.bonus.heal_hp;
									break;
								}
								

								if (my_target.enemy_bonus_slot[i] == Board_C.bonus.give_more_time)
								{
									EditorGUI.indentLevel++;
									my_target.add_time_bonus = EditorGUILayout.FloatField("How much seconds?",my_target.add_time_bonus);
									EditorGUI.indentLevel--;
								}
								else if (my_target.enemy_bonus_slot[i] == Board_C.bonus.heal_hp)
								{
									EditorGUI.indentLevel++;
									my_target.heal_hp_enemy_bonus = EditorGUILayout.IntField("How much heal?",my_target.heal_hp_enemy_bonus);
									EditorGUI.indentLevel--;
								} 
							}
						}
						}
				}
				else if (my_target.give_bonus_select == Board_C.give_bonus.after_big_explosion)
				{
					if (my_target.trigger_by_select == Board_C.trigger_by.OFF)
						GUI.color = Color.red;
					else
						GUI.color = Color.white;
					my_target.trigger_by_select = (Board_C.trigger_by)EditorGUILayout.EnumPopup("trigger by" ,my_target.trigger_by_select);
					GUI.color = Color.white;
					
					my_target.choose_bonus_by_select = (Board_C.choose_bonus_by)EditorGUILayout.EnumPopup("choose bonus by" ,my_target.choose_bonus_by_select);
					EditorGUI.indentLevel++;
					if (my_target.choose_bonus_by_select == Board_C.choose_bonus_by.gem_color)
					{
						//if (my_target.trigger_by_select == Board_C.trigger_by.inventory)
							//{
							for (int i = 0; i < my_target.gem_length; i++)
								{
								my_target.color_explosion_give_bonus[i] = (Board_C.bonus)EditorGUILayout.EnumPopup("color " + i + " give" ,my_target.color_explosion_give_bonus[i]);
								if (my_target.color_explosion_give_bonus[i] == Board_C.bonus.give_more_moves)
									{
									EditorGUI.indentLevel++;
									if (my_target.add_moves_bonus < 1)
										GUI.color = Color.red;
									else
										GUI.color = Color.white;
									my_target.add_moves_bonus = EditorGUILayout.IntField("add moves",my_target.add_moves_bonus);
									GUI.color = Color.white;
									EditorGUI.indentLevel--;
									}
								else if (my_target.color_explosion_give_bonus[i] == Board_C.bonus.give_more_time)
									{
									EditorGUI.indentLevel++;
									if (my_target.add_time_bonus <= 0)
										GUI.color = Color.red;
									else
										GUI.color = Color.white;
									my_target.add_time_bonus = EditorGUILayout.FloatField("add seconds",my_target.add_time_bonus);
									GUI.color = Color.white;
									EditorGUI.indentLevel--;
									}
								else if (my_target.color_explosion_give_bonus[i] == Board_C.bonus.heal_hp)
									{
									EditorGUI.indentLevel++;
									if (my_target.heal_hp_player_bonus < 1)
										GUI.color = Color.red;
									else
										GUI.color = Color.white;
									my_target.heal_hp_player_bonus = EditorGUILayout.IntField("add HP",my_target.heal_hp_player_bonus);
									GUI.color = Color.white;
									EditorGUI.indentLevel--;
									}
								}

							//}
						/*else
							{
							for (int i = 0; i < my_target.gem_length; i++)
							{
								//copy from target to temp
								switch(my_target.color_explosion_give_bonus[i])
								{
								case Board_C.bonus.none:
									on_board_bonus_select[i] = on_board_bonus.none;
									break;
									
								case Board_C.bonus.destroy_3x3:
									on_board_bonus_select[i] = on_board_bonus.destroy_3x3;
									break;
									
								case Board_C.bonus.destroy_horizontal:
									on_board_bonus_select[i] = on_board_bonus.destroy_horizontal;
									break;
									
								case Board_C.bonus.destroy_vertical:
									on_board_bonus_select[i] = on_board_bonus.destroy_vertical;
									break;
									
								case Board_C.bonus.destroy_horizontal_and_vertical:
									on_board_bonus_select[i] = on_board_bonus.destroy_horizontal_and_vertical;
									break;

								case Board_C.bonus.give_more_time:
									on_board_bonus_select[i] = on_board_bonus.give_more_time;
									break;

								case Board_C.bonus.give_more_moves:
									on_board_bonus_select[i] = on_board_bonus.give_more_moves;
									break;

								case Board_C.bonus.heal_hp:
									on_board_bonus_select[i] = on_board_bonus.heal_hp;
									break;
								}
								
								//my_target.color_explosion_give_bonus[i] = (Board_C.bonus)EditorGUILayout.EnumPopup("color " + i + " give" ,my_target.color_explosion_give_bonus[i]);
								on_board_bonus_select[i] = (on_board_bonus)EditorGUILayout.EnumPopup("color " + i + " give"  ,on_board_bonus_select[i]);
								
								//copy from temp to target
								switch(on_board_bonus_select[i])
								{
								case on_board_bonus.none:
									my_target.color_explosion_give_bonus[i] = Board_C.bonus.none;
									break;
									
								case on_board_bonus.destroy_3x3:
									my_target.color_explosion_give_bonus[i] = Board_C.bonus.destroy_3x3;
									break;
									
								case on_board_bonus.destroy_horizontal:
									my_target.color_explosion_give_bonus[i] = Board_C.bonus.destroy_horizontal;
									break;
									
								case on_board_bonus.destroy_vertical:
									my_target.color_explosion_give_bonus[i] = Board_C.bonus.destroy_vertical;
									break;
									
								case on_board_bonus.destroy_horizontal_and_vertical:
									my_target.color_explosion_give_bonus[i] = Board_C.bonus.destroy_horizontal_and_vertical;
									break;

								case on_board_bonus.give_more_time:
									my_target.color_explosion_give_bonus[i] = Board_C.bonus.give_more_time;
									break;

								case on_board_bonus.give_more_moves:
									my_target.color_explosion_give_bonus[i] = Board_C.bonus.give_more_moves;
									break;

								case on_board_bonus.heal_hp:
									my_target.color_explosion_give_bonus[i] = Board_C.bonus.heal_hp;
									break;
								}

							}
						}*/
					}
					else if (my_target.choose_bonus_by_select == Board_C.choose_bonus_by.explosion_magnitude)
					{
						//if (my_target.trigger_by_select == Board_C.trigger_by.inventory)
						//{
							for (int i = 0; i < 4 ; i ++)
							{
								my_target.big_explosion_give_bonus[i] = (Board_C.bonus)EditorGUILayout.EnumPopup("explode " + (i+4) + " gems give" ,my_target.big_explosion_give_bonus[i]);
								if (my_target.big_explosion_give_bonus[i] == Board_C.bonus.give_more_moves)
									{
									EditorGUI.indentLevel++;
										if (my_target.add_moves_bonus < 1)
											GUI.color = Color.red;
										else
											GUI.color = Color.white;
										my_target.add_moves_bonus = EditorGUILayout.IntField("add moves",my_target.add_moves_bonus);
											GUI.color = Color.white;
									EditorGUI.indentLevel--;
									}
								else if (my_target.big_explosion_give_bonus[i] == Board_C.bonus.give_more_time)
									{
									EditorGUI.indentLevel++;
										if (my_target.add_time_bonus <= 0)
											GUI.color = Color.red;
										else
											GUI.color = Color.white;
										my_target.add_time_bonus = EditorGUILayout.FloatField("add seconds",my_target.add_time_bonus);
										GUI.color = Color.white;
									EditorGUI.indentLevel--;
									}
								else if (my_target.big_explosion_give_bonus[i] == Board_C.bonus.heal_hp)
									{
									EditorGUI.indentLevel++;
										if (my_target.heal_hp_player_bonus < 1)
											GUI.color = Color.red;
										else
											GUI.color = Color.white;
										my_target.heal_hp_player_bonus = EditorGUILayout.IntField("add HP",my_target.heal_hp_player_bonus);
										GUI.color = Color.white;
									EditorGUI.indentLevel--;
									}
							}
						//}
						/*else
						{
							for (int i = 0; i < 4 ; i ++)
							{
								//copy from target to temp
								switch(my_target.big_explosion_give_bonus[i])
								{
								case Board_C.bonus.none:
									on_board_bonus_select[i] = on_board_bonus.none;
									break;
									
								case Board_C.bonus.destroy_3x3:
									on_board_bonus_select[i] = on_board_bonus.destroy_3x3;
									break;
									
								case Board_C.bonus.destroy_horizontal:
									on_board_bonus_select[i] = on_board_bonus.destroy_horizontal;
									break;
									
								case Board_C.bonus.destroy_vertical:
									on_board_bonus_select[i] = on_board_bonus.destroy_vertical;
									break;
									
								case Board_C.bonus.destroy_horizontal_and_vertical:
									on_board_bonus_select[i] = on_board_bonus.destroy_horizontal_and_vertical;
									break;

								case Board_C.bonus.give_more_time:
									on_board_bonus_select[i] = on_board_bonus.give_more_time;
									break;

								case Board_C.bonus.give_more_moves:
									on_board_bonus_select[i] = on_board_bonus.give_more_moves;
									break;

								case Board_C.bonus.heal_hp:
									on_board_bonus_select[i] = on_board_bonus.heal_hp;
									break;
								}
								on_board_bonus_select[i] = (on_board_bonus)EditorGUILayout.EnumPopup("explode " + (i+4) + " gems give" ,on_board_bonus_select[i]);

								//copy from temp to target
								switch(on_board_bonus_select[i])
								{
								case on_board_bonus.none:
									my_target.big_explosion_give_bonus[i] = Board_C.bonus.none;
									break;
									
								case on_board_bonus.destroy_3x3:
									my_target.big_explosion_give_bonus[i] = Board_C.bonus.destroy_3x3;
									break;
									
								case on_board_bonus.destroy_horizontal:
									my_target.big_explosion_give_bonus[i] = Board_C.bonus.destroy_horizontal;
									break;
									
								case on_board_bonus.destroy_vertical:
									my_target.big_explosion_give_bonus[i] = Board_C.bonus.destroy_vertical;
									break;
									
								case on_board_bonus.destroy_horizontal_and_vertical:
									my_target.big_explosion_give_bonus[i] = Board_C.bonus.destroy_horizontal_and_vertical;
									break;

								case on_board_bonus.give_more_time:
									my_target.big_explosion_give_bonus[i] = Board_C.bonus.give_more_time;
									break;

								case on_board_bonus.give_more_moves:
									my_target.big_explosion_give_bonus[i] = Board_C.bonus.give_more_moves;
									break;

								case on_board_bonus.heal_hp:
									my_target.big_explosion_give_bonus[i] = Board_C.bonus.heal_hp;
									break;
								}
							}
						}*/
					}
					
					EditorGUI.indentLevel--;
				}
				else if (my_target.give_bonus_select == Board_C.give_bonus.from_stage_file_or_from_gem_emitter)
				{
					if (my_target.trigger_by_select == Board_C.trigger_by.OFF)
						GUI.color = Color.red;
					else
						GUI.color = Color.white;
					my_target.trigger_by_select = (Board_C.trigger_by)EditorGUILayout.EnumPopup("trigger by" ,my_target.trigger_by_select);
					GUI.color = Color.white;
					
				}
				
				
				if (my_target.trigger_by_select == Board_C.trigger_by.inventory)
				{
					EditorGUI.indentLevel++;
					my_target.show_player_inventory_bonus = EditorGUILayout.Foldout(my_target.show_player_inventory_bonus, "Player start the stage with these amount of bonuses:");
					if (my_target.show_player_inventory_bonus)
					{
						EditorGUI.indentLevel++;
						for (int i = 1; i < Enum.GetNames(typeof(Board_C.bonus)).Length ; i ++)
						{
							if (my_target.player_bonus_inventory[i] < 0)
								GUI.color = Color.red;
							else
								GUI.color = Color.white;

							if (i <8)
								my_target.player_bonus_inventory[i] = EditorGUILayout.IntField(Enum.GetName(typeof(Board_C.bonus),i), my_target.player_bonus_inventory[i]);
							else
								{
								if ((i == 8) && (my_target.lose_requirement_selected == Board_C.lose_requirement.timer))
									{
									EditorGUILayout.BeginHorizontal();
									my_target.player_bonus_inventory[i] = EditorGUILayout.IntField(Enum.GetName(typeof(Board_C.bonus),i), my_target.player_bonus_inventory[i]);
									my_target.add_time_bonus = EditorGUILayout.FloatField("add seconds",my_target.add_time_bonus);

									my_target.player_bonus_inventory[9] = 0;
									my_target.player_bonus_inventory[10] = 0;
									EditorGUILayout.EndHorizontal();
									}
								else if ((i == 9) && (my_target.lose_requirement_selected == Board_C.lose_requirement.player_have_zero_moves))
									{
									EditorGUILayout.BeginHorizontal();
									my_target.player_bonus_inventory[i] = EditorGUILayout.IntField(Enum.GetName(typeof(Board_C.bonus),i), my_target.player_bonus_inventory[i]);
									my_target.add_moves_bonus = EditorGUILayout.IntField("add moves",my_target.add_moves_bonus);

									my_target.player_bonus_inventory[8] = 0;
									my_target.player_bonus_inventory[10] = 0;
									EditorGUILayout.EndHorizontal();
									}
								else if ((i == 10) && (my_target.lose_requirement_selected == Board_C.lose_requirement.player_hp_is_zero))
									{
									EditorGUILayout.BeginHorizontal();
									my_target.player_bonus_inventory[i] = EditorGUILayout.IntField(Enum.GetName(typeof(Board_C.bonus),i), my_target.player_bonus_inventory[i]);
									my_target.heal_hp_player_bonus = EditorGUILayout.IntField("add HP",my_target.heal_hp_player_bonus);

									my_target.player_bonus_inventory[8] = 0;
									my_target.player_bonus_inventory[9] = 0;
									EditorGUILayout.EndHorizontal();
									}
								}
							GUI.color = Color.white;
							
						}
						EditorGUI.indentLevel--;
					}

					if ( //(my_target.lose_requirement_selected == Board_C.lose_requirement.player_hp_is_zero) ||
					      (my_target.win_requirement_selected == Board_C.win_requirement.enemy_hp_is_zero)
					    || (my_target.lose_requirement_selected == Board_C.lose_requirement.enemy_collect_gems)
					    || (my_target.lose_requirement_selected == Board_C.lose_requirement.enemy_reach_target_score))
					{
					my_target.show_enemy_inventory_bonus = EditorGUILayout.Foldout(my_target.show_enemy_inventory_bonus, "Enemy start the stage with these amount of bonuses:");
					if (my_target.show_enemy_inventory_bonus)
						{
						EditorGUI.indentLevel++;
						for (int i = 1; i < Enum.GetNames(typeof(Board_C.bonus)).Length ; i ++)
							{
							if (my_target.enemy_bonus_inventory[i] < 0)
								GUI.color = Color.red;
							else
								GUI.color = Color.white;

							if (i <8)
								my_target.enemy_bonus_inventory[i] = EditorGUILayout.IntField(Enum.GetName(typeof(Board_C.bonus),i), my_target.enemy_bonus_inventory[i]);
							else if (i == 10) 
								{
								if (my_target.win_requirement_selected == Board_C.win_requirement.enemy_hp_is_zero)
									{
									EditorGUILayout.BeginHorizontal();
									my_target.enemy_bonus_inventory[i] = EditorGUILayout.IntField(Enum.GetName(typeof(Board_C.bonus),i), my_target.enemy_bonus_inventory[i]);
									my_target.heal_hp_enemy_bonus = EditorGUILayout.IntField("add HP",my_target.heal_hp_enemy_bonus);
									EditorGUILayout.EndHorizontal();
									}
								else
									my_target.enemy_bonus_inventory[10] = 0;
								}
							
							GUI.color = Color.white;
							
							}
						EditorGUI.indentLevel--;
						}
					}
					EditorGUI.indentLevel--;
				}
				if (my_target.give_bonus_select != Board_C.give_bonus.no)
					{
					EditorGUILayout.Space();
					EditorGUILayout.LabelField("linear explosion stop against:");
					EditorGUI.indentLevel++;
						my_target.linear_explosion_stop_against_empty_space = EditorGUILayout.Toggle("empty space",my_target.linear_explosion_stop_against_empty_space);
						my_target.linear_explosion_stop_against_block = EditorGUILayout.Toggle("block",my_target.linear_explosion_stop_against_block);
						my_target.linear_explosion_stop_against_bonus = EditorGUILayout.Toggle("bonus",my_target.linear_explosion_stop_against_bonus);
						my_target.linear_explosion_stop_against_token = EditorGUILayout.Toggle("token",my_target.linear_explosion_stop_against_token);
					EditorGUI.indentLevel--;
					}
				EditorGUI.indentLevel--;
			}
			
			my_target.show_score = EditorGUILayout.Foldout(my_target.show_score, "Score rewards");
			if (my_target.show_score)
			{
				EditorGUI.indentLevel++;
				
				my_target.show_score_of_this_move = EditorGUILayout.Toggle("show move score",my_target.show_score_of_this_move);
				
				if (my_target.score_reward_for_damaging_tiles < 0)
					GUI.color = Color.red;
				else
					GUI.color = Color.white;
				my_target.score_reward_for_damaging_tiles = EditorGUILayout.IntField("damaging a tile", my_target.score_reward_for_damaging_tiles);
				GUI.color = Color.white;
				
				EditorGUILayout.Space();
				for (int i = 0; i < my_target.gem_length ; i ++)
				{
					if (my_target.score_reward_for_explode_gems[i] < 0)
						GUI.color = Color.red;
					else
						GUI.color = Color.white;
					my_target.score_reward_for_explode_gems[i] = EditorGUILayout.IntField("explode " + (3+i) + " gems with a move", my_target.score_reward_for_explode_gems[i]);
				}
				GUI.color = Color.white;
				
				EditorGUILayout.Space();
				
				if (my_target.score_reward_for_each_explode_gems_in_secondary_explosion < 0)
					GUI.color = Color.red;
				else
					GUI.color = Color.white;
				my_target.score_reward_for_each_explode_gems_in_secondary_explosion = EditorGUILayout.IntField("for each exploded gems in secondary explosion", my_target.score_reward_for_each_explode_gems_in_secondary_explosion);
				GUI.color = Color.white;
				
				EditorGUILayout.Space();
				
				EditorGUILayout.LabelField("multipliers");
				EditorGUI.indentLevel++;
				if (my_target.score_reward_for_explode_gems_of_the_same_color_in_two_or_more_turns_subsequently < 1)
					GUI.color = Color.red;
				else
					GUI.color = Color.white;
				my_target.score_reward_for_explode_gems_of_the_same_color_in_two_or_more_turns_subsequently = EditorGUILayout.FloatField("explode same color in two or more turns subsequently", my_target.score_reward_for_explode_gems_of_the_same_color_in_two_or_more_turns_subsequently);
				GUI.color = Color.white;
				
				if (my_target.score_reward_for_secondary_combo_explosions < 1)
					GUI.color = Color.red;
				else
					GUI.color = Color.white;
				my_target.score_reward_for_secondary_combo_explosions = EditorGUILayout.FloatField("combo multiplier", my_target.score_reward_for_secondary_combo_explosions);
				GUI.color = Color.white;
				EditorGUI.indentLevel--;
				
				EditorGUILayout.Space();
				
				if (my_target.every_second_saved_give < 0)
					GUI.color = Color.red;
				else
					GUI.color = Color.white;
				my_target.every_second_saved_give = EditorGUILayout.IntField("for every second saved", my_target.every_second_saved_give);
				GUI.color = Color.white;
				
				if (my_target.every_move_saved_give < 0)
					GUI.color = Color.red;
				else
					GUI.color = Color.white;
				my_target.every_move_saved_give = EditorGUILayout.IntField("for every move saved", my_target.every_move_saved_give);
				GUI.color = Color.white;
				
				if (my_target.every_hp_saved_give < 0)
					GUI.color = Color.red;
				else
					GUI.color = Color.white;
				my_target.every_hp_saved_give = EditorGUILayout.IntField("for every hp saved", my_target.every_hp_saved_give);
				GUI.color = Color.white;
				
				EditorGUI.indentLevel--;
				EditorGUILayout.Space();
			}
			
			if ( //(my_target.lose_requirement_selected == Board_C.lose_requirement.player_hp_is_zero)  ||
			     (my_target.win_requirement_selected == Board_C.win_requirement.enemy_hp_is_zero)
			    || (my_target.lose_requirement_selected == Board_C.lose_requirement.enemy_collect_gems)
			    || (my_target.lose_requirement_selected == Board_C.lose_requirement.enemy_reach_target_score)) 
			{
				my_target.show_versus = EditorGUILayout.Foldout(my_target.show_versus, "Verus Rules:");
				if(my_target.show_versus)
				{
					EditorGUI.indentLevel++;
					
					my_target.lose_turn_if_bad_move = EditorGUILayout.Toggle("lost turn if bad move",my_target.lose_turn_if_bad_move);
					EditorGUILayout.LabelField("gain a turn if explode:");
					EditorGUI.indentLevel++;

					my_target.gain_turn_if_secondary_explosion  = EditorGUILayout.Toggle("secondary explosion",my_target.gain_turn_if_secondary_explosion);
					if (my_target.gain_turn_if_secondary_explosion)
					{
						EditorGUI.indentLevel++;
						if (my_target.seconday_explosion_maginiture_needed_to_gain_a_turn < 3)
							GUI.color = Color.red;
						else
							GUI.color = Color.white;
						my_target.seconday_explosion_maginiture_needed_to_gain_a_turn = EditorGUILayout.IntField("minimum magnitude requested", my_target.seconday_explosion_maginiture_needed_to_gain_a_turn);
						GUI.color = Color.white;

						if (my_target.combo_lenght_needed_to_gain_a_turn < 0)
							GUI.color = Color.red;
						else
							GUI.color = Color.white;
						my_target.combo_lenght_needed_to_gain_a_turn = EditorGUILayout.IntField("minimum combo lenght requested", my_target.combo_lenght_needed_to_gain_a_turn);
						GUI.color = Color.white;

						EditorGUI.indentLevel--;
					}

					my_target.gain_turn_if_explode_same_color_of_previous_move = EditorGUILayout.Toggle("same color of your previous move",my_target.gain_turn_if_explode_same_color_of_previous_move);
					my_target.gain_turn_if_explode_more_than_3_gems  = EditorGUILayout.Toggle("more than 3 gems",my_target.gain_turn_if_explode_more_than_3_gems);

					if ((my_target.gain_turn_if_explode_same_color_of_previous_move) || (my_target.gain_turn_if_explode_more_than_3_gems))
					{
						EditorGUILayout.BeginHorizontal();
						my_target.chain_turns_limit = EditorGUILayout.Toggle("chain limit",my_target.chain_turns_limit);
						if (my_target.chain_turns_limit)
						{
							if (my_target.max_chain_turns <= 0)
								GUI.color = Color.red;
							else
								GUI.color = Color.white;
							my_target.max_chain_turns = EditorGUILayout.IntField("max",my_target.max_chain_turns);
							GUI.color = Color.white;
						}
						EditorGUILayout.EndHorizontal();
					}
					EditorGUI.indentLevel--;				
					if ( (my_target.lose_requirement_selected == Board_C.lose_requirement.player_hp_is_zero) ||
					    (my_target.win_requirement_selected == Board_C.win_requirement.enemy_hp_is_zero) )
					{
						if (my_target.gem_damage_value <= 1)
							GUI.color = Color.red;
						else
							GUI.color = Color.white;
						my_target.gem_damage_value = EditorGUILayout.IntField("base hit damage", my_target.gem_damage_value);
						GUI.color = Color.white;
					}
					
					EditorGUI.indentLevel++;
					if (my_target.lose_requirement_selected == Board_C.lose_requirement.player_hp_is_zero)
					{
						//PLAYER
						my_target.show_player = EditorGUILayout.Foldout(my_target.show_player, "Player");
						if (my_target.show_player)
						{
							EditorGUI.indentLevel++;
							if (my_target.lose_requirement_selected == Board_C.lose_requirement.player_hp_is_zero)
							{
								if (my_target.max_player_hp <= 0)
									GUI.color = Color.red;
								else
									GUI.color = Color.white;
								my_target.max_player_hp = EditorGUILayout.IntField("Player HP",my_target.max_player_hp);
								
								
								//armor mode:
								if (my_target.use_armor)
								{
									EditorGUI.indentLevel++;
									for (int i = 0; i < my_target.gem_length ; i ++)
									{
										my_target.player_armor[i] = (Board_C.armor)EditorGUILayout.EnumPopup("armor vs gem " + i,my_target.player_armor[i]);
									}
									EditorGUI.indentLevel--;
									
								}
								
							}
							
							EditorGUI.indentLevel--;
						}
					}
					//ENEMY
					my_target.show_enemy = EditorGUILayout.Foldout(my_target.show_enemy, "Enemy");
					if (my_target.show_enemy)
					{
						EditorGUI.indentLevel++;
						if (my_target.win_requirement_selected == Board_C.win_requirement.enemy_hp_is_zero)
						{
							if (my_target.max_enemy_hp <= 0)
								GUI.color = Color.red;
							else
								GUI.color = Color.white;
							my_target.max_enemy_hp = EditorGUILayout.IntField("Enemy HP",my_target.max_enemy_hp);
							GUI.color = Color.white;
							
							
							//armor mode:
							if (my_target.use_armor)
							{
								EditorGUI.indentLevel++;
								for (int i = 0; i < my_target.gem_length ; i ++)
								{
									my_target.enemy_armor[i] = (Board_C.armor)EditorGUILayout.EnumPopup("armor vs gem " + i,my_target.enemy_armor[i]);
								}
								EditorGUI.indentLevel--;
								
							}
							
						}
						
						
						
						//enemy AI
						//read from target
						switch(my_target.enemy_AI_select)
						{
						case Board_C.enemy_AI.random:
							enemy_AI_battle_select = enemy_AI_battle.random;
							enemy_AI_gem_collect_select = enemy_AI_gem_collect.random;
							enemy_AI_target_score_select = enemy_AI_target_score.random;
							break;
							
						case Board_C.enemy_AI.collect_gems_from_less_to_more:
							enemy_AI_gem_collect_select = enemy_AI_gem_collect.collect_gems_from_less_to_more;
							break;
							
						case Board_C.enemy_AI.collect_gems_from_more_to_less:
							enemy_AI_gem_collect_select = enemy_AI_gem_collect.collect_gems_from_more_to_less;
							break;
							
						case Board_C.enemy_AI.dynamic_battle:
							enemy_AI_battle_select = enemy_AI_battle.dynamic_battle;
							break;
							
						case Board_C.enemy_AI.by_hand_setup:
							enemy_AI_gem_collect_select = enemy_AI_gem_collect.by_hand_setup;
							enemy_AI_battle_select = enemy_AI_battle.by_hand_setup;
							enemy_AI_target_score_select = enemy_AI_target_score.by_hand_setup;
							break;
						}
						
						//write to target
						if (my_target.lose_requirement_selected == Board_C.lose_requirement.player_hp_is_zero)
						{
							my_target.use_armor = true;
							
							enemy_AI_battle_select = (enemy_AI_battle)EditorGUILayout.EnumPopup("Enemy AI",enemy_AI_battle_select);
							switch(enemy_AI_battle_select)
							{
							case enemy_AI_battle.random:
								my_target.enemy_AI_select = Board_C.enemy_AI.random;
								break;
								
							case enemy_AI_battle.dynamic_battle:
								my_target.enemy_AI_select = Board_C.enemy_AI.dynamic_battle;
								
								break;
								
							case enemy_AI_battle.by_hand_setup:
								my_target.enemy_AI_select = Board_C.enemy_AI.by_hand_setup;
								break;
							}
							
						}
						else if (my_target.lose_requirement_selected == Board_C.lose_requirement.enemy_collect_gems)
						{
							my_target.use_armor = false;
							
							enemy_AI_gem_collect_select = (enemy_AI_gem_collect)EditorGUILayout.EnumPopup("Enemy AI",enemy_AI_gem_collect_select);
							switch(enemy_AI_gem_collect_select)
							{
							case enemy_AI_gem_collect.random:
								my_target.enemy_AI_select = Board_C.enemy_AI.random;
								break;
								
							case enemy_AI_gem_collect.collect_gems_from_less_to_more:
								my_target.enemy_AI_select = Board_C.enemy_AI.collect_gems_from_less_to_more;
								
								break;
								
							case enemy_AI_gem_collect.collect_gems_from_more_to_less:
								my_target.enemy_AI_select = Board_C.enemy_AI.collect_gems_from_more_to_less;
								
								
								break;
								
							case enemy_AI_gem_collect.by_hand_setup:
								my_target.enemy_AI_select = Board_C.enemy_AI.by_hand_setup;
								break;
							}
						}
						else if (my_target.lose_requirement_selected == Board_C.lose_requirement.enemy_reach_target_score)
						{
							
							my_target.use_armor = false;
							
							enemy_AI_target_score_select = (enemy_AI_target_score)EditorGUILayout.EnumPopup("Enemy AI",enemy_AI_target_score_select);
							switch(enemy_AI_target_score_select)
							{
							case enemy_AI_target_score.random:
								my_target.enemy_AI_select = Board_C.enemy_AI.random;
								break;
							case enemy_AI_target_score.by_hand_setup:
								my_target.enemy_AI_select = Board_C.enemy_AI.by_hand_setup;
								break;
							}
							
						}
						else
							my_target.use_armor = false;
						
						EditorGUI.indentLevel++;
						my_target.enemy_move_delay = EditorGUILayout.FloatField("move delay in seconds",my_target.enemy_move_delay);
						
						if (my_target.enemy_AI_select != Board_C.enemy_AI.random)
							my_target.chance_of_use_best_move = EditorGUILayout.IntSlider("% use best move",my_target.chance_of_use_best_move, 0 , 100);
						
						if (my_target.give_bonus_select != Board_C.give_bonus.no)
						{
							my_target.chance_of_use_bonus = EditorGUILayout.IntSlider("% use bonus",my_target.chance_of_use_bonus, 0 , 100);
							EditorGUI.indentLevel++;
							if(my_target.chance_of_use_bonus >0)
								my_target.chance_of_use_best_bonus = EditorGUILayout.IntSlider("% best bonus",my_target.chance_of_use_best_bonus, 0 , 100);
							EditorGUI.indentLevel--;
						}
						
						EditorGUI.indentLevel--;
						
						if (my_target.enemy_AI_select == Board_C.enemy_AI.by_hand_setup)
						{
							EditorGUI.indentLevel++;
							
							for (int i = 0; i < my_target.gem_length ; i ++)
							{
								
								if ((int)my_target.temp_enemy_AI_preference_order[i] > my_target.gem_length-1)
								{
									GUI.color = Color.red;
									EditorGUILayout.LabelField("ERROR! This gem color exceed the current number of 'gem colors' set in 'Advanced' settings");
								}
								else
									GUI.color = Color.white;
								
								my_target.temp_enemy_AI_preference_order[i] = (Board_C.enemy_AI_manual_setup)EditorGUILayout.EnumPopup("Priority order n. " + i,my_target.temp_enemy_AI_preference_order[i]);
							}
							GUI.color = Color.white;
							EditorGUI.indentLevel--;
						}
						EditorGUI.indentLevel--;
						EditorGUILayout.Space();
					}
					
					EditorGUI.indentLevel--;
				}
			}
			
			
			
			EditorGUI.indentLevel = 0;
			
		}
		
		if (EditorGUI.EndChangeCheck ())
		{
			EditorUtility.SetDirty(my_target);
		}
	}
	
	void Camera_setup()
	{
		Board_C my_target = (Board_C)target;
		
		EditorGUI.BeginChangeCheck ();
		my_target.show_camera_setup = EditorGUILayout.Foldout(my_target.show_camera_setup, "Camera");
        Undo.RecordObject(my_target, "edit_camera");
        if (my_target.show_camera_setup)
		{
			EditorGUI.indentLevel = 1;
			
			
			my_target.Board_camera = EditorGUILayout.ObjectField("Board camera: ", my_target.Board_camera, typeof(Camera), true) as Camera;
			
			EditorGUILayout.BeginHorizontal();
			my_target.camera_zoom = EditorGUILayout.FloatField("zoom", my_target.camera_zoom);
			if (my_target.camera_position_choice != Board_C.camera_position.centred_to_move)	
				my_target.adaptive_zoom = EditorGUILayout.Toggle("adaptive zoom", my_target.adaptive_zoom);
			else
				my_target.adaptive_zoom = false;
			EditorGUILayout.EndHorizontal();
			
			my_target.Camera_adjust = EditorGUILayout.Vector3Field("adjust position", my_target.Camera_adjust); 
			
			
			my_target.camera_position_choice = (Board_C.camera_position)EditorGUILayout.EnumPopup("Behavior",my_target.camera_position_choice);
			if (my_target.camera_position_choice == Board_C.camera_position.centred_to_move)
			{
				EditorGUI.indentLevel++;
				my_target.camera_speed = EditorGUILayout.FloatField("speed", my_target.camera_speed);
				my_target.camera_move_tolerance = EditorGUILayout.FloatField("tollerance", my_target.camera_move_tolerance);
				my_target.margin = (Vector2)EditorGUILayout.Vector2Field("margin", my_target.margin); 
				EditorGUI.indentLevel--;
			}
			
			
			EditorGUI.indentLevel = 0;
			EditorGUILayout.Space();
		}
		if (EditorGUI.EndChangeCheck ())
			EditorUtility.SetDirty(my_target);
	}
	
	void Assign_sprites()
	{
		Board_C my_target = (Board_C)target;
		EditorGUI.BeginChangeCheck ();
        Undo.RecordObject(my_target, "edit_assing_sprites");

        my_target.show_sprites = EditorGUILayout.Foldout(my_target.show_sprites, "Sprites");
		if (my_target.show_sprites)
		{
			EditorGUI.indentLevel++;
			
			my_target.show_s_bonus_gui = EditorGUILayout.Foldout(my_target.show_s_bonus_gui, "Bonus_gui");
			if (my_target.show_s_bonus_gui)
			{
				EditorGUI.indentLevel++;

				if (GUILayout.Button("refresh array"))
					my_target.gui_bonus = new Sprite[Enum.GetValues(typeof(Board_C.bonus)).Length]; 

				foreach (Board_C.bonus i in Enum.GetValues(typeof(Board_C.bonus)))
				{
					if (((int)i > 0) && ((int)i < my_target.gui_bonus.Length))
					{
						if (!my_target.gui_bonus[(int)i-1])
							GUI.color = Color.red;
						else
							GUI.color = Color.white;
						my_target.gui_bonus[(int)i-1] = EditorGUILayout.ObjectField(i.ToString(),my_target.gui_bonus[(int)i-1], typeof(Sprite), false) as Sprite;
						
					}
				}
				GUI.color = Color.white;
				EditorGUI.indentLevel--;
			}
			
			my_target.show_s_on_board_bonus = EditorGUILayout.Foldout(my_target.show_s_on_board_bonus, "On board bonus");
			if (my_target.show_s_on_board_bonus)
			{
				EditorGUI.indentLevel++;
				for (int i = 1; i < Enum.GetNames(typeof(Board_C.bonus)).Length; i++)
				{

					if (!my_target.on_board_bonus_sprites[i])
						GUI.color = Color.red;
					else
						GUI.color = Color.white;

					my_target.on_board_bonus_sprites[i] = EditorGUILayout.ObjectField(Enum.GetName(typeof(Board_C.bonus),i).ToString(),my_target.on_board_bonus_sprites[i], typeof(Sprite), false) as Sprite;

				}
				GUI.color = Color.white;
				EditorGUI.indentLevel--;
			}
			
			my_target.show_s_tiles = EditorGUILayout.Foldout(my_target.show_s_tiles, "Tiles");
			if (my_target.show_s_tiles)
			{
				EditorGUI.indentLevel++;
				for (int i = 0; i < 4 ; i ++)
				{
					if (!my_target.tile_hp[i])
						GUI.color = Color.red;
					else
						GUI.color = Color.white;
					my_target.tile_hp[i] = EditorGUILayout.ObjectField("tile hp " + i,my_target.tile_hp[i], typeof(Sprite), false) as Sprite;
				}
				GUI.color = Color.white;
				EditorGUI.indentLevel--;
			}
			
			my_target.show_s_gems = EditorGUILayout.Foldout(my_target.show_s_gems, "Gems");
			if (my_target.show_s_gems)
			{
				EditorGUI.indentLevel++;
				for (int i = 0; i < my_target.gem_length ; i ++)
				{
					if (!my_target.gem_colors[i])
						GUI.color = Color.red;
					else
						GUI.color = Color.white;
					my_target.gem_colors[i] = EditorGUILayout.ObjectField("gem n. " + i,my_target.gem_colors[i], typeof(Sprite), false) as Sprite;
				}
				GUI.color = Color.white;
				EditorGUI.indentLevel--;
			}
			
			my_target.show_s_padlocks = EditorGUILayout.Foldout(my_target.show_s_padlocks, "Padlock");
			if (my_target.show_s_padlocks)
			{
				EditorGUI.indentLevel++;
				for (int i = 0; i < 3 ; i ++)
				{
					if (!my_target.lock_gem_hp[i])
						GUI.color = Color.red;
					else
						GUI.color = Color.white;
					my_target.lock_gem_hp[i] = EditorGUILayout.ObjectField("padlock hp " + (i+1),my_target.lock_gem_hp[i], typeof(Sprite), false) as Sprite;
				}
				GUI.color = Color.white;
				EditorGUI.indentLevel--;
			}
			
			my_target.show_s_blocks = EditorGUILayout.Foldout(my_target.show_s_blocks, "Blocks");
			if (my_target.show_s_blocks)
			{
				EditorGUI.indentLevel++;
				for (int i = 0; i < 3 ; i ++)
				{
					if (!my_target.block_hp[i])
						GUI.color = Color.red;
					else
						GUI.color = Color.white;
					my_target.block_hp[i] = EditorGUILayout.ObjectField("block hp " + (i+1),my_target.block_hp[i], typeof(Sprite), false) as Sprite;
				}
				GUI.color = Color.white;
				EditorGUI.indentLevel--;
			}
			/*
				my_target.show_s_falling_blocks = EditorGUILayout.Foldout(my_target.show_s_falling_blocks, "Falling Blocks");
					if (my_target.show_s_falling_blocks)
						{
						EditorGUI.indentLevel++;
							for (int i = 0; i < 3 ; i ++)
							{
							if (!my_target.falling_block_hp[i])
									GUI.color = Color.red;
								else
									GUI.color = Color.white;
							my_target.falling_block_hp[i] = EditorGUILayout.ObjectField("falling block hp " + (i+1),my_target.falling_block_hp[i], typeof(Sprite), false) as Sprite;
							}
							EditorGUI.indentLevel--;
						}
*/
			my_target.show_s_misc = EditorGUILayout.Foldout(my_target.show_s_misc, "Misc");
			if (my_target.show_s_misc)
			{
				EditorGUI.indentLevel++;
				
				if (!my_target.junk)
					GUI.color = Color.red;
				else
					GUI.color = Color.white;
				my_target.junk = EditorGUILayout.ObjectField("junk",my_target.junk, typeof(Sprite), false) as Sprite;
				GUI.color = Color.white;
				
				if (!my_target.token)
					GUI.color = Color.red;
				else
					GUI.color = Color.white;
				my_target.token = EditorGUILayout.ObjectField("token",my_target.token, typeof(Sprite), false) as Sprite;
				GUI.color = Color.white;
				EditorGUI.indentLevel--;
			}
			
			EditorGUI.indentLevel--;
			EditorGUILayout.Space();
		}
		
		if (EditorGUI.EndChangeCheck ())
			EditorUtility.SetDirty(my_target);
	}
	
	void Audio_sfx()
	{
		Board_C my_target = (Board_C)target;
		EditorGUI.BeginChangeCheck ();
        Undo.RecordObject(my_target, "edit_audio_sfx");

        my_target.show_audio = EditorGUILayout.Foldout(my_target.show_audio, "Audio sfx");
		if (my_target.show_audio)
		{
			EditorGUI.indentLevel++;
			my_target.explosion_sfx = EditorGUILayout.ObjectField("explosion", my_target.explosion_sfx, typeof(AudioClip), false) as AudioClip;
			my_target.end_fall_sfx = EditorGUILayout.ObjectField("end fall", my_target.end_fall_sfx, typeof(AudioClip), false) as AudioClip;
			my_target.bad_move_sfx = EditorGUILayout.ObjectField("bad move", my_target.bad_move_sfx, typeof(AudioClip), false) as AudioClip;
			my_target.win_sfx = EditorGUILayout.ObjectField("win", my_target.win_sfx, typeof(AudioClip), false) as AudioClip;
			my_target.lose_sfx = EditorGUILayout.ObjectField("lose", my_target.lose_sfx, typeof(AudioClip), false) as AudioClip;

			EditorGUILayout.LabelField("Bonus:");
				EditorGUI.indentLevel++;
				if (my_target.bonus_sfx.Length != Enum.GetNames(typeof(Board_C.bonus)).Length)
					my_target.bonus_sfx = new AudioClip[Enum.GetNames(typeof(Board_C.bonus)).Length];

			    for (int i = 1; i < Enum.GetNames(typeof(Board_C.bonus)).Length; i++)
			   	 	{
					my_target.bonus_sfx[i] = EditorGUILayout.ObjectField(Enum.GetName(typeof(Board_C.bonus),i).ToString() + " sfx", my_target.bonus_sfx[i], typeof(AudioClip), false) as AudioClip;
					}
				EditorGUI.indentLevel--;
			EditorGUI.indentLevel--;
		}
		
		if (EditorGUI.EndChangeCheck ())
			EditorUtility.SetDirty(my_target);
	}
	
	void Visual_enhancements()
	{
		Board_C my_target = (Board_C)target;
		EditorGUI.BeginChangeCheck ();
        Undo.RecordObject(my_target, "edit_visual_enhancements");

        my_target.show_visual_fx = EditorGUILayout.Foldout(my_target.show_visual_fx, "Visual enhancements");
		if (my_target.show_visual_fx)
		{
			EditorGUI.indentLevel++;
			
			EditorGUILayout.BeginHorizontal();
			my_target.show_frame_board_decoration = EditorGUILayout.Toggle("Frame", my_target.show_frame_board_decoration);
			if (!my_target.explosions_damages_tiles)
				my_target.show_chess_board_decoration = EditorGUILayout.Toggle("Chessboard", my_target.show_chess_board_decoration);
			EditorGUILayout.EndHorizontal();
			if (my_target.show_frame_board_decoration)
			{
				EditorGUI.indentLevel++;
				my_target.show_frame_elements = EditorGUILayout.Foldout(my_target.show_frame_elements, "elements: ");
				if (my_target.show_frame_elements)
				{
					my_target.frame_pivot =  EditorGUILayout.ObjectField("pivot", my_target.frame_pivot , typeof(Transform), true) as Transform;
					
					EditorGUILayout.Space();
					
					my_target.frame_elements[0] =  EditorGUILayout.ObjectField("up", my_target.frame_elements[0] , typeof(Transform), true) as Transform;
					my_target.frame_elements[1] =  EditorGUILayout.ObjectField("down", my_target.frame_elements[1] , typeof(Transform), true) as Transform;
					
					my_target.frame_elements[2] =  EditorGUILayout.ObjectField("right", my_target.frame_elements[2] , typeof(Transform), true) as Transform;
					my_target.frame_elements[3] =  EditorGUILayout.ObjectField("left", my_target.frame_elements[3] , typeof(Transform), true) as Transform;
					
					my_target.frame_elements[4] =  EditorGUILayout.ObjectField("corner_in_down_R", my_target.frame_elements[4] , typeof(Transform), true) as Transform;
					my_target.frame_elements[5] =  EditorGUILayout.ObjectField("corner_in_down_L", my_target.frame_elements[5] , typeof(Transform), true) as Transform;
					my_target.frame_elements[6] =  EditorGUILayout.ObjectField("corner_in_up_R", my_target.frame_elements[6] , typeof(Transform), true) as Transform;
					my_target.frame_elements[7] =  EditorGUILayout.ObjectField("corner_in_up_L", my_target.frame_elements[7] , typeof(Transform), true) as Transform;
					
					my_target.frame_elements[8] =  EditorGUILayout.ObjectField("corner_out_down_R", my_target.frame_elements[8] , typeof(Transform), true) as Transform;
					my_target.frame_elements[9] =  EditorGUILayout.ObjectField("corner_out_down_L", my_target.frame_elements[9] , typeof(Transform), true) as Transform;
					my_target.frame_elements[10] =  EditorGUILayout.ObjectField("corner_out_up_R", my_target.frame_elements[10] , typeof(Transform), true) as Transform;
					my_target.frame_elements[11] =  EditorGUILayout.ObjectField("corner_out_up_L", my_target.frame_elements[11] , typeof(Transform), true) as Transform;
					
					my_target.frame_elements[12] =  EditorGUILayout.ObjectField("U_R", my_target.frame_elements[12] , typeof(Transform), true) as Transform;
					my_target.frame_elements[13] =  EditorGUILayout.ObjectField("U_L", my_target.frame_elements[13] , typeof(Transform), true) as Transform;
					my_target.frame_elements[14] =  EditorGUILayout.ObjectField("U_up", my_target.frame_elements[14] , typeof(Transform), true) as Transform;
					my_target.frame_elements[15] =  EditorGUILayout.ObjectField("U_down", my_target.frame_elements[15] , typeof(Transform), true) as Transform;
					
					my_target.frame_elements[16] =  EditorGUILayout.ObjectField("O", my_target.frame_elements[16] , typeof(Transform), true) as Transform;
				}
				EditorGUI.indentLevel--;
			}
			
			my_target.gem_explosion_fx_rule_selected = (Board_C.gem_explosion_fx_rule)EditorGUILayout.EnumPopup("Gem explosion fx",my_target.gem_explosion_fx_rule_selected);
			EditorGUI.indentLevel++;
			if (my_target.gem_explosion_fx_rule_selected == Board_C.gem_explosion_fx_rule.for_each_gem)
			{
				EditorGUILayout.LabelField("gem explosion fx by color:");
				EditorGUI.indentLevel++;
				for (int i = 0; i < my_target.gem_length ; i ++)
				{
					if (my_target.gem_explosion_fx[i] == null)
						GUI.color = Color.red;
					else
						GUI.color = Color.white;
					my_target.gem_explosion_fx[i] =  EditorGUILayout.ObjectField("gem " + i, my_target.gem_explosion_fx[i] , typeof(Transform), true) as Transform;
				}
				GUI.color = Color.white;
				EditorGUI.indentLevel--;
			}
			else if (my_target.gem_explosion_fx_rule_selected == Board_C.gem_explosion_fx_rule.only_for_big_explosion)
			{
				EditorGUILayout.LabelField("gem explosion fx by explosion magnitude:");
				EditorGUI.indentLevel++;
				for (int i = 0; i < 4 ; i ++)
				{
					if (my_target.gem_big_explosion_fx[i] == null)
						GUI.color = Color.red;
					else
						GUI.color = Color.white;
					my_target.gem_big_explosion_fx[i] =  EditorGUILayout.ObjectField("explosion " + (i+4), my_target.gem_big_explosion_fx[i] , typeof(Transform), true) as Transform;
				}
				GUI.color = Color.white;
				EditorGUI.indentLevel--;
			}
			EditorGUI.indentLevel--;
			
			my_target.bonus_have_explosion_fx = EditorGUILayout.Toggle("Bonus fx", my_target.bonus_have_explosion_fx);
			if (my_target.bonus_have_explosion_fx)
			{
				EditorGUI.indentLevel++;
				
				if (my_target.destroy_one_fx == null)
					GUI.color = Color.red;
				else
					GUI.color = Color.white;
				my_target.destroy_one_fx =  EditorGUILayout.ObjectField("destroy one", my_target.destroy_one_fx , typeof(Transform), true) as Transform;
				GUI.color = Color.white;
				
				if (my_target.destroy_3x3_fx == null)
					GUI.color = Color.red;
				else
					GUI.color = Color.white;
				my_target.destroy_3x3_fx =  EditorGUILayout.ObjectField("destroy 3x3", my_target.destroy_3x3_fx , typeof(Transform), true) as Transform;
				GUI.color = Color.white;
				
				if (my_target.destroy_horizontal_fx == null)
					GUI.color = Color.red;
				else
					GUI.color = Color.white;
				my_target.destroy_horizontal_fx =  EditorGUILayout.ObjectField("destroy horizontal", my_target.destroy_horizontal_fx , typeof(Transform), true) as Transform;
				GUI.color = Color.white;
				
				if (my_target.destroy_vertical_fx == null)
					GUI.color = Color.red;
				else
					GUI.color = Color.white;
				my_target.destroy_vertical_fx =  EditorGUILayout.ObjectField("destroy vertical", my_target.destroy_vertical_fx , typeof(Transform), true) as Transform;
				GUI.color = Color.white;
				
				if (my_target.destroy_horizontal_and_vertical_fx == null)
					GUI.color = Color.red;
				else
					GUI.color = Color.white;
				my_target.destroy_horizontal_and_vertical_fx =  EditorGUILayout.ObjectField("destroy horizontal and vertical", my_target.destroy_horizontal_and_vertical_fx , typeof(Transform), true) as Transform;
				GUI.color = Color.white;
				
				EditorGUI.indentLevel--;
			}
			
			my_target.praise_the_player = EditorGUILayout.Toggle("Praise the player", my_target.praise_the_player);
			if (my_target.praise_the_player)
			{
				EditorGUI.indentLevel++;
				
				my_target.for_big_explosion = EditorGUILayout.Toggle("for big explosion", my_target.for_big_explosion);
				my_target.for_secondary_explosions = EditorGUILayout.Toggle("for secondary explosions", my_target.for_secondary_explosions);
				my_target.for_explode_same_color_again = EditorGUILayout.Toggle("for explode same color again", my_target.for_explode_same_color_again);
				my_target.for_gain_a_turn = EditorGUILayout.Toggle("for gain a turn", my_target.for_gain_a_turn);
				if (my_target.continue_to_play_after_win_until_lose_happen && my_target.Stage_uGUI_obj)
					my_target.for_gain_a_star = EditorGUILayout.Toggle("for gain a star", my_target.for_gain_a_star);
				else
					my_target.for_gain_a_star = false;

				EditorGUI.indentLevel--;
			}
			
			
			EditorGUI.indentLevel--;
		}
		
		if (EditorGUI.EndChangeCheck ())
			EditorUtility.SetDirty(my_target);
	}
	
	void Advanced()
	{
		Board_C my_target = (Board_C)target;
		EditorGUI.BeginChangeCheck ();
        Undo.RecordObject(my_target, "edit_advanced");

        my_target.show_advanced = EditorGUILayout.Foldout(my_target.show_advanced, "Advanced");
		if (my_target.show_advanced)
		{
			EditorGUI.indentLevel++;
			
			my_target.gem_length = EditorGUILayout.IntSlider("gem colors", my_target.gem_length, 4,7);
			my_target.diagonal_falling = EditorGUILayout.Toggle("diagonal falling", my_target.diagonal_falling);
			//my_target.player_can_move_when_gem_falling = EditorGUILayout.Toggle("player can move when gem falling", my_target.player_can_move_when_gem_falling);
			
			my_target.show_advanced_timing = EditorGUILayout.Foldout(my_target.show_advanced_timing, "animations timing");
			if (my_target.show_advanced_timing)
			{
				EditorGUI.indentLevel++;
				//my_target.duration_switch_gems_move_in_seconds = EditorGUILayout.FloatField("switch duration", my_target.duration_switch_gems_move_in_seconds );
				
				if ((my_target.switch_speed <= 0) || (my_target.switch_speed > 25))
					GUI.color = Color.red;
				else
					GUI.color = Color.white;
				my_target.switch_speed = EditorGUILayout.FloatField("switch speed", my_target.switch_speed);
				GUI.color = Color.white;
				
				if ((my_target.falling_speed <= 0) || (my_target.falling_speed > 50))
					GUI.color = Color.red;
				else
					GUI.color = Color.white;
				my_target.falling_speed = EditorGUILayout.FloatField("falling speed", my_target.falling_speed);
				GUI.color = Color.white;
				
				if ((my_target.accuracy <= 0) || (my_target.accuracy >= 1))
					GUI.color = Color.red;
				else
					GUI.color = Color.white;
				my_target.accuracy = EditorGUILayout.FloatField("accuracy", my_target.accuracy);
				GUI.color = Color.white;
				
				if (GUILayout.Button("Reset to default values"))
				{
					my_target.switch_speed = 5f;
					my_target.falling_speed = 11f;
					my_target.accuracy = 0.2f;
				}
				EditorGUI.indentLevel--;
			}
			
			
			my_target.show_advanced_ugui = EditorGUILayout.Foldout(my_target.show_advanced_ugui, "gui elements");
			if (my_target.show_advanced_ugui)
			{
				EditorGUI.indentLevel++;
				my_target.gui_info_screen =  EditorGUILayout.ObjectField("info screen", my_target.gui_info_screen , typeof(GameObject), true) as GameObject;
				
				my_target.gui_win_screen =  EditorGUILayout.ObjectField("win screen", my_target.gui_win_screen , typeof(GameObject), true) as GameObject;
				my_target.gui_lose_screen =  EditorGUILayout.ObjectField("lose screen", my_target.gui_lose_screen , typeof(GameObject), true) as GameObject;
				
				my_target.gui_bonus_ico =  EditorGUILayout.ObjectField("bonus ico", my_target.gui_bonus_ico , typeof(GameObject), true) as GameObject;
				
				my_target.gui_timer = EditorGUILayout.ObjectField("time bar", my_target.gui_timer, typeof(GameObject), true) as GameObject;
				my_target.gui_board_hp = EditorGUILayout.ObjectField("board hp", my_target.gui_board_hp, typeof(GameObject), true) as GameObject;
				my_target.gui_vs =  EditorGUILayout.ObjectField("vs", my_target.gui_vs, typeof(GameObject), true) as GameObject;
				my_target.the_gui_score_of_this_move = EditorGUILayout.ObjectField("move score", my_target.the_gui_score_of_this_move, typeof(Transform), true) as Transform;
				
				my_target.my_hint = EditorGUILayout.ObjectField("hint", my_target.my_hint, typeof(Transform), true) as Transform;
				my_target.praise_obj =  EditorGUILayout.ObjectField("praise", my_target.praise_obj , typeof(GameObject), true) as GameObject;
				
				
				my_target.color_collect_done = EditorGUILayout.ColorField("gem collected",my_target.color_collect_done);
				
				
				EditorGUILayout.LabelField("player");
				EditorGUI.indentLevel++;
				my_target.gui_player_name = EditorGUILayout.ObjectField("name", my_target.gui_player_name, typeof(GameObject), true) as GameObject;
				my_target.gui_player_hp = EditorGUILayout.ObjectField("hp", my_target.gui_player_hp, typeof(GameObject), true) as GameObject;
				my_target.gui_player_hp_slider = EditorGUILayout.ObjectField("slider hp", my_target.gui_player_hp_slider, typeof(Slider), true) as Slider;
				my_target.gui_player_hp_text = EditorGUILayout.ObjectField("text hp", my_target.gui_player_hp_text, typeof(Text), true) as Text;
				my_target.gui_player_armor = EditorGUILayout.ObjectField("armor", my_target.gui_player_armor, typeof(GameObject), true) as GameObject;
				my_target.gui_player_count = EditorGUILayout.ObjectField("gem count", my_target.gui_player_count, typeof(GameObject), true) as GameObject;
				my_target.gui_player_score = EditorGUILayout.ObjectField("score", my_target.gui_player_score, typeof(Text), true) as Text;
				my_target.gui_player_target_score = EditorGUILayout.ObjectField("target score", my_target.gui_player_target_score, typeof(Text), true) as Text;
				my_target.gui_player_target_score_slider = EditorGUILayout.ObjectField("slider score", my_target.gui_player_target_score_slider, typeof(Slider), true) as Slider;
				my_target.gui_player_left_moves = EditorGUILayout.ObjectField("left moves", my_target.gui_player_left_moves, typeof(Text), true) as Text;
				my_target.gui_bonus_bar =  EditorGUILayout.ObjectField("bonus bar", my_target.gui_bonus_bar , typeof(GameObject), true) as GameObject;
				
				my_target.gui_player_token_count_ico = EditorGUILayout.ObjectField("token count", my_target.gui_player_token_count_ico, typeof(GameObject), true) as GameObject;
				
				my_target.color_player_on = EditorGUILayout.ColorField("player on",my_target.color_player_on);
				my_target.color_player_off = EditorGUILayout.ColorField("player off",my_target.color_player_off);
				EditorGUI.indentLevel--;
				
				EditorGUILayout.LabelField("enemy");
				EditorGUI.indentLevel++;
				my_target.gui_enemy_name = EditorGUILayout.ObjectField("name", my_target.gui_enemy_name, typeof(GameObject), true) as GameObject;
				my_target.gui_enemy_hp = EditorGUILayout.ObjectField("hp", my_target.gui_enemy_hp, typeof(GameObject), true) as GameObject;
				my_target.gui_enemy_hp_slider = EditorGUILayout.ObjectField("slider hp", my_target.gui_enemy_hp_slider, typeof(Slider), true) as Slider;
				my_target.gui_enemy_hp_text = EditorGUILayout.ObjectField("text hp", my_target.gui_enemy_hp_text, typeof(Text), true) as Text;
				my_target.gui_enemy_armor = EditorGUILayout.ObjectField("armor", my_target.gui_enemy_armor, typeof(GameObject), true) as GameObject;
				my_target.gui_enemy_count = EditorGUILayout.ObjectField("gem count", my_target.gui_enemy_count, typeof(GameObject), true) as GameObject;
				my_target.gui_enemy_score = EditorGUILayout.ObjectField("score", my_target.gui_enemy_score, typeof(Text), true) as Text;
				my_target.gui_enemy_target_score = EditorGUILayout.ObjectField("target score", my_target.gui_enemy_target_score, typeof(Text), true) as Text;
				my_target.gui_enemy_target_score_slider = EditorGUILayout.ObjectField("slider score", my_target.gui_enemy_target_score_slider, typeof(Slider), true) as Slider;
				my_target.gui_enemy_left_moves = EditorGUILayout.ObjectField("left moves", my_target.gui_enemy_left_moves, typeof(Text), true) as Text;
				my_target.gui_enemy_bonus_bar =  EditorGUILayout.ObjectField("bonus bar", my_target.gui_enemy_bonus_bar , typeof(GameObject), true) as GameObject;
				
				my_target.color_enemy_on = EditorGUILayout.ColorField("enemy on",my_target.color_enemy_on);
				my_target.color_enemy_off = EditorGUILayout.ColorField("enemy off",my_target.color_enemy_off);
				EditorGUI.indentLevel--;
				
				EditorGUI.indentLevel--;
				
				//EditorGUI.indentLevel--;
			}
			
			my_target.show_garbages = EditorGUILayout.Foldout(my_target.show_garbages, "garbage");
			if (my_target.show_garbages)
			{
				EditorGUI.indentLevel++;
				if (my_target.garbage_recycle == null)
					GUI.color = Color.red;
				else
					GUI.color = Color.white;
				my_target.garbage_recycle =  EditorGUILayout.ObjectField("gem garbage", my_target.garbage_recycle , typeof(Transform), true) as Transform;
				GUI.color = Color.white;
				EditorGUILayout.Space();
				
				EditorGUILayout.LabelField("fx gem explosion garbage:");
				EditorGUI.indentLevel++;
				for (int i = 0; i < my_target.gem_length ; i ++)
				{
					if (my_target.garbage_recycle_gem_explosion_fx[i] == null)
						GUI.color = Color.red;
					else
						GUI.color = Color.white;
					my_target.garbage_recycle_gem_explosion_fx[i] =  EditorGUILayout.ObjectField("gem " + i, my_target.garbage_recycle_gem_explosion_fx[i] , typeof(Transform), true) as Transform;
				}
				GUI.color = Color.white;
				EditorGUI.indentLevel--;
				
				EditorGUILayout.LabelField("fx big explosion garbage:");
				EditorGUI.indentLevel++;
				for (int i = 0; i < 4 ; i ++)
				{
					if (my_target.garbage_recycle_big_explosion_fx[i] == null)
						GUI.color = Color.red;
					else
						GUI.color = Color.white;
					my_target.garbage_recycle_big_explosion_fx[i] =  EditorGUILayout.ObjectField("explode " + (i+4) , my_target.garbage_recycle_big_explosion_fx[i] , typeof(Transform), true) as Transform;
				}
				GUI.color = Color.white;
				EditorGUI.indentLevel--;
				
				EditorGUILayout.LabelField("fx bonus garbage:");
				EditorGUI.indentLevel++;
				my_target.garbage_recycle_bonus_one_fx =  EditorGUILayout.ObjectField("destroy one", my_target.garbage_recycle_bonus_one_fx , typeof(Transform), true) as Transform;
				my_target.garbage_recycle_bonus_3x3_fx =  EditorGUILayout.ObjectField("destroy 3x3", my_target.garbage_recycle_bonus_3x3_fx , typeof(Transform), true) as Transform;
				my_target.garbage_recycle_bonus_horizontal_fx =  EditorGUILayout.ObjectField("destroy horizontal", my_target.garbage_recycle_bonus_horizontal_fx , typeof(Transform), true) as Transform;
				my_target.garbage_recycle_bonus_vertical_fx =  EditorGUILayout.ObjectField("destroy vertical", my_target.garbage_recycle_bonus_vertical_fx , typeof(Transform), true) as Transform;
				my_target.garbage_recycle_bonus_horizontal_and_vertical_fx =  EditorGUILayout.ObjectField("destroy horizontal and vertical", my_target.garbage_recycle_bonus_horizontal_and_vertical_fx , typeof(Transform), true) as Transform;
				EditorGUI.indentLevel--;
				
				EditorGUI.indentLevel--;
			}
			
			my_target.show_menu_kit_setup = EditorGUILayout.Foldout(my_target.show_menu_kit_setup, "Menu kit");
			if (my_target.show_menu_kit_setup)
			{
				my_target.Stage_uGUI_obj =  EditorGUILayout.ObjectField("Stage_uGUI", my_target.Stage_uGUI_obj , typeof(GameObject), true) as GameObject;
				if (my_target.Stage_uGUI_obj != null)
				{
					if (my_target.continue_to_play_after_win_until_lose_happen)
						{
						if (my_target.win_requirement_selected == Board_C.win_requirement.reach_target_score)
							my_target.use_star_progress_bar = EditorGUILayout.Toggle("use star progress bar", my_target.use_star_progress_bar);
						else
							{
							if (my_target.win_requirement_selected == Board_C.win_requirement.collect_gems
							    && my_target.two_stars_target_score == 0 && my_target.three_stars_target_score == 0)
									my_target.use_star_progress_bar = EditorGUILayout.Toggle("use star progress bar", my_target.use_star_progress_bar);
							else
								my_target.use_star_progress_bar = false;
							}
						}
					else
						my_target.use_star_progress_bar = false;

					
					EditorGUILayout.LabelField("2 stars target:");
					EditorGUI.indentLevel++;
					my_target.two_stars_target_score = EditorGUILayout.IntField("score", my_target.two_stars_target_score);
					if (my_target.continue_to_play_after_win_until_lose_happen)
						{	
						if (my_target.win_requirement_selected == Board_C.win_requirement.collect_gems)
							my_target.two_stars_target_additional_gems_collected = EditorGUILayout.IntField("additional gems collected", my_target.two_stars_target_additional_gems_collected);
						else
							my_target.two_stars_target_additional_gems_collected = 0;
						}
					else
						{
						my_target.two_stars_target_additional_gems_collected = 0;

						if (my_target.lose_requirement_selected == Board_C.lose_requirement.enemy_reach_target_score)
							my_target.two_stars_target_score_advantage_vs_enemy = EditorGUILayout.IntField("score advantage on enemy", my_target.two_stars_target_score_advantage_vs_enemy);
						
						if (my_target.lose_requirement_selected == Board_C.lose_requirement.player_hp_is_zero)
							my_target.two_stars_target_player_hp_spared = EditorGUILayout.IntField("HP spared", my_target.two_stars_target_player_hp_spared);
						
						if (my_target.lose_requirement_selected == Board_C.lose_requirement.timer)
							my_target.two_stars_target_time_spared = EditorGUILayout.FloatField("time spared", my_target.two_stars_target_time_spared);
						
						if (my_target.lose_requirement_selected == Board_C.lose_requirement.player_have_zero_moves)
							my_target.two_stars_target_moves_spared = EditorGUILayout.IntField("moves spared", my_target.two_stars_target_moves_spared);
						
						if (my_target.lose_requirement_selected == Board_C.lose_requirement.enemy_collect_gems)
							my_target.two_stars_target_gems_collect_advantage_vs_enemy = EditorGUILayout.IntField("gems collected advantage on enemy", my_target.two_stars_target_gems_collect_advantage_vs_enemy);
						}
					EditorGUI.indentLevel--;

					
					EditorGUILayout.LabelField("3 stars target:");
					EditorGUI.indentLevel++;
					
					my_target.three_stars_target_score = EditorGUILayout.IntField("score", my_target.three_stars_target_score);
					
					if (my_target.continue_to_play_after_win_until_lose_happen)
					{
						if (my_target.win_requirement_selected == Board_C.win_requirement.collect_gems)
							my_target.three_stars_target_additional_gems_collected = EditorGUILayout.IntField("additional gems collected", my_target.three_stars_target_additional_gems_collected);
						else
							my_target.three_stars_target_moves_spared = 0;
					}
					else
					{
						my_target.three_stars_target_moves_spared = 0;
						
						
						if (my_target.lose_requirement_selected == Board_C.lose_requirement.enemy_reach_target_score)
							my_target.three_stars_target_score_advantage_vs_enemy = EditorGUILayout.IntField("score advantage on enemy", my_target.three_stars_target_score_advantage_vs_enemy);
						
						if (my_target.lose_requirement_selected == Board_C.lose_requirement.player_hp_is_zero)
							my_target.three_stars_target_player_hp_spared = EditorGUILayout.IntField("HP spared", my_target.three_stars_target_player_hp_spared);
						
						if (my_target.lose_requirement_selected == Board_C.lose_requirement.timer)
							my_target.three_stars_target_time_spared = EditorGUILayout.FloatField("time spared", my_target.three_stars_target_time_spared);
						
						if (my_target.lose_requirement_selected == Board_C.lose_requirement.player_have_zero_moves)
							my_target.three_stars_target_moves_spared = EditorGUILayout.IntField("moves spared", my_target.three_stars_target_moves_spared);
						
					}
					
					if (my_target.lose_requirement_selected == Board_C.lose_requirement.enemy_collect_gems)
						my_target.three_stars_target_gems_collect_advantage_vs_enemy = EditorGUILayout.IntField("gems collected advantage on enemy", my_target.three_stars_target_gems_collect_advantage_vs_enemy);
					
					EditorGUI.indentLevel--;
				}
				else
					my_target.use_star_progress_bar = false;
				
			}
		}
		
		if (EditorGUI.EndChangeCheck ())
			EditorUtility.SetDirty(my_target);
	}
	
	void Show_gui()
	{
		Board_C my_target = (Board_C)target;
		
		if ( (my_target.win_requirement_selected == Board_C.win_requirement.enemy_hp_is_zero)
		    || (my_target.lose_requirement_selected == Board_C.lose_requirement.enemy_collect_gems)
		    || (my_target.lose_requirement_selected == Board_C.lose_requirement.enemy_reach_target_score)
		    )
		{
			my_target.gui_enemy_name.SetActive(true);
			my_target.gui_vs.SetActive(true);
		}
		else
		{
			my_target.gui_enemy_name.SetActive(false);
			my_target.gui_vs.SetActive(false);
		}
		
		#region lose requirements
		if (my_target.lose_requirement_selected == Board_C.lose_requirement.timer)
			my_target.gui_timer.SetActive(true);
		else
			my_target.gui_timer.SetActive(false);
		
		if (my_target.lose_requirement_selected == Board_C.lose_requirement.player_hp_is_zero)
		{
			my_target.gui_player_hp.SetActive(true);
			if (my_target.use_armor)
				my_target.gui_player_armor.SetActive(true);
			else
				my_target.gui_player_armor.SetActive(false);
		}
		else
		{
			my_target.gui_player_hp.SetActive(false);
			my_target.gui_player_armor.SetActive(false);
		}
		
		if (my_target.lose_requirement_selected == Board_C.lose_requirement.enemy_collect_gems)
		{
			my_target.gui_enemy_count.SetActive(true);
		}
		else
		{
			my_target.gui_enemy_count.SetActive(false);
		}
		
		if (my_target.lose_requirement_selected == Board_C.lose_requirement.enemy_reach_target_score)
			my_target.gui_enemy_target_score_slider.gameObject.SetActive(true);
		else
			my_target.gui_enemy_target_score_slider.gameObject.SetActive(false);
		
		if (my_target.lose_requirement_selected == Board_C.lose_requirement.player_have_zero_moves)
			my_target.gui_player_left_moves.gameObject.SetActive(true);
		else
			my_target.gui_player_left_moves.gameObject.SetActive(false);
		#endregion
		
		#region win requirements
		if (my_target.win_requirement_selected == Board_C.win_requirement.destroy_all_tiles)
			my_target.gui_board_hp.SetActive(true);
		else
			my_target.gui_board_hp.SetActive(false);
		
		if (my_target.win_requirement_selected == Board_C.win_requirement.reach_target_score)
			my_target.gui_player_target_score_slider.gameObject.SetActive(true);
		else
			my_target.gui_player_target_score_slider.gameObject.SetActive(false);
		
		if (my_target.win_requirement_selected == Board_C.win_requirement.take_all_tokens)
			my_target.gui_player_token_count_ico.SetActive(true);
		else
			my_target.gui_player_token_count_ico.SetActive(false);
		
		
		if (my_target.win_requirement_selected == Board_C.win_requirement.enemy_hp_is_zero)
		{
			my_target.gui_enemy_hp.SetActive(true);
			if (my_target.use_armor)
				my_target.gui_enemy_armor.SetActive(true);
			else
				my_target.gui_enemy_armor.SetActive(false);
		}
		else
		{
			my_target.gui_enemy_hp.SetActive(false);
			my_target.gui_enemy_armor.SetActive(false);
		}
		
		if (my_target.win_requirement_selected == Board_C.win_requirement.collect_gems)
			my_target.gui_player_count.SetActive(true);
		else
			my_target.gui_player_count.SetActive(false);
		#endregion
		
		
	}
}
