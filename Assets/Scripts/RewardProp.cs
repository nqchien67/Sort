using Data;
using UnityEngine;
using UnityEngine.UI;

public class RewardProp : MonoBehaviour
{
	[SerializeField] private Image _renderer;

	public void Init(RewardType rewardType)
	{
		_renderer.sprite = RewardHelper.Instance.GetRewardSprite(rewardType);
	}
}