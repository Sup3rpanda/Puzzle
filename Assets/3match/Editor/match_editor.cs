using UnityEngine;
using System.Collections;

using UnityEditor;
using System.IO;
using System;

public class match_editor : EditorWindow {

	int[] temp_array;
	int array_length = 5;
	int[,,] board_array;
		int[,,] undo_board_array;
		int[,,] redo_board_array;
		bool can_redo;
	/* 0 = tile [-1 = "no tile"; 0 = "hp = 0"; 1 = "hp 1"...]
	 * 1 = gem [-99 = no gem; 
	 * 			from 0 to 8 color gem; 
	 * 			9 = special:
	 * 				junk; 
	 * 				token; 
	 * 				bonus;
	 * 			10 = random gem; 
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

	 * 
	 * 				213 = destroy single random gems (meteor shower)
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
	 * 4 = special [0 = no ice; 
	 * 	 * 
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
	 * 				-100 = junk
	 * 				-200 = token
	 */

	#region icons
	public Texture2D no_tile = Resources.Load("editor_icons/no_tile_ico")as Texture2D ;
	public Texture2D tile = Resources.Load("editor_icons/tile_ico")as Texture2D ;
	public Texture2D tile_hp_0 = Resources.Load("editor_icons/tile_hp_0_ico")as Texture2D ;
	public Texture2D tile_hp_1 = Resources.Load("editor_icons/tile_hp_1_ico")as Texture2D ;
	public Texture2D tile_hp_2 = Resources.Load("editor_icons/tile_hp_2_ico")as Texture2D ;
	public Texture2D tile_hp_3 = Resources.Load("editor_icons/tile_hp_3_ico")as Texture2D ;

	public Texture2D gem_random = Resources.Load("editor_icons/gem_random")as Texture2D ;
	public Texture2D gem_color_0 = Resources.Load("editor_icons/gem_color_0")as Texture2D ;
	public Texture2D gem_color_1 = Resources.Load("editor_icons/gem_color_1")as Texture2D ;
	public Texture2D gem_color_2 = Resources.Load("editor_icons/gem_color_2")as Texture2D ;
	public Texture2D gem_color_3 = Resources.Load("editor_icons/gem_color_3")as Texture2D ;
	public Texture2D gem_color_4 = Resources.Load("editor_icons/gem_color_4")as Texture2D ;
	public Texture2D gem_color_5 = Resources.Load("editor_icons/gem_color_5")as Texture2D ;
	public Texture2D gem_color_6 = Resources.Load("editor_icons/gem_color_6")as Texture2D ;

	public Texture2D junk = Resources.Load("editor_icons/junk_ico")as Texture2D ;
	public Texture2D token = Resources.Load("editor_icons/token_ico")as Texture2D ;

	public Texture2D immune_block = Resources.Load("editor_icons/immune_block_ico")as Texture2D ;

	public Texture2D block_1 = Resources.Load("editor_icons/block_1_ico")as Texture2D ;
	public Texture2D block_2 = Resources.Load("editor_icons/block_2_ico")as Texture2D ;
	public Texture2D block_3 = Resources.Load("editor_icons/block_3_ico")as Texture2D ;

	public Texture2D falling_block_1 = Resources.Load("editor_icons/falling_block_1_ico")as Texture2D ;
	public Texture2D falling_block_2 = Resources.Load("editor_icons/falling_block_2_ico")as Texture2D ;
	public Texture2D falling_block_3 = Resources.Load("editor_icons/falling_block_3_ico")as Texture2D ;

	public Texture2D path_tile = Resources.Load("editor_icons/tile_path_ico")as Texture2D ;
	public Texture2D start_tile = Resources.Load("editor_icons/start_ico")as Texture2D ;
	public Texture2D goal_tile = Resources.Load("editor_icons/goal_ico")as Texture2D ;

	public Texture2D padlock_1 = Resources.Load("editor_icons/padlock_1_ico")as Texture2D ;
	public Texture2D padlock_2 = Resources.Load("editor_icons/padlock_2_ico")as Texture2D ;
	public Texture2D padlock_3 = Resources.Load("editor_icons/padlock_3_ico")as Texture2D ;

	public Texture2D time_bomb = Resources.Load("editor_icons/time_bomb_ico")as Texture2D ;

	public Texture2D bonus_bomb = Resources.Load("editor_icons/bonus_bomb_ico")as Texture2D ;
	public Texture2D bonus_horiz = Resources.Load("editor_icons/bonus_horiz_ico")as Texture2D ;
	public Texture2D bonus_vertic = Resources.Load("editor_icons/bonus_vertic_ico")as Texture2D ;
	public Texture2D bonus_horiz_and_vertic = Resources.Load("editor_icons/bonus_horiz_and_vertic_ico")as Texture2D ;
		
