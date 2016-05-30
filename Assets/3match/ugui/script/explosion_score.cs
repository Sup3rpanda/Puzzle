using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class explosion_score : MonoBehaviour {

	public Text my_text;
	public GameObject my_canvas;

	public void Show_score(int score)
	{
		my_text.text = "+"+score.ToString();
		my_canvas.GetComponent<Animation>().Play("score_anim");
	}
}
