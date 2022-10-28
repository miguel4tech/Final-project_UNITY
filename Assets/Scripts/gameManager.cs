using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;

public class gameManager : MonoBehaviour {
	#region VARIABLES
	private const int CoinScoreAmount = 5;

	public static gameManager Instance { set; get;}

	public bool isDead { set; get;}
	private bool isGameStarted = false;
	private RunnerMotor motor;

	//UI and the UI fields
	public Text scoreText, coinText, modifierText, hiscoreText;
	private float score, coinScore, modifierScore;
	private int lastScore;

	//Death Menu
	public Animator deathMenuAnim;
	public Text deadScoreText, deadCoinText;
    #endregion
    private void Awake()
	{
		Instance = this;
		modifierScore = 1;
		motor = GameObject.FindGameObjectWithTag ("Player").GetComponent<RunnerMotor> ();

		modifierText.text = "x" + modifierScore.ToString ("0.0");
		coinText.text = coinScore.ToString ("0");
		scoreText.text = scoreText.text = score.ToString ("0");

		hiscoreText.text = PlayerPrefs.GetInt("Highscore").ToString();

	}
	private void Update()
	{
		if (MobileInput.Instance.Tap && !isGameStarted) 
		{
			isGameStarted = true;
			motor.StartRunning ();
			FindObjectOfType<Glaciers>().IsScrolling = true;
		}
		if (isGameStarted && !isDead) 
		{
			//Bump up the score
			lastScore = (int)score;
			score += (Time.deltaTime * modifierScore);
			if (lastScore == (int)score) 
			{
				scoreText.text = score.ToString ("0");
			}
		}
	}
	public void GetCoin()
	{
		coinScore ++;
		coinText.text = coinScore.ToString ("0");
		score += CoinScoreAmount;
		scoreText.text = scoreText.text = score.ToString ("0");
	}

	public void UpdateModifier(float modifierAmount)
	{
		modifierScore = 1.0f + modifierAmount;
		modifierText.text = "x" + modifierScore.ToString ("0.0");
	}

	public void OnPlayButton()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
	public void OnDeath()
	{
		isDead = true;
		FindObjectOfType<Glaciers>().IsScrolling = false;

		deadScoreText.text = score.ToString ("0");
		deadCoinText.text = coinScore.ToString ("0");
		
		deathMenuAnim.SetTrigger ("Dead");

		FindObjectOfType<Audiomanager>().Stop("Theme");

		FindObjectOfType<Audiomanager>().Play("PlayerDeath");

		//Checks if this is a highscore
		if(score > PlayerPrefs.GetInt("Highscore"))
		{
			float s = score;
			if(s % 1 == 0)
				s += 1;
			PlayerPrefs.SetInt("Highscore", (int)s); //Sets/Put current score as highscore
		}

	}
	
	public void Revive()
	{
			FindObjectOfType<RunnerMotor>().Revive();

			isDead = false;

			FindObjectOfType<Audiomanager>().Play("Theme");

			foreach(Glaciers gs in FindObjectsOfType<Glaciers>())
			gs.IsScrolling = true;

			deathMenuAnim.SetTrigger("Alive");
	}
}
