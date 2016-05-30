using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

using System.IO;
using System;

public class Board_C : MonoBehaviour {

	public static GameObject this_board;

	public bool diagonal_falling;
	bool diagonal_falling_preference_direction_R; //it is alternate in each step

	//public bool player_can_move_when_gem_falling;
	//bool one_step_update_ongoing;

	public enum start_after
	{
		time,
		press_button
	}
	public start_after start_after_selected = start_after.time;
	public float start_after_n_seconds;
	
	public float accuracy;
	public float falling_speed;
	public float switch_speed;
	
	#region custom editor
	public bool show_sprites;
		public bool show_s_bonus_gui;
		public bool show_s_on_board_bonus;
		public bool show_s_tiles;
		public bool show_s_gems;
		public bool show_s_padlocks;
		public bool show_s_blocks;
		public bool show_s_falling_blocks;
		public bool show_s_misc;
	public bool show_rules;
		public bool show_score;
		public bool show_bonus;
			public bool show_player_charge_bonus;
			public bool show_enemy_charge_bonus;
				//these variables help to stop the board analysis when AI find the most high value possible
				int max_value_destroy_one;
				int max_value_switch_gem_teleport_click1;
				int max_value_switch_gem_teleport_click2;
				int max_value_destroy_3x3;
				int max_value_destroy_horizontal;
				int max_value_destroy_vertical;
				int max_value_destroy_horizontal_and_vertical;
			public bool show_player_inventory_bonus;
			public bool show_enemy_inventory_bonus;
		public bool show_move_reward_system;
		public bool show_player;
		public bool show_versus;
			public bool show_enemy;
	public bool show_camera_setup;
	public bool show_audio;
	public bool show_visual_fx;
		public bool show_frame_elements;
	public bool show_advanced;
		public bool show_advanced_timing;
		public bool show_advanced_ugui;
		public bool show_garbages;
	
	#endregion

	#region Menu Kit
	public bool show_menu_kit_setup;
	public GameObject Stage_uGUI_obj;

	public int three_stars_target_score;
	public int three_stars_target_score_advantage_vs_enemy;
	public int three_stars_target_player_hp_spared;
	public float three_stars_target_time_spared;
	public int three_stars_target_moves_spared;
	public int three_stars_target_gems_collect_advantage_vs_enemy;
	public int three_stars_target_additional_gems_collected;

	public int two_stars_target_score;
	public int two_stars_target_score_advantage_vs_enemy;
	public int two_stars_target_player_hp_spared;
	public float two_stars_target_time_spared;
	public int two_stars_target_moves_spared;
	public int two_stars_target_gems_collect_advantage_vs_enemy;
	public int two_stars_target_additional_gems_collected;

	public int additional_gems_collected_by_the_player;
	public int current_star_score;

	public bool use_star_progress_bar;
	#endregion

	#region ugui
		public Text gui_player_left_moves;
		public Text gui_enemy_left_moves;
		public Text gui_player_score;
			public Slider gui_player_target_score_slider;
			public Text gui_player_target_score;
		public Text gui_enemy_score;
			public Slider gui_enemy_target_score_slider;
			public Text gui_enemy_target_score;

		public bool show_score_of_this_move;
		public Transform the_gui_score_of_this_move;

		public GameObject gui_info_screen;
		public GameObject gui_win_screen;
		public GameObject gui_lose_screen;

		public Sprite[] gui_bonus = new Sprite[8];
		public GameObject gui_bonus_ico;
			public GameObject[] gui_bonus_ico_array;
				public GameObject[] gui_enemy_bonus_ico_array;
			public int slot_bonus_ico_selected = -1;
		public GameObject gui_bonus_bar;
			public GameObject gui_enemy_bonus_bar;

		public GameObject gui_timer;
			Slider gui_timer_slider;
		public GameObject gui_board_hp;
			Slider gui_board_hp_slider;
		public GameObject gui_vs;

		public GameObject gui_player_name;
				Text gui_player_name_text;
			public GameObject gui_player_hp;
				public Slider gui_player_hp_slider;
				public Text gui_player_hp_text;
			public GameObject gui_player_armor;
			public GameObject gui_player_count;

			public GameObject gui_player_token_count_ico;
				Text gui_player_token_count;
	
		public GameObject gui_enemy_name;
				Text gui_enemy_name_text;
			public GameObject gui_enemy_hp;
				public Slider gui_enemy_hp_slider;
				public Text gui_enemy_hp_text;
			public GameObject gui_enemy_armor;
			public GameObject gui_enemy_count;

	public Color color_collect_done;
	public Color color_player_on;
	public Color color_player_off;
	public Color color_enemy_on;
	public Color color_enemy_off;
	#endregion

	#region hint
	public Transform my_hint;
	public bool show_hint;
		public float show_hint_after_n_seconds;
	bool use_hint;
	#endregion

	#region score
	public bool show_score_gui;
	public int target_score;

	public int player_score;
	public int enemy_score;
	public int score_of_this_turn_move;
	bool this_is_the_primary_explosion;
	public int n_gems_exploded_with_main_gem;
	public int n_gems_exploded_with_minor_gem;

	public int score_reward_for_damaging_tiles;
	public int[] score_reward_for_explode_gems; //3 gems; 4; 5; 6; and 7
	public int score_reward_for_each_explode_gems_in_secondary_explosion;
	public float score_reward_for_explode_gems_of_the_same_color_in_two_or_more_turns_subsequently;
			int explode_same_color_again_with = 0;//0 = false; 1 = with main gem; 2 =with minor gem
			int[] player_previous_exploded_color = {-1,-1};//[0] = with main gem; [1] = with minor gem
			int player_explode_same_color_n_turn;
			int[] enemy_previous_exploded_color = {-1,-1};//[0] = with main gem; [1] = with minor gem
			int enemy_explode_same_color_n_turn;
	public float score_reward_for_secondary_combo_explosions;
	public int every_second_saved_give;
	public int every_move_saved_give;
	public int every_hp_saved_give;
	#endregion

	#region sound
	public AudioClip explosion_sfx; 
	public AudioClip end_fall_sfx; 
		public float latest_sfx_time;
	public AudioClip bad_move_sfx; 
	public AudioClip win_sfx; 
	public AudioClip lose_sfx; 
	public AudioClip[] bonus_sfx;
	public int play_this_bonus_sfx = -1; //-1 = don't play
	#endregion

	#region visual fx
	public bool show_frame_board_decoration;
		public Transform frame_pivot;
		public Transform[] frame_elements;
	public bool show_chess_board_decoration;

	public enum gem_explosion_fx_rule
	{
		no_fx,
		for_each_gem,
		only_for_big_explosion
	}
	public gem_explosion_fx_rule gem_explosion_fx_rule_selected = gem_explosion_fx_rule.no_fx;

	public bool activate_main_gem_fx_for_big_explosion;
	public bool activate_minor_gem_fx_for_big_explosion;

	public Transform[] gem_explosion_fx;
	public Transform[] gem_big_explosion_fx;

	public bool bonus_have_explosion_fx;
		public Transform destroy_one_fx;
		public Transform destroy_3x3_fx;
		public Transform destroy_horizontal_fx;
		public Transform destroy_vertical_fx;
		public Transform destroy_horizontal_and_vertical_fx;

	public enum gem_explosion_animation_type
	{
		no_animation,
		default_animation,
		custom_animation
	}
	public gem_explosion_animation_type gem_explosion_animation_type_selected = gem_explosion_animation_type.default_animation;

	public bool praise_the_player;
		public bool for_big_explosion;
		public bool for_secondary_explosions;
		public bool for_explode_same_color_again;
		public bool for_gain_a_star;
		public bool for_gain_a_turn;
		public GameObject praise_obj;
		public Praise praise_script;

	#endregion

	#region Camera setup
	public Camera Board_camera;
	public Vector3 Camera_adjust;
	public float camera_zoom;
	public bool adaptive_zoom;
	public enum camera_position
	{
		centred_to_board,
		centred_to_move,
		centred_to_player_avatar,
		manual
	}
	public camera_position camera_position_choice = camera_position.centred_to_board;
	//centred_to_move:
		public float camera_speed;
		float new_camera_position_x;
		float new_camera_position_y;
		Vector3 new_camera_position;
		public float camera_move_tolerance;
		public Vector2 margin;
	#endregion
	
	public TextAsset file_txt_asset;
	public int current_level = 1;

	
	public Transform pivot_board;
	public Transform garbage_recycle;
	public Transform[] garbage_recycle_gem_explosion_fx;
	public Transform[] garbage_recycle_big_explosion_fx;
	public Transform garbage_recycle_bonus_one_fx;
	public Transform garbage_recycle_bonus_3x3_fx;
	public Transform garbage_recycle_bonus_horizontal_fx;
	public Transform garbage_recycle_bonus_vertical_fx;
	public Transform garbage_recycle_bonus_horizontal_and_vertical_fx;

	public Transform cursor;
	public bool player_win;

	#region rules
	public enum win_requirement
	{
		destroy_all_tiles,
		enemy_hp_is_zero,
		collect_gems,
		reach_target_score,
		take_all_tokens, //special token to move until the bottom of the board
		//reach_the_exit 	//move the player avatar to exit-tile
		//destroy_all_gems,
		destroy_all_padlocks,
		destroy_all_blocks,
		play_until_lose

	}
	public win_requirement win_requirement_selected = win_requirement.destroy_all_tiles;
	public bool continue_to_play_after_win_until_lose_happen;

	
	public enum lose_requirement
	{
		timer,
		player_hp_is_zero,
		enemy_collect_gems,
		enemy_reach_target_score,
		player_have_zero_moves,
		relax_mode // player can't lose
		
	}
	public lose_requirement lose_requirement_selected = lose_requirement.timer;

	//manage turns
	public bool versus = false;//true = play versus AI
	public bool player_turn = false;//keep it false here in order to give the first move to player
	public int max_moves;
	public int current_player_moves_left;
	public int current_enemy_moves_left;
	public int move_gained_for_explode_same_color_in_two_adjacent_turn;
	public int[] move_gained_when_explode_more_than_3_gems;

	public bool game_end;


	public bool explosions_damages_tiles;
	public enum tile_destroyed_give
	{
		nothing,
		more_time,
		more_hp,
		more_moves
	}
	public tile_destroyed_give tile_give = tile_destroyed_give.nothing;
	public int tile_gift_int; //how much hp or moves give
	public float tile_gift_float; //how much time give
	public float time_bonus_for_secondary_explosion;
	public float time_bonus_for_gem_explosion;

	public int max_player_hp;
		public int current_player_hp;


	public int max_enemy_hp;
		public int current_enemy_hp;

	public float timer;
	public float time_left;
	public float time_bonus;
	public bool stage_started;
	public float start_time;

	//keep note of the gems destryed
	public int[] number_of_gems_to_destroy_to_win;

