using UI;
using UnityEngine;

namespace MainMenu.DailyQuest
{
	public class DailyQuestPanel : Popup
	{
		public override void EndCloseAnimationTrigger()
		{
			base.EndCloseAnimationTrigger();
			Destroy(gameObject);
		}
	}
}