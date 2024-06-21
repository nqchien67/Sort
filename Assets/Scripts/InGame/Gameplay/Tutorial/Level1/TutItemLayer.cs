using UnityEngine;

namespace InGame.Gameplay.Tutorial.Level1
{
	public class TutItemLayer : ItemLayer
	{
		protected override void Awake()
		{
			base.Awake();
			RepositionItems();
		}

		private void Start()
		{
			Shelf = GetComponentInParent<Shelf>();
			Shelf.Layers.Add(this);
		}

		private void RepositionItems()
		{
			for (int i = 0; i < transform.childCount; i++)
			{
				var child = transform.GetChild(i);
				Items[i] = child.GetComponent<Item>();
				child.localPosition = new Vector3(GetItemLocalPosX(i), 0);
			}
		}
	}
}