			public int[] total_number_of_gems_destroyed_by_the_player;
				public int[] number_of_gems_collect_by_the_player;
					bool[] player_this_gem_color_is_collected;
				public int total_number_of_gems_remaining_for_the_player;
					public int total_number_of_gems_required_colletted;
			public int[] total_number_of_gems_destroyed_by_the_enemy;
				public int[] number_of_gems_collect_by_the_enemy;
					bool[] enemy_this_gem_color_is_collected;
					int[] temp_enemy_gem_count;//this is use to evalutate bonus usefulness
				public int total_number_of_gems_remaining_for_the_enemy;
	//bonus
	public enum give_bonus
		{
		no,
		after_charge,
		after_big_explosion,
		from_stage_file_or_from_gem_emitter
		}
	public give_bonus give_bonus_select;
	//after charge
		public int[] bonus_cost;
			public int[] enemy_bonus_cost;
		public bool[] bonus_ready;
			public bool[] enemy_bonus_ready;
		public int[] filling_player_bonus;
			public int[] filling_enemy_bonus;
		public enum bonus
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
			heal_hp = 10,
		}
		public bonus bonus_select;
		public bonus[] bonus_slot;
			public bonus[] enemy_bonus_slot;
		public int bonus_slot_availables;
			public int enemey_bonus_slot_availables;
		public float add_time_bonus;
		public int add_moves_bonus;
		public bonus[] big_explosion_give_bonus;
		public bonus[] color_explosion_give_bonus;
		public int heal_hp_player_bonus;
		public int heal_hp_enemy_bonus;
	public bool linear_explosion_stop_against_empty_space;
	public bool linear_explosion_stop_against_block;
	public bool linear_explosion_stop_against_bonus;
	public bool linear_explosion_stop_against_token;
	//after_big_explosion
	public enum choose_bonus_by
	{
		gem_color,
		explosion_magnitude
	}
	public choose_bonus_by choose_bonus_by_select;

	public enum trigger_by
	{
		OFF,
		//color,
		click,
		switch_adjacent_gem,
		inventory,
		free_switch
	}
	public trigger_by trigger_by_select;
	bool clickable_bonus_on_boad;
	bool switchable_bonus_on_boad;
	bool free_switchable_bonus_on_boad;
	public int number_of_bonus_on_board;
	public int number_of_junk_on_board;

	public int number_of_token_on_board;
	public int number_of_token_to_collect;
	public int number_of_token_collected;
	public bool show_token_after_all_tiles_are_destroyed;
		bool[,]token_place_card;//if "show_token_after_all_tiles_are_destroyed" is true, use this to keep track of tokens positions
		bool token_showed;

	public int[] player_bonus_inventory;
		public bonus_inventory player_bonus_inventory_script;
	public int[] enemy_bonus_inventory;
		public bonus_inventory enemy_bonus_inventory_script;

	//armors
	public enum armor
	{
		weak,	// = damage * 2
		normal, // = damage * 1
		strong, // = damage * 0.5
		immune,	// = damage = 0
		absorb,
		repel
		
	}
	public bool use_armor;
	public armor[] player_armor;
	public armor[] enemy_armor;


	public int gem_damage_value; //how much damage deliver a gem

	public bool lose_turn_if_bad_move;
	public bool gain_turn_if_explode_same_color_of_previous_move;
	public bool gain_turn_if_explode_more_than_3_gems;
	public bool gain_turn_if_secondary_explosion;
		public int seconday_explosion_maginiture_needed_to_gain_a_turn;
		public int combo_lenght_needed_to_gain_a_turn;
	bool turn_gained;
	public bool chain_turns_limit;
	public int max_chain_turns;
	int current_player_chain_lenght;
	int current_enemy_chain_lenght;
	#endregion

	#region enemy AI
	public int chance_of_use_best_move;
		bool search_best_move;
		int big_move_value;
	public int chance_of_use_bonus;
		public int chance_of_use_best_bonus;
			//search the most useful bonus
			int temp_value = 0;
			int best_value = 0;
		bool enemy_will_use_a_bonus;
		//bool enemy_will_use_a_click_bonus_on_board;
		//bool enemy_will_switch_a_gem_to_trigger_a_bonus_on_board;
		Vector2[] bonus_coordinate;
		//bool enemy_will_use_a_charge_bonus;
		public int enemy_chosen_bonus_slot;
		int enemy_bonus_click_1_x;
		int enemy_bonus_click_1_y;
		public int enemy_bonus_click_2_x;
		public int enemy_bonus_click_2_y;

	public float enemy_move_delay = 1;//how much seconds pass between the enemy clicks (It is of use to show to palyer what the enemy is doing);

	public enum enemy_AI
	{
		random,
		collect_gems_from_less_to_more, //only to gem collect
		collect_gems_from_more_to_less, //only to gem collect
		dynamic_battle, //only to battle_mode
		by_hand_setup
	}
	public enemy_AI enemy_AI_select;

	ArrayList[] gem_color;
	ArrayList available_directions;

	public enum enemy_AI_manual_setup
	{
		gem_0,
		gem_1,
		gem_2,
		gem_3,
		gem_4,
		gem_5,
		gem_6
	}
	public enemy_AI_manual_setup[] temp_enemy_AI_preference_order;
		enemy_AI_manual_setup[] enemy_AI_preference_order;
	int enemy_move_selected;	//main gem choice by the enemy
	int enemy_move_direction;//move direction of the main gem
	#endregion
	
	#region interaction
		public int touch_number;
		public bool player_can_move;
		public int n_combo;//number of combo explosion after main explosion
	
		public bool[,] position_of_gems_that_will_explode;
		/* [0 main_gem ; 1 minor_gem]
			 *0 = the one 2 to up
			 *1 = the one 1 to up
			 *2 = the one 2 to right
			 *3 = the one 1 to right
			 *4 = the one 2 to down
			 *5 = the one 1 to down
			 *6 = the one 2 to left
			 *7 = the one 1 to left
		*/
		public int number_of_padlocks_involved_in_explosion;
		public int number_of_elements_to_damage;
		//public int number_of_gems_to_destroy;
		//public int number_of_blocks_to_update;
		public int gems_useful_moved;//help to know if the move is a double-move (if = 2: main gem AND minor gem will explode)
		
	
		public GameObject avatar_main_gem;
		public int main_gem_selected_x = -10;
		public int main_gem_selected_y = -10;
		public int main_gem_color = -10;
		

		public GameObject avatar_minor_gem;
		public int minor_gem_destination_to_x = -10;
		public int minor_gem_destination_to_y = -10;
		public int minor_gem_color = -10;

	#endregion

	//manage update board after the move
		public int number_of_gems_to_move;
		public int number_of_new_gems_to_create;
		public int number_of_gems_to_mix;
		public bool gem_falling_ongoing;

	//read board
	public int number_of_moves_possible;
	int[,] list_of_moves_possible;
	public int number_of_gems_moveable;

	public int HP_board;//if 0 mean that all tiles are destroyed (victory requirement)
	public GameObject tile_obj;
	public Sprite[] tile_hp; //the avatar of the tile
	public GameObject[,] tiles_array;
		public int total_tiles;
		//public GameObject[] tiles_leader_array;//these tiles will generate new gems
			//int number_of_tiles_leader;//the array length
		public GameObject[] elements_to_damage_array;
		public tile_C[,] bottom_tiles_array;//[board orientation, tile]this help to check when token and junk bust exit from the board 
		int[] number_of_bottom_tiles;//[board orientation]
		public int current_board_orientation;
	

	public GameObject tile_content;
	public GameObject over_gem;
	public Sprite[] gem_colors;//the avatars of the gems
	public int gem_length;

	public Sprite[] lock_gem_hp;
	public int padlock_count;

	public Sprite[] block_hp;
	public int block_count;

	public Sprite[] falling_block_hp;

	public Sprite immune_block;
	public Sprite junk;
	public Sprite token;
	
	public Sprite[] ice_hp;
	public Sprite[] need_color;
	public Sprite[] item_color;
	public Sprite[] key_color;
	public Sprite[] door_color;
	public Sprite[] start_goal_path;
	public Sprite player_avatar;

	//bonus
	public Sprite[] on_board_bonus_sprites;

	string fileContents = string.Empty;
	//public int[,] array_buttons_tiles;
	public int _X_tiles;
	public int _Y_tiles;

	public int[,,] board_array_master;//this keep track of the status of every tiles, gems, lock and so on... in the board
	/* 0 = tile [-1 = "no tile"; 0 = "hp = 0"; 1 = "hp 1"...]
	 * 1 = gem [-99 = no gem; 
	 * 			from 0 to 6 color gem; 
	 * 				7 = special explosion good 
	 * 				8 = special explosion bad 
	 * 				9 = neutre = it don't explode when 3 in a row
	 * 			10 = random gem; 
	 * 			20 = ?; 
	 * 			30 = ?; 
	 * 			40 = immune block
	 * 			41 = block hp 1
	 * 			42 = block hp 2
	 * 			43 = block hp 3
	 * 			51 = falling block hp 1
	 * 			52 = falling block hp 2
	 * 			53 = falling block hp 3
	 * 			60 = need a
	 * 			61 = need b
	 * 			62 = need c
	 * 			63 = need d
	 * 			64 = need e
	 * 			70 = key a
	 * 			71 = key b
	 * 			72 = key c
	 * 			73 = key d
	 * 			74 = key e
	 * 2 = special tile:
	 * 		0 = no special
	 * 		1 = start
	 * 		2 = goal
	 * 		3 = path
	 * 		10 = door a
	 * 		11 = door b
	 * 		12 = door c
	 * 		13 = door d
	 * 		14 = door e
	 * 		20 = item a
	 * 		21 = item b
	 * 		22 = item c
	 * 		23 = item d
	 * 		24 = item e
	 * 3 = restraint [0 = no padlock; 1 = padlock hp1...
	 * 				11 = ice hp1...
	 * 4 = special [-200 = token
	 * 				-100 = junk
	 * 				0 = no
	 * 				1= destroy_one
	 * 				2= Switch_gem_teleport
	 * 				3= bomb
	 * 				4= horiz
	 * 				5= vertic
	 * 				6= horiz_and_vertic
	 * 				7= destroy_all_same_color
	 * 
	 * 				8= more_time
	 * 				9= more_moves
	 * 				10= more_hp
	 * 				11= rotate_board_L
	 * 				12= rotate_board_R
	 * 
	 * 				13 = destroy single random gems (meteor shower)
	 * 
	 * 				100 = time bomb
	 * 
	 * 
	 * 5 = number of useful moves of this gem [from 0 = none, to 4 = all directions]
		 	6 = up [n. how many gem explode if this gem go up]
		 	7 = down [n. how many gem explode if this gem go down]
		 	8 = right [n. how many gem explode if this gem go right]
		 	9 = left [n. how many gem explode if this gem go left]
		10 = this thing can fall (0=false;1=true) (2 = explode if reach board bottom border)
		11 = current tile action in progress (0=none;1=explosion;2=creation;3=falling down;4=falling down R;5=falling down L)
		12 = this tile generate gems (0= no; 1= yes; 2= yes and it is activated)
		13 = tile already checked (0=false;1=true)
		14 = block_hp
	 */
	private int board_array_master_length = 15;

	/* if you have the menu kit, DELETE THIS LINE
	game_master my_game_master;
	public game_uGUI my_game_uGUI;
	//if you have the menu kit, DELETE THIS LINE*/

	void Awake()
		{

		/* if you have the menu kit, DELETE THIS LINE
		if (game_master.game_master_obj)
			my_game_master = (game_master)game_master.game_master_obj.GetComponent("game_master");
		
		my_game_uGUI = (game_uGUI)Stage_uGUI_obj.GetComponent("game_uGUI");
		my_game_uGUI.show_progress_bar = use_star_progress_bar;
		// if you have the menu kit, DELETE THIS LINE*/

		Initiate_variables();


		/* if you have the menu kit, DELETE THIS LINE
		if (my_game_uGUI.my_progress_bar && my_game_uGUI.show_progress_bar)
			{
			if (win_requirement_selected == win_requirement.reach_target_score)
				{
				//setup menu kit progress bar
				my_game_uGUI.progress_bar_use_score = true;
				my_game_uGUI.my_progress_bar.star_target_values[0] = target_score;
				my_game_uGUI.my_progress_bar.star_target_values[1] = two_stars_target_score;
				my_game_uGUI.my_progress_bar.star_target_values[2] = three_stars_target_score;
				//turn off board progress bar

				gui_player_target_score_slider.gameObject.SetActive(false);

				}
			else if (win_requirement_selected == win_requirement.collect_gems)
				{
				my_game_uGUI.progress_bar_use_score = false;
				my_game_uGUI.my_progress_bar.star_target_values[0] = total_number_of_gems_remaining_for_the_player;
				my_game_uGUI.my_progress_bar.star_target_values[1] = total_number_of_gems_remaining_for_the_player + two_stars_target_additional_gems_collected;
				my_game_uGUI.my_progress_bar.star_target_values[2] = total_number_of_gems_remaining_for_the_player + three_stars_target_additional_gems_collected;
				}

			}
		// if you have the menu kit, DELETE THIS LINE*/

		Load_board();
		Setup_camera();
		Create_new_board();
	 	}

	void Setup_camera()//call from Awake
	{
	if (Board_camera)
		{
		if (Stage_uGUI_obj)//avoid 2 AudioListener when use menu kit
			Board_camera.GetComponent<AudioListener>().enabled = false;
		else
			Board_camera.GetComponent<AudioListener>().enabled = true;

		if (camera_position_choice == camera_position.centred_to_board)
			{
			Board_camera.transform.position = new Vector3 (pivot_board.transform.position.x + (_X_tiles-1)*0.5f + Camera_adjust.x, 
			                                               pivot_board.transform.position.y + (_Y_tiles-1)*-0.5f + Camera_adjust.y, 
			                                               pivot_board.transform.position.z + Camera_adjust.z
			                                               );
			}
		else if (camera_position_choice == camera_position.centred_to_move)
			{
				Board_camera.transform.position = (pivot_board.transform.position + Camera_adjust);
			}

		if (adaptive_zoom)
			{
			Board_camera.orthographicSize = (_Y_tiles+(camera_zoom*-2))*0.5f ; 
			}
		else
			{
			Board_camera.orthographicSize = 4 + camera_zoom*-1;
			}

		if (Board_camera.orthographicSize <= 0)
			Board_camera.orthographicSize = 1;
		}
	}

	void Initiate_ugui()//call from Initiate_variables()
		{
		if (give_bonus_select == give_bonus.after_charge)
			{
			if (bonus_slot_availables > 0)
				{
				gui_bonus_bar.SetActive(true);
				for (int i = 0; i < bonus_slot_availables; i++)
					{
					if (bonus_slot[i] != bonus.none)
						{
						gui_bonus_ico_array[i] = (GameObject)Instantiate(gui_bonus_ico);
						gui_bonus_ico_array[i].transform.SetParent(gui_bonus_bar.transform,false);
						gui_bonus_ico_array[i].name = (bonus_slot[i]).ToString();
						gui_bonus_ico_array[i].GetComponent<Image>().sprite = gui_bonus[(int)bonus_slot[i]-1];
						bonus_button _bonus_button = (bonus_button)gui_bonus_ico_array[i].GetComponent("bonus_button");
							_bonus_button.slot_number = i;
							_bonus_button.player = true;
						gui_bonus_ico_array[i].transform.GetChild(1).GetComponent<Image>().sprite = gem_colors[i];
						}
					}
				}
			else
				gui_bonus_bar.SetActive(false);

			if ( (enemey_bonus_slot_availables > 0) && (versus) )
				{
				gui_enemy_bonus_bar.SetActive(true);
				for (int i = 0; i < enemey_bonus_slot_availables; i++)
					{
					if (enemy_bonus_slot[i] != bonus.none)
						{
						gui_enemy_bonus_ico_array[i] = (GameObject)Instantiate(gui_bonus_ico);
						gui_enemy_bonus_ico_array[i].transform.SetParent(gui_enemy_bonus_bar.transform,false);
						gui_enemy_bonus_ico_array[i].name = (enemy_bonus_slot[i]).ToString();
						gui_enemy_bonus_ico_array[i].GetComponent<Image>().sprite = gui_bonus[(int)enemy_bonus_slot[i]-1];
						bonus_button _bonus_button = (bonus_button)gui_enemy_bonus_ico_array[i].GetComponent("bonus_button");
						_bonus_button.slot_number = i;
						_bonus_button.player = false;
						gui_enemy_bonus_ico_array[i].transform.GetChild(1).GetComponent<Image>().sprite = gem_colors[i];
						}
					}
				}
			else
				gui_enemy_bonus_bar.SetActive(false);

			}
		else
			{
			if (trigger_by_select == trigger_by.inventory)
				{
				gui_bonus_bar.SetActive(true);
				player_bonus_inventory_script = gui_bonus_bar.GetComponent<bonus_inventory>();

				if (versus)
					{
					gui_enemy_bonus_bar.SetActive(true);
					enemy_bonus_inventory_script = gui_enemy_bonus_bar.GetComponent<bonus_inventory>();
					}
				else
					gui_enemy_bonus_bar.SetActive(false);
				}
			else
				{
				gui_bonus_bar.SetActive(false);
				gui_enemy_bonus_bar.SetActive(false);
				}
			}

		gui_timer_slider = gui_timer.GetComponent<Slider>();
			gui_timer_slider.maxValue = timer;
			gui_timer_slider.value = time_left;

		gui_board_hp_slider = gui_board_hp.GetComponent<Slider>();
			gui_board_hp_slider.value = 0;

		gui_player_name_text = gui_player_name.GetComponent<Text>();
		gui_enemy_name_text = gui_enemy_name.GetComponent<Text>();

		gui_player_hp_slider.maxValue = max_player_hp;
		gui_player_hp_slider.value = current_player_hp;
		gui_player_hp_text.text = current_player_hp.ToString() + " / " + max_player_hp.ToString();

		gui_enemy_hp_slider.maxValue = max_enemy_hp;
		gui_enemy_hp_slider.value = current_enemy_hp;
		gui_enemy_hp_text.text = current_enemy_hp.ToString() + " / " + max_enemy_hp.ToString();

		gui_player_target_score_slider.maxValue = target_score;
			gui_player_target_score_slider.value = 0;
		gui_enemy_target_score_slider.maxValue = target_score;
			gui_enemy_target_score_slider.value = 0;

		if (win_requirement_selected == win_requirement.take_all_tokens)
			{
			gui_player_token_count_ico.GetComponent<Image>().sprite = token;
			gui_player_token_count = gui_player_token_count_ico.transform.GetChild(0).GetComponent<Text>();
			gui_player_token_count.text = "0 / "+number_of_token_to_collect.ToString();
			gui_player_token_count_ico.gameObject.SetActive(true);

			}
		else
			gui_player_token_count_ico.gameObject.SetActive(false);

		Update_score();
		Auto_setup_gui();
		}

	void Auto_setup_gui()//call from Initiate_ugui()
	{
		praise_script = praise_obj.transform.GetChild(0).GetComponent<Praise>();

		if ( (win_requirement_selected == Board_C.win_requirement.enemy_hp_is_zero)
		    || (lose_requirement_selected == Board_C.lose_requirement.enemy_collect_gems)
		    || (lose_requirement_selected == Board_C.lose_requirement.enemy_reach_target_score)
		    )
		{
			gui_enemy_name.SetActive(true);
			gui_vs.SetActive(true);
		}
		else
		{
			gui_enemy_name.SetActive(false);
			gui_vs.SetActive(false);
		}
		
		#region lose requirements
		if (lose_requirement_selected == Board_C.lose_requirement.timer)
			gui_timer.SetActive(true);
		else
			gui_timer.SetActive(false);
		
		if (lose_requirement_selected == Board_C.lose_requirement.player_hp_is_zero)
		{
			gui_player_hp.SetActive(true);
			if (use_armor)
				{
				gui_player_armor.SetActive(true);

				for (int i = 0; i < gui_player_count.transform.childCount; i++)
					{
					if (i < gem_length)
						{
						gui_player_armor.transform.GetChild(i).gameObject.SetActive(true);
						if (i > 4)
							gui_player_armor.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().sprite = gem_colors[i];
						}
					else
						gui_player_armor.transform.GetChild(i).gameObject.SetActive(false);
				}
					}
			else
				gui_player_armor.SetActive(false);
		}
		else
		{
			gui_player_hp.SetActive(false);
			gui_player_armor.SetActive(false);
		}
		
		if (lose_requirement_selected == Board_C.lose_requirement.enemy_collect_gems)
		{
			gui_enemy_count.SetActive(true);
			for (int i = 0; i < gui_enemy_count.transform.childCount; i++)
			{
				if (i < gem_length && number_of_gems_to_destroy_to_win[i] > 0)
				{
					gui_enemy_count.transform.GetChild(i).gameObject.SetActive(true);
					if (i > 4)
						gui_enemy_count.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().sprite = gem_colors[i];
				}
				else
					{
					gui_enemy_count.transform.GetChild(i).gameObject.SetActive(false);
					if (i < gem_length)
						enemy_this_gem_color_is_collected[i] = true;
					}
			}
		}
		else
		{
			gui_enemy_count.SetActive(false);
		}

		if (lose_requirement_selected == Board_C.lose_requirement.player_have_zero_moves)
			{
			gui_player_left_moves.enabled = true;
			Update_left_moves_text();
			}
		else
			gui_player_left_moves.enabled = false;

		if (lose_requirement_selected == Board_C.lose_requirement.enemy_reach_target_score)
			gui_enemy_target_score_slider.gameObject.SetActive(true);
		else
			gui_enemy_target_score_slider.gameObject.SetActive(false);

		if (lose_requirement_selected == Board_C.lose_requirement.player_have_zero_moves)
			gui_player_left_moves.gameObject.SetActive(true);
		else
			gui_player_left_moves.gameObject.SetActive(false);
		#endregion
		
		#region win requirements
		if (win_requirement_selected == Board_C.win_requirement.destroy_all_tiles)
			gui_board_hp.SetActive(true);
		else
			gui_board_hp.SetActive(false);

		if (win_requirement_selected == Board_C.win_requirement.reach_target_score)
			gui_player_target_score_slider.gameObject.SetActive(true);
		else
			gui_player_target_score_slider.gameObject.SetActive(false);

		
		if (win_requirement_selected == Board_C.win_requirement.enemy_hp_is_zero)
		{
			gui_enemy_hp.SetActive(true);
			if (use_armor)
				{
				gui_enemy_armor.SetActive(true);

				for (int i = 0; i < gui_enemy_armor.transform.childCount; i++)
					{
					if (i < gem_length)
						{
						gui_enemy_armor.transform.GetChild(i).gameObject.SetActive(true);
						if (i > 4)
							gui_enemy_armor.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().sprite = gem_colors[i];
						}
						else
							gui_enemy_armor.transform.GetChild(i).gameObject.SetActive(false);
					}
				}
			else
				gui_enemy_armor.SetActive(false);
		}
		else
		{
			gui_enemy_hp.SetActive(false);
			gui_enemy_armor.SetActive(false);
		}
		
		if (win_requirement_selected == Board_C.win_requirement.collect_gems)
			{
			gui_player_count.SetActive(true);
			for (int i = 0; i < gui_player_count.transform.childCount; i++)
				{
				if (i < gem_length && number_of_gems_to_destroy_to_win[i] > 0)
					{
					gui_player_count.transform.GetChild(i).gameObject.SetActive(true);
					if (i > 4)
						gui_player_count.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().sprite = gem_colors[i];
					}
				else
					{
					gui_player_count.transform.GetChild(i).gameObject.SetActive(false);
					if (i < gem_length)
						player_this_gem_color_is_collected[i] = true;
					}
				}
			}
		else
			gui_player_count.SetActive(false);
		#endregion
	}

	public void Gain_turns(int quantity)
	{
		if (player_turn)
			{
			current_player_moves_left += quantity;
			if (praise_the_player && for_gain_a_turn)
				praise_script.Gain_a_turn(gui_player_name_text.text, quantity);
			}
		else
			{
			current_enemy_moves_left += quantity;
			if (praise_the_player && for_gain_a_turn)
				praise_script.Gain_a_turn(gui_enemy_name_text.text, quantity);
			}

		turn_gained = true;
		Update_left_moves_text();

	}

	public void Update_left_moves_text()//call from Auto_setup_gui(), Switch_gem(int direction), Move_gems_to_target_positions(), Detect_which_gems_will_explode(int __x, int __y, int _gem), Update_turn_order_after_a_bad_move()
	{
		gui_player_left_moves.text = "Left moves: " + current_player_moves_left;
		gui_enemy_left_moves.text = "Left moves: " + current_enemy_moves_left;
	}

	public void Update_token_count()//call from Create_new_board(), tile_C.Check_the_content_of_this_tile()
		{
		if (win_requirement_selected == win_requirement.take_all_tokens)
			{
			gui_player_token_count.text = number_of_token_collected.ToString() + " / " + number_of_token_to_collect.ToString();
			if (number_of_token_collected >= number_of_token_to_collect)
				{
				gui_player_token_count.color = color_collect_done;
				Player_win();
				}
			}
		}

	public void Heal_me()//call from bonus_button.Activate()
	{
		if (player_turn)
			{
			if ((current_player_hp + heal_hp_player_bonus) <= max_player_hp)
				current_player_hp += heal_hp_player_bonus;
			else
				current_player_hp = max_player_hp;

			}
		else
			{
			if ((current_enemy_hp + heal_hp_enemy_bonus) <= max_enemy_hp)
				current_enemy_hp += heal_hp_enemy_bonus;
			else
				current_enemy_hp = max_enemy_hp;

			}

		Update_hp();
		Annotate_potential_moves();
	}
	


	public void Update_board_hp()//call from tile_C.Update_tile_hp()
	{
		HP_board--;
		gui_board_hp_slider.value++;
	}

	public void Update_bonus_fill(int _x, int _y, int n)//call from tile_C.Update_gems_score()
	{
		//if this bonus don't is full yet
		if (player_turn)
			{
			if (gui_bonus_ico_array[n] && (filling_player_bonus[n] < bonus_cost[n]))
				{
				filling_player_bonus[n]++;
				if (filling_player_bonus[n] >= bonus_cost[n])
					{
					filling_player_bonus[n] = bonus_cost[n];
					bonus_ready[n] = true;
					}


				gui_bonus_ico_array[n].GetComponent<bonus_button>().Update_fill();
				}
			}
		else
			{
			if (gui_enemy_bonus_ico_array[n] && (filling_enemy_bonus[n] < enemy_bonus_cost[n]))
				{
				filling_enemy_bonus[n]++;
				if (filling_enemy_bonus[n] >= enemy_bonus_cost[n])
					{
					filling_enemy_bonus[n] = enemy_bonus_cost[n];
					enemy_bonus_ready[n] = true;
					}

				gui_enemy_bonus_ico_array[n].GetComponent<bonus_button>().Update_fill();
				}
			}
	}

	public void Update_inventory_bonus(int bonus_id, int quantity)//call from tile_C: Give_more_time(), Destroy_one(), Destroy_horizontal(), Destroy_all_gems_with_this_color(), Update_tile_hp(), Destroy_3x3(), Destroy_vertical(), Destroy_horizonal_and_vertical(), Check_the_content_of_this_tile(), Check_if_shuffle_is_done()
	{
		if (trigger_by_select == trigger_by.inventory)
			{
			if (player_turn)
				{
				player_bonus_inventory[bonus_id] += quantity;
				player_bonus_inventory_script.Update_bonus_count(bonus_id);
				}
			else
				{
				enemy_bonus_inventory[bonus_id] += quantity;
				enemy_bonus_inventory_script.Update_bonus_count(bonus_id);
				}
			}
	}

	public void Update_score()//call from initiate_ugui(), Order_to_gems_to_explode(), tile_C.Update_tile_hp()
	{
		gui_player_score.text = "score: " + player_score;
		gui_enemy_score.text = "score: " + enemy_score;

		gui_player_target_score_slider.value = player_score;
			gui_player_target_score.text = player_score + " / " + target_score;
		gui_enemy_target_score_slider.value = enemy_score;
			gui_enemy_target_score.text = enemy_score + " / " + target_score;

		if (continue_to_play_after_win_until_lose_happen && player_win)
		{
			if (win_requirement_selected != win_requirement.collect_gems)
			{
				if ((current_star_score < 3) && (player_score >= three_stars_target_score))
					{
					current_star_score = 3;
					praise_script.Win_and_continue_to_play(current_star_score-1);
					}
				else if ((current_star_score < 2) && (player_score >= two_stars_target_score))
					{
					current_star_score = 2;
					praise_script.Win_and_continue_to_play(current_star_score-1);
					}
			}
		}

		if (Stage_uGUI_obj)//use menu kit win screen
		{
			/* if you have the menu kit, DELETE THIS LINE
				my_game_uGUI.int_score = player_score;
				my_game_uGUI.Update_int_score();
				
			// if you have the menu kit, DELETE THIS LINE */
		}
	}

	public void Update_hp()//call from Board_C.Heal_me()
	{
		gui_player_hp_slider.value = current_player_hp;
		gui_player_hp_text.text = current_player_hp.ToString() + " / " + max_player_hp.ToString();

		gui_enemy_hp_slider.value = current_enemy_hp;
		gui_enemy_hp_text.text = current_enemy_hp.ToString() + " / " + max_enemy_hp.ToString();
	}

	void Reset_variables()//call from Initiate_variables()
	{
		player_win = false;
		game_end = false;
		versus = false;
		player_turn = false;
		current_board_orientation = 0;
		number_of_token_collected = 0;
		token_showed = false;
	}
	
	void Initiate_variables()//call from Awake
	{
		Reset_variables();
		this_board = this.gameObject;

		cursor.gameObject.SetActive(false);

		number_of_bottom_tiles = new int[4];

		//bonus
		bonus_ready = new bool[gem_length];
			enemy_bonus_ready = new bool[gem_length];
		filling_player_bonus = new int[gem_length];
			filling_enemy_bonus = new int[gem_length];
		gui_bonus_ico_array = new GameObject[gem_length];
			gui_enemy_bonus_ico_array = new GameObject[gem_length];

		if (bonus_slot.Length == 0)
			{
			bonus_slot = new bonus[gem_length];
			bonus_cost = new int[gem_length];
			}

		if (enemy_bonus_slot.Length == 0)
			{
			enemy_bonus_slot = new bonus[gem_length];
			enemy_bonus_cost = new int[gem_length];
			}

		//gem count
		total_number_of_gems_destroyed_by_the_player = new int[gem_length];
		number_of_gems_collect_by_the_player = new int[gem_length];
		total_number_of_gems_destroyed_by_the_enemy = new int[gem_length];
		number_of_gems_collect_by_the_enemy = new int[gem_length];
		additional_gems_collected_by_the_player = 0;

		//score
		current_star_score = 0;
		player_score = 0;
		enemy_score = 0;

		if (number_of_gems_to_destroy_to_win.Length == 0)
			number_of_gems_to_destroy_to_win = new int[gem_length];

		player_this_gem_color_is_collected = new bool[gem_length];
		enemy_this_gem_color_is_collected = new bool[gem_length];

		enemy_AI_preference_order = new enemy_AI_manual_setup[gem_length];
		for (int n = 0; n < gem_length ; n++)
		{
			total_number_of_gems_remaining_for_the_player += number_of_gems_to_destroy_to_win[n];
			enemy_AI_preference_order[n] = temp_enemy_AI_preference_order[n];
		}
		total_number_of_gems_remaining_for_the_enemy = total_number_of_gems_remaining_for_the_player ;
		total_number_of_gems_required_colletted = 0;

		current_player_hp = max_player_hp;
		current_enemy_hp = max_enemy_hp;

		time_left = timer;
		current_player_moves_left = max_moves;
		current_enemy_moves_left = max_moves;

		if ( (win_requirement_selected == win_requirement.enemy_hp_is_zero)
		    || (lose_requirement_selected == lose_requirement.enemy_collect_gems) 
		    || (lose_requirement_selected == lose_requirement.enemy_reach_target_score))
			versus = true;

		Initiate_ugui();
		}
	
	void Search_max_bonus_values_for_charge_bonus()//call from Create_new_board()
	{
		if (versus && (
		    (give_bonus_select == give_bonus.after_charge) //calculate the max value that be archive using the bonus on board
			|| (trigger_by_select == trigger_by.inventory) )
		    )
		{
			//search most long x_line and y_line
			int most_long_x_line = 0;
				int temp_x_line = 0;
			int most_long_y_line = 0;
				int temp_y_line = 0;

			//search x_line
			for (int y = 0; y < _Y_tiles; y++)
				{
				for (int x = 0; x < _X_tiles; x++)
					{
					if (board_array_master[x,y,0] >= 0) //if there is a tile here
						{
						temp_x_line++;
						}
					else //line discontinued, so check its length
						{
						//Debug.Log("x line break at " + x + "," + y);
						if (temp_x_line > most_long_x_line)
							most_long_x_line = temp_x_line;
						}
					}
				//reach the end of this x line
				if (temp_x_line > most_long_x_line)
					most_long_x_line = temp_x_line;

				temp_x_line = 0;

				if (most_long_x_line == _X_tiles)
				{
					//Debug.Log("you can't find a X line more long of " + most_long_x_line + " so stop the check");
					break;
				}

				}
			//Debug.Log("the most long x_line is: " + most_long_x_line);

			//search y_line
			for (int x = 0; x < _X_tiles; x++)
				{
				for (int y = 0; y < _Y_tiles; y++)
					{
					if (board_array_master[x,y,0] >= 0) //if there is a tile here
						{
						temp_y_line++;
						}
					else //line discontinued, so check its length
						{
						//Debug.Log("y line break at " + x + "," + y);
						if (temp_y_line > most_long_y_line)
							most_long_y_line = temp_y_line;
						}
					}
				//reach the end of this y line
				if (temp_y_line > most_long_y_line)
					most_long_y_line = temp_y_line;

				temp_y_line = 0;

				if (most_long_y_line == _Y_tiles)
					{
					//Debug.Log("you can't find a Y line more long of " + most_long_y_line + " so stop the check");
					break;
					}
				}
			//Debug.Log("the most long y_line is: " + most_long_y_line);
		
		
			switch(lose_requirement_selected)
				{
				case lose_requirement.enemy_collect_gems:
					max_value_destroy_one = 7;
					max_value_switch_gem_teleport_click1 = 9;
					max_value_switch_gem_teleport_click2 = 9;
					max_value_destroy_3x3 = 9;
					max_value_destroy_horizontal = most_long_x_line;
					max_value_destroy_vertical = most_long_y_line;
					max_value_destroy_horizontal_and_vertical = most_long_x_line + most_long_y_line - 1;
					break;
					
				case lose_requirement.player_hp_is_zero:
					max_value_destroy_one = 7 * gem_damage_value * 2;
					max_value_switch_gem_teleport_click1 = (9 * gem_damage_value * 2);
					max_value_switch_gem_teleport_click2 = (9 * gem_damage_value);
					max_value_destroy_3x3 = (6 * gem_damage_value * 2) + (3 * gem_damage_value);
					max_value_destroy_horizontal = (int)Math.Round((most_long_x_line * 0.65f * gem_damage_value * 2) + (most_long_x_line * 0.35f * gem_damage_value));
					max_value_destroy_vertical = (int)Math.Round((most_long_y_line * 0.65f * gem_damage_value * 2) + (most_long_y_line * 0.35f * gem_damage_value));
					max_value_destroy_horizontal_and_vertical = (int)Math.Round((most_long_x_line * 0.65f * gem_damage_value * 2) + (most_long_x_line * 0.35f * gem_damage_value) + (most_long_y_line * 0.65f * gem_damage_value * 2) + (most_long_y_line * 0.35f * gem_damage_value) - gem_damage_value);
					break;
					
				case lose_requirement.enemy_reach_target_score:
					max_value_destroy_one = 9 ;
					max_value_switch_gem_teleport_click1 = 9;
					max_value_switch_gem_teleport_click2 = 9;
					max_value_destroy_3x3 = 9 ;
					max_value_destroy_horizontal = most_long_x_line ;
					max_value_destroy_vertical = most_long_y_line ;
					max_value_destroy_horizontal_and_vertical = (most_long_x_line + most_long_y_line - 1) ;
					break;
				}



		}
	}


	void Start () {

		if (start_after_selected == start_after.time)
			{
			if (start_after_n_seconds <= 0)
				Check_ALL_possible_moves();
			else
				Invoke("Check_ALL_possible_moves",start_after_n_seconds);
			}
		else//show info_screen
			{
			gui_info_screen.SetActive(true);
			}
	}

	public void My_start()//call from Canvas > info_screen > button
	{
		gui_info_screen.SetActive(false);
		Check_ALL_possible_moves();
	}
	
	void Update()
	{
		if (lose_requirement_selected == lose_requirement.timer)
			Timer();
	}

	void Timer()//call from update
	{
		if (stage_started && (game_end != true) )
		{
			time_left = (timer+start_time+time_bonus)-Time.timeSinceLevelLoad;
			gui_timer_slider.value = time_left;
			if (time_left <= 0)
				{
				time_left = 0;
				Player_lose();
				}
		}
	}

	public void Add_time_bonus(float add_this)// call from Check_secondary_explosions(), tile_C.Check_if_shuffle_is_done(), tile_C.Check_if_gem_movements_are_all_done()
	{
		//Debug.Log("add_this = " + add_this + " *** time_left = " + time_left + " *** timer = " + timer);
		if ((time_left + add_this) > timer)
			{
			time_bonus += timer-time_left;
			}
		else
			{
			time_bonus += add_this;
			}

	}


	public void This_gem_color_is_collected(int this_color)//call from tile_C.Update_gems_score()
	{
		if (player_turn)
		{
			if (!player_this_gem_color_is_collected[this_color])
			{
				player_this_gem_color_is_collected[this_color] = true;
				gui_player_count.transform.GetChild(this_color).transform.GetChild(1).GetComponent<Text>().color = color_collect_done;
			}
		}
		else
		{
			if (!enemy_this_gem_color_is_collected[this_color])
				{
				enemy_this_gem_color_is_collected[this_color] = true;
				gui_enemy_count.transform.GetChild(this_color).transform.GetChild(1).GetComponent<Text>().color = color_collect_done;
				}
		}
	}

	public void Reset_board()//call from Canvas > Lose_screen > button and Canvas > Win_screen > button
		{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); 
		}


	//shuffle gems when no move available
	void Shuffle()//call from Check_ALL_possible_moves()
		{
		Debug.Log("shuffle");
		for (int y = 0; y < _Y_tiles; y++)
			{
			for (int x = 0; x < _X_tiles; x++)
				{
					if ((board_array_master[x,y,1]>=0) && (board_array_master[x,y,1]<9) //there is a gem
				    && (board_array_master[x,y,3] == 0) )//and without padlock
					{
						number_of_gems_to_mix++;
						board_array_master[x,y,1] = UnityEngine.Random.Range(0,gem_length);
						board_array_master[x,y,4] = 0; //reset bonus
						Avoid_triple_color_gem(x,y);
						//update gem
						tile_C tile_script = (tile_C)tiles_array[x,y].GetComponent("tile_C");
							tile_script.StartCoroutine(tile_script.Shuffle_update());
					}
				}
			}

		}
	

	#region 1- stage creation
	void Load_board()//call from awake
	{
		if (file_txt_asset)
		{
			fileContents = file_txt_asset.text;

	
		
			string[] parts = fileContents.Split(new string[] { "\r\n" }, StringSplitOptions.None);
		
			_X_tiles = Int16.Parse(parts[0]);
			_Y_tiles = Int16.Parse(parts[1]);

			
			board_array_master = new int[_X_tiles,_Y_tiles,board_array_master_length];
			tiles_array = new GameObject[_X_tiles,_Y_tiles];
			
			for (int y = 0; y < _Y_tiles+2; y++)
			{
				if (y > 1)
				{
					for (int x = 0; x < _X_tiles; x++)
					{
						string[] tile = parts[y].Split(new string[] { "|" }, StringSplitOptions.None);

						for (int z = 0; z < 5; z++)
						{
							string[] tile_characteristic = tile[x].Split(new string[] { "," }, StringSplitOptions.None);
							board_array_master[x,y-2,z] = Int16.Parse(tile_characteristic[z]);
						}
					}
					
				}
			}
		}
	else
		{
		Board_camera.backgroundColor = Color.red;
			Debug.LogError("Stage file is empty");
		}
	}

	void Create_new_board()//call from Awake()
	{
		//int leader_tiles_count = 0;

		if (show_token_after_all_tiles_are_destroyed && win_requirement_selected == win_requirement.take_all_tokens)
			token_place_card = new bool[_X_tiles,_Y_tiles];
		
		for (int y = 0; y < _Y_tiles; y++)
		{
			for (int x = 0; x < _X_tiles; x++)
			{

				if (board_array_master[x,y,0] > -1)//if there is a tile
				{
					Vector2 this_position = new Vector2(x+pivot_board.position.x,-y+pivot_board.position.y);

					//generate tile
					tiles_array[x,y] = (GameObject)Instantiate(tile_obj, this_position,Quaternion.identity);
					tiles_array[x,y].name = "x:"+x+",y:"+y;
					tiles_array[x,y].GetComponent<tile_C>().board = this.GetComponent<Board_C>();
					//tiles_array[x,y].GetComponent<tile_C>().explosion_score_script = the_gui_score_of_this_move.gameObject.GetComponent<explosion_score>();

					tiles_array[x,y].transform.parent = pivot_board;
					total_tiles++;
					//search leader tiles

					if ( (y == 0) || ( (y > 0) && (board_array_master[x,y-1,0] == -1) ) ) //if the tile is on the first row or don't have another tile over
						{
						board_array_master[x,y,12] = 1;//this is a leader tile
						//number_of_tiles_leader++;
						}
					//search bottom tiles for everi orientation:
						//orientation 0 = 0°
						if ( (y == _Y_tiles-1) || ( (y < _Y_tiles-1) && (board_array_master[x,y+1,0] == -1) ) )
							{
							number_of_bottom_tiles[0]++;
							}
						//orientation 1 = 90°
						if ( (x == _X_tiles-1) || ( (x < _X_tiles-1) && (board_array_master[x+1,1,0] == -1) ) )
							{
							number_of_bottom_tiles[1]++;
							}
						//orientation 2 = 180°
						if ( (y == 0) || ( (y > 0) && (board_array_master[x,y-1,0] == -1) ) ) //if the tile is on the first row or don't have another tile over
							{
							number_of_bottom_tiles[2]++;
							}
						//orientation 3 = 270°
						if ( (x == 0) || ( (x > 0) && (board_array_master[x-1,y,0] == -1) ) ) //if the tile is on the first row or don't have another tile over
							{
							number_of_bottom_tiles[3]++;
							}
						

					if (show_frame_board_decoration)
						{
						Transform temp_frame;
						if (y == 0) //make upper border
							{
							temp_frame = (Transform)Instantiate(frame_elements[0], this_position,Quaternion.identity);
								temp_frame.parent = frame_pivot;
							if (x == 0)
								{
								temp_frame = (Transform)Instantiate(frame_elements[11], this_position,Quaternion.identity);
								 	temp_frame.parent = frame_pivot;

								temp_frame = (Transform)Instantiate(frame_elements[3], this_position,Quaternion.identity); //left border
									temp_frame.parent = frame_pivot;
								}
							else if (x == _X_tiles -1)
								{
								temp_frame = (Transform)Instantiate(frame_elements[10], this_position,Quaternion.identity);
									temp_frame.parent = frame_pivot;
								temp_frame = (Transform)Instantiate(frame_elements[2], this_position,Quaternion.identity);
									temp_frame.parent = frame_pivot;
								}
							}
						else if (y == _Y_tiles -1)//make bottom border
							{
							temp_frame = (Transform)Instantiate(frame_elements[1], this_position,Quaternion.identity);
								temp_frame.parent = frame_pivot;
							if (x == 0)
								{
								temp_frame = (Transform)Instantiate(frame_elements[9], this_position,Quaternion.identity);
									temp_frame.parent = frame_pivot;
								temp_frame = (Transform)Instantiate(frame_elements[3], this_position,Quaternion.identity);
									temp_frame.parent = frame_pivot;
								}
							else if (x == _X_tiles -1)
								{
								temp_frame = (Transform)Instantiate(frame_elements[8], this_position,Quaternion.identity);
									temp_frame.parent = frame_pivot;
								temp_frame = (Transform)Instantiate(frame_elements[2], this_position,Quaternion.identity);
									temp_frame.parent = frame_pivot;
								}
							}
						else
							{
							if (x == 0) //make left border
								{
								temp_frame = (Transform)Instantiate(frame_elements[3], this_position,Quaternion.identity);
									temp_frame.parent = frame_pivot;
								}
							else if (x == _X_tiles -1)//right border
								{
								temp_frame = (Transform)Instantiate(frame_elements[2], this_position,Quaternion.identity); //left border
									temp_frame.parent = frame_pivot;
								}
							}
						}
				}
				else // no tile here
					{
					if (show_frame_board_decoration)
						{
						Transform temp_frame;
						Vector2 this_position = new Vector2(x+pivot_board.position.x,-y+pivot_board.position.y);

						if (y == 0) 
							{
							if (x == 0) 
								{
								if ((board_array_master[x+1,y,0] > -1)//tile R and down
									&& (board_array_master[x,y+1,0] > -1))
										{
										//corner in
										temp_frame = (Transform)Instantiate(frame_elements[4], this_position,Quaternion.identity);
											temp_frame.parent = frame_pivot;

										//corners out
										temp_frame = (Transform)Instantiate(frame_elements[11], this_position+Vector2.right,Quaternion.identity);
											temp_frame.parent = frame_pivot;
										temp_frame = (Transform)Instantiate(frame_elements[11], this_position+(-Vector2.up),Quaternion.identity);
											temp_frame.parent = frame_pivot;
										}
								else if (board_array_master[x+1,y,0] > -1)//tile R
										{
										temp_frame = (Transform)Instantiate(frame_elements[3], this_position+Vector2.right,Quaternion.identity);
											temp_frame.parent = frame_pivot;

										//corner out
										temp_frame = (Transform)Instantiate(frame_elements[11], this_position+Vector2.right,Quaternion.identity);
											temp_frame.parent = frame_pivot;
										}
								else if (board_array_master[x,y+1,0] > -1) //tile down
										{
										temp_frame = (Transform)Instantiate(frame_elements[0], this_position+(-Vector2.up),Quaternion.identity);
										temp_frame.parent = frame_pivot;

										temp_frame = (Transform)Instantiate(frame_elements[11], this_position+(-Vector2.up),Quaternion.identity);
											temp_frame.parent = frame_pivot;
										}
								else if (board_array_master[x+1,y+1,0] > -1)//only corner
										{
										temp_frame = (Transform)Instantiate(frame_elements[11], this_position+Vector2.right+(-Vector2.up),Quaternion.identity); //corner out 
											temp_frame.parent = frame_pivot;
										}
								}
							else if (x == _X_tiles -1)//top right corner
								{
								if ((board_array_master[x-1,y,0] > -1)
								    && (board_array_master[x,y+1,0] > -1)) 
									{
									//corne in
									temp_frame = (Transform)Instantiate(frame_elements[5], this_position,Quaternion.identity);
										temp_frame.parent = frame_pivot;

									//corners out
									temp_frame = (Transform)Instantiate(frame_elements[10], this_position-Vector2.right,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									temp_frame = (Transform)Instantiate(frame_elements[10], this_position+(-Vector2.up),Quaternion.identity);
									temp_frame.parent = frame_pivot;
									}
								else if (board_array_master[x-1,y,0] > -1)//tile L
									{
									temp_frame = (Transform)Instantiate(frame_elements[2], this_position-Vector2.right,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									
									//corner out
									temp_frame = (Transform)Instantiate(frame_elements[10], this_position-Vector2.right,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									}
								else if (board_array_master[x,y+1,0] > -1) //tile down
									{
									temp_frame = (Transform)Instantiate(frame_elements[0], this_position+(-Vector2.up),Quaternion.identity);
									temp_frame.parent = frame_pivot;
									
									temp_frame = (Transform)Instantiate(frame_elements[10], this_position+(-Vector2.up),Quaternion.identity);
									temp_frame.parent = frame_pivot;
									}
								else if (board_array_master[x-1,y+1,0] > -1)//only corner
									{
									temp_frame = (Transform)Instantiate(frame_elements[10], this_position-Vector2.right+(-Vector2.up),Quaternion.identity); //corner out 
									temp_frame.parent = frame_pivot;
									}

								}
							else //top line, middle 
								{
								if ((board_array_master[x+1,y,0] > -1)//tile R 
								    && (board_array_master[x-1,y,0] > -1) // L
								    && (board_array_master[x,y+1,0] > -1)) //down
								   	 	{
										//U shape
										temp_frame = (Transform)Instantiate(frame_elements[15], this_position,Quaternion.identity);
											temp_frame.parent = frame_pivot;

										//corners out
										temp_frame = (Transform)Instantiate(frame_elements[10], this_position-Vector2.right,Quaternion.identity);
										temp_frame.parent = frame_pivot;
										temp_frame = (Transform)Instantiate(frame_elements[11], this_position+Vector2.right,Quaternion.identity);
										temp_frame.parent = frame_pivot;
										}
								else if ((board_array_master[x+1,y,0] > -1)//tile R 
								            && (board_array_master[x-1,y,0] > -1)) // L
										{

										temp_frame = (Transform)Instantiate(frame_elements[2], this_position-Vector2.right,Quaternion.identity);
										temp_frame.parent = frame_pivot;

										temp_frame = (Transform)Instantiate(frame_elements[3], this_position+Vector2.right,Quaternion.identity);
										temp_frame.parent = frame_pivot;

										//corners out
										temp_frame = (Transform)Instantiate(frame_elements[10], this_position-Vector2.right,Quaternion.identity);
										temp_frame.parent = frame_pivot;
										temp_frame = (Transform)Instantiate(frame_elements[11], this_position+Vector2.right,Quaternion.identity);
										temp_frame.parent = frame_pivot;
										}
								else if ((board_array_master[x+1,y,0] > -1)//tile R 
								         && (board_array_master[x,y+1,0] > -1)) //down
										{
										temp_frame = (Transform)Instantiate(frame_elements[4], this_position,Quaternion.identity);
											temp_frame.parent = frame_pivot;

										//corners out
										temp_frame = (Transform)Instantiate(frame_elements[11], this_position+Vector2.right,Quaternion.identity);
										temp_frame.parent = frame_pivot;
										}
								else if ((board_array_master[x-1,y,0] > -1)//tile L 
								         && (board_array_master[x,y+1,0] > -1)) //down
									{
									temp_frame = (Transform)Instantiate(frame_elements[5], this_position,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									
									//corners out
									temp_frame = (Transform)Instantiate(frame_elements[10], this_position-Vector2.right,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									}
								else if (board_array_master[x,y+1,0] > -1) //down
									{
									temp_frame = (Transform)Instantiate(frame_elements[0], this_position-Vector2.up,Quaternion.identity);
										temp_frame.parent = frame_pivot;
									}
								else if (board_array_master[x-1,y,0] > -1)//tile L 
									{
									temp_frame = (Transform)Instantiate(frame_elements[2], this_position-Vector2.right,Quaternion.identity);
									temp_frame.parent = frame_pivot;

									//corners out
									temp_frame = (Transform)Instantiate(frame_elements[10], this_position-Vector2.right,Quaternion.identity);
									temp_frame.parent = frame_pivot;

									if (board_array_master[x+1,y+1,0] > -1)
										{
										temp_frame = (Transform)Instantiate(frame_elements[11], this_position+Vector2.right-Vector2.up,Quaternion.identity);
										temp_frame.parent = frame_pivot;
										}


									}
								else if (board_array_master[x+1,y,0] > -1)//tile R 
									{
									temp_frame = (Transform)Instantiate(frame_elements[3], this_position+Vector2.right,Quaternion.identity);
									temp_frame.parent = frame_pivot;

									//corners out
									temp_frame = (Transform)Instantiate(frame_elements[11], this_position+Vector2.right,Quaternion.identity);
									temp_frame.parent = frame_pivot;

									if (board_array_master[x-1,y+1,0] > -1)
										{
										temp_frame = (Transform)Instantiate(frame_elements[10], this_position-Vector2.right-Vector2.up,Quaternion.identity);
										temp_frame.parent = frame_pivot;
										}
									}
								//check corners
								else 
									{
									if (board_array_master[x+1,y+1,0] > -1)//only corner
										{
										temp_frame = (Transform)Instantiate(frame_elements[11], this_position+Vector2.right+(-Vector2.up),Quaternion.identity); //corner out 
										temp_frame.parent = frame_pivot;
										}
									if (board_array_master[x-1,y+1,0] > -1)//only corner
										{
										temp_frame = (Transform)Instantiate(frame_elements[10], this_position-Vector2.right+(-Vector2.up),Quaternion.identity); //corner out 
										temp_frame.parent = frame_pivot;
										}
									}
								}
							}
						else if (y == _Y_tiles -1)//last line
							{
							if (x == 0) 
								{
								if ((board_array_master[x+1,y,0] > -1)//tile R 
								    && (board_array_master[x,y-1,0] > -1))//up
									{
									//corner in
									temp_frame = (Transform)Instantiate(frame_elements[6], this_position,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									
									//corners out
									temp_frame = (Transform)Instantiate(frame_elements[9], this_position+Vector2.right,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									temp_frame = (Transform)Instantiate(frame_elements[9], this_position+Vector2.up,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									}
								else if (board_array_master[x+1,y,0] > -1)//tile R 
									{
									//corner in
									temp_frame = (Transform)Instantiate(frame_elements[3], this_position+Vector2.right,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									
									//corners out
									temp_frame = (Transform)Instantiate(frame_elements[9], this_position+Vector2.right,Quaternion.identity);
									temp_frame.parent = frame_pivot;

									}
								else if (board_array_master[x,y-1,0] > -1) //up
									{
									temp_frame = (Transform)Instantiate(frame_elements[1], this_position+Vector2.up,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									
									//corners out
									temp_frame = (Transform)Instantiate(frame_elements[9], this_position+Vector2.up,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									}
								else if (board_array_master[x+1,y-1,0] > -1)//only corner
									{
									temp_frame = (Transform)Instantiate(frame_elements[9], this_position+Vector2.right+Vector2.up,Quaternion.identity); //corner out 
									temp_frame.parent = frame_pivot;
									}
								}
							else if (x == _X_tiles -1)//bottom right corner
								{
								if ((board_array_master[x-1,y,0] > -1) //L
								    && (board_array_master[x,y-1,0] > -1)) //up
									{
									//corner in
									temp_frame = (Transform)Instantiate(frame_elements[7], this_position,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									
									//corners out
									temp_frame = (Transform)Instantiate(frame_elements[8], this_position-Vector2.right,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									temp_frame = (Transform)Instantiate(frame_elements[8], this_position+Vector2.up,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									}
								else if (board_array_master[x-1,y,0] > -1) //L
									{
									//corne in
									temp_frame = (Transform)Instantiate(frame_elements[2], this_position-Vector2.right,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									
									//corners out
									temp_frame = (Transform)Instantiate(frame_elements[8], this_position-Vector2.right,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									}
								else if (board_array_master[x,y-1,0] > -1) //up
									{
									temp_frame = (Transform)Instantiate(frame_elements[1], this_position+Vector2.up,Quaternion.identity);
									temp_frame.parent = frame_pivot;

									//corners out
									temp_frame = (Transform)Instantiate(frame_elements[8], this_position+Vector2.up,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									}
								else if (board_array_master[x-1,y-1,0] > -1)//only corner
									{
									temp_frame = (Transform)Instantiate(frame_elements[8], this_position-Vector2.right+Vector2.up,Quaternion.identity); //corner out 
									temp_frame.parent = frame_pivot;
									}
								}
							else //bottom middle line
								{
								if ((board_array_master[x+1,y,0] > -1)//tile R 
								    && (board_array_master[x-1,y,0] > -1) // L
								    && (board_array_master[x,y-1,0] > -1)) //up
									{
									//U shape
									temp_frame = (Transform)Instantiate(frame_elements[14], this_position,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									
									//corners out
									temp_frame = (Transform)Instantiate(frame_elements[8], this_position-Vector2.right,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									temp_frame = (Transform)Instantiate(frame_elements[9], this_position+Vector2.right,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									}
								else if ((board_array_master[x+1,y,0] > -1)//tile R 
								         && (board_array_master[x-1,y,0] > -1)) // L
									{
									
									temp_frame = (Transform)Instantiate(frame_elements[2], this_position-Vector2.right,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									
									temp_frame = (Transform)Instantiate(frame_elements[3], this_position+Vector2.right,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									
									//corners out
									temp_frame = (Transform)Instantiate(frame_elements[9], this_position+Vector2.right,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									temp_frame = (Transform)Instantiate(frame_elements[8], this_position-Vector2.right,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									}
								///
								else if ((board_array_master[x+1,y,0] > -1)//tile R 
								         && (board_array_master[x,y-1,0] > -1)) //up
									{
									temp_frame = (Transform)Instantiate(frame_elements[6], this_position,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									
									//corners out
									temp_frame = (Transform)Instantiate(frame_elements[9], this_position+Vector2.right,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									}
								else if ((board_array_master[x-1,y,0] > -1)//tile L 
								         && (board_array_master[x,y-1,0] > -1)) //up
									{
									temp_frame = (Transform)Instantiate(frame_elements[7], this_position,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									
									//corners out
									temp_frame = (Transform)Instantiate(frame_elements[8], this_position-Vector2.right,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									}
								else if (board_array_master[x,y-1,0] > -1) //down
									{
									temp_frame = (Transform)Instantiate(frame_elements[1], this_position+Vector2.up,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									}
								else if (board_array_master[x-1,y,0] > -1)//tile L 
									{
									temp_frame = (Transform)Instantiate(frame_elements[2], this_position-Vector2.right,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									
									//corners out
									temp_frame = (Transform)Instantiate(frame_elements[8], this_position-Vector2.right,Quaternion.identity);
									temp_frame.parent = frame_pivot;

									if (board_array_master[x+1,y-1,0] > -1)
										{
										temp_frame = (Transform)Instantiate(frame_elements[9], this_position+Vector2.right+Vector2.up,Quaternion.identity);
										temp_frame.parent = frame_pivot;
										}
									}
								else if (board_array_master[x+1,y,0] > -1)//tile R 
									{
									temp_frame = (Transform)Instantiate(frame_elements[3], this_position+Vector2.right,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									
									//corners out
									temp_frame = (Transform)Instantiate(frame_elements[9], this_position+Vector2.right,Quaternion.identity);
									temp_frame.parent = frame_pivot;

									if (board_array_master[x-1,y-1,0] > -1)
										{
										temp_frame = (Transform)Instantiate(frame_elements[8], this_position-Vector2.right+Vector2.up,Quaternion.identity);
										temp_frame.parent = frame_pivot;
										}
									}
								else //check corners
									{
									if (board_array_master[x+1,y-1,0] > -1)//only corner
										{
										temp_frame = (Transform)Instantiate(frame_elements[9], this_position+Vector2.right+Vector2.up,Quaternion.identity); //corner out 
										temp_frame.parent = frame_pivot;
										}
									if (board_array_master[x-1,y-1,0] > -1)//only corner
										{
										temp_frame = (Transform)Instantiate(frame_elements[8], this_position-Vector2.right+Vector2.up,Quaternion.identity); //corner out 
										temp_frame.parent = frame_pivot;
										}
									}
								}

							}
						else//middle lines
							{
							if (x == 0)//first column
								{
								if ((board_array_master[x+1,y,0] > -1)//tile R 
									&& (board_array_master[x,y-1,0] > -1) //up
									&& (board_array_master[x,y+1,0] > -1)) //down
										{
										//U shape
										temp_frame = (Transform)Instantiate(frame_elements[12], this_position,Quaternion.identity);
										temp_frame.parent = frame_pivot;

										//corners out
										temp_frame = (Transform)Instantiate(frame_elements[9], this_position+Vector2.up,Quaternion.identity);
										temp_frame.parent = frame_pivot;
										temp_frame = (Transform)Instantiate(frame_elements[11], this_position-Vector2.up,Quaternion.identity);
										temp_frame.parent = frame_pivot;
										}
								else if ((board_array_master[x,y-1,0] > -1) //up
								         && (board_array_master[x,y+1,0] > -1)) //down
									{
									//up
									temp_frame = (Transform)Instantiate(frame_elements[1], this_position+Vector2.up,Quaternion.identity);
									temp_frame.parent = frame_pivot;

									//down
									temp_frame = (Transform)Instantiate(frame_elements[0], this_position-Vector2.up,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									
									//corners out
									temp_frame = (Transform)Instantiate(frame_elements[9], this_position+Vector2.up,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									temp_frame = (Transform)Instantiate(frame_elements[11], this_position-Vector2.up,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									}
								else if ((board_array_master[x+1,y,0] > -1)//tile R 
								         && (board_array_master[x,y-1,0] > -1)) //up
									{
									//corner in
									temp_frame = (Transform)Instantiate(frame_elements[6], this_position,Quaternion.identity);
									temp_frame.parent = frame_pivot;

									//corners out
									temp_frame = (Transform)Instantiate(frame_elements[9], this_position+Vector2.up,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									}
								else if ((board_array_master[x+1,y,0] > -1)//tile R 
								         && (board_array_master[x,y+1,0] > -1)) //down
									{
									//corner in
									temp_frame = (Transform)Instantiate(frame_elements[4], this_position,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									
									//corners out
									temp_frame = (Transform)Instantiate(frame_elements[11], this_position-Vector2.up,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									}
								else if (board_array_master[x+1,y,0] > -1)//tile R 
								    {
									//corner in
									temp_frame = (Transform)Instantiate(frame_elements[3], this_position+Vector2.right,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									}
								else if (board_array_master[x,y+1,0] > -1) //down
									{
									//down
									temp_frame = (Transform)Instantiate(frame_elements[0], this_position-Vector2.up,Quaternion.identity);
									temp_frame.parent = frame_pivot;

									//corner
									temp_frame = (Transform)Instantiate(frame_elements[11], this_position+(-Vector2.up),Quaternion.identity); //corner out 
									temp_frame.parent = frame_pivot;

									if (board_array_master[x+1,y-1,0] > -1)//only corner
										{
										temp_frame = (Transform)Instantiate(frame_elements[9], this_position+Vector2.right+Vector2.up,Quaternion.identity); //corner out 
										temp_frame.parent = frame_pivot;
										}
									}
								else if (board_array_master[x,y-1,0] > -1)//up
									{
									//up
									temp_frame = (Transform)Instantiate(frame_elements[1], this_position+Vector2.up,Quaternion.identity);
									temp_frame.parent = frame_pivot;

									//corner
									temp_frame = (Transform)Instantiate(frame_elements[9], this_position+Vector2.up,Quaternion.identity); //corner out 
									temp_frame.parent = frame_pivot;

									if (board_array_master[x+1,y+1,0] > -1)//only corner
										{
										temp_frame = (Transform)Instantiate(frame_elements[11], this_position+Vector2.right+(-Vector2.up),Quaternion.identity); //corner out 
										temp_frame.parent = frame_pivot;
										}
									}
								else //check corners
									{
									if (board_array_master[x+1,y+1,0] > -1)//only corner
										{
										temp_frame = (Transform)Instantiate(frame_elements[11], this_position+Vector2.right+(-Vector2.up),Quaternion.identity); //corner out 
										temp_frame.parent = frame_pivot;
										}
									if (board_array_master[x+1,y-1,0] > -1)//only corner
										{
										temp_frame = (Transform)Instantiate(frame_elements[9], this_position+Vector2.right+Vector2.up,Quaternion.identity); //corner out 
										temp_frame.parent = frame_pivot;
										}
									}
								}
							else if (x == _X_tiles -1)//last column
								{
								if ((board_array_master[x-1,y,0] > -1)//tile L 
								    && (board_array_master[x,y-1,0] > -1) //up
								    && (board_array_master[x,y+1,0] > -1)) //down
									{
									//U shape
									temp_frame = (Transform)Instantiate(frame_elements[13], this_position,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									
									//corners out
									temp_frame = (Transform)Instantiate(frame_elements[10], this_position-Vector2.up,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									temp_frame = (Transform)Instantiate(frame_elements[8], this_position+Vector2.up,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									}
								else if ((board_array_master[x,y-1,0] > -1) //up
								         && (board_array_master[x,y+1,0] > -1)) //down
									{
									//up
									temp_frame = (Transform)Instantiate(frame_elements[1], this_position+Vector2.up,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									
									//down
									temp_frame = (Transform)Instantiate(frame_elements[0], this_position-Vector2.up,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									
									//corners out
									temp_frame = (Transform)Instantiate(frame_elements[10], this_position-Vector2.up,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									temp_frame = (Transform)Instantiate(frame_elements[8], this_position+Vector2.up,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									}
								else if ((board_array_master[x-1,y,0] > -1)//tile L 
								         && (board_array_master[x,y-1,0] > -1)) //up
									{
									//corner in
									temp_frame = (Transform)Instantiate(frame_elements[7], this_position,Quaternion.identity);
									temp_frame.parent = frame_pivot;

									//corners out
									temp_frame = (Transform)Instantiate(frame_elements[8], this_position+Vector2.up,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									}
								else if ((board_array_master[x-1,y,0] > -1)//tile L 
								         && (board_array_master[x,y+1,0] > -1)) //down
									{
									//corner in
									temp_frame = (Transform)Instantiate(frame_elements[5], this_position,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									
									//corners out
									temp_frame = (Transform)Instantiate(frame_elements[10], this_position-Vector2.up,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									}
								else if (board_array_master[x-1,y,0] > -1)//tile L 
									{
									temp_frame = (Transform)Instantiate(frame_elements[2], this_position-Vector2.right,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									}
								else if (board_array_master[x,y-1,0] > -1) //up
									{
									Debug.Log(x+","+y);
									//up
									temp_frame = (Transform)Instantiate(frame_elements[1], this_position+Vector2.up,Quaternion.identity);
									temp_frame.parent = frame_pivot;

									//corner
									temp_frame = (Transform)Instantiate(frame_elements[8], this_position+Vector2.up,Quaternion.identity);
									temp_frame.parent = frame_pivot;

									if (board_array_master[x-1,y+1,0] > -1)//only corner
										{
										temp_frame = (Transform)Instantiate(frame_elements[10], this_position-Vector2.right+(-Vector2.up),Quaternion.identity); //corner out 
										temp_frame.parent = frame_pivot;
										}
									}
								else if (board_array_master[x,y+1,0] > -1) //down
									{
									//down
									temp_frame = (Transform)Instantiate(frame_elements[0], this_position-Vector2.up,Quaternion.identity);
									temp_frame.parent = frame_pivot;

									//corner
									temp_frame = (Transform)Instantiate(frame_elements[10], this_position-Vector2.up,Quaternion.identity);
									temp_frame.parent = frame_pivot;

									if (board_array_master[x-1,y-1,0] > -1)//only corner
										{
										temp_frame = (Transform)Instantiate(frame_elements[8], this_position-Vector2.right+Vector2.up,Quaternion.identity); //corner out 
										temp_frame.parent = frame_pivot;
										}
									}
								else //check corners
									{
									if (board_array_master[x-1,y-1,0] > -1)//only corner
										{
										temp_frame = (Transform)Instantiate(frame_elements[8], this_position-Vector2.right+Vector2.up,Quaternion.identity); //corner out 
										temp_frame.parent = frame_pivot;
										}
									if (board_array_master[x-1,y+1,0] > -1)//only corner
										{
										temp_frame = (Transform)Instantiate(frame_elements[10], this_position-Vector2.right+(-Vector2.up),Quaternion.identity); //corner out 
										temp_frame.parent = frame_pivot;
										}
									}
								}
							else //middle columns
								{
								if ((board_array_master[x+1,y,0] > -1)//tile R 
								    && (board_array_master[x-1,y,0] > -1)//tile L
								    && (board_array_master[x,y-1,0] > -1) //up
								    && (board_array_master[x,y+1,0] > -1)) //down
									{
									temp_frame = (Transform)Instantiate(frame_elements[16], this_position,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									}
								//vertical
								else if ((board_array_master[x+1,y,0] > -1)//tile R 
								    && (board_array_master[x-1,y,0] > -1)//tile L
								    && (board_array_master[x,y-1,0] > -1)) //up
									{
									temp_frame = (Transform)Instantiate(frame_elements[14], this_position,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									}
								else if ((board_array_master[x+1,y,0] > -1)//tile R 
								    && (board_array_master[x-1,y,0] > -1)//tile L
								    && (board_array_master[x,y+1,0] > -1)) //down
									{
									temp_frame = (Transform)Instantiate(frame_elements[15], this_position,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									}
								else if ((board_array_master[x+1,y,0] > -1)//tile R 
								         && (board_array_master[x-1,y,0] > -1))//tile L
									{
									temp_frame = (Transform)Instantiate(frame_elements[2], this_position-Vector2.right,Quaternion.identity);
									temp_frame.parent = frame_pivot;

									temp_frame = (Transform)Instantiate(frame_elements[3], this_position+Vector2.right,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									}
								//horizontal
								else if ((board_array_master[x+1,y,0] > -1)//tile R 
								    && (board_array_master[x,y-1,0] > -1) //up
								    && (board_array_master[x,y+1,0] > -1)) //down
									{
									temp_frame = (Transform)Instantiate(frame_elements[12], this_position,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									}
								else if ((board_array_master[x-1,y,0] > -1)//tile L
								    && (board_array_master[x,y-1,0] > -1) //up
								    && (board_array_master[x,y+1,0] > -1)) //down
									{
									temp_frame = (Transform)Instantiate(frame_elements[13], this_position,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									}
								else if ((board_array_master[x,y-1,0] > -1) //up
								         && (board_array_master[x,y+1,0] > -1)) //down
									{
									temp_frame = (Transform)Instantiate(frame_elements[0], this_position-Vector2.up,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									
									temp_frame = (Transform)Instantiate(frame_elements[1], this_position+Vector2.up,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									}
								//L shape corners
								else if ((board_array_master[x+1,y,0] > -1)//tile R 
								         && (board_array_master[x,y-1,0] > -1)) //up

									{
									//corner in
									temp_frame = (Transform)Instantiate(frame_elements[6], this_position,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									if (board_array_master[x-1,y+1,0] > -1)//corner out
										{
										temp_frame = (Transform)Instantiate(frame_elements[10], this_position-Vector2.up-Vector2.right,Quaternion.identity);
										temp_frame.parent = frame_pivot;
										}
									}
								else if ((board_array_master[x+1,y,0] > -1)//tile R 
								         && (board_array_master[x,y+1,0] > -1)) //down
									
									{
									//corner in
									temp_frame = (Transform)Instantiate(frame_elements[4], this_position,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									//corner out
									if (board_array_master[x-1,y-1,0] > -1)//corner out
										{
										temp_frame = (Transform)Instantiate(frame_elements[8], this_position+Vector2.up-Vector2.right,Quaternion.identity);
										temp_frame.parent = frame_pivot;
										}
									}
								else if ((board_array_master[x-1,y,0] > -1)//tile L
								         && (board_array_master[x,y+1,0] > -1)) //down
									
									{
									//corner in
									temp_frame = (Transform)Instantiate(frame_elements[5], this_position,Quaternion.identity);
									temp_frame.parent = frame_pivot;
									if (board_array_master[x+1,y-1,0] > -1)//corner out
										{
										temp_frame = (Transform)Instantiate(frame_elements[9], this_position+Vector2.up+Vector2.right,Quaternion.identity);
										temp_frame.parent = frame_pivot;
										}
									}
								else if ((board_array_master[x-1,y,0] > -1)//tile L
								         && (board_array_master[x,y-1,0] > -1)) //up
									
									{
									//corner in
									temp_frame = (Transform)Instantiate(frame_elements[7], this_position,Quaternion.identity);
									temp_frame.parent = frame_pivot;

									if (board_array_master[x+1,y+1,0] > -1)//corner out
									    {
										temp_frame = (Transform)Instantiate(frame_elements[11], this_position-Vector2.up+Vector2.right,Quaternion.identity);
										temp_frame.parent = frame_pivot;
										}
									}
								//T shape
								else if (board_array_master[x,y-1,0] > -1) //up
									{
									//side
									temp_frame = (Transform)Instantiate(frame_elements[1], this_position+Vector2.up,Quaternion.identity);
									temp_frame.parent = frame_pivot;

									if (board_array_master[x+1,y+1,0] > -1)//corner out
										{
										temp_frame = (Transform)Instantiate(frame_elements[11], this_position-Vector2.up+Vector2.right,Quaternion.identity);
										temp_frame.parent = frame_pivot;
										}

									if (board_array_master[x-1,y+1,0] > -1)//corner out
										{
										temp_frame = (Transform)Instantiate(frame_elements[10], this_position-Vector2.up-Vector2.right,Quaternion.identity);
										temp_frame.parent = frame_pivot;
										}
									}
								else if (board_array_master[x,y+1,0] > -1) //down
									{
									//side
									temp_frame = (Transform)Instantiate(frame_elements[0], this_position-Vector2.up,Quaternion.identity);
									temp_frame.parent = frame_pivot;

									if (board_array_master[x+1,y-1,0] > -1)//corner out
										{
										temp_frame = (Transform)Instantiate(frame_elements[9], this_position+Vector2.up+Vector2.right,Quaternion.identity);
										temp_frame.parent = frame_pivot;
										}
									if (board_array_master[x-1,y-1,0] > -1)//corner out
										{
										temp_frame = (Transform)Instantiate(frame_elements[8], this_position+Vector2.up-Vector2.right,Quaternion.identity);
										temp_frame.parent = frame_pivot;
										}
									}
								else if (board_array_master[x+1,y,0] > -1)//tile R
									{
									//side
									temp_frame = (Transform)Instantiate(frame_elements[3], this_position+Vector2.right,Quaternion.identity);
									temp_frame.parent = frame_pivot;

									if (board_array_master[x-1,y-1,0] > -1)//corner out
										{
										temp_frame = (Transform)Instantiate(frame_elements[8], this_position+Vector2.up-Vector2.right,Quaternion.identity);
										temp_frame.parent = frame_pivot;
										}
									if (board_array_master[x-1,y+1,0] > -1)//corner out
										{
										temp_frame = (Transform)Instantiate(frame_elements[10], this_position-Vector2.up-Vector2.right,Quaternion.identity);
										temp_frame.parent = frame_pivot;
										}
									}
								else if (board_array_master[x-1,y,0] > -1)//tile L
									{
									//side
									temp_frame = (Transform)Instantiate(frame_elements[2], this_position-Vector2.right,Quaternion.identity);
									temp_frame.parent = frame_pivot;

									if (board_array_master[x+1,y+1,0] > -1)//corner out
										{
										temp_frame = (Transform)Instantiate(frame_elements[11], this_position-Vector2.up+Vector2.right,Quaternion.identity);
										temp_frame.parent = frame_pivot;
										}
									if (board_array_master[x+1,y-1,0] > -1)//corner out
										{
										temp_frame = (Transform)Instantiate(frame_elements[9], this_position+Vector2.up+Vector2.right,Quaternion.identity);
										temp_frame.parent = frame_pivot;
										}
									}
								else //check only corners
									{
									if (board_array_master[x+1,y+1,0] > -1)//corner out
										{
										temp_frame = (Transform)Instantiate(frame_elements[11], this_position-Vector2.up+Vector2.right,Quaternion.identity);
										temp_frame.parent = frame_pivot;
										}
									if (board_array_master[x+1,y-1,0] > -1)//corner out
										{
										temp_frame = (Transform)Instantiate(frame_elements[9], this_position+Vector2.up+Vector2.right,Quaternion.identity);
										temp_frame.parent = frame_pivot;
										}
									if (board_array_master[x-1,y+1,0] > -1)//corner out
										{
										temp_frame = (Transform)Instantiate(frame_elements[10], this_position-Vector2.up-Vector2.right,Quaternion.identity);
										temp_frame.parent = frame_pivot;
										}
									if (board_array_master[x-1,y-1,0] > -1)//corner out
										{
										temp_frame = (Transform)Instantiate(frame_elements[8], this_position+Vector2.up-Vector2.right,Quaternion.identity);
										temp_frame.parent = frame_pivot;
										}
									}

								}
							}
						}
					}

				if ((board_array_master[x,y,4]==-200) && (show_token_after_all_tiles_are_destroyed))//token
					{
					//take note of the token position and create a normal gem here
					number_of_token_to_collect++;
					token_place_card[x,y]=true;
					board_array_master[x,y,4] = 0;
					board_array_master[x,y,1] = 10;
					}

				if (board_array_master[x,y,1] == 10)//if there is a random gem here
					{
					board_array_master[x,y,1] = UnityEngine.Random.Range(0,gem_length);
					}


				if (board_array_master[x,y,1] >= 0)//if there is something in this tile
					{
					//this thing can fall?
					if (board_array_master[x,y,3] == 0) // no restraint
						{
						if ( (board_array_master[x,y,1] < 40) //is a gem, junk or token
						    || ( (board_array_master[x,y,1] >= 51) && (board_array_master[x,y,1] <= 59) ) // is a falling block
						    || ( (board_array_master[x,y,1] >= 70) && (board_array_master[x,y,1] <= 79) ) ) // is a key

							{
							board_array_master[x,y,10] = 1;
							//if is junk or token: explode when reach the bottom of the board
							if ( (board_array_master[x,y,1] >= 20) && (board_array_master[x,y,1] <= 39) )
								board_array_master[x,y,10] = 2;
							}
						}
					}
			}
		}
		
		//tiles_leader_array = new GameObject[number_of_tiles_leader];//the tiles in this array will create the falling gems

		//feed bottom tiles array:
			//search most long bottom tile list:
			int temp_bottom_tiles_array_lenght = 0;
			for (int i = 0; i < 4; i++)
				{
				if (number_of_bottom_tiles[i] > temp_bottom_tiles_array_lenght)
					temp_bottom_tiles_array_lenght = number_of_bottom_tiles[i];
				}
			bottom_tiles_array = new tile_C[4,temp_bottom_tiles_array_lenght];
			int[] temp_bottom_tiles_array_count = new int[4];

		for (int y = 0; y < _Y_tiles; y++)
		{
			for (int x = 0; x < _X_tiles; x++)
			{
				if (board_array_master[x,y,0] > -1)//if there is a tile
				{
					Vector2 this_position = new Vector2(x+pivot_board.position.x,-y+pivot_board.position.y);

					//and is leader...
					//if (board_array_master[x,y,12] == 1)
					//{
						//tiles_leader_array[leader_tiles_count] = tiles_array[x,y];
						//leader_tiles_count++;
					//}

					Avoid_triple_color_gem(x,y);

					//create visual representation 
					//the tile
						tile_C tile_script = (tile_C)tiles_array[x,y].GetComponent("tile_C");
						tile_script._x = x;
						tile_script._y = y;

						//orientation 0 = 0°
						if ( (y == _Y_tiles-1) || ( (y < _Y_tiles-1) && (board_array_master[x,y+1,0] == -1) ) )
							{
							bottom_tiles_array[0,temp_bottom_tiles_array_count[0]] = tile_script;
							temp_bottom_tiles_array_count[0]++;
							}
						//orientation 1 = 90°
						if ( (x == _X_tiles-1) || ( (x < _X_tiles-1) && (board_array_master[x+1,1,0] == -1) ) )
							{
							bottom_tiles_array[1,temp_bottom_tiles_array_count[1]] = tile_script;
							temp_bottom_tiles_array_count[1]++;
							}
						//orientation 2 = 180°
						if ( (y == 0) || ( (y > 0) && (board_array_master[x,y-1,0] == -1) ) ) 
							{
							bottom_tiles_array[2,temp_bottom_tiles_array_count[2]] = tile_script;
							temp_bottom_tiles_array_count[2]++;
							}
						//orientation 3 = 270°
						if ( (x == 0) || ( (x > 0) && (board_array_master[x-1,y,0] == -1) ) ) 
							{
							bottom_tiles_array[3,temp_bottom_tiles_array_count[3]] = tile_script;
							temp_bottom_tiles_array_count[3]++;
							}
						
						SpriteRenderer sprite_hp = tile_script.GetComponent<SpriteRenderer>();
						if (show_chess_board_decoration)
							{
							if (y%2==0)
								{
								if (x%2==0)
									sprite_hp.sprite = tile_hp[0];
								else
									sprite_hp.sprite = tile_hp[1];
								}
							else
								{
								if (x%2==0)
									sprite_hp.sprite = tile_hp[1];
								else
									sprite_hp.sprite = tile_hp[0];
								}
							}
						else
							sprite_hp.sprite = tile_hp[board_array_master[x,y,0]];

						if ( (board_array_master[x,y,0] == 0) && (board_array_master[x,y,2] > 0) ) //if this is a special tile and is visible
							{
							if ( (board_array_master[x,y,2] >=1 ) && (board_array_master[x,y,2] <= 9 ) )
								sprite_hp.sprite = start_goal_path[board_array_master[x,y,2]-1];
							else if ( (board_array_master[x,y,2] >=10 ) && (board_array_master[x,y,2] <= 19 ) )
								sprite_hp.sprite = door_color[board_array_master[x,y,2]-10];

							}
						//update hp board
						HP_board += board_array_master[x,y,0];
						
						
						//create gem
						if ( (board_array_master[x,y,1]>=0) && (board_array_master[x,y,1]<9) )//if here go a gem
						{
							//I can put the gem on the board
							tile_script.my_gem = (GameObject)Instantiate(tile_content, this_position,Quaternion.identity);//"-y" in order to have 0,0 in the up-left corner
							tile_script.my_gem.transform.parent = pivot_board.transform;

							tile_script.my_gem.name = "gem"+board_array_master[x,y,1].ToString();
							//gem color:
							SpriteRenderer sprite_gem = tile_script.my_gem.GetComponent<SpriteRenderer>();
								sprite_gem.sprite = gem_colors[board_array_master[x,y,1]];

							//create padlock or ice
							if (board_array_master[x,y,3]>0)
							{
								tile_script.my_padlock = (GameObject)Instantiate(over_gem, this_position,Quaternion.identity);
								tile_script.my_padlock.transform.parent = pivot_board;

								if (board_array_master[x,y,3]<11)
									{
									padlock_count++;
									tile_script.my_padlock.name = "padlock";
									SpriteRenderer sprite_lock = tile_script.my_padlock.GetComponent<SpriteRenderer>();
										sprite_lock.sprite = lock_gem_hp[board_array_master[x,y,3]-1];
									}
								else
									{
									tile_script.my_padlock.name = "ice";
									SpriteRenderer sprite_lock = tile_script.my_padlock.GetComponent<SpriteRenderer>();
										sprite_lock.sprite = ice_hp[board_array_master[x,y,3]-11];
									}
							}


						}
						else //there is somethin that not is a gem
						{
							//auxiliary gem in garbage that will be use when this tile will be free
							((GameObject)Instantiate(tile_content, new Vector2(x,-y),Quaternion.identity)).transform.parent = garbage_recycle;

							if (board_array_master[x,y,1]==9) //this is a special content
								{
								tile_script.my_gem = (GameObject)Instantiate(tile_content, this_position,Quaternion.identity);//"-y" in order to have 0,0 in the up-left corner
								tile_script.my_gem.transform.parent = pivot_board;
								if (board_array_master[x,y,4]==-100)//junk
									{
									tile_script.my_gem.name = "junk";
									number_of_junk_on_board++;
									SpriteRenderer sprite_gem = tile_script.my_gem.GetComponent<SpriteRenderer>();
									sprite_gem.sprite = junk;
									}
								else if (board_array_master[x,y,4]==-200)//token
									{
									number_of_token_on_board++;

										tile_script.my_gem.name = "token";
										SpriteRenderer sprite_gem = tile_script.my_gem.GetComponent<SpriteRenderer>();
										sprite_gem.sprite = token;
										
									}
								else if (board_array_master[x,y,4]>0)//bonus
									{
									number_of_bonus_on_board++;
									tile_script.my_gem.name = "bonus";
									SpriteRenderer sprite_gem = tile_script.my_gem.GetComponent<SpriteRenderer>();
									sprite_gem.sprite = on_board_bonus_sprites[board_array_master[x,y,4]];
									}
								}

							else if (board_array_master[x,y,1] == 40)//immune block
								{
								tile_script.my_gem = (GameObject)Instantiate(tile_content, this_position,Quaternion.identity);//"-y" in order to have 0,0 in the up-left corner
								tile_script.my_gem.transform.parent = pivot_board;
								tile_script.my_gem.name = "immune_block";
								
								SpriteRenderer sprite_gem = tile_script.my_gem.GetComponent<SpriteRenderer>();
								sprite_gem.sprite = immune_block;
								}
							else if ( (board_array_master[x,y,1] > 40) && (board_array_master[x,y,1] < 50) )//block
								{
								board_array_master[x,y,14] = (board_array_master[x,y,1]-40);

								tile_script.my_gem = (GameObject)Instantiate(tile_content, this_position,Quaternion.identity);//"-y" in order to have 0,0 in the up-left corner
								tile_script.my_gem.transform.parent = pivot_board;
								tile_script.my_gem.name = "block";
								block_count++;
								
								SpriteRenderer sprite_gem = tile_script.my_gem.GetComponent<SpriteRenderer>();
									sprite_gem.sprite = block_hp[board_array_master[x,y,1]-41];
								}
							else if ( (board_array_master[x,y,1] > 50) && (board_array_master[x,y,1] < 60) )// falling block
								{
								board_array_master[x,y,14] = (board_array_master[x,y,1]-50);

								tile_script.my_gem = (GameObject)Instantiate(tile_content, this_position,Quaternion.identity);//"-y" in order to have 0,0 in the up-left corner
								tile_script.my_gem.transform.parent = pivot_board;
								tile_script.my_gem.name = "falling_block";
								block_count++;
								
								SpriteRenderer sprite_gem = tile_script.my_gem.GetComponent<SpriteRenderer>();
									sprite_gem.sprite = falling_block_hp[board_array_master[x,y,1]-51];
								}
							else if ( (board_array_master[x,y,1] >= 60) && (board_array_master[x,y,1] < 70) )// need
								{
								tile_script.my_gem = (GameObject)Instantiate(tile_content, this_position,Quaternion.identity);//"-y" in order to have 0,0 in the up-left corner
								tile_script.my_gem.transform.parent = pivot_board;
								tile_script.my_gem.name = "need";
								
								SpriteRenderer sprite_gem = tile_script.my_gem.GetComponent<SpriteRenderer>();
								sprite_gem.sprite = need_color[board_array_master[x,y,1]-60];
								}
							else if ( (board_array_master[x,y,1] >= 70) && (board_array_master[x,y,1] < 80) )// key
								{
								tile_script.my_gem = (GameObject)Instantiate(tile_content, this_position,Quaternion.identity);//"-y" in order to have 0,0 in the up-left corner
								tile_script.my_gem.transform.parent = pivot_board;
								tile_script.my_gem.name = "key";
								
								SpriteRenderer sprite_gem = tile_script.my_gem.GetComponent<SpriteRenderer>();
								sprite_gem.sprite = key_color[board_array_master[x,y,1]-70];
								}


							else if ( (board_array_master[x,y,2] >=20 ) && (board_array_master[x,y,2] <= 29 ) )//item
								{
								tile_script.my_gem = (GameObject)Instantiate(tile_content, this_position,Quaternion.identity);//"-y" in order to have 0,0 in the up-left corner
								tile_script.my_gem.transform.parent = pivot_board;
								tile_script.my_gem.name = "item"+board_array_master[x,y,2].ToString();

								SpriteRenderer sprite_gem = tile_script.my_gem.GetComponent<SpriteRenderer>();
								sprite_gem.sprite = item_color[board_array_master[x,y,2]-20];

								}
						}
				}
				else //there are no tile here
					board_array_master[x,y,1] = -99;//no color here
			}
			
		}

		if (win_requirement_selected == win_requirement.take_all_tokens)
			{
			if (number_of_token_on_board > 0)
				number_of_token_to_collect = number_of_token_on_board;
			else
				{
				if (show_token_after_all_tiles_are_destroyed && (HP_board == 0) )
					{
					Show_all_token_on_board();
					}
				}
			if (number_of_token_to_collect > 0)
				Update_token_count();
			else
				Debug.LogWarning("win condition is 'Take_all_tolens' but this stage file don't have token!");
			}

		if ((number_of_bonus_on_board > 0) && (trigger_by_select == trigger_by.OFF))
			{
			Debug.LogWarning("This stage file have on board bonus, but you don't have setup any rule to trigger it. So, by default, these bonus will be trigger on click");
			trigger_by_select = trigger_by.click;
			}

		Debug.Log("Board created. HP board = " + HP_board);
		gui_board_hp_slider.maxValue = HP_board;
		Search_max_bonus_values_for_charge_bonus();
	}


	public void Show_all_token_on_board()//call from Create_new_board(), tile_C.Update_tile_hp()
		{
		if (!token_showed)
			{
			token_showed = true;

			for (int y = 0; y < _Y_tiles; y++)
				{
				for (int x = 0; x < _X_tiles; x++)
					{
					if (token_place_card[x,y])
						{
						board_array_master[x,y,1] = 9;
						board_array_master[x,y,4] = -200;
						tile_C tile_script = (tile_C)tiles_array[x,y].GetComponent("tile_C");
						tile_script.StartCoroutine(tile_script.Show_token());
						}
					}
				}
			}
		}

	void Avoid_triple_color_gem(int x, int y)//call from Shuffle(), Create_new_board()
	{
		int attempt_count = 0;
		if ( (board_array_master[x,y,1] >= 0) &&  (board_array_master[x,y,1] < 9) )//if this is a gem
		{
			if ( ((x+1<_X_tiles)&&(x-1>=0)) && ((y+1<_Y_tiles)&&(y-1>=0)) )
			{
				attempt_count = 0;
				while ( (attempt_count<=gem_length) 
				       && ( (board_array_master[x,y,1]==board_array_master[x+1,y,1])&&(board_array_master[x,y,1]==board_array_master[x-1,y,1]) 
				    || (board_array_master[x,y,1]==board_array_master[x,y+1,1])&&(board_array_master[x,y,1]==board_array_master[x,y-1,1])) )
				{
					//vertical check
					if (board_array_master[x,y,1]+1<gem_length)
						board_array_master[x,y,1]++;
					else
						board_array_master[x,y,1]=0;
					
					attempt_count++;
				}
			}
			else if ( ((x+1<_X_tiles)&&(x-1>=0)) )
			{
				attempt_count = 0;
				while ( (attempt_count<=gem_length) 
				       && ( (board_array_master[x,y,1]==board_array_master[x+1,y,1])&&(board_array_master[x,y,1]==board_array_master[x-1,y,1]) ) )
				{
					//horizontal check
					if (board_array_master[x,y,1]+1<gem_length)
						board_array_master[x,y,1]++;
					else
						board_array_master[x,y,1]=0;
					
					attempt_count++;
				}
			}
			else if ( ((y+1<_Y_tiles)&&(y-1>=0)) )
			{
				attempt_count = 0;
				while ( (attempt_count<=gem_length) 
				       && ((board_array_master[x,y,1]==board_array_master[x,y+1,1])&&(board_array_master[x,y,1]==board_array_master[x,y-1,1])) ) 
				{
					//Debug.Log("while verticale");
					if (board_array_master[x,y,1]+1<gem_length)
						board_array_master[x,y,1]++;
					else
						board_array_master[x,y,1]=0;
					
					attempt_count++;
				}
			}
			if ((x-2>=0)&&(x-1>=0))
				if ((board_array_master[x,y,1]==board_array_master[x-1,y,1])&&(board_array_master[x,y,1]==board_array_master[x-2,y,1]))
			{
				if (board_array_master[x,y,1]+1<gem_length)
					board_array_master[x,y,1]++;
				else
					board_array_master[x,y,1]=0;
			}
			if ((y-2>=0)&&(y-1>=0))
				if ((board_array_master[x,y,1]==board_array_master[x,y-1,1])&&(board_array_master[x,y,1]==board_array_master[x,y-2,1]))
			{
				if (board_array_master[x,y,1]+1<gem_length)
					board_array_master[x,y,1]++;
				else
					board_array_master[x,y,1]=0;
			}
		}
	}

	#endregion
	
	#region 2- moves analysis
	public void Check_ALL_possible_moves() //call from: board_C.Start(),board_C.My_Start(),board_C.Start_gem_fall(),board_C.Check_secondary_explosions(), tile_C.Check_if_shuffle_is_done()
		{
		//Debug.LogWarning("Check_ALL_possible_moves()");
		//reset count
		number_of_moves_possible = 0;
		clickable_bonus_on_boad = false;
		switchable_bonus_on_boad = false;
		free_switchable_bonus_on_boad = false;

		for (int y = 0; y < _Y_tiles; y++)
        	{
	            for (int x = 0; x < _X_tiles; x++)
	            {
				//reset checks
				board_array_master[x,y,13] = 0;

				Check_moves_of_this_gem(x,y);
				}
			}
		
		if ((number_of_moves_possible <= 0) && (!clickable_bonus_on_boad))
			{
			Shuffle();
			}
		else
			{
			if ((clickable_bonus_on_boad) || (switchable_bonus_on_boad) || free_switchable_bonus_on_boad)
				elements_to_damage_array = new GameObject[total_tiles]; 
			else
				elements_to_damage_array = new GameObject[14];//14 is the maximum number of gem that can be explode with a move

			Annotate_potential_moves();
			}

		}
	
	void Annotate_potential_moves()//call from Check_ALL_possible_moves(), Update_turn_order_after_a_bad_move(), Player_lose()
		{		
		Debug.Log("number_of_bonus_on_board: " + number_of_bonus_on_board);
		//reset variables of the previous move
		n_combo = 0;
		number_of_padlocks_involved_in_explosion = 0;
		number_of_elements_to_damage = 0;
		number_of_gems_to_move = 0;
		number_of_new_gems_to_create = 0;
		gems_useful_moved = 0;
		bonus_select = bonus.none;

		if (lose_requirement_selected == lose_requirement.player_have_zero_moves)
		{
			if (current_player_moves_left <= 0)
			{
				player_can_move = false;
				game_end = true;
				if (!player_win)
					player_win = false;
			}
		}

		if (!stage_started)
			{
			start_time = Time.timeSinceLevelLoad;
			stage_started = true;
			}
		if(!game_end)
			{

			list_of_moves_possible = new int[number_of_moves_possible,8]; 
			/*
			0 = max gems that will explode with this move
			1 = x
			2 = y
			3 = color
			 	4= up [n. how many gem explode if this gem go up]
			 	5= down [n. how many gem explode if this gem go down]
			 	6= right [n. how many gem explode if this gem go right]
			 	7= left [n. how many gem explode if this gem go left]
			 	--8 = most big explosion
			*/
				number_of_gems_moveable = 0;
				for (int y = 0; y < _Y_tiles; y++)
				{
					for (int x = 0; x < _X_tiles; x++)
					{
						if (board_array_master[x,y,5] >0) //if this gem have at least one move
						{
							//avaible moves of this gem
							//list_of_moves_possible[number_of_gems_moveable,0] = board_array_master[x,y,5];
							//gem color
							list_of_moves_possible[number_of_gems_moveable,3] = board_array_master[x,y,1];
							
							//gem position
							list_of_moves_possible[number_of_gems_moveable,1] = x;
							list_of_moves_possible[number_of_gems_moveable,2] = y;

							
							
							//moves
							if (board_array_master[x,y,6]>0)
								{
								list_of_moves_possible[number_of_gems_moveable,4] = board_array_master[x,y,6];
								if ( board_array_master[x,y,6] > list_of_moves_possible[number_of_gems_moveable,0] )
									list_of_moves_possible[number_of_gems_moveable,0] = board_array_master[x,y,6];
								}
							if (board_array_master[x,y,7]>0)
								{
								list_of_moves_possible[number_of_gems_moveable,5] = board_array_master[x,y,7];
								if ( board_array_master[x,y,7] > list_of_moves_possible[number_of_gems_moveable,0] )
									list_of_moves_possible[number_of_gems_moveable,0] = board_array_master[x,y,7];
								}
							if (board_array_master[x,y,8]>0)
								{
								list_of_moves_possible[number_of_gems_moveable,6] = board_array_master[x,y,8];
								if ( board_array_master[x,y,8] > list_of_moves_possible[number_of_gems_moveable,0] )
									list_of_moves_possible[number_of_gems_moveable,0] = board_array_master[x,y,8];
								}
							if (board_array_master[x,y,9]>0)
								{
								list_of_moves_possible[number_of_gems_moveable,7] = board_array_master[x,y,9];
								if ( board_array_master[x,y,9] > list_of_moves_possible[number_of_gems_moveable,0] )
									list_of_moves_possible[number_of_gems_moveable,0] = board_array_master[x,y,9];
								}
							

						//DEBUG show all moves
						/*
							tile_C tile_script = (tile_C)tiles_array[x,y].GetComponent("tile_C");
							if  (list_of_moves_possible[number_of_gems_moveable,0] >= 3) //)&& (_show_all_moves) )
								tile_script.Debug_show_available_moves(list_of_moves_possible[number_of_gems_moveable,0]);
							else
								tile_script.Debug_show_available_moves(0);
						*/

						number_of_gems_moveable++;
						}
					/*
					else //DEBUG
						{
						if (board_array_master[x,y,0] != -1)
							{
							tile_C tile_script = (tile_C)tiles_array[x,y].GetComponent("tile_C");
								tile_script.Debug_show_available_moves(0);
							}
						}
						*/
					}
				}
			if (!versus)//if player play alone
				{
				Debug.Log("Player's turn - not versus");
				if (turn_gained)
					{
					turn_gained = false;
					current_player_chain_lenght++;
					}
				else
					current_player_chain_lenght = 0;
				//Debug.Log("chain_turns_limit: " + chain_turns_limit + " * current_player_chain_lenght: " + current_player_chain_lenght + " * max_chain_turns: " + max_chain_turns);

				player_turn = true;
				player_can_move = true;

				if (show_hint)
					{
					use_hint = true;
					Invoke("Show_hint",show_hint_after_n_seconds);
					}
				}
			else //if player play against AI
				{
				if (turn_gained)
					{
					turn_gained = false;
					if (player_turn)
						current_player_chain_lenght++;
					else
						current_enemy_chain_lenght++;

					//Debug.Log("chain_turns_limit: " + chain_turns_limit + " * current_player_chain_lenght: " + current_player_chain_lenght + " * max_chain_turns: " + max_chain_turns);
					//Debug.Log("chain_turns_limit: " + chain_turns_limit + " * current_enemy_chain_lenght: " + current_enemy_chain_lenght + " * max_chain_turns: " + max_chain_turns);

					}
				else
					{
					if (player_turn)//if the previous was of the player
						{
						player_turn = false;// now is enemy turn
						current_player_chain_lenght = 0;
						}
					else
						{
						player_turn = true;//else is palyer turn
						current_enemy_chain_lenght = 0;
						}
					}

				if (player_turn)
					{
					Debug.Log("Player's turn");
					player_can_move = true;
					gui_player_name_text.color = color_player_on;
					gui_enemy_name_text.color = color_enemy_off;

					if (show_hint)
						{
						use_hint = true;
						Invoke("Show_hint",show_hint_after_n_seconds);
						}
					}
				else
					{
					gui_player_name_text.color = color_player_off;
					gui_enemy_name_text.color = color_enemy_on;
					Enemy_play();
					}
				}
			}
		else
			{
			//game is end, so...
			if (player_win)
				Show_win_screen();
			else
				Show_lose_screen();
			}

		}

	void Show_hint()//call from Annotate_potential_moves()
	{
		if (use_hint)
		{
		if (number_of_moves_possible > 0)//show a gem move
			{
			Debug.Log("move hint");
			int random_hint = UnityEngine.Random.Range(0,number_of_gems_moveable-1);
			my_hint.position = 	tiles_array[list_of_moves_possible[random_hint,1],list_of_moves_possible[random_hint,2]].transform.position;
			my_hint.GetComponent<Animation>().Play("hint_anim");
			my_hint.gameObject.SetActive(true);
			for (int i = 4; i <= 7; i++)
				{
				if ( list_of_moves_possible[random_hint,i] > 0)
						{
						my_hint.GetChild(i-4).gameObject.SetActive(true);
						}
					else
						my_hint.GetChild(i-4).gameObject.SetActive(false);
				}
			}
		else //show a clickable bonus
			{
			Debug.Log("bonus hint");
			Locate_all_bonus_on_board();
			int random_temp = UnityEngine.Random.Range(0,number_of_bonus_on_board-1);
			my_hint.position = new Vector3(bonus_coordinate[random_temp].x,-bonus_coordinate[random_temp].y,my_hint.position.z);
			my_hint.GetComponent<Animation>().Play("hint_anim_click_here");
			my_hint.gameObject.SetActive(true);

			for (int i = 0; i < 4; i++)
				{
				my_hint.GetChild(i).gameObject.SetActive(true);
				}
			}

		}
	}

	void Check_moves_of_this_gem(int x, int y) //call from  Check_ALL_possible_moves()
		{
		//reset array
		board_array_master[x,y,5] = 0; //number of useful moves of this gem [from 0 = none, to 4 = all directions]
			board_array_master[x,y,6] = 0; //up
			board_array_master[x,y,7] = 0; //down
			board_array_master[x,y,8] = 0; //right
			board_array_master[x,y,9] = 0; //left
			
		if ( (board_array_master[x,y,1] >= 0) && (board_array_master[x,y,1] <= 8) && (board_array_master[x,y,3] == 0) )//this gem exist and can move
			{
			# region move to right (x+1,y) if... 
			//it is feasible move to right
			if( ( (x+1)<_X_tiles ) && (board_array_master[x+1,y,0] > -1) //there is a tile
				&& (board_array_master[x+1,y,3] == 0) //no padlock
				&& (board_array_master[x+1,y,1] >= 0) && (board_array_master[x+1,y,1] <= 9) )// there is a gem
				{
				//2 up
					if ((y+2)<_Y_tiles)
					{
						if ((board_array_master[x,y,1]==board_array_master[x+1,y+2,1]) && (board_array_master[x,y,1]==board_array_master[x+1,y+1,1]))
						{
							if (board_array_master[x,y,8]==0)//annotate this move
							{
								number_of_moves_possible++;
								board_array_master[x,y,5]+=1;
							}
							board_array_master[x,y,8]+=2;//this move will make explode +2 gem
						}
					}
				//2 down
					if ((y-2)>=0)
					{
						if ((board_array_master[x,y,1]==board_array_master[x+1,y-2,1]) && (board_array_master[x,y,1]==board_array_master[x+1,y-1,1]))
						{
							if (board_array_master[x,y,8]==0)//annotate this move
							{
								number_of_moves_possible++;
								board_array_master[x,y,5]+=1;
							}
							board_array_master[x,y,8]+=2;//this move will make explode +2 gem
						}
					}
				//1 up and 1 down
					if (((y-1)>=0)&&((y+1)<_Y_tiles))
					{
						if ((board_array_master[x,y,1]==board_array_master[x+1,y-1,1]) && (board_array_master[x,y,1]==board_array_master[x+1,y+1,1]))
						{
							if (board_array_master[x,y,8]==0)//annotate this move
							{
								number_of_moves_possible++;
								board_array_master[x,y,5]+=1;
							}
							//count explosions of this move
							if ((y-2)<0)
								board_array_master[x,y,8]+=1;
							else if ((board_array_master[x,y,1]!=board_array_master[x+1,y-2,1]))
								board_array_master[x,y,8]+=1;
							
							if ((y+2)>=_Y_tiles)
								board_array_master[x,y,8]+=1;
							else if (board_array_master[x,y,1]!=board_array_master[x+1,y+2,1])
								board_array_master[x,y,8]+=1;	
						}
					}
				//2 to right
					if ((x+3)<_X_tiles)
					{
						if ((board_array_master[x,y,1]==board_array_master[x+2,y,1]) && (board_array_master[x,y,1]==board_array_master[x+3,y,1]))
						{
							if (board_array_master[x,y,8]==0)//annotate this move
							{
								number_of_moves_possible++;
								board_array_master[x,y,5]+=1;
							}
							board_array_master[x,y,8]+=2;//this move will make explode +2 gem
						}
					}
				}
			#endregion
			#region move to left (x-1,y) if...
			if( ((x-1) >= 0) && ((board_array_master[x-1,y,0] > -1))  //there is a tile
			  		&& (board_array_master[x-1,y,3] == 0) //no padlock
			 		&& (board_array_master[x-1,y,1] >= 0) && (board_array_master[x-1,y,1] <= 9) )// there is a gem
			   {
				//2 up
				if ((y+2)<_Y_tiles)
				{
					if ((board_array_master[x,y,1]==board_array_master[x-1,y+2,1]) && (board_array_master[x,y,1]==board_array_master[x-1,y+1,1]))
					{
						if (board_array_master[x,y,9]==0)
						{
							number_of_moves_possible++;
							board_array_master[x,y,5]+=1;
						}
						board_array_master[x,y,9]+=2;
					}
				}
				//2 down
				if ((y-2)>=0)
				{
					if ((board_array_master[x,y,1]==board_array_master[x-1,y-2,1]) && (board_array_master[x,y,1]==board_array_master[x-1,y-1,1]))
					{
						if (board_array_master[x,y,9]==0)
						{
							number_of_moves_possible++;
							board_array_master[x,y,5]+=1;
						}							
						board_array_master[x,y,9]+=2;
					}
				}
				//1 up and 1 down
				if (((y-1)>=0)&&((y+1)<_Y_tiles))
				{
					if ((board_array_master[x,y,1]==board_array_master[x-1,y-1,1]) && (board_array_master[x,y,1]==board_array_master[x-1,y+1,1]))
					{
						if (board_array_master[x,y,9]==0)
						{
							number_of_moves_possible++;
							board_array_master[x,y,5]+=1;
						}				
						//count explosions of this move
						if ((y-2)<0)
							board_array_master[x,y,9]+=1;
						else if ((board_array_master[x,y,1]!=board_array_master[x-1,y-2,1]))
							board_array_master[x,y,9]+=1;
						
						if ((y+2)>=_Y_tiles)
							board_array_master[x,y,9]+=1;
						else if (board_array_master[x,y,1]!=board_array_master[x-1,y+2,1])
							board_array_master[x,y,9]+=1;
						
						
					}
				}
				//2 right
				if (x-3>=0)
				{
					if ((board_array_master[x,y,1]==board_array_master[x-2,y,1]) && (board_array_master[x,y,1]==board_array_master[x-3,y,1]))
					{
						if (board_array_master[x,y,9]==0)
						{
							number_of_moves_possible++;
							board_array_master[x,y,5]+=1;
						}
						board_array_master[x,y,9]+=2;
					}
				}
			}
			#endregion
			#region move up (x,y+1) if...
			if( ((y+1)<_Y_tiles) &&  (board_array_master[x,y+1,0]  > -1)  
			   && (board_array_master[x,y+1,3] == 0) //no padlock
			   && (board_array_master[x,y+1,1] >= 0) && (board_array_master[x,y+1,1] <= 9) )// there is a gem
			   {
				//2 up left
				if ((x+2)<_X_tiles)
				{
					if ((board_array_master[x,y,1]==board_array_master[x+2,y+1,1]) && (board_array_master[x,y,1]==board_array_master[x+1,y+1,1]))
					{
						if (board_array_master[x,y,6]==0)
						{
							number_of_moves_possible++;
							board_array_master[x,y,5]+=1;
						}
						board_array_master[x,y,6]+=2;
					}
				}
				//2 up right
				if ((x-2)>=0)
				{
					if ((board_array_master[x,y,1]==board_array_master[x-2,y+1,1]) && (board_array_master[x,y,1]==board_array_master[x-1,y+1,1]))
					{
						if (board_array_master[x,y,6]==0)
						{
							number_of_moves_possible++;
							board_array_master[x,y,5]+=1;
						}
						board_array_master[x,y,6]+=2;
					}
				}
				//due up (1 right and 1 left)
				if (((x-1)>=0)&&((x+1)<_X_tiles))
				{
					if ((board_array_master[x,y,1]==board_array_master[x-1,y+1,1]) && (board_array_master[x,y,1]==board_array_master[x+1,y+1,1]))
					{
						if (board_array_master[x,y,6]==0)
						{
							number_of_moves_possible++;
							board_array_master[x,y,5]+=1;
						}
						
						if ((x-2)<0)
							board_array_master[x,y,6]+=1;
						else if (board_array_master[x,y,1]!=board_array_master[x-2,y+1,1])
							board_array_master[x,y,6]+=1;
						
						if ((x+2)>=_X_tiles)
							board_array_master[x,y,6]+=1;
						else if (board_array_master[x,y,1]!=board_array_master[x+2,y+1,1])
							board_array_master[x,y,6]+=1;
						
						
					}
				}
				//2 up
				if (y+3<_Y_tiles)
				{
					if ((board_array_master[x,y,1]==board_array_master[x,y+2,1]) && (board_array_master[x,y,1]==board_array_master[x,y+3,1]))
					{
						if (board_array_master[x,y,6]==0)
						{
							number_of_moves_possible++;
							board_array_master[x,y,5]+=1;
						}
						board_array_master[x,y,6]+=2;
					}
				}
			}
			#endregion
			#region move up (x,y-1) if...
			if( ((y-1)>=0) &&  (board_array_master[x,y-1,0] > -1)
			   && (board_array_master[x,y-1,3] == 0) //no padlock
			   && (board_array_master[x,y-1,1] >= 0) && (board_array_master[x,y-1,1] <= 9) )// there is a gem
			   
			   {
				if ((x+2)<_X_tiles)
				{
					if ((board_array_master[x,y,1]==board_array_master[x+2,y-1,1]) && (board_array_master[x,y,1]==board_array_master[x+1,y-1,1]))
					{
						if (board_array_master[x,y,7]==0)
						{
							number_of_moves_possible++;
							board_array_master[x,y,5]+=1;
						}
						board_array_master[x,y,7]+=2;
					}
				}
				//2 down right
				if ((x-2)>=0)
				{
					if ((board_array_master[x,y,1]==board_array_master[x-2,y-1,1]) && (board_array_master[x,y,1]==board_array_master[x-1,y-1,1]))
					{
						if (board_array_master[x,y,7]==0)
						{
							number_of_moves_possible++;
							board_array_master[x,y,5]+=1;
						}
						board_array_master[x,y,7]+=2;
					}
				}
				//2 down (1 right and 1 left)
				if (((x-1)>=0)&&((x+1)<_X_tiles))
				{
					if ((board_array_master[x,y,1]==board_array_master[x-1,y-1,1]) && (board_array_master[x,y,1]==board_array_master[x+1,y-1,1]))
					{
						if (board_array_master[x,y,7]==0)
						{
							number_of_moves_possible++;
							board_array_master[x,y,5]+=1;
						}
						
						
						if ((x-2)<0)
							board_array_master[x,y,7]+=1;
						else if (board_array_master[x,y,1]!=board_array_master[x-2,y-1,1])
							board_array_master[x,y,7]+=1;
						
						if ((x+2)>=_X_tiles)
							board_array_master[x,y,7]+=1;
						else if (board_array_master[x,y,1]!=board_array_master[x+2,y-1,1])
							board_array_master[x,y,7]+=1;
						
						
					}
				}
				//2 down
				if (y-3>=0)
				{
					if ((board_array_master[x,y,1]==board_array_master[x,y-2,1]) && (board_array_master[x,y,1]==board_array_master[x,y-3,1]))
					{
						if (board_array_master[x,y,7]==0)
						{
							number_of_moves_possible++;
							board_array_master[x,y,5]+=1;
						}
						board_array_master[x,y,7]+=2;
					}
				}
			}
			#endregion

			//count myself in the explosion
			if (board_array_master[x,y,6] > 0)
				board_array_master[x,y,6]++;
			if (board_array_master[x,y,7] > 0)
				board_array_master[x,y,7]++;
			if (board_array_master[x,y,8] > 0)
				board_array_master[x,y,8]++;
			if (board_array_master[x,y,9] > 0)
				board_array_master[x,y,9]++;
		}
		else if (board_array_master[x,y,4] > 0) //if there is a bonus on the board
			{
			if (trigger_by_select == trigger_by.click)
				clickable_bonus_on_boad = true;
			else if (trigger_by_select == trigger_by.switch_adjacent_gem)
				{
				//if this bonus can be trigger by a gem
				switchable_bonus_on_boad = true;
				}
			else if (trigger_by_select == trigger_by.free_switch)
				{
				//if this bonus can be move
				free_switchable_bonus_on_boad = true;
				}
			}

		
	}
	
	public void Switch_gem(int direction)// 4 = down ; 5 = up ; 6 = right; 7 = left
	{
		/*
		Debug.Log("Switch_gem " + direction + " ... " 
		          	+ main_gem_selected_x+","+main_gem_selected_y + " : " + board_array_master[main_gem_selected_x,main_gem_selected_y,5]
					+ " *** " + minor_gem_destination_to_x+","+minor_gem_destination_to_y + " : "+ board_array_master[minor_gem_destination_to_x,minor_gem_destination_to_y,5]);
		*/


		bool main_explode = false;
		bool minor_explode = false;

		explode_same_color_again_with = 0;//0 = false; 1 = with main gem; 2 =with minor gem

		score_of_this_turn_move = 0;
		n_gems_exploded_with_main_gem = 0;
		n_gems_exploded_with_minor_gem = 0;
		this_is_the_primary_explosion = true;

		player_can_move = false;
		//if (!player_can_move_when_gem_falling)
			cursor.gameObject.SetActive(false);


		//if the main gem or the minor gem have at least a useful move
		if ( ( (board_array_master[main_gem_selected_x,main_gem_selected_y,5]>0) || (board_array_master[minor_gem_destination_to_x,minor_gem_destination_to_y,5]>0) ) &&
		    //if this move make explode something
		    (direction==4 && ((board_array_master[main_gem_selected_x,main_gem_selected_y,6] > 0)
		                  || (board_array_master[minor_gem_destination_to_x,minor_gem_destination_to_y,7] >0) ) )
		    
		    ||(direction==5 && ((board_array_master[main_gem_selected_x,main_gem_selected_y,7] > 0)
		                    || (board_array_master[minor_gem_destination_to_x,minor_gem_destination_to_y,6] >0) ) )
		    
		    
		    || (direction==6 && ((board_array_master[main_gem_selected_x,main_gem_selected_y,8] > 0)
		                     || (board_array_master[minor_gem_destination_to_x,minor_gem_destination_to_y,9] >0) ) )
		    
		    
		    || (direction==7 && ((board_array_master[main_gem_selected_x,main_gem_selected_y,9] > 0)
		                     || (board_array_master[minor_gem_destination_to_x,minor_gem_destination_to_y,8] >0) ) ) 
		    )
			
		{
			//Debug.Log("there is an useful move");


			//create array to note the explosions
			position_of_gems_that_will_explode = new bool[2,8];

			Move_gems_to_target_positions();
			Center_camera_to_move();

			//explosion
			/*
			Debug.Log("previous color main = " + player_previous_exploded_color[0] 
			          + " ; minor = " + player_previous_exploded_color[1] 
			          + " __ main now = " + board_array_master[minor_gem_destination_to_x,minor_gem_destination_to_y,1]
			          + " __ minor now = " + board_array_master[main_gem_selected_x,main_gem_selected_y,1]);*/
			//check if is the main gem that explode
			if (
				(direction==4 && (board_array_master[main_gem_selected_x,main_gem_selected_y,6] > 0))
				|| (direction==5 && (board_array_master[main_gem_selected_x,main_gem_selected_y,7] > 0))
				|| (direction==6 && (board_array_master[main_gem_selected_x,main_gem_selected_y,8] > 0))
				|| (direction==7 && (board_array_master[main_gem_selected_x,main_gem_selected_y,9] > 0))
				)
			{
				gems_useful_moved++;
				n_gems_exploded_with_main_gem++;
				main_explode = true;

				//Debug.Log("check main, My: " + board_array_master[minor_gem_destination_to_x,minor_gem_destination_to_y,1] + " ; previous " + player_previous_exploded_color[0] + " and " + player_previous_exploded_color[1]);
				if(player_turn)
					{
					if ( (board_array_master[minor_gem_destination_to_x,minor_gem_destination_to_y,1] == player_previous_exploded_color[0]) //if this gem have the same color of the gem exploded in the previous move
						|| (board_array_master[minor_gem_destination_to_x,minor_gem_destination_to_y,1] == player_previous_exploded_color[1]) )
						{
						//Debug.Log("player explode same color with main gem");
						player_previous_exploded_color[0] = board_array_master[minor_gem_destination_to_x,minor_gem_destination_to_y,1];
						explode_same_color_again_with = 1; //main gem
						if (gain_turn_if_explode_same_color_of_previous_move)
							{
							if ( (!chain_turns_limit) || (current_player_chain_lenght < max_chain_turns) )
								{
								//Debug.Log("chain_turns_limit: " + chain_turns_limit + " * current_player_chain_lenght: " + current_player_chain_lenght + " * max_chain_turns: " + max_chain_turns);
								Debug.Log("player gain a move with main gem - same color");
								Gain_turns(move_gained_for_explode_same_color_in_two_adjacent_turn);
								}
							}
						}
					else
						player_previous_exploded_color[0] = board_array_master[minor_gem_destination_to_x,minor_gem_destination_to_y,1];
					}
				else //enemy turn
					{
					if ( (board_array_master[minor_gem_destination_to_x,minor_gem_destination_to_y,1] == enemy_previous_exploded_color[0]) //if this gem have the same color of the gem exploded in the previous move
					    || (board_array_master[minor_gem_destination_to_x,minor_gem_destination_to_y,1] == enemy_previous_exploded_color[1]) )
						{
							//Debug.Log("enemy explode same color with main gem");
							enemy_previous_exploded_color[0] = board_array_master[minor_gem_destination_to_x,minor_gem_destination_to_y,1];
							explode_same_color_again_with = 1; //main gem
							if (gain_turn_if_explode_same_color_of_previous_move)
								{
								if ( (!chain_turns_limit) || (current_enemy_chain_lenght < max_chain_turns) )
									{
									//Debug.Log("chain_turns_limit: " + chain_turns_limit + " * current_enemy_chain_lenght: " + current_enemy_chain_lenght + " * max_chain_turns: " + max_chain_turns);
									Debug.Log("enemy gain a move with main gem  - same color");
									Gain_turns(move_gained_for_explode_same_color_in_two_adjacent_turn);
									}
								}
						}
					else
						enemy_previous_exploded_color[0] = board_array_master[minor_gem_destination_to_x,minor_gem_destination_to_y,1];
					}
				
				//I use the coordinate of the minor gem because by now the switch is done
				Detect_which_gems_will_explode(minor_gem_destination_to_x,minor_gem_destination_to_y,0);
			}


			//check if minor gem explode
			if (
				(direction==4 && (board_array_master[minor_gem_destination_to_x,minor_gem_destination_to_y,7] >0))
				|| 	(direction==5 && (board_array_master[minor_gem_destination_to_x,minor_gem_destination_to_y,6] >0))
				|| 	(direction==6 && (board_array_master[minor_gem_destination_to_x,minor_gem_destination_to_y,9] >0))
				|| 	(direction==7 && (board_array_master[minor_gem_destination_to_x,minor_gem_destination_to_y,8] >0))
				)
					{
						gems_useful_moved++;
						n_gems_exploded_with_minor_gem++;
						minor_explode = true;
					//if this gem have the same color of the gem exploded in the previous move
					//Debug.Log("check minor, My: " + board_array_master[main_gem_selected_x,main_gem_selected_y,1] + " ; previous " + player_previous_exploded_color[0] + " and " + player_previous_exploded_color[1]);
					if(player_turn)
						{
						if ( (board_array_master[main_gem_selected_x,main_gem_selected_y,1] == player_previous_exploded_color[1]) //if this gem have the same color of the gem exploded in the previous move
					    || (board_array_master[main_gem_selected_x,main_gem_selected_y,1] == player_previous_exploded_color[0])) 
							{
							//Debug.Log("player explode same color with minor gem");
							player_previous_exploded_color[1] = board_array_master[main_gem_selected_x,main_gem_selected_y,1];
							explode_same_color_again_with = 2; //minor gem
							if (gain_turn_if_explode_same_color_of_previous_move)
								{
								if ( (!chain_turns_limit) || (current_player_chain_lenght < max_chain_turns) )
									{
									Debug.Log("player gain a move with minor gem  - same color");
									//Debug.Log("chain_turns_limit: " + chain_turns_limit + " * current_player_chain_lenght: " + current_player_chain_lenght + " * max_chain_turns: " + max_chain_turns);
									Gain_turns(move_gained_for_explode_same_color_in_two_adjacent_turn);
									}
								}
							}
						else
							player_previous_exploded_color[1] = board_array_master[main_gem_selected_x,main_gem_selected_y,1];
						}
					else //enemy turn
						{
						if ( (board_array_master[main_gem_selected_x,main_gem_selected_y,1] == enemy_previous_exploded_color[1]) //if this gem have the same color of the gem exploded in the previous move
					   	 	|| (board_array_master[main_gem_selected_x,main_gem_selected_y,1] == enemy_previous_exploded_color[0])) 
							{
								//Debug.Log("enemy explode same color with minor gem");
								enemy_previous_exploded_color[1] = board_array_master[main_gem_selected_x,main_gem_selected_y,1];
								explode_same_color_again_with = 2; //minor gem
								if (gain_turn_if_explode_same_color_of_previous_move)
									{
									if ( (!chain_turns_limit) || (current_enemy_chain_lenght < max_chain_turns) )
										{
										Debug.Log("enemy gain a move with minor gem  - same color");
										//Debug.Log("chain_turns_limit: " + chain_turns_limit + " * current_player_chain_lenght: " + current_player_chain_lenght + " * max_chain_turns: " + max_chain_turns);
										Gain_turns(move_gained_for_explode_same_color_in_two_adjacent_turn);
										}
									}
							}
						else
							enemy_previous_exploded_color[1] = board_array_master[main_gem_selected_x,main_gem_selected_y,1];
							
						}
						
						//I use the coordinate of the minor gem because by now the switch is done
						Detect_which_gems_will_explode(main_gem_selected_x,main_gem_selected_y,1);
					}


			Empty_main_and_minor_gem_selections();
		}
		else //this move is useless (no explosions)
			{
			//...but is a good move if move a bonus with free_switch
			if ( trigger_by_select == trigger_by.free_switch &&
				((board_array_master[main_gem_selected_x,main_gem_selected_y,4]>0) || (board_array_master[minor_gem_destination_to_x,minor_gem_destination_to_y,4]>0)) )
				{
				//Debug.Log("free switch without main explosion");
				Move_gems_to_target_positions();
				Center_camera_to_move();

				//trigger bonus
				elements_to_damage_array = new GameObject[15];
				if (board_array_master[main_gem_selected_x,main_gem_selected_y,4] > 0)  
					{
					tile_C tile_script = (tile_C)tiles_array[main_gem_selected_x,main_gem_selected_y].GetComponent("tile_C");
					tile_script.Trigger_bonus(false);
					}
				if  (board_array_master[minor_gem_destination_to_x,minor_gem_destination_to_y,4] > 0) 
					{
					tile_C tile_script = (tile_C)tiles_array[minor_gem_destination_to_x,minor_gem_destination_to_y].GetComponent("tile_C");
					tile_script.Trigger_bonus(false);
					}


				Empty_main_and_minor_gem_selections();
				}
			else //is a bad move
				StartCoroutine(Start_bad_switch_animation());
			}

		if (player_turn)
			{
			if (!main_explode)
				player_previous_exploded_color[0] = -1; //so no color to keep track here
			if (!minor_explode)
				player_previous_exploded_color[1] = -1; //so no color to keep track here
			}
		else
			{
			if (!main_explode)
				enemy_previous_exploded_color[0] = -1; //so no color to keep track here
			if (!minor_explode)
				enemy_previous_exploded_color[1] = -1; //so no color to keep track here
			}

		//keep track of score
		if (player_turn)
			{
			if (explode_same_color_again_with > 0)
				player_explode_same_color_n_turn++;
			else
				player_explode_same_color_n_turn = 0;
			}
		else
			{
			if (explode_same_color_again_with > 0)
				enemy_explode_same_color_n_turn++;
			else
				enemy_explode_same_color_n_turn = 0;
			}

		if (number_of_elements_to_damage>0)//if there is something to explode
		{
			//if (player_can_move_when_gem_falling && !one_step_update_ongoing)
			//	Update_board_by_one_step();
			//else
				Order_to_gems_to_explode();
		}

	}

	void Move_gems_to_target_positions()
		{
		int temp_main_bonus = 0;
		int temp_minor_bonus = 0;

		//count moves
		if (player_turn)
			current_player_moves_left--;
		else
			current_enemy_moves_left--;
		
		Update_left_moves_text();
		
		//update color in array
		board_array_master[main_gem_selected_x,main_gem_selected_y,1] = minor_gem_color;
		board_array_master[minor_gem_destination_to_x,minor_gem_destination_to_y,1] = main_gem_color;
		
		//update bonus position
		temp_main_bonus = board_array_master[main_gem_selected_x,main_gem_selected_y,4];
		temp_minor_bonus = board_array_master[minor_gem_destination_to_x,minor_gem_destination_to_y,4];
		board_array_master[main_gem_selected_x,main_gem_selected_y,4] = temp_minor_bonus;
		board_array_master[minor_gem_destination_to_x,minor_gem_destination_to_y,4] = temp_main_bonus;
		
		//update gem representation
		tile_C tile_script_main_gem = (tile_C)tiles_array[main_gem_selected_x,main_gem_selected_y].GetComponent("tile_C");
		tile_C tile_script_minor_gem = (tile_C)tiles_array[minor_gem_destination_to_x,minor_gem_destination_to_y].GetComponent("tile_C");
		
		
		tile_script_main_gem.my_gem = avatar_minor_gem;
		
		tile_script_minor_gem.my_gem = avatar_main_gem;
		
		
		StartCoroutine(tile_script_main_gem.Switch_gem_animation());
		StartCoroutine(tile_script_minor_gem.Switch_gem_animation());
		}

	void Center_camera_to_move()
		{
		if (camera_position_choice == camera_position.centred_to_move)
			{
				
				if ( Vector2.Distance(new Vector2 (main_gem_selected_x,main_gem_selected_y), new Vector2 (new_camera_position_x,Mathf.Abs(new_camera_position_y))) >= camera_move_tolerance)
				{
					
					new_camera_position_x = main_gem_selected_x - pivot_board.position.x;
					if (new_camera_position_x < margin.x)
						new_camera_position_x = margin.x;
					else if (new_camera_position_x > _X_tiles - margin.x)
						new_camera_position_x = _X_tiles - margin.x;
					
					new_camera_position_y = pivot_board.position.y - main_gem_selected_y;
					if (new_camera_position_y*-1 < margin.y)
						new_camera_position_y = margin.y*-1;
					else if (new_camera_position_y*-1 > _Y_tiles - margin.y)
						new_camera_position_y = (_Y_tiles - margin.y)*-1;
					
					
					new_camera_position = new Vector3(new_camera_position_x,new_camera_position_y,Board_camera.transform.position.z);
					
					StartCoroutine(Move_Camera());
				}
			}
		}

	void Empty_main_and_minor_gem_selections()
	{
		//Debug.Log("Empty_main_and_minor_gem_selections() " + player_can_move);
		main_gem_selected_x = -10;
		main_gem_selected_y = -10;
		avatar_main_gem = null;
		main_gem_color = -10;
		minor_gem_destination_to_x = -10;
		minor_gem_destination_to_y = -10;
		avatar_minor_gem = null;
		minor_gem_color = -10;

	}

	void Cancell_hint()
		{
		CancelInvoke("Show_hint");
		use_hint = false;
		my_hint.gameObject.SetActive(false);
		for (int i = 0; i < 4; i++)
			{
			my_hint.GetChild(i).gameObject.SetActive(false);
			}
		}
	
	
	IEnumerator Start_bad_switch_animation()
	{
		//Debug.Log("Start_bad_switch_animation() " + player_can_move);
		if (Vector3.Distance(tiles_array[minor_gem_destination_to_x,minor_gem_destination_to_y].transform.position, avatar_main_gem.transform.position) > accuracy)
		{
			yield return new WaitForSeconds(0.015f);

			avatar_main_gem.transform.Translate(((tiles_array[minor_gem_destination_to_x,minor_gem_destination_to_y].transform.position - avatar_main_gem.transform.position).normalized) * falling_speed * Time.deltaTime, Space.World);
			avatar_minor_gem.transform.Translate(((tiles_array[main_gem_selected_x,main_gem_selected_y].transform.position - avatar_minor_gem.transform.position).normalized) * falling_speed * Time.deltaTime, Space.World);


			StartCoroutine(Start_bad_switch_animation());
		}
		else
		{
			
			if(Vector3.Distance(tiles_array[minor_gem_destination_to_x,minor_gem_destination_to_y].transform.position, avatar_main_gem.transform.position) != 0)
				avatar_main_gem.transform.position = tiles_array[minor_gem_destination_to_x,minor_gem_destination_to_y].transform.position;

			if(Vector3.Distance(tiles_array[main_gem_selected_x,main_gem_selected_y].transform.position, avatar_minor_gem.transform.position) != 0)
				avatar_minor_gem.transform.position = tiles_array[main_gem_selected_x,main_gem_selected_y].transform.position;

			Play_sfx(bad_move_sfx);
			StartCoroutine(End_bad_switch_animation());
			
			yield return null;
		}
		
	}



	IEnumerator End_bad_switch_animation()
	{
		//Debug.Log("End_bad_switch_animation() " + player_can_move);
		if (Vector3.Distance(tiles_array[main_gem_selected_x,main_gem_selected_y].transform.position, avatar_main_gem.transform.position) > accuracy)
		{

			avatar_main_gem.transform.Translate(((tiles_array[main_gem_selected_x,main_gem_selected_y].transform.position - avatar_main_gem.transform.position).normalized) * switch_speed * Time.deltaTime, Space.World);
			avatar_minor_gem.transform.Translate(((tiles_array[minor_gem_destination_to_x,minor_gem_destination_to_y].transform.position - avatar_minor_gem.transform.position).normalized) * switch_speed * Time.deltaTime, Space.World);
			yield return new WaitForSeconds(0.015f);
			StartCoroutine(End_bad_switch_animation());
		}
		else
		{
			
			if(Vector3.Distance(tiles_array[main_gem_selected_x,main_gem_selected_y].transform.position, avatar_main_gem.transform.position) != 0)
				avatar_main_gem.transform.position = tiles_array[main_gem_selected_x,main_gem_selected_y].transform.position;
			
			if(Vector3.Distance(tiles_array[minor_gem_destination_to_x,minor_gem_destination_to_y].transform.position, avatar_minor_gem.transform.position) != 0)
				avatar_minor_gem.transform.position = tiles_array[minor_gem_destination_to_x,minor_gem_destination_to_y].transform.position;

			Empty_main_and_minor_gem_selections();
			Update_turn_order_after_a_bad_move();
			
			yield return null;
		}
		
	}
	

	IEnumerator Move_Camera()
	{

		while (Vector3.Distance(Board_camera.gameObject.transform.position, new_camera_position) > accuracy*10)
		{
			yield return new WaitForSeconds(0.015f);
			Board_camera.gameObject.transform.Translate(((new_camera_position - Board_camera.gameObject.transform.position).normalized) * camera_speed * Time.deltaTime, Space.World);
		}

		
	}
	
	#endregion

	#region 3- play the move
	


	void Update_turn_order_after_a_bad_move()
		{

			if (versus)
				{
				if (lose_turn_if_bad_move)
					{
					if (player_turn)
						{
						current_player_moves_left--;
						current_player_chain_lenght = 0;
						Update_left_moves_text();

						player_turn = false;
						player_can_move = false;
						Enemy_play();
						}
					else
						{
						current_enemy_moves_left--;
						current_enemy_chain_lenght = 0;
						Update_left_moves_text();

						player_turn = true;
						player_can_move = true;
						}
					}
				else
					{
					current_player_chain_lenght = 0;
					player_turn = true;
					player_can_move = true;
					}
				}
			else //solo
				{
				if (lose_turn_if_bad_move)
					{
					if (player_turn)
						{
						current_player_moves_left--;
						current_player_chain_lenght = 0;
						}
					else
						{
						current_enemy_moves_left--;
						current_enemy_chain_lenght = 0;
						}

					Update_left_moves_text();
						if (lose_requirement_selected == lose_requirement.player_have_zero_moves)
							{
							if (current_player_moves_left > 0)
								player_can_move = true;
							else
								{
								Annotate_potential_moves();
								}
							}
						else
							player_can_move = true;
					}
				else
					{
					current_player_chain_lenght = 0;
					player_can_move = true;
					}
				}
		}
	
	void Detect_which_gems_will_explode(int __x, int __y, int _gem)
	{
		//_gem 0 = main gem 
		//_gem 1 = minor gem 
		//vertical check
		//2 up and down
		if (((__y-1)>=0)&&((__y+1)<_Y_tiles))
		{
			if ((board_array_master[__x,__y,1]==board_array_master[__x,__y-1,1]) && (board_array_master[__x,__y,1]==board_array_master[__x,__y+1,1]))
			{
				
				position_of_gems_that_will_explode[_gem,1] = true;
				Annotate_explosions(__x,__y+1);
				
				
				position_of_gems_that_will_explode[_gem,5] = true;
				Annotate_explosions(__x,__y-1);
				
				//if there is one more down
				if ( ((__y-2)>=0) && (board_array_master[__x,__y,1]==board_array_master[__x,__y-2,1]) )
					{
					position_of_gems_that_will_explode[_gem,4] = true;
					Annotate_explosions(__x,__y-2);
					}
				
				//if there is one more up
				if ( ((__y+2)<_Y_tiles) && (board_array_master[__x,__y,1]==board_array_master[__x,__y+2,1]) )
					{
					position_of_gems_that_will_explode[_gem,0] = true;
					Annotate_explosions(__x,__y+2);
					}
				
			}
		}
		//2 up
		if ((__y+2)<_Y_tiles)
		{
			if ((board_array_master[__x,__y,1]==board_array_master[__x,__y+2,1]) && (board_array_master[__x,__y,1]==board_array_master[__x,__y+1,1]))
			{
				
				position_of_gems_that_will_explode[_gem,0] = true;
				Annotate_explosions(__x,__y+2);
				
				
				position_of_gems_that_will_explode[_gem,1] = true;
				Annotate_explosions(__x,__y+1);
				
			}
		}
		//2 down
		if ((__y-2)>=0)
		{
			if ((board_array_master[__x,__y,1]==board_array_master[__x,__y-2,1]) && (board_array_master[__x,__y,1]==board_array_master[__x,__y-1,1]))
			{
				
				position_of_gems_that_will_explode[_gem,5] = true;
				Annotate_explosions(__x,__y-1);
				
				position_of_gems_that_will_explode[_gem,4] = true;
				Annotate_explosions(__x,__y-2);
				
			}
		}
		//horizontal check
		//2 right and left
		if (((__x-1)>=0)&&((__x+1)<_X_tiles))
		{
			if ((board_array_master[__x,__y,1]==board_array_master[__x-1,__y,1]) && (board_array_master[__x,__y,1]==board_array_master[__x+1,__y,1]))
			{
				
				position_of_gems_that_will_explode[_gem,3] = true;
				Annotate_explosions(__x+1,__y);
				
				position_of_gems_that_will_explode[_gem,7] = true;
				Annotate_explosions(__x-1,__y);
				
				//if there is one more at left
				if ( ((__x-2)>=0) && (board_array_master[__x,__y,1]==board_array_master[__x-2,__y,1]) )
					{
					position_of_gems_that_will_explode[_gem,6] = true;
					Annotate_explosions(__x-2,__y);
					}
				//if there is one more at right
				if ( ((__x+2)<_X_tiles) && (board_array_master[__x,__y,1]==board_array_master[__x+2,__y,1]) )
					{
					position_of_gems_that_will_explode[_gem,2] = true;
					Annotate_explosions(__x+2,__y);
					}
			}
		}
		//2 right
		if ((__x+2)<_X_tiles)
		{
			if ((board_array_master[__x,__y,1]==board_array_master[__x+2,__y,1]) && (board_array_master[__x,__y,1]==board_array_master[__x+1,__y,1]))
			{
				
				position_of_gems_that_will_explode[_gem,3] = true;
				Annotate_explosions(__x+1,__y);
				
				position_of_gems_that_will_explode[_gem,2] = true;
				Annotate_explosions(__x+2,__y);
				
			}
		}
		//2 left
		if ((__x-2)>=0)
		{
			if ((board_array_master[__x,__y,1]==board_array_master[__x-2,__y,1]) && (board_array_master[__x,__y,1]==board_array_master[__x-1,__y,1]))
			{
				
				position_of_gems_that_will_explode[_gem,7] = true;
				Annotate_explosions(__x-1,__y);
				
				position_of_gems_that_will_explode[_gem,6] = true;
				Annotate_explosions(__x-2,__y);
				
			}
		}



		//explode the gem moved
		if (_gem == 0) //main gem
		{
			//calculate score
			for (int i = 0; i < 8; i++)
			{
			if (position_of_gems_that_will_explode[_gem,i])
					n_gems_exploded_with_main_gem++;
			}
			Annotate_explosions(__x,__y);
		}
		if (_gem == 1) //minor gem
		{
			//calculate score
			for (int i = 0; i < 8; i++)
			{
				if (position_of_gems_that_will_explode[_gem,i])
					n_gems_exploded_with_minor_gem++;
			}
			Annotate_explosions(__x,__y);
		}

		//if this is a big explosion
		if ((n_gems_exploded_with_main_gem > 3) || (n_gems_exploded_with_minor_gem > 3))
			{
			if (gain_turn_if_explode_more_than_3_gems)
				{
				if (player_turn)
					{
					if ((!chain_turns_limit) || (current_player_chain_lenght < max_chain_turns))
						{
						//Debug.Log("chain_turns_limit: " + chain_turns_limit + " * current_player_chain_lenght: " + current_player_chain_lenght + " * max_chain_turns: " + max_chain_turns);
						if ((n_gems_exploded_with_main_gem == 4) || (n_gems_exploded_with_minor_gem == 4))
							Gain_turns(move_gained_when_explode_more_than_3_gems[0]);
						else if ((n_gems_exploded_with_main_gem == 5) || (n_gems_exploded_with_minor_gem == 5))
							Gain_turns(move_gained_when_explode_more_than_3_gems[1]);
						else if ((n_gems_exploded_with_main_gem == 6) || (n_gems_exploded_with_minor_gem == 6))
							Gain_turns(move_gained_when_explode_more_than_3_gems[2]);
						else if ((n_gems_exploded_with_main_gem == 7) || (n_gems_exploded_with_minor_gem == 7))
							Gain_turns(move_gained_when_explode_more_than_3_gems[3]);
			
						}
						
					}
				else //enemy turn
					{
					if ((!chain_turns_limit) || (current_enemy_chain_lenght < max_chain_turns))
						{
						//Debug.Log("chain_turns_limit: " + chain_turns_limit + " * current_enemy_chain_lenght: " + current_enemy_chain_lenght + " * max_chain_turns: " + max_chain_turns);
						if ((n_gems_exploded_with_main_gem == 4) || (n_gems_exploded_with_minor_gem == 4))
							Gain_turns(move_gained_when_explode_more_than_3_gems[0]);
						else if ((n_gems_exploded_with_main_gem == 5) || (n_gems_exploded_with_minor_gem == 5))
							Gain_turns(move_gained_when_explode_more_than_3_gems[1]);
						else if ((n_gems_exploded_with_main_gem == 6) || (n_gems_exploded_with_minor_gem == 6))
							Gain_turns(move_gained_when_explode_more_than_3_gems[2]);
						else if ((n_gems_exploded_with_main_gem == 7) || (n_gems_exploded_with_minor_gem == 7))
							Gain_turns(move_gained_when_explode_more_than_3_gems[3]);

						}
					}

				}

			//give bonus afther big explosion
			if (give_bonus_select == give_bonus.after_big_explosion)
				{
				if ((_gem == 0) && (n_gems_exploded_with_main_gem > 3)) //check if main gem will become a bonus
					{
					//Debug.Log("main gem " + minor_gem_destination_to_x  + "," + minor_gem_destination_to_y + " will become a bonus");
					Assign_in_board_bonus(minor_gem_destination_to_x,minor_gem_destination_to_y,n_gems_exploded_with_main_gem);
					}

				if ((_gem == 1) && (n_gems_exploded_with_minor_gem > 3)) //check if minor gem will become a bonus
					{
					//Debug.Log("minor gem " + main_gem_selected_x + "," + main_gem_selected_y + " will become a bonus");
					Assign_in_board_bonus(main_gem_selected_x,main_gem_selected_y,n_gems_exploded_with_minor_gem);
					}
				}

			if (gem_explosion_fx_rule_selected == gem_explosion_fx_rule.only_for_big_explosion)
				{
				if ((_gem == 0) && (n_gems_exploded_with_main_gem > 3)) //check if main gem will become a bonus
					{
					tiles_array[minor_gem_destination_to_x,minor_gem_destination_to_y].GetComponent<tile_C>().use_fx_big_explosion_here = n_gems_exploded_with_main_gem;
					}
				if ((_gem == 1) && (n_gems_exploded_with_minor_gem > 3)) //check if minor gem will become a bonus
					{
					tiles_array[main_gem_selected_x,main_gem_selected_y].GetComponent<tile_C>().use_fx_big_explosion_here = n_gems_exploded_with_minor_gem;
					}
				}

			}

		//switch bonus
		if ( ((trigger_by_select == trigger_by.switch_adjacent_gem) || (trigger_by_select == trigger_by.free_switch))
		    && 
		    ( (give_bonus_select == give_bonus.after_big_explosion) || (give_bonus_select == give_bonus.from_stage_file_or_from_gem_emitter)) )
		{
			if  ( (_gem == 0) && (board_array_master[main_gem_selected_x,main_gem_selected_y,4] > 0) ) //if main gem explode and minor is a bonus
				{
				tile_C tile_script = (tile_C)tiles_array[main_gem_selected_x,main_gem_selected_y].GetComponent("tile_C");
					tile_script.Trigger_bonus(false);
				}
			else if ( (_gem == 1) && (board_array_master[minor_gem_destination_to_x,minor_gem_destination_to_y,4] > 0) ) //if minor gem explode and main is a bonus
				{
				tile_C tile_script = (tile_C)tiles_array[minor_gem_destination_to_x,minor_gem_destination_to_y].GetComponent("tile_C");
					tile_script.Trigger_bonus(false);
				}
		}



	}

	void Assign_in_board_bonus(int xx, int yy, int explosion_magnitude)
	{
		//Debug.Log("Assign_in_board_bonus");
		tile_C tile_script = (tile_C)tiles_array[xx,yy].GetComponent("tile_C");

		if (choose_bonus_by_select == choose_bonus_by.explosion_magnitude)
			{
			//Debug.Log("magnitude: " + explosion_magnitude);
			tile_script.after_explosion_I_will_become_this_bonus = (int)big_explosion_give_bonus[explosion_magnitude-4];
			}
		else if (choose_bonus_by_select == choose_bonus_by.gem_color)
			{
			//Debug.Log("color:" + board_array_master[xx,yy,1]);
			tile_script.after_explosion_I_will_become_this_bonus = (int)color_explosion_give_bonus[board_array_master[xx,yy,1]];
			}
	}
	
	public void Annotate_explosions(int xx, int yy)
		{
		if  (board_array_master[xx,yy,11] == 0) // if this explosion not is already marked
			{
			//Debug.Log("Annotate_explosions " + xx +"," + yy + " *** number_of_elements_to_damage: " + number_of_elements_to_damage + " *** elements_to_damage_array: " + elements_to_damage_array.Length);
			board_array_master[xx,yy,11] = 1;// this gem explode
			elements_to_damage_array[number_of_elements_to_damage] = tiles_array[xx,yy];
			//Debug.Log("elements_to_damage_array["+number_of_elements_to_damage+"] = " + elements_to_damage_array[number_of_elements_to_damage]);
			number_of_elements_to_damage++;


			if (board_array_master[xx,yy,3]>0)
				number_of_padlocks_involved_in_explosion++;

			if (board_array_master[xx,yy,4] > 0)//if this is a bonus, trigger it!
				{
				//Debug.Log("annotate explosion trigger bonus");
				tile_C tile_script = (tile_C)tiles_array[xx,yy].GetComponent("tile_C");
					tile_script.Trigger_bonus(false);
				}
			}
		}

	void Calculate_primary_explosion_score(int number_of_elements_to_damage_temp)
	{
		this_is_the_primary_explosion = false;
		
		if (explode_same_color_again_with == 0) //no same color
		{
			//Debug.Log(n_gems_exploded_with_main_gem + " and " + n_gems_exploded_with_minor_gem);
			if ((n_gems_exploded_with_main_gem > 0) && (n_gems_exploded_with_minor_gem > 0))
				score_of_this_turn_move = score_reward_for_explode_gems[n_gems_exploded_with_main_gem-3] + score_reward_for_explode_gems[n_gems_exploded_with_minor_gem-3]; // "-3" because the array length (because 0, 1 and 2 explosion are impossible)
			else if (n_gems_exploded_with_main_gem > 0)
				score_of_this_turn_move = score_reward_for_explode_gems[n_gems_exploded_with_main_gem-3];
			else if (n_gems_exploded_with_minor_gem > 0)
				score_of_this_turn_move = score_reward_for_explode_gems[n_gems_exploded_with_minor_gem-3];
		}
		else if (explode_same_color_again_with == 1) //same color with main gem
		{
			score_of_this_turn_move = score_reward_for_explode_gems[n_gems_exploded_with_main_gem-3];
			
			if (player_turn)
				score_of_this_turn_move += (int)Math.Ceiling(player_explode_same_color_n_turn*score_reward_for_explode_gems_of_the_same_color_in_two_or_more_turns_subsequently);
			else
				score_of_this_turn_move += (int)Math.Ceiling(enemy_explode_same_color_n_turn*score_reward_for_explode_gems_of_the_same_color_in_two_or_more_turns_subsequently);
			
			if (n_gems_exploded_with_minor_gem > 0)
				score_of_this_turn_move += score_reward_for_explode_gems[n_gems_exploded_with_minor_gem-3];
		}
		else if (explode_same_color_again_with == 2) //same color with minor gem
		{
			score_of_this_turn_move = score_reward_for_explode_gems[n_gems_exploded_with_minor_gem-3];
			
			if (player_turn)
				score_of_this_turn_move += (int)Math.Ceiling(player_explode_same_color_n_turn*score_reward_for_explode_gems_of_the_same_color_in_two_or_more_turns_subsequently);
			else
				score_of_this_turn_move += (int)Math.Ceiling(enemy_explode_same_color_n_turn*score_reward_for_explode_gems_of_the_same_color_in_two_or_more_turns_subsequently);
			
			if (n_gems_exploded_with_main_gem > 0)
				score_of_this_turn_move += score_reward_for_explode_gems[n_gems_exploded_with_main_gem-3];
		}
		
		if (player_turn)
		{
			player_score += score_of_this_turn_move;
			if (praise_the_player)
			{
				if (for_big_explosion)
				{
					if (gems_useful_moved == 1 && number_of_elements_to_damage_temp > 3)
					{
						praise_obj.SetActive(true);
						praise_script.Big_explosion(number_of_elements_to_damage_temp);
					}
					else if ((gems_useful_moved == 2)
					         &&(n_gems_exploded_with_main_gem > 3 || n_gems_exploded_with_minor_gem > 3))
					{
						if (n_gems_exploded_with_main_gem >= n_gems_exploded_with_minor_gem)
						{
							praise_obj.SetActive(true);
							praise_script.Big_explosion(n_gems_exploded_with_main_gem);
						}
						else
						{
							praise_obj.SetActive(true);
							praise_script.Big_explosion(n_gems_exploded_with_minor_gem);
						}
					}
				}
				
				if (gain_turn_if_explode_same_color_of_previous_move || number_of_elements_to_damage_temp <= 3)
				{
					if (for_explode_same_color_again && player_explode_same_color_n_turn > 0)
					{
						praise_obj.SetActive(true);
						praise_script.Combo_color(player_explode_same_color_n_turn);
					}
				}
				
			}
		}
		else
			enemy_score += score_of_this_turn_move;
	}

	void Calculate_secondary_explosion_score(int number_of_elements_to_damage_temp)
	{
		score_of_this_turn_move = 0;
		score_of_this_turn_move = (int)Math.Ceiling(score_reward_for_each_explode_gems_in_secondary_explosion*number_of_elements_to_damage*(1+(n_combo*score_reward_for_secondary_combo_explosions)));
		if (player_turn)
			player_score += score_of_this_turn_move;
		else
			enemy_score += score_of_this_turn_move;
		
		if (gain_turn_if_secondary_explosion && number_of_elements_to_damage >= seconday_explosion_maginiture_needed_to_gain_a_turn && n_combo >= combo_lenght_needed_to_gain_a_turn && bonus_select == bonus.none)
		{
			Gain_turns(1);
		}
	}

	void Calculate_score(int number_of_elements_to_damage_temp)
	{
		if (this_is_the_primary_explosion)//calculate score
		{
			Calculate_primary_explosion_score(number_of_elements_to_damage_temp);
		}
		else //this is a secondary explosion
		{
			Calculate_secondary_explosion_score(number_of_elements_to_damage_temp);
		}
		
		Update_score();
		
		if (win_requirement_selected == win_requirement.reach_target_score)
		{
			if (player_turn && (player_score >= target_score))
			{
				Player_win();
			}
		}
		
		if (lose_requirement_selected == lose_requirement.enemy_reach_target_score)
		{
			if (!player_turn && (enemy_score >= target_score))
			{
				Player_lose();
			}
		}
	}
	

	public void Order_to_gems_to_explode() 
		{
		//Debug.Log("Order_to_gems_to_explode() ");
		//Debug.Log("number_of_padlocks_involved_in_explosion = " +number_of_padlocks_involved_in_explosion);

		Cancell_hint();
		int number_of_elements_to_damage_temp = number_of_elements_to_damage;
		Calculate_score (number_of_elements_to_damage_temp);

		//Debug.Log("number_of_elements_to_damage_temp: " + number_of_elements_to_damage_temp + " *** elements_to_damage_array:" + elements_to_damage_array.Length);
		for (int n = 0; n < number_of_elements_to_damage_temp ; n++)
			{
				tile_C script_gem = (tile_C)elements_to_damage_array[n].GetComponent("tile_C");
					script_gem.Explosion();
			}
		}
	#endregion
	
	#region 4- aftermath of the explosion(s)
	public void Debug_Board()
	{
		/*
		for (int y = 0; y < _Y_tiles; y++)
		{
			for (int x = 0; x < _X_tiles; x++)
			{
				tiles_array[x,y].GetComponent<Renderer>().material.color = Color.white;
				
			}
		}
*/
		for (int y = 0; y < _Y_tiles; y++)
		{
			for (int x = 0; x < _X_tiles; x++)
			{
				tiles_array[x,y].GetComponent<Renderer>().material.color = Color.white;
				
			}
		}
		/*
		for (int y = 0; y < _Y_tiles; y++)
		{
			for (int x = 0; x < _X_tiles; x++)
			{

				if (board_array_master[x,y,11] != 0)
					tiles_array[x,y].GetComponent<Renderer>().material.color = Color.red;
			}
		}*/

		for (int y = 0; y < _Y_tiles; y++)
		{
			for (int x = 0; x < _X_tiles; x++)
			{
				
				tile_C tile_script = (tile_C)tiles_array[x,y].GetComponent("tile_C");
				tile_script.Debug_show_my_color();
				
			}
		}
		//Debug.Break ();
		/*
		for (int y = 0; y < _Y_tiles; y++)
		{
			for (int x = 0; x < _X_tiles; x++)
			{
				tiles_array[x,y].GetComponent<Renderer>().material.color = Color.white;
				
			}
		}
		 */

		Start_update_board ();
	}


	public void Start_update_board()//call from "tile_C.If_all_explosion_are_completed()"
	{
		//Debug.Log ("Start_update_board " + number_of_gems_to_move + " *** " + number_of_new_gems_to_create);

		//one_step_update_ongoing = true;

		//if (!player_can_move_when_gem_falling)
			cursor.gameObject.SetActive(false);

		if (diagonal_falling)
		{
			//check new gem to generate
			for (int y = 0; y < _Y_tiles; y++)
			{
				for (int x = 0; x < _X_tiles; x++)
				{

					if ((board_array_master[x,y,12] == 1)//this is a leader tile
					    && (board_array_master[x,y,13] ==  0) //not check yet
					    && (board_array_master[x,y,1] == -99)) //empty
					{
						board_array_master[x,y,11] = 2;//gem creation
						board_array_master[x,y,13] = 1;
						number_of_new_gems_to_create++;
						//tiles_array[x,y].GetComponent<Renderer>().material.color = Color.green;

						//empty tile are reserved to this generated new gem
						for(int yy = y+1; yy < _Y_tiles; yy++)
						{
							if ((board_array_master[x,yy,0] > -1) && (board_array_master[x,yy,1] == -99))
							{
								board_array_master[x,yy,13] = 1;
								//tiles_array[x,yy].GetComponent<Renderer>().material.color = Color.yellow;
							}
							else
								break;
						}
					}
				}
			}

			//check vertical falling
			for (int y = 0; y < _Y_tiles-1; y++)//don't check last line
			{
				for (int x = 0; x < _X_tiles; x++)
				{
					if ((board_array_master[x,y,13] ==  0) && (board_array_master[x,y,10] == 1))//this could be fall
					{
						//check if have an empty tile under
						if ((board_array_master[x,y+1,0] > -1) && (board_array_master[x,y+1,1] == -99))
						{
							//vertical fall
							//tiles_array[x,y].GetComponent<Renderer>().material.color = Color.gray;
							board_array_master[x,y,11] = 3;//vertical fall
							board_array_master[x,y,13] = 1;//tile checked
							number_of_gems_to_move++;

							//all empty tiles under this gem are reserved to it
							for(int yy = y+1; yy < _Y_tiles; yy++)
							{
								if ((board_array_master[x,yy,0] > -1) && (board_array_master[x,yy,1] == -99))
								{
								board_array_master[x,yy,13] = 1;
								//tiles_array[x,yy].GetComponent<Renderer>().material.color = Color.grey;
								}
								else
									break;
							}
						}
					}
				}
			}
			//check diagonal falling
			diagonal_falling_preference_direction_R = !diagonal_falling_preference_direction_R;
			for (int y = 0; y < _Y_tiles-1; y++)//don't check last line
			{
				for (int x = 0; x < _X_tiles; x++)
				{
					if ((board_array_master[x,y,13] ==  0) && (board_array_master[x,y,10] == 1))//this could be fall
					{
						{
						if (diagonal_falling_preference_direction_R)
							{
								Diagonal_fall_R(x,y);
								Diagonal_fall_L(x,y);
							}
						else
							{
								Diagonal_fall_L(x,y);
								Diagonal_fall_R(x,y);
							}
							
						}
					}
				}
			}

			//Debug.Log ("Start_update_board " + number_of_gems_to_move + " *** " + number_of_new_gems_to_create);

			//Debug.Break();

			//now you know what gems must be fall, so...
			Update_board_by_one_step();

		}
		else //diagonal falling not allowed
		{
			//read board form down to up (and left to right)
			for (int y = _Y_tiles-1; y >= 0; y--)
			{
				for (int x = 0; x < _X_tiles; x++)
				{
					if ( (board_array_master[x,y,0] > -1) && (board_array_master[x,y,13] == 0) )//if there is a tile unchecked...
					{
						//...and it is empty
						if (board_array_master[x,y,1] == -99)
							{
							tile_C tile_script = (tile_C)tiles_array[x,y].GetComponent("tile_C");
							tile_script.Count_how_much_gem_there_are_to_move_over_me();
							}
					}
				}
			}
			//now you know what gems must be fall, so...
			Start_gem_fall();
		}

	}

	void Diagonal_fall_R(int x, int y)
	{
		if (x+1 <_X_tiles)
		{
		if ((board_array_master[x+1,y+1,13] ==  0) && (board_array_master[x+1,y+1,0] > -1) && (board_array_master[x+1,y+1,1] == -99))
			{
			//tiles_array[x,y].GetComponent<Renderer>().material.color = Color.red;
			board_array_master[x,y,11] = 4;//R falling
			board_array_master[x,y,13] = 1;
			number_of_gems_to_move++;
			
			board_array_master[x+1,y+1,13] = 1;//reserved target position
			}
		}
	}

	void Diagonal_fall_L(int x, int y)
	{
		if (x-1 >= 0)
		{
			if ((board_array_master[x-1,y+1,13] ==  0) && (board_array_master[x-1,y+1,0] > -1) && (board_array_master[x-1,y+1,1] == -99))
			{
				//if (tiles_array[x,y].GetComponent<Renderer>().material.color != Color.red)
				//	tiles_array[x,y].GetComponent<Renderer>().material.color = Color.blue;
				//else
				//	tiles_array[x,y].GetComponent<Renderer>().material.color = Color.magenta;
				board_array_master[x,y,11] = 5;//L falling
				board_array_master[x,y,13] = 1;
				number_of_gems_to_move++;
				
				board_array_master[x-1,y+1,13] = 1;//reserved target position
			}
		}
	}

	void Update_board_by_one_step()
	{


		if ( (number_of_gems_to_move > 0) || (number_of_new_gems_to_create > 0) || (number_of_elements_to_damage > 0))
		{
			if (number_of_elements_to_damage > 0)
				{
				Cancell_hint();
				Calculate_score (number_of_elements_to_damage);
				}

			//Debug.Log ("Update_board_by_one_step()");
			//for (int y = 0; y < _Y_tiles; y++)
			for (int y = _Y_tiles-1; y >= 0; y--)//from bottom to top
			{
				for (int x = 0; x < _X_tiles; x++)
				{
					//Debug.Log(x + "," + y + " = " + board_array_master[x,y,11]);
					board_array_master[x,y,13] = 0;//no checked

					if (board_array_master[x,y,11] == 1)//destroy
					{
						tile_C tile_script = (tile_C)tiles_array[x,y].GetComponent("tile_C");
						tile_script.Explosion();
					}
					else if (board_array_master[x,y,11] == 2)//creation
					{
						//Debug.Log(x + "," + y + " = " + "create gem");
						tile_C tile_script = (tile_C)tiles_array[x,y].GetComponent("tile_C");
						tile_script.StartCoroutine(tile_script.Create_gem());
					}
					else if (board_array_master[x,y,11] >= 3)//falling
						{
						//Debug.Log(x + "," + y + " = " + "falling gem");
						tile_C tile_script = (tile_C)tiles_array[x,y].GetComponent("tile_C");
						tile_script.Fall_by_one_step(board_array_master[x,y,11]-3);
						}
				}
			}

		}
		else 
		{
			if (!gem_falling_ongoing)
				Check_secondary_explosions();
		}
	}

	void Start_gem_fall()//call from "board_C.Start_update_board()()"
	{
		if ( (number_of_gems_to_move > 0) || (number_of_new_gems_to_create > 0) )
			{
				gem_falling_ongoing = true;

				for (int y = _Y_tiles-1; y >= 0; y--)
					{
						for (int x = 0; x < _X_tiles; x++)
						{
						if	(board_array_master[x,y,0] > -1)//if there is a tile
							{
								//search free tiles
								if	((board_array_master[x,y,1] == -99)  //if this tile is empty
						   			 && (board_array_master[x,y,13] <= 0)) //and not yet checked
								{
									tile_C tile_script = (tile_C)tiles_array[x,y].GetComponent("tile_C");
										tile_script.Make_fall_all_free_gems_over_this_empty_tile();
								}
							}
						}
					}
				
			}
		else 
			{
			if (!gem_falling_ongoing)
				Check_ALL_possible_moves();
			}
	}

	void Check_bottom_tiles()
		{
		for (int i = 0; i < number_of_bottom_tiles[current_board_orientation]; i++)
			{
			bottom_tiles_array[current_board_orientation,i].Check_the_content_of_this_tile();
			}
		}

	public void Check_secondary_explosions()//after the creation of new gems, there are new triples to explode?
		{
		elements_to_damage_array = new GameObject[total_tiles];
		number_of_elements_to_damage = 0;
		number_of_padlocks_involved_in_explosion = 0;
		number_of_gems_to_move = 0;
		number_of_new_gems_to_create = 0;
		score_of_this_turn_move = 0;
		play_this_bonus_sfx = -1;

		if ( ( (trigger_by_select == trigger_by.inventory) && (number_of_bonus_on_board > 0) )
		    ||(number_of_token_on_board > 0)  
		    || (number_of_junk_on_board > 0))
			Check_bottom_tiles();

		for (int y = 0; y < _Y_tiles; y++)
        	{
	            for (int x = 0; x < _X_tiles; x++)
	            {
				//take advantage of this cycle to rest these variables:
				board_array_master[x,y,13] = 0;//tile not checked

				if ( (board_array_master[x,y,1] >= 0)  && (board_array_master[x,y,1] < 9) )//if there is a gem here
					{
					if ((x+1<_X_tiles)&&(x-1>=0))//horizontal
						if ( (board_array_master[x,y,1]==board_array_master[x+1,y,1])&&(board_array_master[x,y,1]==board_array_master[x-1,y,1]) ) //if the adjacent tiles have gems with the same color
							{
								Annotate_explosions(x,y);
								Annotate_explosions(x+1,y);
								if ( (x+2<_X_tiles) && (board_array_master[x+2,y,1] == board_array_master[x,y,1]) )
										Annotate_explosions(x+2,y);
								Annotate_explosions(x-1,y);
								if ( (x-2>=0) && (board_array_master[x-2,y,1] == board_array_master[x,y,1]) )
										Annotate_explosions(x-2,y);	
							}
					if ( ((y+1<_Y_tiles)&&(y-1>=0)) )//vertical
						if ( (board_array_master[x,y,1]==board_array_master[x,y+1,1])&&(board_array_master[x,y,1]==board_array_master[x,y-1,1]) )//if the adjacent tiles have gems with the same color
							{
								Annotate_explosions(x,y);
								Annotate_explosions(x,y+1);
								if ( (y+2<_Y_tiles) && (board_array_master[x,y+2,1] == board_array_master[x,y,1]) )
										Annotate_explosions(x,y+2);
								Annotate_explosions(x,y-1);
								if ( (y-2>=0) && (board_array_master[x,y-2,1] == board_array_master[x,y,1]) )
										Annotate_explosions(x,y-2);
							}
					}
				}
			}
		
		if (number_of_elements_to_damage > 0)//if there is at least an explosion
			{
			n_combo++;
			Add_time_bonus(time_bonus_for_secondary_explosion);
			Order_to_gems_to_explode();
			if (praise_the_player && for_secondary_explosions)
				{
				praise_obj.SetActive(true);
				praise_script.Combo_secondary_explosion(n_combo);
				}
			}
		else
			Check_ALL_possible_moves();
			
	
		}
	#endregion	

	#region enemy AI

	void Enemy_play()//call from Update_turn_order_after_a_bad_move(), Annotate_potential_moves()
	{
		Debug.Log("Enemy_turn");
		/*
		if (lose_requirement_selected == lose_requirement.player_hp_is_zero)
			use_armor = true;
		else
			use_armor = false;*/

		enemy_move_selected = -1;
		enemy_will_use_a_bonus = false;

		if (chance_of_use_best_move == 0)
			search_best_move = false;
		else if (chance_of_use_best_move == 100)
			search_best_move = true;
		else
			{
			if (UnityEngine.Random.Range(1,100) <= chance_of_use_best_move) 
				search_best_move = true;
			else
				search_best_move = false;
			}

				switch(enemy_AI_select)
				{
				case enemy_AI.random:
				if (UnityEngine.Random.Range(1,100) <= chance_of_use_bonus) 
					{
					temp_enemy_gem_count = new int[gem_length];
					Search_bonus(false);
					}
				if (enemy_will_use_a_bonus)
					{
					Debug.Log("random AI - bonus found, so use bonus");
					Enemy_use_bonus();
					}
				else
					{
					Debug.Log("random AI - bonus not found");
						//random choose among useful moves
						enemy_move_selected = UnityEngine.Random.Range(0,number_of_gems_moveable-1);

						if (lose_requirement_selected != lose_requirement.enemy_collect_gems)
							{
							Enemy_select_main_gem(enemy_move_selected);
							}
						else
							{
							if (number_of_gems_moveable>1)
								{
								//If I need the color choosed
								if (number_of_gems_collect_by_the_enemy[list_of_moves_possible[enemy_move_selected,3]] 
								    < number_of_gems_to_destroy_to_win[list_of_moves_possible[enemy_move_selected,3]])
									{
										Enemy_select_main_gem(enemy_move_selected);
									}
								else//I try to find a better color
									{
									int temp_count = 0;
									for (int i = 0; i < number_of_gems_moveable; i++)
										{
										if ((enemy_move_selected+i) < number_of_gems_moveable)
											{
											temp_count++;
											if (number_of_gems_collect_by_the_enemy[list_of_moves_possible[enemy_move_selected+i,3]] < number_of_gems_to_destroy_to_win[list_of_moves_possible[enemy_move_selected+i,3]])
												{
												enemy_move_selected = enemy_move_selected+i;
												Enemy_select_main_gem(enemy_move_selected);
												break;
												}
											}
										else 
											{
											if (number_of_gems_collect_by_the_enemy[list_of_moves_possible[i-temp_count,3]] < number_of_gems_to_destroy_to_win[list_of_moves_possible[i-temp_count,3]])
												{
												enemy_move_selected = i-temp_count;
												Enemy_select_main_gem(enemy_move_selected);
												break;
												}
											}
										}
									//If not exist a move with a color that I need, I make a random move
									if (avatar_main_gem == null)
										{
										Enemy_select_main_gem(enemy_move_selected);
										}
									}
								}

							else
								{
								Enemy_select_main_gem(enemy_move_selected);
								}

							}

						Enemy_select_minor_gem();

						Enemy_move();
					}



				break;

		
				case enemy_AI.collect_gems_from_less_to_more:

					Arrange_gems_by_necessity();
					Array.Reverse(enemy_AI_preference_order);
					Subdivide_moves_by_color();

					Enemy_decide_what_to_do();

				break;


				case enemy_AI.collect_gems_from_more_to_less:

					Arrange_gems_by_necessity();
					Subdivide_moves_by_color();

					Enemy_decide_what_to_do();

				break;


				case enemy_AI.dynamic_battle:

					if (use_armor)//search the gem with the color most effective againts the player armor
						{
							Arrange_gems_by_effectiveness_against_player_armor();
						}
					else
						{
							if ( (current_player_hp <= 3) || (current_player_hp <= current_enemy_hp) )
								Aggressive();
							else
								Defensive();
						}

					Subdivide_moves_by_color();

					Enemy_decide_what_to_do();

				break;


				case enemy_AI.by_hand_setup:

					Subdivide_moves_by_color();

					Enemy_decide_what_to_do();

				break;
				}

	}

	void Enemy_decide_what_to_do()
	{
		if (search_best_move)
		{
			//Debug.Log("search best move");
			Enemy_search_best_main_gem();
		}
		else
		{
			Enemy_decide_if_use_a_bonus_or_search_the_main_gem();
		}
		
		if (enemy_will_use_a_bonus)
		{
			//Debug.Log("bonus found, so use bonus");
			Enemy_use_bonus();
		}
		else
		{
			Enemy_select_main_gem(enemy_move_selected);
			Enemy_select_minor_gem();
			Enemy_move();
		}
	}

	void Enemy_search_best_main_gem()
	{
		//Debug.Log("Enemy_search_best_main_gem");
		//if there is a bonus and it can be used
		//else..
		if ( (!chain_turns_limit) || (current_enemy_chain_lenght < max_chain_turns) ) //if enemy can gain a turn
			{
			if (gain_turn_if_explode_more_than_3_gems)
				{
				//Debug.Log("enemy try to gain a turn with a big explosion");
				Search_big_explosion();
				if ((enemy_move_selected == -1) && (gain_turn_if_explode_same_color_of_previous_move)) //if no big explosion found, but same color give a turn...
					{
					//Debug.Log("enemy try to gain a turn using same color of previous move");
					Search_same_color_of_previous_move();
					}
				}
			else if ( (!gain_turn_if_explode_more_than_3_gems) && (gain_turn_if_explode_same_color_of_previous_move) )
				{
				//Debug.Log("enemy try to gain a turn using same color of previous move");
				if ( (enemy_previous_exploded_color[0] >= 0) || (enemy_previous_exploded_color[0] >= 0) )
					Search_same_color_of_previous_move();
				else
					{
					Search_big_move();
					}
				}
			else
				{
				Search_big_move();
				}
			}
		else
			{
			Search_big_move();
			}

		//if no move yet found...
		if (enemy_move_selected == -1)
			{
			Enemy_decide_if_use_a_bonus_or_search_the_main_gem();
			}
		else //check if there is a bonus that can do a better result
			{
			if (UnityEngine.Random.Range(1,100) <= chance_of_use_bonus) 
				{
				temp_enemy_gem_count = new int[gem_length];
				Search_bonus(true);
				}
			}

	}

	void Enemy_decide_if_use_a_bonus_or_search_the_main_gem()
	{
		if (UnityEngine.Random.Range(1,100) <= chance_of_use_bonus) 
			{
			Debug.Log("use bonus if there is one");
			temp_enemy_gem_count = new int[gem_length];
			Search_bonus(false);
			}
		
		//if (!enemy_will_use_a_click_bonus_on_board && ! enemy_will_switch_a_gem_to_trigger_a_bonus_on_board)
		if (!enemy_will_use_a_bonus)
			{
			Debug.Log("no utilizable bonus found, so search main gem");
			Enemy_search_main_gem();
			}
	}



	void Enemy_use_bonus()
		{
		Debug.Log("enemy use bonus in " + enemy_bonus_click_1_x + "," +enemy_bonus_click_1_y);

			if (give_bonus_select == give_bonus.after_charge) 
				{
					bonus_select = enemy_bonus_slot[enemy_chosen_bonus_slot];
					if (enemy_chosen_bonus_slot >= 0)
						{
						gui_enemy_bonus_ico_array[enemy_chosen_bonus_slot].GetComponent<bonus_button>().Activate();

						if (enemy_bonus_click_1_x >= 0)//if this bonus require a click on the board
							{
							tile_C script_main_gem = (tile_C)tiles_array[enemy_bonus_click_1_x,enemy_bonus_click_1_y].GetComponent("tile_C");
								script_main_gem.Invoke("Try_to_use_bonus_on_this_tile",enemy_move_delay);
							}
						}
				}
			else if (trigger_by_select == trigger_by.inventory)
				{
				Debug.Log("enemy_chosen_bonus_slot: " + enemy_bonus_inventory_script.bonus_list[enemy_chosen_bonus_slot].name);

				enemy_bonus_inventory_script.bonus_list[enemy_chosen_bonus_slot].GetComponent<inventory_bonus_button>().Activate();

				if (enemy_bonus_click_1_x >= 0)//if this bonus require a click on the board
					{
					tile_C script_main_gem = (tile_C)tiles_array[enemy_bonus_click_1_x,enemy_bonus_click_1_y].GetComponent("tile_C");
						script_main_gem.Invoke("Try_to_use_bonus_on_this_tile",enemy_move_delay);
					}
				}
				else if ((give_bonus_select == give_bonus.after_big_explosion) || (give_bonus_select == give_bonus.from_stage_file_or_from_gem_emitter))
				{
					if (trigger_by_select == trigger_by.click)
						{
						cursor.position = tiles_array[enemy_bonus_click_1_x,enemy_bonus_click_1_y].transform.position;
						tile_C script_main_gem = (tile_C)tiles_array[enemy_bonus_click_1_x,enemy_bonus_click_1_y].GetComponent("tile_C");
							script_main_gem.Invoke("Enemy_click_on_this_bonus",enemy_move_delay);
						}
					else if ((trigger_by_select == trigger_by.switch_adjacent_gem) || (trigger_by_select == trigger_by.free_switch))
						{
							Debug.Log("switch_a_gem_to_trigger_a_bonus");
							cursor.position = tiles_array[enemy_bonus_click_1_x,enemy_bonus_click_1_y].transform.position;
							
							if (enemy_bonus_click_1_y < enemy_bonus_click_2_y)
								enemy_move_direction = 4;
							else if (enemy_bonus_click_1_y > enemy_bonus_click_2_y)
								enemy_move_direction = 5;
							else if (enemy_bonus_click_1_x < enemy_bonus_click_2_x)
								enemy_move_direction = 6;
							else if (enemy_bonus_click_1_x > enemy_bonus_click_2_x)
								enemy_move_direction = 7;
							
							tile_C script_main_gem = (tile_C)tiles_array[enemy_bonus_click_1_x,enemy_bonus_click_1_y].GetComponent("tile_C");
							script_main_gem.I_become_main_gem();
							
							tile_C script_minor_gem = (tile_C)tiles_array[enemy_bonus_click_2_x,enemy_bonus_click_2_y].GetComponent("tile_C");
							script_minor_gem.I_become_minor_gem(enemy_move_direction);
						}
				}

		}

	void Search_bonus(bool compare_with_big_move_value)
	{
		//Debug.Log("Search_bonus");
		//reset variables before use
			bonus_select = bonus.none;
			enemy_chosen_bonus_slot = -1;
			enemy_bonus_click_1_x = -1;
			enemy_bonus_click_1_y = -1;
			enemy_bonus_click_2_x = -1;
			enemy_bonus_click_2_y = -1;



		if (give_bonus_select == give_bonus.after_charge)
			Enemy_check_charge_bonus(compare_with_big_move_value);
		else if ( (give_bonus_select == give_bonus.after_big_explosion) || (give_bonus_select == give_bonus.from_stage_file_or_from_gem_emitter) )
			{
			if (trigger_by_select == trigger_by.inventory)
				Enemy_check_inventory_bonus(compare_with_big_move_value);
			else
				{
				if (number_of_bonus_on_board > 0)
					{
					Locate_all_bonus_on_board();
					if (trigger_by_select == trigger_by.click)
						Enemy_check_on_board_click_bonus(compare_with_big_move_value);
					else if (trigger_by_select == trigger_by.switch_adjacent_gem)
						Enemy_check_on_board_switch_bonus(compare_with_big_move_value);
					else if (trigger_by_select == trigger_by.free_switch)
						Enemy_check_on_board_free_switch_bonus(compare_with_big_move_value);
					}
				}
			}

		
	}

	void Enemy_charge_bonus_heal_hp(int my_slot)
		{
		Debug.Log("Enemy_charge_bonus_heal_hp()");
		if (win_requirement_selected == win_requirement.enemy_hp_is_zero)
			{
			bool already_checked = false;
			if ((current_enemy_hp + heal_hp_enemy_bonus) < max_enemy_hp)//if it is useful heal the enemy now
				{
				if (lose_requirement_selected == lose_requirement.player_hp_is_zero)
					{
					already_checked = true;
					if (current_player_hp > gem_damage_value*3)//enemy can't win with next move
						{
						Enemy_valutate_if_use_heal(my_slot);
						}
					}
				if (!already_checked)
					Enemy_valutate_if_use_heal(my_slot);
				}
			}
		}

	void Enemy_valutate_if_use_heal(int my_slot)
	{
		float necessity = 1;

		if (current_enemy_hp < current_player_hp) //enemy is in disadvantage
			necessity = 1.5f;

		if (current_enemy_hp < gem_damage_value*3) //enemy is nearly dead
			necessity = 2;

		if (heal_hp_enemy_bonus*necessity > best_value)
			{
			best_value = heal_hp_enemy_bonus;
			enemy_chosen_bonus_slot = my_slot;
			Debug.Log("enemy use heal");
			}
	}

	void Enemy_charge_bonus_destroy_all_gem_with_this_color(int i)
	{
		int[] temp_value_for_every_color = new int[gem_length];
		int select_this_color = -1;
		temp_value = 0;
		temp_enemy_gem_count = new int[gem_length];
		
		//count how much gems for every color there are on board
		for (int y = 0; y < _Y_tiles; y++)
		{
			for (int x = 0; x <  _X_tiles; x++)
			{
				if ((board_array_master[x,y,1] >= 0) && (board_array_master[x,y,1] < 9)) //this tile have something in itself
				{
					if (lose_requirement_selected == lose_requirement.player_hp_is_zero)
						temp_value_for_every_color[board_array_master[x,y,1]]  += Calculate_damage_of_this_gem(board_array_master[x,y,1]);
					else if (lose_requirement_selected == lose_requirement.enemy_collect_gems)
					{
						if (Check_if_enemy_need_this_gem_color(board_array_master[x,y,1]))
							temp_value_for_every_color[board_array_master[x,y,1]]++;
					}
					else
					{
						if (board_array_master[x,y,1] < 9)
							temp_value_for_every_color[board_array_master[x,y,1]]++;
					}
				}
			}
		}
		
		
		//search the best color
		int random_start_point = UnityEngine.Random.Range(0,gem_length-1);;
		for (int g = random_start_point; g < gem_length; g++)
		{
			if (temp_value < temp_value_for_every_color[g])
			{
				temp_value = temp_value_for_every_color[g];
				select_this_color = g;
				//Debug.Log("color " + g + " will give: " + temp_value );
			}
		}
		for (int g = 0; g < random_start_point; g++)
		{
			if (temp_value < temp_value_for_every_color[g])
			{
				temp_value = temp_value_for_every_color[g];
				select_this_color = g;
				//Debug.Log("(form 0 to random color) color " + g + " will give: " + temp_value );
			}
		}
		
		
		//search a tile with that color
		if (temp_value > best_value)
		{
			for (int y = 0; y < _Y_tiles; y++)
			{
				for (int x = 0; x <  _X_tiles; x++)
				{
					if (board_array_master[x,y,1] == select_this_color)
					{
						enemy_chosen_bonus_slot = i;
						enemy_bonus_click_1_x = x;
						enemy_bonus_click_1_y = y;
						best_value = temp_value;
					}
				}
			}
		}
	}

	void Enemy_charge_bonus_destroy_horizontal_and_vertical(int random_start_point_x, int random_start_point_y, int i, bool search_best_place)
	{
		if (best_value < max_value_destroy_horizontal_and_vertical)
		{
			//Debug.Log("bonus.destroy_horizontal_and_vertical");
			for (int y = random_start_point_y; y < _Y_tiles; y++)
			{
				for (int x = random_start_point_x; x < _X_tiles; x++)
				{
					if (board_array_master[x,y,1] >= 0) //this tile have something in itself
					{
						temp_value = 0;
						if (lose_requirement_selected == lose_requirement.player_hp_is_zero)
						{
							temp_value += Calculate_damage_of_this_gem(board_array_master[x,y,1]);
							
							temp_value += Calculate_horiz_damage_R(x,y);
							temp_value += Calculate_horiz_damage_L(x,y);
							
							temp_value += Calculate_up_damage(x,y);
							temp_value += Calculate_down_damage(x,y);
						}
						else if (lose_requirement_selected == lose_requirement.enemy_collect_gems)
						{
							temp_enemy_gem_count = new int[gem_length];
							if (Check_if_enemy_need_this_gem_color(board_array_master[x,y,1]))
								temp_value++;
							
							temp_value += Calculate_horiz_gems_R(x,y);
							temp_value += Calculate_horiz_gems_L(x,y);
							
							temp_value += Calculate_up_gems(x,y);
							temp_value += Calculate_down_gems(x,y);
						}
						else
						{
							temp_value++;
							temp_value += R_how_much_gems_will_explode(x,y);
							temp_value += L_how_much_gems_will_explode(x,y);
							
							temp_value += Up_how_much_gems_will_explode(x,y);
							temp_value += Down_how_much_gems_will_explode(x,y);
						}
						
						
						if (temp_value > best_value)
						{
							enemy_chosen_bonus_slot = i;
							enemy_bonus_click_1_x = x;
							enemy_bonus_click_1_y = y;
							best_value = temp_value;
							//Debug.Log("best value = " + best_value + " in " + x + "," + y);

							if (!search_best_place)
								return;
							
							if (best_value >= max_value_destroy_vertical) //you can't overcome this outcome, so don't check further
							{
								//Debug.Log("vertical and horizontal stop this check at " + x + "," + y);
								break;
							}
						}
						
					}
				}
			}
			if (best_value < max_value_destroy_vertical)
			{
				if (random_start_point_x > 0 || random_start_point_y > 0)
				{
					//Debug.Log("continue search from 0,0");
					for (int y = 0; y < random_start_point_y; y++)
					{
						for (int x = 0; x < random_start_point_x; x++)
						{
							if (board_array_master[x,y,1] >= 0) //this tile have something in itself
							{
								temp_value = 0;
								if (lose_requirement_selected == lose_requirement.player_hp_is_zero)
								{
									temp_value += Calculate_damage_of_this_gem(board_array_master[x,y,1]);
									
									temp_value += Calculate_horiz_damage_R(x,y);
									temp_value += Calculate_horiz_damage_L(x,y);
									
									temp_value += Calculate_up_damage(x,y);
									temp_value += Calculate_down_damage(x,y);
								}
								else if (lose_requirement_selected == lose_requirement.enemy_collect_gems)
								{
									temp_enemy_gem_count = new int[gem_length];
									if (Check_if_enemy_need_this_gem_color(board_array_master[x,y,1]))
										temp_value++;
									
									temp_value += Calculate_horiz_gems_R(x,y);
									temp_value += Calculate_horiz_gems_L(x,y);
									
									temp_value += Calculate_up_gems(x,y);
									temp_value += Calculate_down_gems(x,y);
								}
								else
								{
									temp_value++;
									temp_value += R_how_much_gems_will_explode(x,y);
									temp_value += L_how_much_gems_will_explode(x,y);
									
									temp_value += Up_how_much_gems_will_explode(x,y);
									temp_value += Down_how_much_gems_will_explode(x,y);
								}
								
								if (temp_value > best_value)
								{
									enemy_chosen_bonus_slot = i;
									enemy_bonus_click_1_x = x;
									enemy_bonus_click_1_y = y;
									best_value = temp_value;
									//Debug.Log("NEW best value = " + best_value + " in " + x + "," + y);
								}
							}
						}
					}
				}
			}
		}
	}

	void Enemy_charge_bonus_destroy_vertical(int random_start_point_x, int random_start_point_y, int i, bool search_best_place)
	{
		if (best_value < max_value_destroy_vertical)
		{
			//Debug.Log("bonus.destroy_vertical");
			for (int y = random_start_point_y; y < _Y_tiles; y++)
			{
				for (int x = random_start_point_x; x < _X_tiles; x++)
				{
					if (board_array_master[x,y,1] >= 0) //this tile have something in itself
					{
						temp_value = 0;
						if (lose_requirement_selected == lose_requirement.player_hp_is_zero)
						{
							temp_value += Calculate_damage_of_this_gem(board_array_master[x,y,1]);
							
							temp_value += Calculate_up_damage(x,y);
							temp_value += Calculate_down_damage(x,y);
						}
						else if (lose_requirement_selected == lose_requirement.enemy_collect_gems)
						{
							temp_enemy_gem_count = new int[gem_length];
							if (Check_if_enemy_need_this_gem_color(board_array_master[x,y,1]))
								temp_value++;
							
							temp_value += Calculate_up_gems(x,y);
							temp_value += Calculate_down_gems(x,y);
						}
						else
						{
							temp_value++;
							temp_value += Up_how_much_gems_will_explode(x,y);
							temp_value += Down_how_much_gems_will_explode(x,y);
						}
						
						if (temp_value > best_value)
						{
							enemy_chosen_bonus_slot = i;
							enemy_bonus_click_1_x = x;
							enemy_bonus_click_1_y = y;
							best_value = temp_value;
							//Debug.Log("best value = " + best_value + " in " + x + "," + y);

							if (!search_best_place)
								return;
							
							if (best_value >= max_value_destroy_vertical) //you can't overcome this outcome, so don't check further
							{
								//Debug.Log("vertical stop this check at " + x + "," + y);
								break;
							}
						}
						
					}
				}
			}
			if (best_value < max_value_destroy_vertical)
			{
				if (random_start_point_x > 0 || random_start_point_y > 0)
				{
					//	Debug.Log("continue search from 0,0");
					for (int y = 0; y < random_start_point_y; y++)
					{
						for (int x = 0; x < random_start_point_x; x++)
						{
							if (board_array_master[x,y,1] >= 0) //this tile have something in itself
							{
								temp_value = 0;
								if (lose_requirement_selected == lose_requirement.player_hp_is_zero)
								{
									temp_value += Calculate_damage_of_this_gem(board_array_master[x,y,1]);
									
									temp_value += Calculate_up_damage(x,y);
									temp_value += Calculate_down_damage(x,y);
								}
								else if (lose_requirement_selected == lose_requirement.enemy_collect_gems)
								{
									temp_enemy_gem_count = new int[gem_length];
									if (Check_if_enemy_need_this_gem_color(board_array_master[x,y,1]))
										temp_value++;
									
									temp_value += Calculate_up_gems(x,y);
									temp_value += Calculate_down_gems(x,y);
								}
								else
								{
									temp_value++;
									temp_value += Up_how_much_gems_will_explode(x,y);
									temp_value += Down_how_much_gems_will_explode(x,y);
								}
								
								if (temp_value > best_value)
								{
									enemy_chosen_bonus_slot = i;
									enemy_bonus_click_1_x = x;
									enemy_bonus_click_1_y = y;
									best_value = temp_value;
									//Debug.Log("NEW best value = " + best_value + " in " + x + "," + y);
								}
							}
						}
					}
				}
			}
			//else
			//	Debug.Log("vertical skip check from 0,0");
		}
	}

	void Enemy_charge_bonus_destroy_horizontal(int random_start_point_x, int random_start_point_y, int i, bool search_best_place)
	{
		if (best_value < max_value_destroy_horizontal)
		{
			//Debug.Log("bonus.destroy_horizontal");
			for (int y = random_start_point_y; y < _Y_tiles; y++)
			{
				for (int x = random_start_point_x; x < _X_tiles; x++)
				{
					if (board_array_master[x,y,1] >= 0) //this tile have something in itself
					{
						temp_value = 0;
						if (lose_requirement_selected == lose_requirement.player_hp_is_zero)
						{
							temp_value += Calculate_damage_of_this_gem(board_array_master[x,y,1]);
							
							temp_value += Calculate_horiz_damage_R(x,y);
							temp_value += Calculate_horiz_damage_L(x,y);
						}
						else if (lose_requirement_selected == lose_requirement.enemy_collect_gems)
						{
							temp_enemy_gem_count = new int[gem_length];
							if (Check_if_enemy_need_this_gem_color(board_array_master[x,y,1]))
								temp_value++;
							
							temp_value += Calculate_horiz_gems_R(x,y);
							temp_value += Calculate_horiz_gems_L(x,y);
						}
						else
						{
							temp_value++;
							temp_value += R_how_much_gems_will_explode(x,y);
							temp_value += L_how_much_gems_will_explode(x,y);
						}
						
						
						if (temp_value > best_value)
						{
							enemy_chosen_bonus_slot = i;
							enemy_bonus_click_1_x = x;
							enemy_bonus_click_1_y = y;
							best_value = temp_value;
							//Debug.Log("best value = " + best_value + " in " + x + "," + y);

							if (!search_best_place)
								return;
							
							if (best_value >= max_value_destroy_horizontal) //you can't overcome this outcome, so don't check further
							{
								//Debug.Log("horizontal stop this check at " + x + "," + y);
								break;
							}
						}
					}
				}
			}
			if (best_value < max_value_destroy_horizontal)
			{
				if (random_start_point_x > 0 || random_start_point_y > 0)
				{
					//Debug.Log("continue search from 0,0");
					for (int y = 0; y < random_start_point_y; y++)
					{
						for (int x = 0; x < random_start_point_x; x++)
						{
							if (board_array_master[x,y,1] >= 0) //this tile have something in itself
							{
								temp_value = 0;
								if (lose_requirement_selected == lose_requirement.player_hp_is_zero)
								{
									temp_value += Calculate_damage_of_this_gem(board_array_master[x,y,1]);
									
									temp_value += Calculate_horiz_damage_R(x,y);
									temp_value += Calculate_horiz_damage_L(x,y);
								}
								else if (lose_requirement_selected == lose_requirement.enemy_collect_gems)
								{
									temp_enemy_gem_count = new int[gem_length];
									if (Check_if_enemy_need_this_gem_color(board_array_master[x,y,1]))
										temp_value++;
									
									temp_value += Calculate_horiz_gems_R(x,y);
									temp_value += Calculate_horiz_gems_L(x,y);
								}
								else
								{
									temp_value++;
									temp_value += R_how_much_gems_will_explode(x,y);
									temp_value += L_how_much_gems_will_explode(x,y);
								}
								
								if (temp_value > best_value)
								{
									enemy_chosen_bonus_slot = i;
									enemy_bonus_click_1_x = x;
									enemy_bonus_click_1_y = y;
									best_value = temp_value;
									//Debug.Log("NEW best value = " + best_value + " in " + x + "," + y);
								}
							}
						}
					}
				}
			}
			//else
			//Debug.Log("horizontal skip check from 0,0");
		}
	}

	void Enemy_charge_bonus_destroy_3x3(int random_start_point_x, int random_start_point_y, int i, bool search_best_place)
	{
		if (best_value < max_value_destroy_3x3) //skip this bonus if it can't overcome the current best value
		{

			//Debug.Log("bonus.destroy_3x3");
			for (int y = random_start_point_y; y < _Y_tiles; y++)
			{
				for (int x = random_start_point_x; x < _X_tiles; x++)
				{
					if (board_array_master[x,y,1] >= 0) //this tile have something in itself
					{
						temp_value = 0;
						
						if (lose_requirement_selected == lose_requirement.player_hp_is_zero)
							temp_value = Calculate_bomb_damage(x,y);
						else if (lose_requirement_selected == lose_requirement.enemy_collect_gems)
						{
							temp_enemy_gem_count = new int[gem_length];
							temp_value = Calculate_bomb_gems(x,y);
						}
						else
							temp_value = Bomb_how_much_gems_will_explode(x,y);
						
						if (temp_value > best_value)
						{
							enemy_chosen_bonus_slot = i;
							enemy_bonus_click_1_x = x;
							enemy_bonus_click_1_y = y;
							best_value = temp_value;

							if (!search_best_place)
								return;

							//Debug.Log("best value = " + best_value + " in " + x + "," + y);
							if (best_value >= max_value_destroy_3x3) //you can't overcome this outcome, so don't check further
							{
								//Debug.Log("bomb stop this check at " + x + "," + y);
								break;
							}
						}
						
					}
				}
			}
			//search from 0,0 to random point
			if (best_value < max_value_destroy_3x3)//if you don't have found the best value yet
			{
				if (random_start_point_x > 0 || random_start_point_y > 0)
				{
					//Debug.Log("continue search from 0,0");
					for (int y = 0; y < random_start_point_y; y++)
					{
						for (int x = 0; x < random_start_point_x; x++)
						{
							if (board_array_master[x,y,1] >= 0) //this tile have something in itself
							{
								temp_value = 0;
								
								if (lose_requirement_selected == lose_requirement.player_hp_is_zero)
									temp_value = Calculate_bomb_damage(x,y);
								else if (lose_requirement_selected == lose_requirement.enemy_collect_gems)
								{
									temp_enemy_gem_count = new int[gem_length];
									temp_value = Calculate_bomb_gems(x,y);
								}
								else
									temp_value = Bomb_how_much_gems_will_explode(x,y);
								
								if (temp_value > best_value)
								{
									enemy_chosen_bonus_slot = i;
									enemy_bonus_click_1_x = x;
									enemy_bonus_click_1_y = y;
									best_value = temp_value;
									//Debug.Log("NEW best value = " + best_value + " in " + x + "," + y);
								}
								
							}
						}
					}
				}
			}
			//	else
			//Debug.Log("bomb skip check from 0,0");
		}
	}

	void Enemy_charge_bonus_teleport(int random_start_point_x, int random_start_point_y, int i, bool search_best_place)
	{
		if (best_value < (max_value_switch_gem_teleport_click1 + max_value_switch_gem_teleport_click2 ))
		{
			//Debug.Log("bonus.switch_gem_teleport");
			
			int temp_value_teleport_click_1 = 0;
			int temp_best_value_teleport_click_1 = 0;
			int[] teleport_main_color_quantity = new int[gem_length];
			int main_explosion_color = -1;
			
			//search best click 1
			for (int y = random_start_point_y; y < _Y_tiles; y++)
			{
				for (int x = random_start_point_x; x < _X_tiles; x++)
				{
					if ((board_array_master[x,y,1] >= 0) && (board_array_master[x,y,1] <9)
					    && board_array_master[x,y,3] == 0) //no padlock
					{
						teleport_main_color_quantity = new int[gem_length];
						
						if ( (y-1>=0) && (board_array_master[x,y-1,1] >= 0) && (board_array_master[x,y-1,1] <9) )
						{
							teleport_main_color_quantity[board_array_master[x,y-1,1]]++;
							if ( (y-2>=0) && (board_array_master[x,y-1,1] == board_array_master[x,y-2,1]) )
								teleport_main_color_quantity[board_array_master[x,y-1,1]]++;
						}
						
						if ( ((y+1)< _Y_tiles)  && (board_array_master[x,y+1,1] >= 0) && (board_array_master[x,y+1,1] <9) )
						{
							teleport_main_color_quantity[board_array_master[x,y+1,1]]++;
							if ( ((y+2)< _Y_tiles) && (board_array_master[x,y+1,1] == board_array_master[x,y+2,1]) )
								teleport_main_color_quantity[board_array_master[x,y+1,1]]++;
						}
						
						if ( (x-1>=0) && (board_array_master[x-1,y,1] >= 0) && (board_array_master[x-1,y,1] <9) )
						{
							teleport_main_color_quantity[board_array_master[x-1,y,1]]++;
							if ( (x-2>=0) && (board_array_master[x-1,y,1] == board_array_master[x-2,y,1]) )
								teleport_main_color_quantity[board_array_master[x-1,y,1]]++;
						}
						
						if ( ((x+1)< _X_tiles)  && (board_array_master[x+1,y,1] >= 0) && (board_array_master[x+1,y,1] <9) )
						{
							teleport_main_color_quantity[board_array_master[x+1,y,1]]++;
							if ( ((x+2)< _X_tiles) && (board_array_master[x+1,y,1] == board_array_master[x+2,y,1]) )
								teleport_main_color_quantity[board_array_master[x+1,y,1]]++;
						}
						
						
						for (int c = 0; c < gem_length; c++)
						{
							if (teleport_main_color_quantity[c] >= 3) //if this is a big explosion 
							{
								//Debug.Log("in " + x +","+y + " *** teleport_main_color_quantity["+c+"] = " + teleport_main_color_quantity[c]);
								
								teleport_main_color_quantity[c]++;//count the gem that will be telepor there
								
								temp_value_teleport_click_1 = 0;
								
								if (lose_requirement_selected == lose_requirement.player_hp_is_zero)
									temp_value_teleport_click_1 = (Calculate_damage_of_this_gem(c)*teleport_main_color_quantity[c]);
								else if (lose_requirement_selected == lose_requirement.enemy_collect_gems)
								{
									temp_enemy_gem_count = new int[gem_length];
									for (int cc = 0; cc < teleport_main_color_quantity[c]; cc++)
									{
										if (Check_if_enemy_need_this_gem_color(c))
											temp_value_teleport_click_1++;
									}
								}
								else
									temp_value_teleport_click_1 = teleport_main_color_quantity[c]; 
								
								
								//Debug.Log("temp_value_teleport_click_1: " + temp_value_teleport_click_1 + " *** temp_best_value_teleport_click_1: " + temp_best_value_teleport_click_1);
								if (temp_value_teleport_click_1 > temp_best_value_teleport_click_1)
								{
									temp_best_value_teleport_click_1 = temp_value_teleport_click_1;
									enemy_bonus_click_1_x = x;
									enemy_bonus_click_1_y = y;
									main_explosion_color = teleport_main_color_quantity[c];

									if (!search_best_place)
										break;

									if (temp_best_value_teleport_click_1 < max_value_switch_gem_teleport_click1)
										break;
								}
								
							}	
						}
					}
				}
			}
			
			if (temp_best_value_teleport_click_1 < max_value_switch_gem_teleport_click1)
			{
				for (int y = 0; y < random_start_point_y; y++)
				{
					for (int x = 0; x < random_start_point_x; x++)
					{
						if ((board_array_master[x,y,1] >= 0) && (board_array_master[x,y,1] <9)
						    && board_array_master[x,y,3] == 0) //no padlock
						{
							teleport_main_color_quantity = new int[gem_length];
							
							
							if ( (y-1>=0) && (board_array_master[x,y-1,1] >= 0) && (board_array_master[x,y-1,1] <9) )
							{
								teleport_main_color_quantity[board_array_master[x,y-1,1]]++;
								if ( (y-2>=0) && (board_array_master[x,y-1,1] == board_array_master[x,y-2,1]) )
									teleport_main_color_quantity[board_array_master[x,y-1,1]]++;
							}
							
							if ( ((y+1)< _Y_tiles)  && (board_array_master[x,y+1,1] >= 0) && (board_array_master[x,y+1,1] <9) )
							{
								teleport_main_color_quantity[board_array_master[x,y+1,1]]++;
								if ( ((y+2)< _Y_tiles) && (board_array_master[x,y+1,1] == board_array_master[x,y+2,1]) )
									teleport_main_color_quantity[board_array_master[x,y+1,1]]++;
							}
							
							if ( (x-1>=0) && (board_array_master[x-1,y,1] >= 0) && (board_array_master[x-1,y,1] <9) )
							{
								teleport_main_color_quantity[board_array_master[x-1,y,1]]++;
								if ( (x-2>=0) && (board_array_master[x-1,y,1] == board_array_master[x-2,y,1]) )
									teleport_main_color_quantity[board_array_master[x-1,y,1]]++;
							}
							
							if ( ((x+1)< _X_tiles)  && (board_array_master[x+1,y,1] >= 0) && (board_array_master[x+1,y,1] <9) )
							{
								teleport_main_color_quantity[board_array_master[x+1,y,1]]++;
								if ( ((x+2)< _X_tiles) && (board_array_master[x+1,y,1] == board_array_master[x+2,y,1]) )
									teleport_main_color_quantity[board_array_master[x+1,y,1]]++;
							}
							
							
							for (int c = 0; c < gem_length; c++)
							{
								if (teleport_main_color_quantity[c] >= 3) //if this is a big explosion 
								{
									//Debug.Log("in " + x +","+y + " *** teleport_main_color_quantity["+c+"] = " + teleport_main_color_quantity[c]);
									
									teleport_main_color_quantity[c]++;//count the gem that will be telepor there
									
									temp_value_teleport_click_1 = 0;
									
									if (lose_requirement_selected == lose_requirement.player_hp_is_zero)
										temp_value_teleport_click_1 = (Calculate_damage_of_this_gem(c)*teleport_main_color_quantity[c]);
									else if (lose_requirement_selected == lose_requirement.enemy_collect_gems)
									{
										temp_enemy_gem_count = new int[gem_length];
										for (int cc = 0; cc < teleport_main_color_quantity[c]; cc++)
										{
											if (Check_if_enemy_need_this_gem_color(c))
												temp_value_teleport_click_1++;
										}
									}
									else
										temp_value_teleport_click_1 = teleport_main_color_quantity[c]; 
									
									
									//Debug.Log("temp_value_teleport_click_1: " + temp_value_teleport_click_1 + " *** temp_best_value_teleport_click_1: " + temp_best_value_teleport_click_1);
									if (temp_value_teleport_click_1 > temp_best_value_teleport_click_1)
									{
										temp_best_value_teleport_click_1 = temp_value_teleport_click_1;
										enemy_bonus_click_1_x = x;
										enemy_bonus_click_1_y = y;
										main_explosion_color = teleport_main_color_quantity[c];
										//	Debug.Log("teleport click 1: " + enemy_bonus_click_1_x + "," + enemy_bonus_click_1_y + " *** best value = " + temp_best_value_teleport_click_1);

										if (!search_best_place)
											break;

										if (temp_best_value_teleport_click_1 < max_value_switch_gem_teleport_click1)
											break;
									}
								}
							}
						}
					}
				}
			}
			
			
			
			//search best minor explosion IF the minor color not is absorb or repell for the player
			if (temp_best_value_teleport_click_1 > 0)
			{
				//Debug.Log("search secondary gem for teleport");
				int temp_value_teleport_click_2 = 0;
				int temp_best_value_teleport_click_2 = 0;
				
				//exclude the gems target from this seach
				bool[,] reserved_gem = new bool[_X_tiles,_Y_tiles];
				if (enemy_bonus_click_1_y-1>=0) 
				{
					reserved_gem[enemy_bonus_click_1_x,enemy_bonus_click_1_y-1] = true;
					
					if ( (enemy_bonus_click_1_y-2>=0) && (board_array_master[enemy_bonus_click_1_x,enemy_bonus_click_1_y-2,1] == main_explosion_color) )
						reserved_gem[enemy_bonus_click_1_x,enemy_bonus_click_1_y-2] = true;
					
				}
				
				
				if  ((enemy_bonus_click_1_y+1)< _Y_tiles)  
				{
					reserved_gem[enemy_bonus_click_1_x,enemy_bonus_click_1_y+1] = true;
					
					if ( ((enemy_bonus_click_1_y+2)< _Y_tiles) && (board_array_master[enemy_bonus_click_1_x,enemy_bonus_click_1_y+2,1] == main_explosion_color) )
						reserved_gem[enemy_bonus_click_1_x,enemy_bonus_click_1_y+2] = true;
				}
				
				if  (enemy_bonus_click_1_x-1>=0) 
				{
					reserved_gem[enemy_bonus_click_1_x-1,enemy_bonus_click_1_y] = true;
					
					if ( (enemy_bonus_click_1_x-2>=0) && (board_array_master[enemy_bonus_click_1_x-2,enemy_bonus_click_1_y,1] == main_explosion_color) )
						reserved_gem[enemy_bonus_click_1_x-2,enemy_bonus_click_1_y] = true;
				}
				
				
				if  ((enemy_bonus_click_1_x+1)< _X_tiles) 
				{
					reserved_gem[enemy_bonus_click_1_x+1,enemy_bonus_click_1_y] = true;
					
					if ( ((enemy_bonus_click_1_x+2)< _X_tiles) && (board_array_master[enemy_bonus_click_1_x+2,enemy_bonus_click_1_y,1] == main_explosion_color) )
						reserved_gem[enemy_bonus_click_1_x+2,enemy_bonus_click_1_y] = true;
				}
				
				bool secondary_color_must_explode = true;
				if ( (lose_requirement_selected == lose_requirement.player_hp_is_zero)
				    && Calculate_damage_of_this_gem(board_array_master[enemy_bonus_click_1_x,enemy_bonus_click_1_y,1]) <= 0)
					secondary_color_must_explode = false;
				
				if (secondary_color_must_explode)
				{
					
					//Debug.Log("the secondary color must explode");
					int minor_color_quantity = 0;
					
					for (int y = random_start_point_y; y < _Y_tiles; y++)
					{
						for (int x = random_start_point_x; x < _X_tiles; x++)
						{
							if (board_array_master[x,y,1] == main_explosion_color)//if this gem have the same color of the explosion that I want trigger
							{
								if ((board_array_master[x,y,1] >= 0) && (board_array_master[x,y,1] <9)
								    && board_array_master[x,y,3] == 0 //no padlock
								    && !reserved_gem[x,y])//no reserved gem
								{
									
									minor_color_quantity = 0;
									
									if ( (y-1>=0) && (board_array_master[x,y-1,1] == board_array_master[enemy_bonus_click_1_x,enemy_bonus_click_1_y,1]) )
									{
										minor_color_quantity++;
										if ( (y-2>=0) && (board_array_master[enemy_bonus_click_1_x,enemy_bonus_click_1_y,1] == board_array_master[x,y-2,1]) )
											minor_color_quantity++;
									}
									
									if ( ((y+1)< _Y_tiles)  && (board_array_master[x,y+1,1] == board_array_master[enemy_bonus_click_1_x,enemy_bonus_click_1_y,1]) )
									{
										minor_color_quantity++;
										if ( ((y+2)< _Y_tiles) && (board_array_master[enemy_bonus_click_1_x,enemy_bonus_click_1_y,1] == board_array_master[x,y+2,1]) )
											minor_color_quantity++;
									}
									
									if ( (x-1>=0) && (board_array_master[x-1,y,1] == board_array_master[enemy_bonus_click_1_x,enemy_bonus_click_1_y,1] ) )
									{
										minor_color_quantity++;
										if ( (x-2>=0) && (board_array_master[enemy_bonus_click_1_x,enemy_bonus_click_1_y,1] == board_array_master[x-2,y,1]) )
											minor_color_quantity++;
									}
									
									if ( ((x+1)< _X_tiles)  && (board_array_master[x+1,y,1] == board_array_master[enemy_bonus_click_1_x,enemy_bonus_click_1_y,1]) )
									{
										minor_color_quantity++;
										if ( ((x+2)< _X_tiles) && (board_array_master[enemy_bonus_click_1_x,enemy_bonus_click_1_y,1] == board_array_master[x+2,y,1]) )
											minor_color_quantity++;
									}
									
									
									temp_value_teleport_click_2 = 0;
									
									if (lose_requirement_selected == lose_requirement.player_hp_is_zero)
										temp_value_teleport_click_2 = (Calculate_damage_of_this_gem(board_array_master[enemy_bonus_click_1_x,enemy_bonus_click_1_y,1])*minor_color_quantity);
									else if (lose_requirement_selected == lose_requirement.enemy_collect_gems)
									{
										temp_enemy_gem_count = new int[gem_length];
										for (int cc = 0; cc < minor_color_quantity; cc++)
										{
											if (Check_if_enemy_need_this_gem_color(board_array_master[enemy_bonus_click_1_x,enemy_bonus_click_1_y,1]))
												temp_value_teleport_click_2++;
										}
									}
									else
										temp_value_teleport_click_2 = minor_color_quantity; 
									
									//check if this is the best choice
									if (temp_value_teleport_click_2 > temp_best_value_teleport_click_2)
									{
										enemy_chosen_bonus_slot = i;
										enemy_bonus_click_2_x = x;
										enemy_bonus_click_2_y = y;
										temp_best_value_teleport_click_2 = temp_value_teleport_click_2;
										
										//Debug.Log("teleport click 2: " + enemy_bonus_click_2_x + "," + enemy_bonus_click_2_y + " *** best value = " + temp_best_value_teleport_click_2);

										if (!search_best_place)
											break;

										if (temp_best_value_teleport_click_2 >= max_value_switch_gem_teleport_click2)
											break;
									}
									
								}
							}
						}
					}
					if (temp_best_value_teleport_click_2 < max_value_switch_gem_teleport_click2)
					{
						for (int y = 0; y < random_start_point_y; y++)
						{
							for (int x = 0; x < random_start_point_x; x++)
							{
								if ((board_array_master[x,y,1] >= 0) && (board_array_master[x,y,1] <9)
								    && board_array_master[x,y,3] == 0 //no padlock
								    && !reserved_gem[x,y])//no reserved gem
								{
									minor_color_quantity = 0;
									
									if ( (y-1>=0) && (board_array_master[x,y-1,1] == board_array_master[enemy_bonus_click_1_x,enemy_bonus_click_1_y,1]) )
									{
										minor_color_quantity++;
										if ( (y-2>=0) && (board_array_master[enemy_bonus_click_1_x,enemy_bonus_click_1_y,1] == board_array_master[x,y-2,1]) )
											minor_color_quantity++;
									}
									
									if ( ((y+1)< _Y_tiles)  && (board_array_master[x,y+1,1] == board_array_master[enemy_bonus_click_1_x,enemy_bonus_click_1_y,1]) )
									{
										minor_color_quantity++;
										if ( ((y+2)< _Y_tiles) && (board_array_master[enemy_bonus_click_1_x,enemy_bonus_click_1_y,1] == board_array_master[x,y+2,1]) )
											minor_color_quantity++;
									}
									
									if ( (x-1>=0) && (board_array_master[x-1,y,1] == board_array_master[enemy_bonus_click_1_x,enemy_bonus_click_1_y,1] ) )
									{
										minor_color_quantity++;
										if ( (x-2>=0) && (board_array_master[enemy_bonus_click_1_x,enemy_bonus_click_1_y,1] == board_array_master[x-2,y,1]) )
											minor_color_quantity++;
									}
									
									if ( ((x+1)< _X_tiles)  && (board_array_master[x+1,y,1] == board_array_master[enemy_bonus_click_1_x,enemy_bonus_click_1_y,1]) )
									{
										minor_color_quantity++;
										if ( ((x+2)< _X_tiles) && (board_array_master[enemy_bonus_click_1_x,enemy_bonus_click_1_y,1] == board_array_master[x+2,y,1]) )
											minor_color_quantity++;
									}
									
									
									temp_value_teleport_click_2 = 0;
									
									if (lose_requirement_selected == lose_requirement.player_hp_is_zero)
										temp_value_teleport_click_2 = (Calculate_damage_of_this_gem(board_array_master[enemy_bonus_click_1_x,enemy_bonus_click_1_y,1])*minor_color_quantity);
									else if (lose_requirement_selected == lose_requirement.enemy_collect_gems)
									{
										temp_enemy_gem_count = new int[gem_length];
										for (int cc = 0; cc < minor_color_quantity; cc++)
										{
											if (Check_if_enemy_need_this_gem_color(board_array_master[enemy_bonus_click_1_x,enemy_bonus_click_1_y,1]))
												temp_value_teleport_click_2++;
										}
									}
									else
										temp_value_teleport_click_2 = minor_color_quantity; 
									
									//check if this is the best choice
									if (temp_value_teleport_click_2 > temp_best_value_teleport_click_2)
									{
										enemy_chosen_bonus_slot = i;
										enemy_bonus_click_2_x = x;
										enemy_bonus_click_2_y = y;
										temp_best_value_teleport_click_2 = temp_value_teleport_click_2;
										
										//Debug.Log("teleport click 2: " + enemy_bonus_click_2_x + "," + enemy_bonus_click_2_y + " *** best value = " + temp_best_value_teleport_click_2);

										if (!search_best_place)
											break;

										if (temp_best_value_teleport_click_2 >= max_value_switch_gem_teleport_click2)
											break;
									}
									
								}
							}
						}
					}
					
					
					
				}
				else //the secondary color don't must explode!
				{
					//Debug.Log("the secondary color don't must explode");
					for (int y = random_start_point_y; y < _Y_tiles; y++)
					{
						for (int x = random_start_point_x; x < _X_tiles; x++)
						{
							if (Count_how_much_gems_with_this_color_there_are_around_this_coordinates(x,y,board_array_master[enemy_bonus_click_1_x,enemy_bonus_click_1_y,1]) == 0)
							{
								if ((board_array_master[x,y,1] >= 0) && (board_array_master[x,y,1] <9)
								    && board_array_master[x,y,3] == 0 //no padlock
								    && !reserved_gem[x,y])//no reserved gem
								{
									enemy_chosen_bonus_slot = i;
									enemy_bonus_click_2_x = x;
									enemy_bonus_click_2_y = y;
									best_value = temp_best_value_teleport_click_1;
									break;
								}
							}
						}
					}
					if (enemy_bonus_click_2_x == -1)
					{
						for (int y = 0 ; y < random_start_point_y; y++)
						{
							for (int x = 0; x < random_start_point_x; x++)
							{
								if (Count_how_much_gems_with_this_color_there_are_around_this_coordinates(x,y,board_array_master[enemy_bonus_click_1_x,enemy_bonus_click_1_y,1]) == 0)
								{
									if ((board_array_master[x,y,1] >= 0) && (board_array_master[x,y,1] <9)
									    && board_array_master[x,y,3] == 0 //no padlock
									    && !reserved_gem[x,y])//no reserved gem
									{
										enemy_chosen_bonus_slot = i;
										enemy_bonus_click_2_x = x;
										enemy_bonus_click_2_y = y;
										best_value = temp_best_value_teleport_click_1;
										break;
									}
								}
							}
						}
					}
					
				}
				if (enemy_bonus_click_2_x >= 0)
				{
					if ((temp_best_value_teleport_click_1 + temp_best_value_teleport_click_2) > best_value)
						best_value = temp_best_value_teleport_click_1 + temp_best_value_teleport_click_2;
				}
				//else
				//Debug.Log("click_2 not found");
				
			}
			
		}
	}

	void Enemy_charge_bonus_destroy_one(int random_start_point_x, int random_start_point_y, int i, bool search_best_place)
	{
		if (best_value < max_value_destroy_one)
		{
			//Debug.Log("bonus.destroy_one");

				int use_the_bonus_here_will_make_explode_n_gems = 0;
				
				for (int y = random_start_point_y; y < _Y_tiles; y++)
				{
					for (int x = random_start_point_x; x < _X_tiles; x++)
					{
						if (board_array_master[x,y,1] >= 0) //this tile have something in itself
						{
							if ( ((y-2)>=0) && ((y+2)<_Y_tiles) )
							{
								
								if ((board_array_master[x,y-1,1] >= 0) && (board_array_master[x,y-1,1] < 9) //there is a gem over me
								    && (board_array_master[x,y-1,1] == board_array_master[x,y-2,1]) //two gem over me
								    && (board_array_master[x,y-1,1] == board_array_master[x,y+1,1]) //one under
								    && (board_array_master[x,y-1,1] == board_array_master[x,y+2,1]) //another under
								    ) //then use bonus here will explode at least 4 gems
								{
									use_the_bonus_here_will_make_explode_n_gems = 4;
									//search for lateral gems
									if (((x-1) >= 0) && ((x+1)<_X_tiles))
									{
										if ((board_array_master[x,y-1,1] == board_array_master[x+1,y,1])
										    && (board_array_master[x,y-1,1] == board_array_master[x-1,y,1]))
										{
											use_the_bonus_here_will_make_explode_n_gems += 2;
											if ( ((x-2) >= 0) && (board_array_master[x,y-1,1] == board_array_master[x-2,y,1]))
												use_the_bonus_here_will_make_explode_n_gems++;
											if ( ((x+2)<_X_tiles) &&  (board_array_master[x,y-1,1] == board_array_master[x+2,y,1]))
												use_the_bonus_here_will_make_explode_n_gems++;
										}
									}
									else if ((x-2) >= 0)
									{
										if ((board_array_master[x,y-1,1] == board_array_master[x-1,y,1])
										    && (board_array_master[x,y-1,1] == board_array_master[x-2,y,1]) )
											use_the_bonus_here_will_make_explode_n_gems += 2;
									}
									else if ((x+2)<_X_tiles)
									{
										if ((board_array_master[x,y-1,1] == board_array_master[x+1,y,1])
										    && (board_array_master[x,y-1,1] == board_array_master[x+2,y,1]) )
											use_the_bonus_here_will_make_explode_n_gems += 2;
									}
									
									//calculate the value of this move:
									temp_value = 0;
									
									if (lose_requirement_selected == lose_requirement.player_hp_is_zero)
										temp_value = Calculate_damage_of_this_gem(board_array_master[x,y-1,1]) * use_the_bonus_here_will_make_explode_n_gems;
									else if (lose_requirement_selected == lose_requirement.enemy_collect_gems)
									{
										temp_enemy_gem_count = new int[gem_length];
										for (int c = 0; c < use_the_bonus_here_will_make_explode_n_gems; c++)
										{
											if (Check_if_enemy_need_this_gem_color(board_array_master[x,y-1,1]))
												temp_value++;
										}
									}
									else
										temp_value = use_the_bonus_here_will_make_explode_n_gems; 
									
									if (temp_value > best_value)
									{
										enemy_chosen_bonus_slot = i;
										enemy_bonus_click_1_x = x;
										enemy_bonus_click_1_y = y;
										best_value = temp_value;
										//Debug.Log("best value = " + best_value + " in " + x + "," + y);

										if (!search_best_place)
											return;

										if (best_value >= max_value_destroy_one) //you can't overcome this outcome, so don't check further
										{
											//Debug.Log("hammer stop this check at " + x + "," + y);
											break;
										}
									}
								}
							}
						}
					}
				}
				if (best_value < max_value_destroy_one)//if you don't have found the best value yet
				{
					if (random_start_point_x > 0 || random_start_point_y > 0)
					{
						//Debug.Log("continue search from 0,0");
						for (int y = 0; y < random_start_point_y; y++)
						{
							for (int x = 0; x < random_start_point_x; x++)
							{
								if (board_array_master[x,y,1] >= 0) //this tile have something in itself
								{
									if ( ((y-2)>=0) && ((y+2)<_Y_tiles) )
									{
										
										if ((board_array_master[x,y-1,1] >= 0) && (board_array_master[x,y-1,1] < 9) //there is a gem over me
										    && (board_array_master[x,y-1,1] == board_array_master[x,y-2,1]) //two gem over me
										    && (board_array_master[x,y-1,1] == board_array_master[x,y+1,1]) //one under
										    && (board_array_master[x,y-1,1] == board_array_master[x,y+2,1]) //another under
										    ) //then use bonus here will explode at least 4 gems
										{
											use_the_bonus_here_will_make_explode_n_gems = 4;
											//search for lateral gems
											if (((x-1) >= 0) && ((x+1)<_X_tiles))
											{
												if ((board_array_master[x,y-1,1] == board_array_master[x+1,y,1])
												    && (board_array_master[x,y-1,1] == board_array_master[x-1,y,1]))
												{
													use_the_bonus_here_will_make_explode_n_gems += 2;
													if ( ((x-2) >= 0) && (board_array_master[x,y-1,1] == board_array_master[x-2,y,1]))
														use_the_bonus_here_will_make_explode_n_gems++;
													if ( ((x+2)<_X_tiles) &&  (board_array_master[x,y-1,1] == board_array_master[x+2,y,1]))
														use_the_bonus_here_will_make_explode_n_gems++;
												}
											}
											else if ((x-2) >= 0)
											{
												if ((board_array_master[x,y-1,1] == board_array_master[x-1,y,1])
												    && (board_array_master[x,y-1,1] == board_array_master[x-2,y,1]) )
													use_the_bonus_here_will_make_explode_n_gems += 2;
											}
											else if ((x+2)<_X_tiles)
											{
												if ((board_array_master[x,y-1,1] == board_array_master[x+1,y,1])
												    && (board_array_master[x,y-1,1] == board_array_master[x+2,y,1]) )
													use_the_bonus_here_will_make_explode_n_gems += 2;
											}
											
											//calculate the value of this move:
											temp_value = 0;
											
											if (lose_requirement_selected == lose_requirement.player_hp_is_zero)
												temp_value = Calculate_damage_of_this_gem(board_array_master[x,y-1,1]) * use_the_bonus_here_will_make_explode_n_gems;
											else if (lose_requirement_selected == lose_requirement.enemy_collect_gems)
											{
												temp_enemy_gem_count = new int[gem_length];
												for (int c = 0; c < use_the_bonus_here_will_make_explode_n_gems; c++)
												{
													if (Check_if_enemy_need_this_gem_color(board_array_master[x,y-1,1]))
														temp_value++;
												}
											}
											else
												temp_value = use_the_bonus_here_will_make_explode_n_gems; 
											
											if (temp_value > best_value)
											{
												enemy_chosen_bonus_slot = i;
												enemy_bonus_click_1_x = x;
												enemy_bonus_click_1_y = y;
												best_value = temp_value;
												//	Debug.Log("best value = " + best_value + " in " + x + "," + y);
												if (best_value >= max_value_destroy_one) //you can't overcome this outcome, so don't check further
												{
													//Debug.Log("hammer stop this check at " + x + "," + y);
													break;
												}
											}
										}
									}
								}
							}
						}
					}
				}
			
		
			
		}
	}



	void Enemy_check_inventory_bonus(bool compare_with_big_move_value)
		{
		temp_value = 0;
		best_value = 0;
		
		enemy_will_use_a_bonus = false;
		
		int random_start_point_x = UnityEngine.Random.Range(0,_X_tiles-1);
		int random_start_point_y = UnityEngine.Random.Range(0,_Y_tiles-1);

		if (UnityEngine.Random.Range(1,100) <= chance_of_use_best_bonus) //search the most strong bonus avaible
			{
			for (int i = 1; i < Enum.GetNames(typeof(Board_C.bonus)).Length ; i ++)
				{
				if (enemy_bonus_inventory[i] > 0)
					Enemy_try_inventory_bonus(random_start_point_x, random_start_point_y, i, true);
				}
			}
		else //pick a random bonus
			{
			//select a random bonus slot
			int random_start_slot = UnityEngine.Random.Range(0,Enum.GetNames(typeof(Board_C.bonus)).Length-1);
			int select_this_slot = -1;
			for (int i = random_start_slot; i < Enum.GetNames(typeof(Board_C.bonus)).Length; i++)
			{
				if (enemy_bonus_inventory[i] > 0)//if there are bonuses that can be used
				{
					select_this_slot = i;
					break;
				}
			}
			if (select_this_slot == -1)
			{
				for (int i = 0;  i < random_start_slot; i++)
				{
					if (enemy_bonus_inventory[i] > 0)//if there are bonuses that can be used
					{
						select_this_slot = i;
						break;
					}
				}
			}
			
			
			if (select_this_slot > 0)//you have found a bonus ready to use, now select a random place where use it
				{
				Enemy_try_inventory_bonus(random_start_point_x, random_start_point_y, select_this_slot, false);
				}
			}

		if (best_value > 0)
			enemy_will_use_a_bonus = true;
		}

	void Enemy_check_charge_bonus(bool compare_with_big_move_value)
	{
		temp_value = 0;
		best_value = 0;

		enemy_will_use_a_bonus = false;

		int random_start_point_x = UnityEngine.Random.Range(0,_X_tiles-1);
		int random_start_point_y = UnityEngine.Random.Range(0,_Y_tiles-1);

		if (UnityEngine.Random.Range(1,100) <= chance_of_use_best_bonus) //search the most strong bonus avaible
		{
			for (int i = 0; i < enemey_bonus_slot_availables; i++)
			{
				if (enemy_bonus_ready[i])//if there are bonuses that can be used
				{
					Enemy_try_charge_bonus(random_start_point_x, random_start_point_y, i, true);
				}
			}
		}
		else//pick out a random avaible bonus
		{

			//select a random bonus slot
			int random_start_slot = UnityEngine.Random.Range(0,enemey_bonus_slot_availables-1);
			int select_this_slot = -1;
			for (int i = random_start_slot; i < enemey_bonus_slot_availables; i++)
				{
				if (enemy_bonus_ready[i])//if there are bonuses that can be used
					{
					select_this_slot = i;
					break;
					}
				}
			if (select_this_slot == -1)
				{
				for (int i = 0;  i < random_start_slot; i++)
					{
					if (enemy_bonus_ready[i])//if there are bonuses that can be used
						{
						select_this_slot = i;
						break;
						}
					}
				}


			if (select_this_slot >= 0)//you have found a bonus ready to use, now select a random place where use it
				{
				Enemy_try_charge_bonus(random_start_point_x, random_start_point_y, select_this_slot, false);
				}
		}

		if (best_value > 0)
			enemy_will_use_a_bonus = true;

	}

	void Enemy_try_charge_bonus(int random_start_point_x, int random_start_point_y, int selected_slot, bool search_best_place)
		{

		switch(enemy_bonus_slot[selected_slot])//search best place on the board, where use this bonus
		{
		case bonus.destroy_one:
			Enemy_charge_bonus_destroy_one(random_start_point_x, random_start_point_y, selected_slot, search_best_place);
			break;
			
		case bonus.switch_gem_teleport: 
			Enemy_charge_bonus_teleport(random_start_point_x, random_start_point_y, selected_slot, search_best_place);
			break;
			
		case bonus.destroy_3x3:
			Enemy_charge_bonus_destroy_3x3(random_start_point_x, random_start_point_y, selected_slot, search_best_place);
			break;
			
		case bonus.destroy_horizontal:
			Enemy_charge_bonus_destroy_horizontal(random_start_point_x, random_start_point_y, selected_slot, search_best_place);					
			break;
			
		case bonus.destroy_vertical:
			Enemy_charge_bonus_destroy_vertical(random_start_point_x, random_start_point_y, selected_slot, search_best_place);
			break;
			
		case bonus.destroy_horizontal_and_vertical:
			Enemy_charge_bonus_destroy_horizontal_and_vertical(random_start_point_x, random_start_point_y, selected_slot, search_best_place);
			break;
			
		case bonus.destroy_all_gem_with_this_color:
			Enemy_charge_bonus_destroy_all_gem_with_this_color(selected_slot);
			break;
			
		case bonus.heal_hp:
			Enemy_charge_bonus_heal_hp(selected_slot);
			break;
		}
	}

	void Enemy_try_inventory_bonus(int random_start_point_x, int random_start_point_y, int selected_slot, bool search_best_place)
	{
		Debug.Log("Enemy_try_inventory_bonus: " + selected_slot.ToString());

		if (selected_slot == 1)
			Enemy_charge_bonus_destroy_one(random_start_point_x, random_start_point_y, selected_slot, search_best_place);
		else if (selected_slot == 2)
			Enemy_charge_bonus_teleport(random_start_point_x, random_start_point_y, selected_slot, search_best_place);
		else if (selected_slot == 3)
			Enemy_charge_bonus_destroy_3x3(random_start_point_x, random_start_point_y, selected_slot, search_best_place);
		else if (selected_slot == 4)
			Enemy_charge_bonus_destroy_horizontal(random_start_point_x, random_start_point_y, selected_slot, search_best_place);					
		else if (selected_slot == 5)
			Enemy_charge_bonus_destroy_vertical(random_start_point_x, random_start_point_y, selected_slot, search_best_place);
		else if (selected_slot == 6)
			Enemy_charge_bonus_destroy_horizontal_and_vertical(random_start_point_x, random_start_point_y, selected_slot, search_best_place);
		else if (selected_slot == 7)
			Enemy_charge_bonus_destroy_all_gem_with_this_color(selected_slot);
		else if (selected_slot == 10)
			Enemy_charge_bonus_heal_hp(selected_slot);

	}

	void Locate_all_bonus_on_board()
	{
		Debug.Log("Locate_all_bonus_on_board()");
		bonus_coordinate = new Vector2[number_of_bonus_on_board];
		int bonus_count = 0;
		for (int y = 0; y < _Y_tiles; y++)
			{
				for (int x = 0; x < _X_tiles; x++)
				{
					if (board_array_master[x,y,4] > 0)//if this is a bonus
					{
						bonus_coordinate[bonus_count] = new Vector2(x,y);
						bonus_count++;
					}
				}
			}
	}

	void Check_if_I_can_use_this_switch_bonus(int i)
	{
		//up gem can go down over me?
		if ( ((int)bonus_coordinate[i].y > 0) && (board_array_master[(int)bonus_coordinate[i].x,(int)bonus_coordinate[i].y-1,6] > 0) )
			Search_the_most_useful_bonus_on_board((int)bonus_coordinate[i].x,(int)bonus_coordinate[i].y,0,-1);

		//down gem can go up over me?
		if ( ((int)bonus_coordinate[i].y+1 < _Y_tiles) && (board_array_master[(int)bonus_coordinate[i].x,(int)bonus_coordinate[i].y+1,7] > 0) )
			Search_the_most_useful_bonus_on_board((int)bonus_coordinate[i].x,(int)bonus_coordinate[i].y,0,1);

		//R gem can go L over me?
		if ( ((int)bonus_coordinate[i].x+1 < _X_tiles) && (board_array_master[(int)bonus_coordinate[i].x+1,(int)bonus_coordinate[i].y,9] > 0) )
			Search_the_most_useful_bonus_on_board((int)bonus_coordinate[i].x,(int)bonus_coordinate[i].y,1,0);

		//L gem can go R over me?
		if ( ((int)bonus_coordinate[i].x > 0) && (board_array_master[(int)bonus_coordinate[i].x-1,(int)bonus_coordinate[i].y,8] > 0) )
			Search_the_most_useful_bonus_on_board((int)bonus_coordinate[i].x,(int)bonus_coordinate[i].y,-1,0);

	}

	void Check_if_I_can_use_this_free_switch_bonus(int i)
	{
		//up gem can go down over me?
		if ( ((int)bonus_coordinate[i].y > 0) 
		    && (board_array_master[(int)bonus_coordinate[i].x,(int)bonus_coordinate[i].y-1,1] >= 0) && (board_array_master[(int)bonus_coordinate[i].x,(int)bonus_coordinate[i].y-1,1] <= 9) //is a gem or a special gem
		    )
				{
		   		if (board_array_master[(int)bonus_coordinate[i].x,(int)bonus_coordinate[i].y-1,3] == 0) //no padlock
					Search_the_most_useful_bonus_on_board((int)bonus_coordinate[i].x,(int)bonus_coordinate[i].y,0,-1);
				}
		
		//down gem can go up over me?
		if ( ((int)bonus_coordinate[i].y+1 < _Y_tiles) 
		    && (board_array_master[(int)bonus_coordinate[i].x,(int)bonus_coordinate[i].y+1,1] >= 0) && (board_array_master[(int)bonus_coordinate[i].x,(int)bonus_coordinate[i].y+1,1] <= 9))
				{
				if (board_array_master[(int)bonus_coordinate[i].x,(int)bonus_coordinate[i].y+1,3] == 0)
					Search_the_most_useful_bonus_on_board((int)bonus_coordinate[i].x,(int)bonus_coordinate[i].y,0,1);
				}
		
		//R gem can go L over me?
		if ( ((int)bonus_coordinate[i].x+1 < _X_tiles) 
		    && (board_array_master[(int)bonus_coordinate[i].x+1,(int)bonus_coordinate[i].y,1] >= 0)  && (board_array_master[(int)bonus_coordinate[i].x+1,(int)bonus_coordinate[i].y,1] <= 9))
				{
				if (board_array_master[(int)bonus_coordinate[i].x+1,(int)bonus_coordinate[i].y,3] == 0)
					Search_the_most_useful_bonus_on_board((int)bonus_coordinate[i].x,(int)bonus_coordinate[i].y,1,0);
				}
		
		//L gem can go R over me?
		if ( ((int)bonus_coordinate[i].x > 0) 
		    && (board_array_master[(int)bonus_coordinate[i].x-1,(int)bonus_coordinate[i].y,1] >= 0) && (board_array_master[(int)bonus_coordinate[i].x-1,(int)bonus_coordinate[i].y,1] <= 9))
				{
				if (board_array_master[(int)bonus_coordinate[i].x-1,(int)bonus_coordinate[i].y,3] == 0)
					Search_the_most_useful_bonus_on_board((int)bonus_coordinate[i].x,(int)bonus_coordinate[i].y,-1,0);
				}
	}

	void Enemy_check_on_board_free_switch_bonus(bool compare_with_big_move_value)
	{
		Debug.LogWarning("Enemy_check_on_board_free_switch_bonus: " + number_of_bonus_on_board);
		temp_value = 0;
		best_value = 0;
		
		if (UnityEngine.Random.Range(1,100) <= chance_of_use_best_bonus) //search the most useful bonus
		{
			Debug.Log("Enemy_check_on_board_switch_bonus: Best");
			
			for (int i = 0; i < number_of_bonus_on_board; i++)
				Check_if_I_can_use_this_free_switch_bonus(i);
			
			if (compare_with_big_move_value)
			{
				if (best_value > big_move_value)
					enemy_will_use_a_bonus = true;
				//enemy_will_switch_a_gem_to_trigger_a_bonus_on_board = true;
			}
			else
			{
				if (best_value > 0)
					enemy_will_use_a_bonus = true;
				//enemy_will_switch_a_gem_to_trigger_a_bonus_on_board = true;
			}
		}
		else //select a random bonus
		{
			Debug.Log("Enemy_check_on_board_switch_bonus: Random");
			
			if (number_of_bonus_on_board == 1)
			{
				Check_if_I_can_use_this_free_switch_bonus(0);
				if (best_value > 0)
					enemy_will_use_a_bonus = true;
				//enemy_will_switch_a_gem_to_trigger_a_bonus_on_board = true;
			}
			else //start check bonus from a random start point and stop to first bonus usable
			{
				int random_start_point = UnityEngine.Random.Range(0,number_of_bonus_on_board-1);
				
				for (int i = random_start_point; i < number_of_bonus_on_board; i++)
				{
					
					Check_if_I_can_use_this_free_switch_bonus(i);
					
					if (best_value > 0)
					{
						enemy_will_use_a_bonus = true;
						//enemy_will_switch_a_gem_to_trigger_a_bonus_on_board = true;
						return;
					}
				}
				//if you don't have found an usable bonus yet, check from 0 start point to random start point
				if (best_value == 0)
				{
					for (int i = 0; i < random_start_point; i++)
					{
						Check_if_I_can_use_this_free_switch_bonus(i);
						
						if (best_value > 0)
						{
							//enemy_will_switch_a_gem_to_trigger_a_bonus_on_board = true;
							enemy_will_use_a_bonus = true;
							return;
						}
					}
				}
			}
		}
	}

	void Enemy_check_on_board_switch_bonus(bool compare_with_big_move_value)
	{
		Debug.LogWarning("Enemy_check_on_board_switch_bonus: " + number_of_bonus_on_board);
		temp_value = 0;
		best_value = 0;

		if (UnityEngine.Random.Range(1,100) <= chance_of_use_best_bonus) //search the most useful bonus
		{
			Debug.Log("Enemy_check_on_board_switch_bonus: Best");

			for (int i = 0; i < number_of_bonus_on_board; i++)
				Check_if_I_can_use_this_switch_bonus(i);

			if (compare_with_big_move_value)
			{
				if (best_value > big_move_value)
					enemy_will_use_a_bonus = true;
					//enemy_will_switch_a_gem_to_trigger_a_bonus_on_board = true;
			}
			else
			{
				if (best_value > 0)
					enemy_will_use_a_bonus = true;
					//enemy_will_switch_a_gem_to_trigger_a_bonus_on_board = true;
			}
		}
		else //select a random bonus
		{
			Debug.Log("Enemy_check_on_board_switch_bonus: Random");

			if (number_of_bonus_on_board == 1)
				{
				Check_if_I_can_use_this_switch_bonus(0);
				if (best_value > 0)
					enemy_will_use_a_bonus = true;
					//enemy_will_switch_a_gem_to_trigger_a_bonus_on_board = true;
				}
			else //start check bonus from a random start point and stop to first bonus usable
				{
				int random_start_point = UnityEngine.Random.Range(0,number_of_bonus_on_board-1);

				for (int i = random_start_point; i < number_of_bonus_on_board; i++)
					{

					Check_if_I_can_use_this_switch_bonus(i);

					if (best_value > 0)
						{
						enemy_will_use_a_bonus = true;
						//enemy_will_switch_a_gem_to_trigger_a_bonus_on_board = true;
						return;
						}
					}
				//if you don't have found an usable bonus yet, check from 0 start point to random start point
				if (best_value == 0)
					{
					for (int i = 0; i < random_start_point; i++)
						{
						Check_if_I_can_use_this_switch_bonus(i);
						
						if (best_value > 0)
							{
							//enemy_will_switch_a_gem_to_trigger_a_bonus_on_board = true;
							enemy_will_use_a_bonus = true;
							return;
							}
						}
					}
				}
		}

	}

	void Search_the_most_useful_bonus_on_board(int x, int y, int x_correction, int y_correction)
		{
		temp_value = Enemy_check_value_of_this_bonus_on_board(x,y,0,0);
		
		if (temp_value > best_value)
			{
			enemy_bonus_click_1_x = x + x_correction;
			enemy_bonus_click_1_y = y + y_correction;

			enemy_bonus_click_2_x = x;
			enemy_bonus_click_2_y = y;

			best_value = temp_value;
			}
		else if (temp_value == best_value)
			{
			if ( UnityEngine.Random.Range(1,100) > UnityEngine.Random.Range(40,60) )
				{
				enemy_bonus_click_1_x = x + x_correction;
				enemy_bonus_click_1_y = y + y_correction;

				enemy_bonus_click_2_x = x;
				enemy_bonus_click_2_y = y;

				best_value = temp_value;
				}
			}
		}

	void Enemy_check_on_board_click_bonus(bool compare_with_big_move_value)
	{

			if (UnityEngine.Random.Range(1,100) <= chance_of_use_best_bonus) 
				{
				//search the most useful bonus
				temp_value = 0;
				best_value = 0;
				for (int i = 0; i < number_of_bonus_on_board; i++)
					{
					Search_the_most_useful_bonus_on_board((int)bonus_coordinate[i].x,(int)bonus_coordinate[i].y,0,0);
					}

				if (compare_with_big_move_value)
					{
					if (best_value > big_move_value)
						enemy_will_use_a_bonus = true;
						//enemy_will_use_a_click_bonus_on_board = true;
					}
				else
					{
					if (best_value > 0)
						enemy_will_use_a_bonus = true;
						//enemy_will_use_a_click_bonus_on_board = true;
					}
				}
			else //select a random bonus
				{
				int random_bonus_selected = 0;
					random_bonus_selected = UnityEngine.Random.Range(0,bonus_coordinate.Length-1);
						enemy_bonus_click_1_x = (int)bonus_coordinate[random_bonus_selected].x;
						enemy_bonus_click_1_y = (int)bonus_coordinate[random_bonus_selected].y;

				if (compare_with_big_move_value)
					{
					if (Enemy_check_value_of_this_bonus_on_board(enemy_bonus_click_1_x,enemy_bonus_click_1_y,0,0) > big_move_value)
						enemy_will_use_a_bonus = true;
						//enemy_will_use_a_click_bonus_on_board = true;
					}
				else
					enemy_will_use_a_bonus = true;
					//enemy_will_use_a_click_bonus_on_board = true;
				}
		
	}

	int Enemy_check_value_of_this_bonus_on_board(int x, int y, int x_correction, int y_correction)
	{
		int temp_value = 0;

		if (board_array_master[x,y,4] == 3)//bomb
		{
			if (lose_requirement_selected == lose_requirement.player_hp_is_zero)
				temp_value = Calculate_bomb_damage(x+x_correction,y+y_correction);
			else if (lose_requirement_selected == lose_requirement.enemy_collect_gems)
				temp_value = Calculate_bomb_gems(x+x_correction,y+y_correction);
			else
				temp_value = Bomb_how_much_gems_will_explode(x+x_correction,y+y_correction);
		}
		else if (board_array_master[x,y,4] == 4)//horiz
		{
			if (lose_requirement_selected == lose_requirement.player_hp_is_zero)
				{
				temp_value += Calculate_horiz_damage_R(x+x_correction,y+y_correction);
				temp_value += Calculate_horiz_damage_L(x+x_correction,y+y_correction);
				}
			else if (lose_requirement_selected == lose_requirement.enemy_collect_gems)
				{
				temp_value += Calculate_horiz_gems_R(x+x_correction,y+y_correction);
				temp_value += Calculate_horiz_gems_L(x+x_correction,y+y_correction);
				}
			else
				{
				temp_value += R_how_much_gems_will_explode(x+x_correction,y+y_correction);
				temp_value += L_how_much_gems_will_explode(x+x_correction,y+y_correction);
				}
		}
		else if (board_array_master[x,y,4] == 5)//vertic
		{
			if (lose_requirement_selected == lose_requirement.player_hp_is_zero)
				{
				temp_value  += Calculate_up_damage(x+x_correction,y+y_correction);
				temp_value  += Calculate_down_damage(x+x_correction,y+y_correction);
				}
			else if (lose_requirement_selected == lose_requirement.enemy_collect_gems)
				{
				temp_value  += Calculate_up_gems(x+x_correction,y+y_correction);
				temp_value  += Calculate_down_gems(x+x_correction,y+y_correction);
				}
			else
				{
				temp_value  += Up_how_much_gems_will_explode(x+x_correction,y+y_correction);
				temp_value  += Down_how_much_gems_will_explode(x+x_correction,y+y_correction);
				}
		}
		else if (board_array_master[x,y,4] == 6)//horiz_and_vertic
		{
			if (lose_requirement_selected == lose_requirement.player_hp_is_zero)
				{
				temp_value += Calculate_horiz_damage_R(x+x_correction,y+y_correction);
				temp_value += Calculate_horiz_damage_L(x+x_correction,y+y_correction);
				
				temp_value += Calculate_up_damage(x+x_correction,y+y_correction);
				temp_value += Calculate_down_damage(x+x_correction,y+y_correction);
				}
			else if (lose_requirement_selected == lose_requirement.enemy_collect_gems)
				{
				temp_value += Calculate_horiz_gems_R(x+x_correction,y+y_correction);
				temp_value += Calculate_horiz_gems_L(x+x_correction,y+y_correction);
				
				temp_value += Calculate_up_gems(x+x_correction,y+y_correction);
				temp_value += Calculate_down_gems(x+x_correction,y+y_correction);
				}
			else
				{
				temp_value += R_how_much_gems_will_explode(x+x_correction,y+y_correction);
				temp_value += L_how_much_gems_will_explode(x+x_correction,y+y_correction);

				temp_value  += Up_how_much_gems_will_explode(x+x_correction,y+y_correction);
				temp_value  += Down_how_much_gems_will_explode(x+x_correction,y+y_correction);
				}

		}

		return temp_value;
	}


	#region count how much gems will explode
	public int Count_how_much_gems_with_this_color_there_are_around_this_coordinates(int x, int y, int this_color)
		{
		int secondary_explosion_magnitude = 0;
											
		//vertical
		if ( (y-1>=0) && (board_array_master[x,y-1,1] == this_color) )//check if gem over me have same color of teleported gem
			{
			//check if there is another gem with the same color over
			if ( (y-2>=0) && (board_array_master[x,y-2,1] == this_color) )
				{
				secondary_explosion_magnitude += 2;
				if (((y+1)< _Y_tiles) && (board_array_master[x,y+1,1] == this_color) )
					{
						secondary_explosion_magnitude += 1;
						//check if there is another gem with the same color under me
						if (((y+2)< _Y_tiles) && (board_array_master[x,y+2,1] == this_color) )
						{
							secondary_explosion_magnitude += 1;
						}
					}
				}
				//check if there is a gem with the same color under me
			else if (((y+1)< _Y_tiles) && (board_array_master[x,y+1,1] == this_color) )
				{
				secondary_explosion_magnitude += 2;
				//check if there is another gem with the same color under me
				if (((y+2)< _Y_tiles) && (board_array_master[x,y+2,1] == this_color) )
					{
					secondary_explosion_magnitude += 1;
					}
				}
			}
		else //check if there are 2 gem with the same color under
			{
			if (	((y+1)< _Y_tiles) && (board_array_master[x,y+1,1] == this_color) 
				&&	((y+2)< _Y_tiles) && (board_array_master[x,y+2,1] == this_color) 
				 )
				{
				secondary_explosion_magnitude += 2;
				}
			}
												
		//horizontal
		if ( (x-1>=0) && (board_array_master[x-1,y,1] == this_color) )
			{
			if ( (x-2>=0) && (board_array_master[x-2,y,1] == this_color) )
				{
				secondary_explosion_magnitude += 2;
				if (((x+1)< _X_tiles) && (board_array_master[x+1,y,1] == this_color) )
					{
						secondary_explosion_magnitude += 1;
						if (((x+2)< _X_tiles) && (board_array_master[x+2,y,1] == this_color) )
						{
							secondary_explosion_magnitude += 1;
						}
					}
				}
			else if (((x+1)< _X_tiles) && (board_array_master[x+1,y,1] == this_color) )
				{
				secondary_explosion_magnitude += 2;
				if (((x+2)< _X_tiles) && (board_array_master[x+2,y,1] == this_color) )
					{
					secondary_explosion_magnitude += 1;
					}
				}
			}
		else
			{
			if (	((x+1)< _X_tiles) && (board_array_master[x+1,y,1] == this_color) 
				&&	((x+2)< _X_tiles) && (board_array_master[x+2,y,1] == this_color) 
				)
				{
				secondary_explosion_magnitude += 2;
				}
			}
			
		return secondary_explosion_magnitude;
		}




	bool Check_if_this_will_explode(int x, int y)
		{
		if ( (board_array_master[x,y,1] >= 0) && (board_array_master[x,y,1] < 9) ) //is a gem
			return true;
		else
			return false;
		}

	int Bomb_how_much_gems_will_explode(int _x, int _y)
	{
		int gems_that_will_explode = 0;
		
		for (int y = (_y-1); y < ((_y-1)+3); y++)
		{
			if ((y >= 0) && (y < _Y_tiles))
			{
				for (int x = (_x-1); x < ((_x-1)+3); x++)
				{
					if ((x >= 0) && (x < _X_tiles))
					{
						if (board_array_master[x,y,1] >= 0) //if this tile have something
						{
							if ( (board_array_master[x,y,1] >= 0) && (board_array_master[x,y,1] < 9) ) //is a gem
							{
								if (Check_if_this_will_explode(x,y))
									gems_that_will_explode++;
							}
						}
					}
				}
			}
		}
		return gems_that_will_explode;
	}

	int Up_how_much_gems_will_explode(int _x, int _y)
	{
		int gems_that_will_explode = 0;
		for (int y = (_y-1); y >= 0; y--)
		{
			if ((board_array_master[_x,y,1] >= 0) && (board_array_master[_x,y,1] < 30))
			{
				if (Check_if_this_will_explode(_x,y))
					gems_that_will_explode++;
			}
			else
				return gems_that_will_explode;
		}
		return gems_that_will_explode;
	}

	int Down_how_much_gems_will_explode(int _x, int _y)
	{
		int gems_that_will_explode = 0;
		for (int y = (_y+1); y < _Y_tiles; y++)
		{
			if ((board_array_master[_x,y,1] >= 0) && (board_array_master[_x,y,1] < 30))
			{
				if (Check_if_this_will_explode(_x,y))
					gems_that_will_explode++;
			}
			else
				return gems_that_will_explode;
		}
		return gems_that_will_explode;
	}

	int R_how_much_gems_will_explode(int _x, int _y)
	{
		int gems_that_will_explode = 0;
		for (int x = (_x+1); x < _X_tiles; x++)
		{
			if ((board_array_master[x,_y,1] >= 0) && (board_array_master[x,_y,1] < 30))
			{
				if (Check_if_this_will_explode(x,_y))
					gems_that_will_explode++;
			}
			else
				return gems_that_will_explode;
		}
		return gems_that_will_explode;
	}
	
	int L_how_much_gems_will_explode(int _x, int _y)
	{
		int gems_that_will_explode = 0;;
		for (int x = (_x-1); x >= 0; x--)
		{
			if ((board_array_master[x,_y,1] >= 0) && (board_array_master[x,_y,1] < 30))
			{
				if (Check_if_this_will_explode(x,_y))
					gems_that_will_explode++;
			}
			else
				return gems_that_will_explode;
		}
		return gems_that_will_explode;
	}
	#endregion

	#region count useful gems
	bool Check_if_enemy_need_this_gem_color(int g_color)
	{
		if (g_color <= gem_length)
			{
			if ((number_of_gems_to_destroy_to_win[g_color]
			     -number_of_gems_collect_by_the_enemy[g_color] - temp_enemy_gem_count[g_color]) > 0 ) //if enemy need this color
				{
				temp_enemy_gem_count[g_color]++;
				return true;
				}
			else
				return false;
			}
		else
			return false;
	}

	int Calculate_bomb_gems(int _x, int _y)
		{
			int useful_gems = 0;

			for (int y = (_y-1); y < ((_y-1)+3); y++)
			{
				if ((y >= 0) && (y < _Y_tiles))
				{
					for (int x = (_x-1); x < ((_x-1)+3); x++)
					{
						if ((x >= 0) && (x < _X_tiles))
						{
							if (board_array_master[x,y,1] >= 0) //if this tile have something
							{
								if ( (board_array_master[x,y,1] >= 0) && (board_array_master[x,y,1] < 9) ) //is a gem
								{
									if (Check_if_enemy_need_this_gem_color(board_array_master[x,y,1]))
										useful_gems++;
								}
							}
						}
					}
				}
			}
			return useful_gems;
		}

	int Calculate_up_gems(int _x, int _y)
		{
			int useful_gems = 0;
			for (int y = (_y-1); y >= 0; y--)
			{
				if ((board_array_master[_x,y,1] >= 0) && (board_array_master[_x,y,1] < 30))
					{
						if (Check_if_enemy_need_this_gem_color(board_array_master[_x,y,1]))
							useful_gems++;
					}
				else
					return useful_gems;
			}
			return useful_gems;
		}

	int Calculate_down_gems(int _x, int _y)
	{
		int useful_gems = 0;
		for (int y = (_y+1); y < _Y_tiles; y++)
		{
			if ((board_array_master[_x,y,1] >= 0) && (board_array_master[_x,y,1] < 30))
				{
				if (Check_if_enemy_need_this_gem_color(board_array_master[_x,y,1]))
					useful_gems++;
				}
			else
				return useful_gems;
		}
		return useful_gems;
	}

	int Calculate_horiz_gems_R(int _x, int _y)
	{
		int useful_gems = 0;
		for (int x = (_x+1); x < _X_tiles; x++)
		{
			if ((board_array_master[x,_y,1] >= 0) && (board_array_master[x,_y,1] < 30))
				{
				if (Check_if_enemy_need_this_gem_color(board_array_master[x,_y,1]))
					useful_gems++;
				}
			else
				return useful_gems;
		}
		return useful_gems;
	}
	
	int Calculate_horiz_gems_L(int _x, int _y)
	{
		int useful_gems = 0;;
		for (int x = (_x-1); x >= 0; x--)
		{
			if ((board_array_master[x,_y,1] >= 0) && (board_array_master[x,_y,1] < 30))
				{
				if (Check_if_enemy_need_this_gem_color(board_array_master[x,_y,1]))
					useful_gems++;
				}
			else
				return useful_gems;
		}
		return useful_gems;
	}
	#endregion

	#region calculate damage
	

	int Calculate_bomb_damage(int _x, int _y)
		{
		int temp_big_damage = 0;
		for (int y = (_y-1); y < ((_y-1)+3); y++)
			{
			if ((y >= 0) && (y < _Y_tiles))
				{
				for (int x = (_x-1); x < ((_x-1)+3); x++)
					{
					if ((x >= 0) && (x < _X_tiles))
						{
						if (board_array_master[x,y,1] >= 0) //if this tile have something
							{
							if ( (board_array_master[x,y,1] >= 0) && (board_array_master[x,y,1] < 9) ) //is a gem
								{
								temp_big_damage += Calculate_damage_of_this_gem(board_array_master[x,y,1]);
								}
							}
						}
					}
				}
			}
		return temp_big_damage;
		}

	int Calculate_up_damage(int _x, int _y)
		{
		int temp_big_damage = 0;
		for (int y = (_y-1); y >= 0; y--)
			{
			if ((board_array_master[_x,y,1] >= 0) && (board_array_master[_x,y,1] < 30))
				temp_big_damage += Calculate_damage_of_this_gem(board_array_master[_x,y,1]);
			else
				return temp_big_damage;
			}
		return temp_big_damage;
		}

	int Calculate_down_damage(int _x, int _y)
		{
		int temp_big_damage = 0;
		for (int y = (_y+1); y < _Y_tiles; y++)
			{
			if ((board_array_master[_x,y,1] >= 0) && (board_array_master[_x,y,1] < 30))
				temp_big_damage += Calculate_damage_of_this_gem(board_array_master[_x,y,1]);
			else
				return temp_big_damage;
			}
		return temp_big_damage;
		}

	int Calculate_horiz_damage_R(int _x, int _y)
		{
		int temp_big_damage = 0;
		for (int x = (_x+1); x < _X_tiles; x++)
			{
			if ((board_array_master[x,_y,1] >= 0) && (board_array_master[x,_y,1] < 30))
				temp_big_damage += Calculate_damage_of_this_gem(board_array_master[x,_y,1]);
			else
				return temp_big_damage;
			}
		return temp_big_damage;
		}

	int Calculate_horiz_damage_L(int _x, int _y)
	{
		int temp_big_damage = 0;
		for (int x = (_x-1); x >= 0; x--)
		{
			if ((board_array_master[x,_y,1] >= 0) && (board_array_master[x,_y,1] < 30))
				temp_big_damage += Calculate_damage_of_this_gem(board_array_master[x,_y,1]);
			else
				return temp_big_damage;
		}
		return temp_big_damage;
	}

	int Calculate_damage_of_this_gem(int this_gem_color)
	{
		int temp_big_damage = 0;
		if (this_gem_color < gem_length)
			{

			switch(player_armor[this_gem_color])
			{
			case Board_C.armor.weak:
				temp_big_damage += gem_damage_value*2;
				break;
				
				
			case Board_C.armor.normal:
				temp_big_damage +=  gem_damage_value;
				break;
				
				
			case Board_C.armor.strong:
				temp_big_damage +=  (int)(gem_damage_value*0.5f);
				break;
				
				
			case Board_C.armor.immune:
				
				break;
				
			case Board_C.armor.absorb:
				if ( (current_player_hp + gem_damage_value) > max_player_hp )
					temp_big_damage -= (max_player_hp-current_player_hp);
				else
					temp_big_damage -= gem_damage_value;
				break;
				
			case Board_C.armor.repel:
				if ((current_enemy_hp - gem_damage_value) < 0)
					temp_big_damage -= 99999;
				else
					temp_big_damage -= gem_damage_value;
				break;
			}
			}

		return temp_big_damage;
	}

	#endregion

	void Search_big_move()
	{
		big_move_value = 0; //reset varaible before use it
		if (lose_requirement_selected == lose_requirement.player_hp_is_zero)
			Search_big_damage();
		else
			Search_big_explosion();
	}

	void Search_big_damage()
	{
		//Debug.Log("Search_big_damage");
		int temp_big_damage = 0;
		int big_damage = 0;

		if (use_armor)
		{
			for (int i = 0; i < number_of_gems_moveable; i++)
			{
				if ( (player_armor[list_of_moves_possible[i,3]] != armor.absorb) 
				    && (player_armor[list_of_moves_possible[i,3]] != armor.immune)  
				    && (player_armor[list_of_moves_possible[i,3]] != armor.repel))
				{
					if (player_armor[list_of_moves_possible[i,3]] == armor.weak)
						temp_big_damage = list_of_moves_possible[i,0]*2;
					else if (player_armor[list_of_moves_possible[i,3]] == armor.normal)
						temp_big_damage = list_of_moves_possible[i,0];
					else if (player_armor[list_of_moves_possible[i,3]] == armor.strong)
						temp_big_damage = (int)(list_of_moves_possible[i,0]*0.5f);

					if (temp_big_damage > big_damage)
						{
						big_damage = temp_big_damage;
						enemy_move_selected = i;
						}
					else if (temp_big_damage == big_damage)
						{
						if ( UnityEngine.Random.Range(1,100) > UnityEngine.Random.Range(40,60) ) //random update gem with the same explosive power
							enemy_move_selected = i;
						}
				}
			}
		}

		big_move_value = big_damage;
	}

	void Search_big_explosion()//search only explosion >= 4
		{
		//Debug.Log("Search_big_explosion");
		if (use_armor)
		{
			Debug.Log("armor mode");
			for (int i = 0; i < number_of_gems_moveable; i++)
				{
				if ( (player_armor[list_of_moves_possible[i,3]] != armor.absorb) 
			    || (player_armor[list_of_moves_possible[i,3]] != armor.repel)) //gems color to avoid
					{

						if (list_of_moves_possible[i,0] >= 4)//if this move can explode 4 or more gems
							{
							if ( enemy_move_selected == -1)
								enemy_move_selected = i;
							else
								{
								if (list_of_moves_possible[i,0] > list_of_moves_possible[enemy_move_selected,0]) //if there are a better gem, select it
									enemy_move_selected = i;
								else if (list_of_moves_possible[i,0] == list_of_moves_possible[enemy_move_selected,0]) 
									{
									if ( UnityEngine.Random.Range(1,100) > UnityEngine.Random.Range(40,60) ) //random update gem with the same explosive power
										enemy_move_selected = i;
									}
								}
							}

					}
				}
		}
		else
		{
			//Debug.Log("NOT armor mode");
			for (int i = 0; i < number_of_gems_moveable; i++)
			{

					if ((number_of_gems_to_destroy_to_win[list_of_moves_possible[i,3]]
				     -number_of_gems_collect_by_the_enemy[list_of_moves_possible[i,3]]) > 0 ) //if enemy need this color
					{
						if (list_of_moves_possible[i,0] >= 4)//if this move can explode 4 or more gems
						{
							if ( enemy_move_selected == -1)
								enemy_move_selected = i;
							else
							{
								if (list_of_moves_possible[i,0] > list_of_moves_possible[enemy_move_selected,0]) //if there are a better gem, select it
									enemy_move_selected = i;
								else if (list_of_moves_possible[i,0] == list_of_moves_possible[enemy_move_selected,0]) 
								{
									if ( UnityEngine.Random.Range(1,100) > UnityEngine.Random.Range(40,60) ) //random update gem with the same explosive power
										enemy_move_selected = i;
								}
							}
						}
					}
				
			}
		}

		if (enemy_move_selected >= 0)
			big_move_value = list_of_moves_possible[enemy_move_selected,0];

		//Debug:
		/*
		if (enemy_move_selected >= 0)
			{
			Debug.Log ("Search_big_explosion: " + list_of_moves_possible[enemy_move_selected,1] 
			           + ";" + list_of_moves_possible[enemy_move_selected,2] 
			           + " exp = " + list_of_moves_possible[enemy_move_selected,0]);
			}
		else
			Debug.Log("no big explosion found");
			*/
		}

	int Search_most_big_explosion_with_this_color(int color_explosion)
		{
		Debug.Log("Search_most_big_explosion_with_this_color: " + color_explosion);
		int gem_with_most_big_explosion = 0;

		if (gem_color[color_explosion].Count >0)
			{
			gem_with_most_big_explosion = ((int)gem_color[color_explosion][0]);
			for (int i = 0; i < gem_color[color_explosion].Count; i++)
				{
				/*
				Debug.Log("check :" //+ list_of_moves_possible[(int)gem_color[color_explosion][i],1] + "," + + list_of_moves_possible[(int)gem_color[color_explosion][i],2] 
				          + "; color: " + list_of_moves_possible[(int)gem_color[color_explosion][i],3]
				         // + "; exp: " + list_of_moves_possible[(int)gem_color[color_explosion][i],0]
				          );
				Debug.Log ("if " + list_of_moves_possible[(int)gem_color[color_explosion][i],0] + " > " + list_of_moves_possible[gem_with_most_big_explosion,0]);
				*/
				if ( list_of_moves_possible[(int)gem_color[color_explosion][i],0] > list_of_moves_possible[gem_with_most_big_explosion,0])
					{
					gem_with_most_big_explosion = ((int)gem_color[color_explosion][i]);
					//Debug.Log("***color: " + list_of_moves_possible[gem_with_most_big_explosion,3]);
					}
				else if ( list_of_moves_possible[(int)gem_color[color_explosion][i],0] == list_of_moves_possible[gem_with_most_big_explosion,0])
					{
					if ( UnityEngine.Random.Range(1,100) > UnityEngine.Random.Range(40,60) ) //random update gem with the same explosive power
						gem_with_most_big_explosion = ((int)gem_color[color_explosion][i]);
					}
				}

			/*
			Debug.Log("Search_most_big_explosion_with_this_color, find move: " 
			         // + list_of_moves_possible[gem_with_most_big_explosion,1] + "," + + list_of_moves_possible[gem_with_most_big_explosion,2]
			          + "; color: " + list_of_moves_possible[gem_with_most_big_explosion,3]
			         // + "; exp: " + list_of_moves_possible[gem_with_most_big_explosion,0]
			          );*/
			return gem_with_most_big_explosion;
			}
		else
			return -1;//no explosion with this color
		}

	void Search_same_color_of_previous_move()
		{
		Debug.Log("Search_same_color_of_previous_move");
		if ( (enemy_previous_exploded_color[0] < 0) && (enemy_previous_exploded_color[0] < 0) )
			return;

		int best_explosion_with_previous_main_gem_color = -1;
		int best_explosion_with_previous_minor_gem_color = -1;

		//Debug.Log("previous main: " + enemy_previous_exploded_color[0]);
		//Debug.Log("previous minor: " + enemy_previous_exploded_color[1]);

		//check previous main color
			if ( (enemy_previous_exploded_color[0] >= 0) //il previous main gem have exploded something
			    &&(gem_color[enemy_previous_exploded_color[0]].Count > 0) ) //and there are same color available now
					{
					if (use_armor)
						{
						if ( (player_armor[enemy_previous_exploded_color[0]] != armor.absorb) || (player_armor[enemy_previous_exploded_color[0]] != armor.repel) ) //avoid to use a color useful for the player
							best_explosion_with_previous_main_gem_color = Search_most_big_explosion_with_this_color(enemy_previous_exploded_color[0]);
						}
					else
						best_explosion_with_previous_main_gem_color = Search_most_big_explosion_with_this_color(enemy_previous_exploded_color[0]);

					}

		//check previous minor color
			if ( (enemy_previous_exploded_color[1] >= 0) //il previous main gem have exploded something
			    &&(gem_color[enemy_previous_exploded_color[1]].Count > 0) ) //and there are same color available now
				{
				if (use_armor)
					{
					if ( (player_armor[enemy_previous_exploded_color[1]] != armor.absorb) || (player_armor[enemy_previous_exploded_color[1]] != armor.repel) ) //avoid to use a color useful for the player
						best_explosion_with_previous_minor_gem_color = Search_most_big_explosion_with_this_color(enemy_previous_exploded_color[1]);
					}
					else
						best_explosion_with_previous_minor_gem_color = Search_most_big_explosion_with_this_color(enemy_previous_exploded_color[1]);
				}

		//choose the best color between main and minor
		if ( (best_explosion_with_previous_main_gem_color >= 0) && (best_explosion_with_previous_minor_gem_color >= 0) )
			{
			if (list_of_moves_possible[best_explosion_with_previous_main_gem_color,0] >= list_of_moves_possible[best_explosion_with_previous_minor_gem_color,0])
				enemy_move_selected = list_of_moves_possible[best_explosion_with_previous_main_gem_color,0];
			else
				enemy_move_selected = list_of_moves_possible[best_explosion_with_previous_minor_gem_color,0];
			}
		else if (best_explosion_with_previous_main_gem_color >= 0)
			enemy_move_selected = best_explosion_with_previous_main_gem_color;
		else if (best_explosion_with_previous_minor_gem_color >= 0)
			enemy_move_selected = best_explosion_with_previous_minor_gem_color;

		//debug
		/*
		if (enemy_move_selected >= 0)
			{
			Debug.Log("enemy_move_selected " + enemy_move_selected);
			Debug.Log("Search_same_color_of_previous_move find: " + list_of_moves_possible[enemy_move_selected,1] + ";" + list_of_moves_possible[enemy_move_selected,2] + "exp = " + list_of_moves_possible[enemy_move_selected,0]);
			}*/
		}

	void Aggressive()
	{
		//check if you have an attack spell ready
		//else perform aggressive strategy (use manual array 1)
	}

	void Defensive()
	{
		//check if you have a defensive spell ready
		//else perform defensive strategy (use manual array 2)
	}

	void Arrange_gems_by_effectiveness_against_player_armor()
	{
		int temp_count = 1000;
		int temp_array_place = 0;
		bool[] gem_aready_checked = new bool[gem_length];
		for (int n = 0; n < gem_length; n++)
		{
			for (int i = 0; i < gem_length; i++)
			{
				if (gem_aready_checked[i] == false)
				{
					if ( (int)player_armor[i] <= temp_count)
					{
						temp_count = (int)player_armor[i];
						temp_array_place = i;
						enemy_AI_preference_order[n] = (enemy_AI_manual_setup)i;
					}
				}
				if (i == gem_length - 1)//if the loop is end
				{
					//check the gem
					gem_aready_checked[temp_array_place] = true;
					temp_count = 1000;
				}
			}
		}
	}


	void Arrange_gems_by_necessity()
	{
		//Arrange gem priority from gems more need to less need
		int temp_count = 0;
		int temp_array_place = 0;
		bool[] gem_aready_checked = new bool[gem_length];
		for (int n = 0; n < gem_length; n++)
		{
			for (int i = 0; i < gem_length; i++)
			{
				if (gem_aready_checked[i] == false)
				{
					//Debug.Log("i = " + i + "number_of_gems_to_destroy_to_win[i] = "  + number_of_gems_to_destroy_to_win[i] + " ** number_of_gems_collect_by_the_enemy[i] = " + number_of_gems_collect_by_the_enemy[i]);
					if ((number_of_gems_to_destroy_to_win[i] - number_of_gems_collect_by_the_enemy[i]) > temp_count)
					{
						temp_count = (number_of_gems_to_destroy_to_win[i] - number_of_gems_collect_by_the_enemy[i]);
						temp_array_place = i;
						enemy_AI_preference_order[n] = (enemy_AI_manual_setup)i;
					}
				}
				if (i == gem_length - 1)//if loop end
				{
					//check the gem
					gem_aready_checked[temp_array_place] = true;
					temp_count = 0;
				}
			}
		}
	}

	void Enemy_search_main_gem()
	{
		for (int i = 0; i < enemy_AI_preference_order.Length; i++)
		{
			foreach (enemy_AI_manual_setup p in Enum.GetValues(typeof(enemy_AI_manual_setup)))
			{
				if ( (enemy_AI_preference_order[i] == p) )//&& ((int)p < gem_length) )
				{
					if (gem_color[(int)p].Count > 0)
					{
						if ( (gem_color[(int)p].Count > 1) && ((number_of_gems_to_destroy_to_win[(int)p]-number_of_gems_collect_by_the_enemy[(int)p]) > 0 ) )
							enemy_move_selected = ((int)gem_color[(int)p][UnityEngine.Random.Range(0,(int)gem_color[(int)p].Count)]);
						else
							enemy_move_selected = ((int)gem_color[(int)p][0]);

						return;
					}
				}
				
			}
			if (i == enemy_AI_preference_order.Length-1)// If you can't find a useful move, choose one by random
				{
				Debug.Log("If you can't find a useful move, choose one by random");
				enemy_move_selected = UnityEngine.Random.Range(0,number_of_gems_moveable-1);
				}
		}
	}

	void Enemy_select_minor_gem()
	{
		available_directions = new ArrayList();

		if (search_best_move)
			{
			int best_direction = 4;


			for (int i = 4; i <= 7; i++)//search big explosion
				{
				if ( list_of_moves_possible[enemy_move_selected,i] > list_of_moves_possible[enemy_move_selected,best_direction])
					best_direction = i;
				else if ( list_of_moves_possible[enemy_move_selected,i] == list_of_moves_possible[enemy_move_selected,best_direction])
					{
					if ( UnityEngine.Random.Range(1,100) > UnityEngine.Random.Range(40,60) ) //random update gem with the same explosive power
						best_direction = i;
					}
				}

			enemy_move_direction = best_direction;
			}
		else
			{
			for (int i = 4; i <= 7; i++)
				{
				if ( list_of_moves_possible[enemy_move_selected,i] > 0)
					available_directions.Add(i);
				}

			if (available_directions.Count > 1)
				enemy_move_direction = (int)available_directions[UnityEngine.Random.Range(0,available_directions.Count)];
			else
				enemy_move_direction = (int)available_directions[0];
			}

	}


	void Subdivide_moves_by_color()
	{
		gem_color = new ArrayList[gem_length];
		for (int i = 0; i < gem_length; i++)
			gem_color[i] = new ArrayList();
		
		for (int i = 0; i < number_of_gems_moveable; i++)
		{
			gem_color[list_of_moves_possible[i,3]].Add(i);
		}
		/*
		Debug.Log("red: " + gem_color[0].Count
		          + "; blue: " + gem_color[1].Count
		          + "; violet: " + gem_color[2].Count
		          + "; yellow: " + gem_color[3].Count
		          + "; green: " + gem_color[4].Count
		          );
		if (gem_color[0].Count > 0)
		{
			for (int i = 0; i < gem_color[0].Count; i++)
				Debug.Log("red: " + list_of_moves_possible[(int)gem_color[0][i],1]
				          +","
				          + list_of_moves_possible[(int)gem_color[0][i],2]
				          );
		}
		if (gem_color[1].Count > 0)
		{
			for (int i = 0; i < gem_color[1].Count; i++)
				Debug.Log("blue: " + list_of_moves_possible[(int)gem_color[1][i],1]
				          +","
				          + list_of_moves_possible[(int)gem_color[1][i],2]
				          );
		}
		if (gem_color[2].Count > 0)
		{
			for (int i = 0; i < gem_color[2].Count; i++)
				Debug.Log("violet: " + list_of_moves_possible[(int)gem_color[2][i],1]
				          +","
				          + list_of_moves_possible[(int)gem_color[2][i],2]
				          );
		}
		if (gem_color[3].Count > 0)
		{
			for (int i = 0; i < gem_color[3].Count; i++)
				Debug.Log("yellow: " + list_of_moves_possible[(int)gem_color[3][i],1]
				          +","
				          + list_of_moves_possible[(int)gem_color[3][i],2]
				          );
		}
		if (gem_color[4].Count > 0)
		{
			for (int i = 0; i < gem_color[4].Count; i++)
				Debug.Log("green: " + list_of_moves_possible[(int)gem_color[4][i],1]
				          +","
				          + list_of_moves_possible[(int)gem_color[4][i],2]
				          );
		}
		*/
	}


	void Enemy_select_main_gem(int move_number)
	{
			cursor.position = tiles_array[list_of_moves_possible[move_number,1],list_of_moves_possible[move_number,2]].transform.position;
			tile_C script_main_gem = (tile_C)tiles_array[list_of_moves_possible[move_number,1],list_of_moves_possible[move_number,2]].GetComponent("tile_C");
				script_main_gem.I_become_main_gem();

	}

	void Enemy_move()
	{
		if (enemy_move_direction == 4)//down
		{
			tile_C script_minor_gem = (tile_C)tiles_array[list_of_moves_possible[enemy_move_selected,1],list_of_moves_possible[enemy_move_selected,2]+1].GetComponent("tile_C");
				script_minor_gem.I_become_minor_gem(enemy_move_direction);
		}
		else if (enemy_move_direction == 5)//up
		{
			tile_C script_minor_gem = (tile_C)tiles_array[list_of_moves_possible[enemy_move_selected,1],list_of_moves_possible[enemy_move_selected,2]-1].GetComponent("tile_C");
				script_minor_gem.I_become_minor_gem(enemy_move_direction);
			
		}
		else if (enemy_move_direction == 6)//R
		{
			tile_C script_minor_gem = (tile_C)tiles_array[list_of_moves_possible[enemy_move_selected,1]+1,list_of_moves_possible[enemy_move_selected,2]].GetComponent("tile_C");
				script_minor_gem.I_become_minor_gem(enemy_move_direction);
			
		}
		else if (enemy_move_direction == 7)//L
		{
			tile_C script_minor_gem = (tile_C)tiles_array[list_of_moves_possible[enemy_move_selected,1]-1,list_of_moves_possible[enemy_move_selected,2]].GetComponent("tile_C");
				script_minor_gem.I_become_minor_gem(enemy_move_direction);
		}
	}

	#endregion


	#region win and lose
	public void Player_win()
	{
		if (!game_end && !player_win)
			{
			player_win = true;

			if (!continue_to_play_after_win_until_lose_happen)
				{
				game_end = true;
				player_can_move = false;
				}
			else
				{
				praise_script.Win_and_continue_to_play(0);
				if (current_star_score < 1)
					current_star_score = 1;
				}
			//Debug.LogWarning("You win!");
			}
	}

	public void Player_lose()
	{
		if (!game_end)
			{
			game_end = true;
			if (player_can_move)
				{
				player_can_move = false;
				Annotate_potential_moves();
				}
			//Debug.LogWarning("You lose!");
			}
	}

	void Show_win_screen()
	{
		if (lose_requirement_selected == lose_requirement.player_hp_is_zero)
			player_score += (every_hp_saved_give * current_player_hp);
		else if (lose_requirement_selected == lose_requirement.timer)
			player_score += (int)(every_second_saved_give * time_left);
		else if (lose_requirement_selected == lose_requirement.player_have_zero_moves)
			player_score +=  (every_move_saved_give * current_player_moves_left);

		if (Stage_uGUI_obj)//use menu kit win screen
			{
			/* if you have the menu kit, DELETE THIS LINE

			Debug.Log("time_left: "  + time_left + " ** three_stars_target_time_spared: "+ three_stars_target_time_spared + " ** two_stars_target_time_spared: " + two_stars_target_time_spared);
			if (player_score >= three_stars_target_score) //3 stars
				{
				if (continue_to_play_after_win_until_lose_happen)
					{
					//
					//if (win_requirement_selected == win_requirement.collect_gems)
					//	{
					//	if (three_stars_target_additional_gems_collected <= additiona_gems_collected_by_the_player)
					//		current_star_score= 3;
					//	}
					//else
					//	current_star_score = 3;
					}
				else
					{
					if (((lose_requirement_selected == lose_requirement.enemy_reach_target_score) && ( (player_score-enemy_score) >= three_stars_target_score_advantage_vs_enemy))
					|| ((lose_requirement_selected == lose_requirement.player_hp_is_zero) && (current_player_hp >= three_stars_target_player_hp_spared))
					|| ((lose_requirement_selected == lose_requirement.timer) && (time_left >= three_stars_target_time_spared))
					|| ((lose_requirement_selected == lose_requirement.player_have_zero_moves) && (current_player_moves_left >= three_stars_target_moves_spared))
					|| ((lose_requirement_selected == lose_requirement.enemy_collect_gems) && ( total_number_of_gems_remaining_for_the_enemy >= three_stars_target_gems_collect_advantage_vs_enemy))
					)
						{
						current_star_score = 3;
						}
					}
				}
			else if (player_score >= two_stars_target_score)//2 stars
				{
				if (continue_to_play_after_win_until_lose_happen)
					{

					//if (win_requirement_selected == win_requirement.collect_gems)
					//{
					//	if (two_stars_target_additional_gems_collected <= additiona_gems_collected_by_the_player)
					//		current_star_score = 2;
					//}
					//else
					//	current_star_score = 2;
					//}
				else
					{
					if (((lose_requirement_selected == lose_requirement.enemy_reach_target_score) && ( (player_score-enemy_score) >= two_stars_target_score_advantage_vs_enemy))
					|| ((lose_requirement_selected == lose_requirement.player_hp_is_zero) && (current_player_hp >= two_stars_target_player_hp_spared))
					|| ((lose_requirement_selected == lose_requirement.timer) && (time_left >= two_stars_target_time_spared))
					|| ((lose_requirement_selected == lose_requirement.player_have_zero_moves) && (current_player_moves_left >= two_stars_target_moves_spared))
					|| ((lose_requirement_selected == lose_requirement.enemy_collect_gems) && ( total_number_of_gems_remaining_for_the_enemy >= two_stars_target_gems_collect_advantage_vs_enemy))
					)
						{
						current_star_score = 2;
						}
					}
				}
			else //1 star
				{
				current_star_score = 1;
				}

			my_game_uGUI.star_number = current_star_score;
			Debug.Log(my_game_uGUI.star_number + " stars");
			my_game_uGUI.Victory();
			//if you have the menu kit, DELETE THIS LINE */
			}
		else //use default win screen
			{
			gui_win_screen.SetActive(true);
			Debug.LogWarning("show win screen!");
			Play_sfx(win_sfx);
			}

	}

	void Show_lose_screen()
	{
		if (Stage_uGUI_obj)//use menu kit win screen
			{
			/* if you have the menu kit, DELETE THIS LINE
				my_game_uGUI.Defeat();
			//if you have the menu kit, DELETE THIS LINE */
			}
		else //use default win screen
			{
			gui_lose_screen.SetActive(true);
			Debug.LogWarning("show lose screen!");
			Play_sfx(lose_sfx);
			}
	}

	#endregion

	public void Play_sfx(AudioClip my_clip)
	{
		if (Stage_uGUI_obj)//use menu kit win screen
			{
			/* if you have the menu kit, DELETE THIS LINE
			if (my_game_master && my_clip)
				my_game_master.Gui_sfx(my_clip);
			//if you have the menu kit, DELETE THIS LINE */
			}
		else
			{
			if (my_clip)
				{
				GetComponent<AudioSource>().clip = my_clip;
				GetComponent<AudioSource>().Play();
				}
			}
	}

	public void Play_bonus_sfx(int bonus_id, bool call_from_GUIbutton = false)
	{
		if (bonus_sfx [bonus_id] != null)
		{
			play_this_bonus_sfx = bonus_id;
			Play_sfx(bonus_sfx[bonus_id]);
		}
		else
			play_this_bonus_sfx = -1;

		if(call_from_GUIbutton)
			play_this_bonus_sfx = -1;
		
	}


}