	public Texture2D ice_1 = Resources.Load("editor_icons/ice_1_ico")as Texture2D ;
	public Texture2D ice_2 = Resources.Load("editor_icons/ice_2_ico")as Texture2D ;
	public Texture2D ice_3 = Resources.Load("editor_icons/ice_3_ico")as Texture2D ;

	public Texture2D door_a = Resources.Load("editor_icons/door_a_ico")as Texture2D ;
	public Texture2D door_b = Resources.Load("editor_icons/door_b_ico")as Texture2D ;
	public Texture2D door_c = Resources.Load("editor_icons/door_c_ico")as Texture2D ;
	public Texture2D door_d = Resources.Load("editor_icons/door_d_ico")as Texture2D ;
	public Texture2D door_e = Resources.Load("editor_icons/door_e_ico")as Texture2D ;

	public Texture2D key_a = Resources.Load("editor_icons/key_a_ico")as Texture2D ;
	public Texture2D key_b = Resources.Load("editor_icons/key_b_ico")as Texture2D ;
	public Texture2D key_c = Resources.Load("editor_icons/key_c_ico")as Texture2D ;
	public Texture2D key_d = Resources.Load("editor_icons/key_d_ico")as Texture2D ;
	public Texture2D key_e = Resources.Load("editor_icons/key_e_ico")as Texture2D ;

	public Texture2D item_a = Resources.Load("editor_icons/item_a_ico")as Texture2D ;
	public Texture2D item_b = Resources.Load("editor_icons/item_b_ico")as Texture2D ;
	public Texture2D item_c = Resources.Load("editor_icons/item_c_ico")as Texture2D ;
	public Texture2D item_d = Resources.Load("editor_icons/item_d_ico")as Texture2D ;
	public Texture2D item_e = Resources.Load("editor_icons/item_e_ico")as Texture2D ;

	public Texture2D need_a = Resources.Load("editor_icons/need_a_ico")as Texture2D ;
	public Texture2D need_b = Resources.Load("editor_icons/need_b_ico")as Texture2D ;
	public Texture2D need_c = Resources.Load("editor_icons/need_c_ico")as Texture2D ;
	public Texture2D need_d = Resources.Load("editor_icons/need_d_ico")as Texture2D ;
	public Texture2D need_e = Resources.Load("editor_icons/need_e_ico")as Texture2D ;
	#endregion

	enum tile_hp
	{
		no_tile,
		hp_0,
		hp_1,
		hp_2,
		hp_3
	}
	tile_hp tile_selected = tile_hp.hp_1;

	enum special_0_hp_tile
	{
		none
		/*
		start,
		goal,
		path,
		door_a,
		door_b,
		door_c,
		door_d,
		door_e,
		item_a,
		item_b,
		item_c,
		item_d,
		item_e*/
	}
	special_0_hp_tile special_0_hp_tile_selected = special_0_hp_tile.none;

	enum tile_content
	{
		empty,
		random_gem,
		gem_color_0,
		gem_color_1,
		gem_color_2,
		gem_color_3,
		gem_color_4,
		gem_color_5,
		gem_color_6,
		block_hp_1,
		block_hp_2,
		block_hp_3,
		/*
		immune_block,
		falling_block_hp_1,
		falling_block_hp_2,
		falling_block_hp_3,
		*/
		junk,
		token,
		//bonus_destroy_one,
		//bonus_switch_gem_teleport,
		bonus_bomb,
		bonus_horiz,
		bonus_vertic,
		bonus_horiz_and_vertic
		//bonus_destroy_all_same_color,
		/*
		bonus_more_time
		bonus_more_moves
		bonus_more_hp
		bonus_rotate_board_L
		bonus_rotate_board_R

		need_a,
		need_b,
		need_c,
		need_d,
		need_e,
		key_a,
		key_b,
		key_c,
		key_d,
		key_e*/

	}
	tile_content tile_content_selected = tile_content.random_gem;

	enum restraint
	{
		none,
		padlock_hp_1,
		padlock_hp_2,
		padlock_hp_3
		/*
		ice_hp_1,
		ice_hp_2,
		ice_hp_3*/

	}
	restraint restraint_selected = restraint.none;
	/*
	enum special
	{
		none
		time_bomb,

	}
	special special_selected = special.none;*/

