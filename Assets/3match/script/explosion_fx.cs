using UnityEngine;
using System.Collections;

public class explosion_fx : MonoBehaviour {

	Transform my_garbage;
	public float go_to_garbage_after_n_seconds;

	public Transform end_r;
	public Transform end_l;
	public Transform end_up;
	public Transform end_down;

	public Transform long_r;
	public Transform long_l;
	public Transform long_up;
	public Transform long_down;

	public void Activate_me(Transform garbage) //call this the the first time, when tile create the fx
		{
		my_garbage = garbage;
		Invoke("Go_to_garbage",go_to_garbage_after_n_seconds);
		}

	void Go_to_garbage()
		{
		transform.parent = my_garbage;
		}

	public void Show_me(Vector3 new_position)//call this when recycle the fx
		{
		transform.parent = null;
		transform.position = new_position;
		this.gameObject.SetActive(true);
		Invoke("Go_to_garbage",go_to_garbage_after_n_seconds);
		}

	void Reset_vertical()
		{
		end_up.position =  this.transform.position;
		long_up.position = this.transform.position;
		long_up.localScale = Vector3.one;

		end_down.position =  this.transform.position;
		long_down.position = this.transform.position;
		long_down.localScale = Vector3.one;
		}

	void Reset_horizontal()
		{
		end_r.position =  this.transform.position;
		long_r.position = this.transform.position;
		long_r.localScale = Vector3.one;
		
		end_l.position =  this.transform.position;
		long_l.position = this.transform.position;
		long_l.localScale = Vector3.one;
		}

	void Check_up(int end_up_y)
	{
		if (end_up_y > 0)
		{
			end_up.gameObject.SetActive(true);
			end_up.position = new Vector3(this.transform.position.x,
										 this.transform.position.y+end_up_y,
			                             this.transform.position.z);
			if (end_up_y > 1)
			{
				long_up.gameObject.SetActive(true);
				long_up.position = new Vector3(this.transform.position.x,
											  this.transform.position.y+end_up_y*0.5f,
				                              this.transform.position.z);
				long_up.localScale = new Vector3(1,end_up_y-1,1);
			}
			else
				long_up.gameObject.SetActive(false);
		}
		else
		{
			end_up.gameObject.SetActive(false);
			long_up.gameObject.SetActive(false);
		}
	}

	void Check_down(int end_down_y)
	{
		if (end_down_y > 0)
		{
			end_down.gameObject.SetActive(true);
			end_down.position = new Vector3(this.transform.position.x,
			                              this.transform.position.y-end_down_y,
			                              this.transform.position.z);
			if (end_down_y > 1)
			{
				long_down.gameObject.SetActive(true);
				long_down.position = new Vector3(this.transform.position.x,
				                               this.transform.position.y-end_down_y*0.5f,
				                               this.transform.position.z);
				long_down.localScale = new Vector3(1,end_down_y-1,1);
			}
			else
				long_down.gameObject.SetActive(false);
		}
		else
		{
			end_down.gameObject.SetActive(false);
			long_down.gameObject.SetActive(false);
		}
	}

	void Check_r(int end_r_x)
		{
		if (end_r_x > 0)
			{
			end_r.gameObject.SetActive(true);
			end_r.position = new Vector3(this.transform.position.x+end_r_x,
			                             this.transform.position.y,
			                             this.transform.position.z);
			if (end_r_x > 1)
				{
				long_r.gameObject.SetActive(true);
				long_r.position = new Vector3(this.transform.position.x+end_r_x*0.5f,
				                              this.transform.position.y,
				                              this.transform.position.z);
				long_r.localScale = new Vector3(1,end_r_x-1,1);
				}
			else
				long_r.gameObject.SetActive(false);
			}
		else
			{
			end_r.gameObject.SetActive(false);
			long_r.gameObject.SetActive(false);
			}
		}

	void Check_l(int end_l_x)
		{
		if (end_l_x > 0)
			{
			end_l.gameObject.SetActive(true);
			end_l.position = new Vector3(this.transform.position.x-end_l_x,
			                             this.transform.position.y,
			                             this.transform.position.z);
			if (end_l_x > 1)
				{
				long_l.gameObject.SetActive(true);
				long_l.position = new Vector3(this.transform.position.x-end_l_x*0.5f,
				                              this.transform.position.y,
				                              this.transform.position.z);
				long_l.localScale = new Vector3(1,end_l_x-1,1);
				}
			else
				long_l.gameObject.SetActive(false);
			}
		else
			{
			end_l.gameObject.SetActive(false);
			long_l.gameObject.SetActive(false);
			}
		}

	public void Setup_horizontal_and_vertical(int end_up_y, int end_down_y,
	                                          int end_r_x, int end_l_x)
		{
		Reset_vertical();
		Reset_horizontal();

		Check_r(end_r_x);
		Check_l(end_l_x);

		Check_up(end_up_y);
		Check_down(end_down_y);
		}

	public void Setup_horizontal(int end_r_x, int end_l_x)
		{
		Reset_horizontal();

		Check_r(end_r_x);
		Check_l(end_l_x);
		}

	public void Setup_vertical(int end_up_y, int end_down_y)
		{
		Reset_vertical();

		Check_up(end_up_y);
		Check_down(end_down_y);
		}
}
