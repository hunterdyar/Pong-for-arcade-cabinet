using TMPro;

namespace Pong.UI
{
	public class UIVictoryText : OnVictoryBase
	{
		private TMP_Text _text;

		private void Awake()
		{
			_text = GetComponent<TMP_Text>();
			_text.color = _winningPlayer.Color;
			_text.enabled = false;
		}

		protected override void OnMyPlayerWon()
		{
			_text.enabled = true;
			_text.text = "Victory!";
		}

		protected override void OnMyPlayerLost()
		{
			_text.enabled = true;
			_text.text = "Defeat!";
		}
	}
}