	bool a_tile_is_selected;
	bool allow_padlock_and_complication;
	//bool start_or_goal_is_selected;
	

	public GameObject editable_tile_obj;
	

	float size_button = 50;

	
	int x_button_group;
	int y_button_group;
	
	//board dimensions
		int x_tiles = 15;
		int board_x_tiles;
		int y_tiles = 15;
		int board_y_tiles;
		//limits of board dimensions
		int MAX_x = 30;
		int MAX_y = 30;
		int MIN_x = 5;
		int MIN_y = 5;
	bool updated_x_value;
	bool updated_y_value;
	
	#region board tileset variables

	public static bool _show_board = false; 
	#endregion

	//save
	string save_string;
	string content_txt;
	//load
	string load_path;
	string fileContents = string.Empty;

	string be_careful = "You'll lose all unsaved modifications";


	Rect windowRect;
	int window_edges = 20;
	int up_start_position = 100;

	Rect create_board_window = new Rect(20,
	                                  	0,
	                                 	0,
	                                    0);
	Rect tiles_window 		= new Rect(20 + 430,
	                            		0,
	                            		300,0);
	Rect preview_window 	= new Rect(20 + 735,
	                                0,
	                                80,118);
		Rect preview_tile = new Rect(10,60,60,60);




	// Scroll position
	Vector2 scrollPos = Vector2.zero;
	


	[MenuItem("Window/3Match Editor")]  
	static void Init () {

		match_editor window = (match_editor)EditorWindow.CreateInstance(typeof(match_editor));
		window.Show();
		
		
	}

	void Awake()
		{
		temp_array = new int[array_length];
		}

	void OnGUI () {

		BeginWindows ();
			create_board_window  = GUILayout.Window(0,create_board_window ,Create_board_window, "File");
			GUILayout.Window(1,preview_window,Tile_preview, "Preview" , GUILayout.Width(80),GUILayout.Height(100));	
			GUILayout.Window(1,preview_window,Tile_preview, "Preview" , "window");	

			tiles_window = GUILayout.Window(2,tiles_window,Tiles_tools, "Tile proprieties");
			if (_show_board)
				{
				windowRect = new Rect(window_edges, window_edges+up_start_position, position.width-window_edges*2, position.height-window_edges*2-up_start_position);
				windowRect = GUILayout.Window(3, windowRect, Board_window, "Board");				
				}
		EndWindows ();
	

	}

	void Board_creation()
	{
		_show_board = false;
		board_x_tiles = x_tiles;
		board_y_tiles = y_tiles;
		
		board_array = new int[x_tiles,y_tiles,array_length];

		Fill();

		undo_board_array = new int[x_tiles,y_tiles,array_length];
		can_redo = false;
		Array.Copy(board_array, undo_board_array, board_array.Length);
		_show_board = true;
	}

	void Create_board_window(int windowID)
	{
		if (GUILayout.Button("Create board"))
			{
			if (_show_board)
				{
				if (EditorUtility.DisplayDialog("New board",be_careful,"ok","cancel") )
					{
					Board_creation();
					}
				}
			else
				Board_creation();
			}

		EditorGUILayout.BeginHorizontal();
			x_tiles = EditorGUILayout.IntSlider ("x", x_tiles, MIN_x, MAX_x);
			y_tiles = EditorGUILayout.IntSlider ("y", y_tiles, MIN_y, MAX_y);
		EditorGUILayout.EndHorizontal();

		
		
		if (GUILayout.Button("Load board"))
		{
			if (_show_board)
				{
				if (EditorUtility.DisplayDialog("Load board",be_careful,"ok","cancel") )
					{
					Open_load();
					}
				}
			else
				Open_load();
		}
		if (GUILayout.Button("Save board"))
		{
			save_string = EditorUtility.SaveFilePanel(
				"Save level as TXT",
				"You MUST save this file in -Resources- folder of this project",
				"3match_stage" + ".txt",
				"txt");
			if (save_string.Length != 0)
			{
				Save(save_string);
				save_string = string.Empty;
			}
		}
		

	}

	void Open_load()
	{
		Reset();
		load_path = EditorUtility.OpenFilePanel(
				"Load board file",
				"",
				"txt");
		if (load_path.Length != 0)
			{
			Load(load_path);
			_show_board = true;
			}
	}

	void Fill()
	{
		if (_show_board)
			{
			Array.Copy(board_array, undo_board_array, board_array.Length);
			can_redo = false;
			}

		for(int y = 0; y < board_y_tiles; y++)
		{
			for (int x = 0; x < board_x_tiles; x++)
			{
				for (int n = 0; n < array_length ; n++)
					board_array[x,y,n] = temp_array[n];;
			}
		}
	}

	void Tiles_tools(int windowID)
	{


			if (tile_selected != tile_hp.no_tile)
				{
				a_tile_is_selected = true;
				/*
				if ( (special_0_hp_tile_selected == special_0_hp_tile.start) || (special_0_hp_tile_selected == special_0_hp_tile.goal) )
					{
					start_or_goal_is_selected = true;

					temp_array[0] = 0;
					tile_selected = tile_hp.hp_0;

					temp_array[1] = -99;
					tile_content_selected = tile_content.empty;
					
					temp_array[3] = 0;
					restraint_selected = restraint.none;

					temp_array[4] = 0;
					special_selected = special.none;

					}
				else
					{
					start_or_goal_is_selected = false;
					}
				*/
				if (special_0_hp_tile_selected == special_0_hp_tile.none)
					temp_array[2] = 0;

				if (tile_selected == tile_hp.hp_0)
					{
					/*
					if ( (special_0_hp_tile_selected == special_0_hp_tile.item_a)
						|| (special_0_hp_tile_selected == special_0_hp_tile.item_b)
						|| (special_0_hp_tile_selected == special_0_hp_tile.item_c)
						|| (special_0_hp_tile_selected == special_0_hp_tile.item_d)
						|| (special_0_hp_tile_selected == special_0_hp_tile.item_e) )
							{
							temp_array[1] = -99;
							tile_content_selected = tile_content.empty;

							allow_padlock_and_complication = false;
							}
							*/
					}
				}
			else
				{
				temp_array[0] = -1;
				a_tile_is_selected = false;

				temp_array[1] = -99;
				tile_content_selected = tile_content.empty;

				temp_array[2] = 0;
				special_0_hp_tile_selected = special_0_hp_tile.none;
				
				temp_array[3] = 0;
				restraint_selected = restraint.none;

				temp_array[4] = 0;
				//special_selected = special.none;
				}



			//if (!start_or_goal_is_selected)
				tile_selected = (tile_hp)EditorGUILayout.EnumPopup("tile",tile_selected);
			
			if ( (tile_content_selected == tile_content.random_gem)
			    || (tile_content_selected == tile_content.gem_color_0)
			    || (tile_content_selected == tile_content.gem_color_1)
			    || (tile_content_selected == tile_content.gem_color_2)
			    || (tile_content_selected == tile_content.gem_color_3)
			    || (tile_content_selected == tile_content.gem_color_4)
		    	|| (tile_content_selected == tile_content.gem_color_5)
		    	|| (tile_content_selected == tile_content.gem_color_6)
		   	 	)
			{
				allow_padlock_and_complication = true;
			}
			else
				{
				allow_padlock_and_complication = false;

						temp_array[3] = 0;
						restraint_selected = restraint.none;
						temp_array[4] = 0;
						//special_selected = special.none;

				}
			
			if (a_tile_is_selected)
				{
				//special_0_hp_tile_selected = (special_0_hp_tile)EditorGUILayout.EnumPopup("special 0 hp tile",special_0_hp_tile_selected);

				//if (!start_or_goal_is_selected)
					//{
					tile_content_selected = (tile_content)EditorGUILayout.EnumPopup("tile content",tile_content_selected);
					if (allow_padlock_and_complication)
						{
						restraint_selected = (restraint)EditorGUILayout.EnumPopup("restraint",restraint_selected);
						//special_selected = (special)EditorGUILayout.EnumPopup("special",special_selected);
						}
					//}
				}
		
	}

	void Tile_preview(int windowID)
	{
		if (tile_selected == tile_hp.no_tile)
			GUI.Label(preview_tile,no_tile);
		else
			{
			/*
			if ( (special_0_hp_tile_selected == special_0_hp_tile.path) || (special_0_hp_tile_selected == special_0_hp_tile.goal) || (special_0_hp_tile_selected == special_0_hp_tile.start) )
				{
				GUI.Label(preview_tile,path_tile);
				temp_array[2] = 3;
				}
			else*/
				GUI.Label(preview_tile,tile);

			if (tile_selected == tile_hp.hp_0)
				{
				GUI.Label(preview_tile,tile_hp_0);
				temp_array[0] = 0;
				}
			else if (tile_selected == tile_hp.hp_1)
				{
				GUI.Label(preview_tile,tile_hp_1);
				temp_array[0] = 1;
				}
			else if (tile_selected == tile_hp.hp_2)
				{
				GUI.Label(preview_tile,tile_hp_2);
				temp_array[0] = 2;
				}
			else if (tile_selected == tile_hp.hp_3)
				{
				GUI.Label(preview_tile,tile_hp_3);
				temp_array[0] = 3;
				}
			/*
			if (special_0_hp_tile_selected == special_0_hp_tile.start)
				{
				GUI.Label(preview_tile,start_tile);
				temp_array[2] = 1;
				}
			else if (special_0_hp_tile_selected == special_0_hp_tile.goal)
				{
				GUI.Label(preview_tile,goal_tile);
				temp_array[2] = 2;
				}
			else if (special_0_hp_tile_selected == special_0_hp_tile.door_a)
				{
				GUI.Label(preview_tile,door_a);
				temp_array[2] = 10;
				}
			else if (special_0_hp_tile_selected == special_0_hp_tile.door_b)
				{
				GUI.Label(preview_tile,door_b);
				temp_array[2] = 11;
				}
			else if (special_0_hp_tile_selected == special_0_hp_tile.door_c)
				{
				GUI.Label(preview_tile,door_c);
				temp_array[2] = 12;
				}
			else if (special_0_hp_tile_selected == special_0_hp_tile.door_d)
				{
				GUI.Label(preview_tile,door_d);
				temp_array[2] = 13;
				}
			else if (special_0_hp_tile_selected == special_0_hp_tile.door_e)
				{
				GUI.Label(preview_tile,door_e);
				temp_array[2] = 14;
				}
			else if (special_0_hp_tile_selected == special_0_hp_tile.item_a)
				{
				GUI.Label(preview_tile,item_a);
				temp_array[2] = 20;
				}
			else if (special_0_hp_tile_selected == special_0_hp_tile.item_b)
				{
				GUI.Label(preview_tile,item_b);
				temp_array[2] = 21;
				}
			else if (special_0_hp_tile_selected == special_0_hp_tile.item_c)
				{
				GUI.Label(preview_tile,item_c);
				temp_array[2] = 22;
				}
			else if (special_0_hp_tile_selected == special_0_hp_tile.item_d)
				{
				GUI.Label(preview_tile,item_d);
				temp_array[2] = 23;
				}
			else if (special_0_hp_tile_selected == special_0_hp_tile.item_e)
				{
				GUI.Label(preview_tile,item_e);
				temp_array[2] = 24;
				}
			*/

			if (tile_content_selected == tile_content.empty)
			{
				temp_array[1] = -99;
			}
			else if (tile_content_selected == tile_content.random_gem)
				{
				GUI.Label(preview_tile,gem_random);
				temp_array[1] = 10;
				}
			else if (tile_content_selected == tile_content.gem_color_0)
				{
				GUI.Label(preview_tile,gem_color_0);
				temp_array[1] = 0;
				}
			else if (tile_content_selected == tile_content.gem_color_1)
				{
				GUI.Label(preview_tile,gem_color_1);
				temp_array[1] = 1;
				}
			else if (tile_content_selected == tile_content.gem_color_2)
				{
				GUI.Label(preview_tile,gem_color_2);
				temp_array[1] = 2;
				}
			else if (tile_content_selected == tile_content.gem_color_3)
				{
				GUI.Label(preview_tile,gem_color_3);
				temp_array[1] = 3;
				}
			else if (tile_content_selected == tile_content.gem_color_4)
				{
				GUI.Label(preview_tile,gem_color_4);
				temp_array[1] = 4;
				}
			else if (tile_content_selected == tile_content.gem_color_5)
				{
				GUI.Label(preview_tile,gem_color_5);
				temp_array[1] = 5;
				}
			else if (tile_content_selected == tile_content.gem_color_6)
				{
				GUI.Label(preview_tile,gem_color_6);
				temp_array[1] = 6;
				}
			/*
			else if (tile_content_selected == tile_content.immune_block)
				{
				GUI.Label(preview_tile,immune_block);
				temp_array[1] = 40;
				}
				*/
			else if (tile_content_selected == tile_content.block_hp_1)
				{
				GUI.Label(preview_tile,block_1);
				temp_array[1] = 41;
				}
			else if (tile_content_selected == tile_content.block_hp_2)
				{
				GUI.Label(preview_tile,block_2);
				temp_array[1] = 42;
				}
			else if (tile_content_selected == tile_content.block_hp_3)
				{
				GUI.Label(preview_tile,block_3);
				temp_array[1] = 43;
				}
			/*
			else if (tile_content_selected == tile_content.falling_block_hp_1)
				{
				GUI.Label(preview_tile,falling_block_1);
				temp_array[1] = 51;
				}
			else if (tile_content_selected == tile_content.falling_block_hp_2)
				{
				GUI.Label(preview_tile,falling_block_2);
				temp_array[1] = 52;
				}
			else if (tile_content_selected == tile_content.falling_block_hp_3)
				{
				GUI.Label(preview_tile,falling_block_3);
				temp_array[1] = 53;
				}
			else if (tile_content_selected == tile_content.need_a)
				{
				GUI.Label(preview_tile,need_a);
				temp_array[1] = 60;
				}
			else if (tile_content_selected == tile_content.need_b)
				{
				GUI.Label(preview_tile,need_b);
				temp_array[1] = 61;
				}
			else if (tile_content_selected == tile_content.need_c)
				{
				GUI.Label(preview_tile,need_c);
				temp_array[1] = 62;
				}
			else if (tile_content_selected == tile_content.need_d)
				{
				GUI.Label(preview_tile,need_d);
				temp_array[1] = 63;
				}
			else if (tile_content_selected == tile_content.need_e)
				{
				GUI.Label(preview_tile,need_e);
				temp_array[1] = 64;
				}
			else if (tile_content_selected == tile_content.key_a)
				{
				GUI.Label(preview_tile,key_a);
				temp_array[1] = 70;
				}
			else if (tile_content_selected == tile_content.key_b)
				{
				GUI.Label(preview_tile,key_b);
				temp_array[1] = 71;
				}
			else if (tile_content_selected == tile_content.key_c)
				{
				GUI.Label(preview_tile,key_c);
				temp_array[1] = 72;
				}
			else if (tile_content_selected == tile_content.key_d)
				{
				GUI.Label(preview_tile,key_d);
				temp_array[1] = 73;
				}
			else if (tile_content_selected == tile_content.key_e)
				{
				GUI.Label(preview_tile,key_e);
				temp_array[1] = 74;
				}
				*/
			else if (tile_content_selected == tile_content.junk)
				{
				GUI.Label(preview_tile,junk);
				temp_array[1] = 9;
				temp_array[4] = -100;
				}
			else if (tile_content_selected == tile_content.token)
				{
				GUI.Label(preview_tile,token);
				temp_array[1] = 9;
				temp_array[4] = -200;
				}
			else if (tile_content_selected == tile_content.bonus_bomb)
				{
				GUI.Label(preview_tile,bonus_bomb);
				temp_array[1] = 9;
				temp_array[4] = 3;
				}
			else if (tile_content_selected == tile_content.bonus_horiz)
				{
				GUI.Label(preview_tile,bonus_horiz);
				temp_array[1] = 9;
				temp_array[4] = 4;
				}
			else if (tile_content_selected == tile_content.bonus_vertic)
				{
				GUI.Label(preview_tile,bonus_vertic);
				temp_array[1] = 9;
				temp_array[4] = 5;
				}
			else if (tile_content_selected == tile_content.bonus_horiz_and_vertic)
				{
				GUI.Label(preview_tile,bonus_horiz_and_vertic);
				temp_array[1] = 9;
				temp_array[4] = 6;
				}
			
			
			
			if (restraint_selected == restraint.none)
				{
				temp_array[3] = 0;
				}
			else if (restraint_selected == restraint.padlock_hp_1)
				{
				GUI.Label(preview_tile,padlock_1);
				temp_array[3] = 1;
				}
			else if (restraint_selected == restraint.padlock_hp_2)
				{
				GUI.Label(preview_tile,padlock_2);
				temp_array[3] = 2;
				}
			else if (restraint_selected == restraint.padlock_hp_3)
				{
				GUI.Label(preview_tile,padlock_3);
				temp_array[3] = 3;
				}
			/*
			else if (restraint_selected == restraint.ice_hp_1)
			{
				GUI.Label(preview_tile,ice_1);
				temp_array[3] = 11;
			}
			else if (restraint_selected == restraint.ice_hp_2)
			{
				GUI.Label(preview_tile,ice_2);
				temp_array[3] = 12;
			}
			else if (restraint_selected == restraint.ice_hp_3)
			{
				GUI.Label(preview_tile,ice_3);
				temp_array[3] = 13;
			}

			if (special_selected == special.time_bomb)
				{
				GUI.Label(preview_tile,time_bomb);
				temp_array[4] = 1;
				}
			*/


			}

		//if ( (special_0_hp_tile_selected != special_0_hp_tile.goal) && (special_0_hp_tile_selected != special_0_hp_tile.start) )
			//{
			
		if (_show_board)
			{
			if (GUILayout.Button("Fill"))
				{
				Fill();
				}
			if (can_redo)
				{
				if (GUILayout.Button("Redo"))
					{
					Board_redo();
					}
				}
			else
				{
				if (GUILayout.Button("Undo"))
					{
					Board_undo();
					}
				}
			}
			//}



	}

	void Board_redo()
	{
		Array.Copy(redo_board_array, board_array, redo_board_array.Length);
		can_redo = false;
	}

	void Board_undo()
	{
		redo_board_array = new int[x_tiles,y_tiles,array_length];
		Array.Copy(board_array, redo_board_array, board_array.Length);

		Array.Copy(undo_board_array, board_array, undo_board_array.Length);
		can_redo = true;
	}

	void Board_window(int windowID) {

		float window_size_x = (float)(size_button*board_x_tiles);
		float window_size_y = (float)(size_button*board_y_tiles);

		GUILayout.Label("");

		scrollPos = GUI.BeginScrollView (
			new Rect (0, 15, windowRect.width, windowRect.height-15), 
			scrollPos, 
			new Rect (0, 0, window_size_x, window_size_y)
			);

			for(int y = 0; y < board_y_tiles; y++)
				{
				for (int x = 0; x < board_x_tiles; x++)
					{
					Show_tile(x,y);
					}
				}


			GUI.EndScrollView ();
		
	}

	void Show_tile(int x, int y)
	{

		if (GUI.Button(new Rect(x*size_button, y*size_button, size_button, size_button),tile))
			{
			Array.Copy(board_array, undo_board_array, board_array.Length);
			can_redo = false;
			for (int n = 0; n < array_length ; n++)
				board_array[x,y,n] = temp_array[n];

			Debug.Log("tile: " + x + "," + y + " ** 1= " + board_array[x,y,1] + " ** 4 = " + board_array[x,y,4]);

		}

		if (board_array[x,y,0] <= -1)
			GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),no_tile);
		else
			{
			if ( (board_array[x,y,2] > 0) &&  (board_array[x,y,2] < 10))
				{
				GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),path_tile);
				if (board_array[x,y,2] == 1)
					GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),start_tile);
				else if (board_array[x,y,2] == 2)
					GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),goal_tile);
				}
			else
				GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),tile);

			if (board_array[x,y,0] == 0)
				GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),tile_hp_0);
			else if (board_array[x,y,0] == 1)
				GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),tile_hp_1);
			else if (board_array[x,y,0] == 2)
				GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),tile_hp_2);
			else if (board_array[x,y,0] == 3)
				GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),tile_hp_3);

			if (board_array[x,y,1] == 0)
				GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),gem_color_0);
			else if (board_array[x,y,1] == 1)
				GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),gem_color_1);
			else if (board_array[x,y,1] == 2)
				GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),gem_color_2);
			else if (board_array[x,y,1] == 3)
				GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),gem_color_3);
			else if (board_array[x,y,1] == 4)
				GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),gem_color_4);
			else if (board_array[x,y,1] == 5)
				GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),gem_color_5);
			else if (board_array[x,y,1] == 6)
				GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),gem_color_6);
			else if (board_array[x,y,1] == 9)
				{
				if (board_array[x,y,4] == 3)
					GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),bonus_bomb);
				else if (board_array[x,y,4] == 4)
					GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),bonus_horiz);
				else if (board_array[x,y,4] == 5)
					GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),bonus_vertic);
				else if (board_array[x,y,4] == 6)
					GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),bonus_horiz_and_vertic);

				else if (board_array[x,y,4] == -100)
					GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),junk);
				else if (board_array[x,y,4] == -200)
					GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),token);
				}
			else if (board_array[x,y,1] == 10)
				GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),gem_random);
			/*
			else if (board_array[x,y,1] == 20)
				GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),junk);
			else if (board_array[x,y,1] == 30)
				GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),token);
				*/
			else if (board_array[x,y,1] == 40)
				GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),immune_block);
			else if (board_array[x,y,1] == 41)
				GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button), block_1);
			else if (board_array[x,y,1] == 42)
				GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),block_2);
			else if (board_array[x,y,1] == 43)
				GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),block_3);
			else if (board_array[x,y,1] == 51)
				GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),falling_block_1);
			else if (board_array[x,y,1] == 52)
				GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),falling_block_2);
			else if (board_array[x,y,1] == 53)
				GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),falling_block_3);
			else if (board_array[x,y,1] == 60)
				GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),need_a);
			else if (board_array[x,y,1] == 61)
				GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),need_b);
			else if (board_array[x,y,1] == 62)
				GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),need_c);
			else if (board_array[x,y,1] == 63)
				GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),need_d);
			else if (board_array[x,y,1] == 64)
				GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),need_e);
			else if (board_array[x,y,1] == 70)
				GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),key_a);
			else if (board_array[x,y,1] == 71)
				GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),key_b);
			else if (board_array[x,y,1] == 72)
				GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),key_c);
			else if (board_array[x,y,1] == 73)
				GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),key_d);
			else if (board_array[x,y,1] == 74)
				GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),key_e);


			if (board_array[x,y,2] == 10)
				GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),door_a);
			else if (board_array[x,y,2] == 11)
				GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),door_b);
			else if (board_array[x,y,2] == 12)
				GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),door_c);
			else if (board_array[x,y,2] == 13)
				GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),door_d);
			else if (board_array[x,y,2] == 14)
				GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),door_e);
			else if (board_array[x,y,2] == 20)
				GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),item_a);
			else if (board_array[x,y,2] == 21)
				GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),item_b);
			else if (board_array[x,y,2] == 22)
				GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),item_c);
			else if (board_array[x,y,2] == 23)
				GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),item_d);
			else if (board_array[x,y,2] == 24)
				GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),item_e);


			if (board_array[x,y,3] == 1)
				GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),padlock_1);
			else if (board_array[x,y,3] == 2)
				GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),padlock_2);
			else if (board_array[x,y,3] == 3)
				GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),padlock_3);

			/*
			if (board_array[x,y,4] == 1)
				GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),ice_1);
			else if (board_array[x,y,4] == 2)
				GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),ice_2);
			else if (board_array[x,y,4] == 3)
				GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),ice_3);
			else if (board_array[x,y,4] == 10)
				GUI.Label(new Rect(x*size_button, y*size_button, size_button, size_button),time_bomb);
			*/
			}

	}

	#region manage files
	void Reset()//empty array
	{
		fileContents = string.Empty;
		_show_board = false;
		
		//_x_tiles = string.Empty;
		x_tiles = MIN_x;
		//_y_tiles = string.Empty;
		y_tiles = MIN_y;
	}

	void Load(string _load_path)
	{
		try
		{
			using (StreamReader reader = new StreamReader(_load_path))
			{
				fileContents = reader.ReadToEnd().ToString();
				Debug.Log("File "+ _load_path +" found");
				
				
			}
		}
		catch 
		{
			Debug.Log("File "+ _load_path +" not found");
		}

		string[] parts = fileContents.Split(new string[] { "\r\n" }, StringSplitOptions.None);
		
		x_tiles = Int16.Parse(parts[0]);
			board_x_tiles = x_tiles;
		y_tiles = Int16.Parse(parts[1]);
			board_y_tiles = y_tiles;

		board_array = new int[x_tiles,y_tiles,array_length];


		for (int y = 0; y < board_y_tiles+2; y++)
		{
			if (y > 1)
			{
				for (int x = 0; x < board_x_tiles; x++)
					{
					string[] tile = parts[y].Split(new string[] { "|" }, StringSplitOptions.None);
					//Debug.Log("x pars: " + tiles_a[x]);

					for (int z = 0; z < array_length; z++)
						{
						string[] tile_characteristic = tile[x].Split(new string[] { "," }, StringSplitOptions.None);
						//Debug.Log("z pars: " + tile_characteristic[z]);
						board_array[x,y-2,z] = Int16.Parse(tile_characteristic[z]);
						}
					}

			}
		}

		undo_board_array = new int[x_tiles,y_tiles,array_length];
		can_redo = false;
		Array.Copy(board_array, undo_board_array, board_array.Length);

	}

	void Save(string path)
	{
		//content_txt = board_x_tiles + Environment.NewLine + board_y_tiles + Environment.NewLine;
		content_txt = board_x_tiles + "\r\n" + board_y_tiles + "\r\n";


		for (int y = 0; y < board_y_tiles; y++)
		{
			for (int x = 0; x < board_x_tiles; x++)
			{
				for (int z = 0; z < array_length; z++)
				{
					content_txt += board_array[x,y,z];
					if ( (x+1 >= board_x_tiles) && (y+1<board_y_tiles) && (z+1 == array_length) )
						content_txt += "\r\n";
					else if ( (x+1 <= board_x_tiles) && (z+1 < array_length) )
						content_txt += ",";
					else if (z+1 == array_length)
						content_txt += "|";
				}
			}
		}
		System.IO.File.WriteAllText(@path,content_txt);
	}
	#endregion
}
