using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Data
{
	public class SpritesCollection : SingletonCore<SpritesCollection>
	{
		public Sprite[] SimpleItemSprites;
		public Sprite[] DetailItemSprites;
		public Sprite[] PurchasableItemSprites;
		public Sprite[] UnlockableItemSprites;
		public Sprite[] BackgroundSkinIconSprites;
		public Sprite[] EffectIconSprites;
		public Sprite CurrentAvatarSprite;

		public SkinData SkinData
		{
			get => DataController.Instance.SkinData;
			set => DataController.Instance.SkinData = value;
		}

		public List<ItemSkin> PriorityItems
		{
			get => SkinData.PriorityItems.ToList();
			set => SkinData.PriorityItems = value.ToArray();
		}

		public ItemSkin[] PurchasableItemSkins => SkinData.PurchasableItemSkins;
		public UnlockableItemSkin[] UnlockableItemSkins => SkinData.UnlockableItemSkins;
		public Skin[] BackgroundSkins => SkinData.BackgroundSkins;
		public Skin[] Effects => SkinData.Effects;

		public int UnlockedItemSkinsCount => UnlockableItemSkins.Where(itemSkin => itemSkin.Unlocked).ToList().Count;

		public Skin BackgroundSkinInUse
		{
			get
			{
				if (SkinData.BackgroundSkinInUse != null
				    && SkinData.BackgroundSkinInUse.Id != 0
				    && SkinData.BackgroundSkinInUse.InUse)
				{
					return SkinData.BackgroundSkinInUse;
				}

				foreach (var skin in BackgroundSkins)
				{
					if (!skin.InUse) continue;

					SkinData.BackgroundSkinInUse = skin;
					return skin;
				}

				return BackgroundSkins[0];
			}
		}

		public Skin EffectInUse
		{
			get
			{
				if (SkinData.EffectInUse != null)
					return SkinData.EffectInUse;

				foreach (var skin in Effects)
				{
					if (!skin.InUse) continue;

					SkinData.EffectInUse = skin;
					return skin;
				}

				return null;
			}
		}

		public SkinData InitSkinData()
		{
			return new SkinData(PurchasableItemSprites.Length, UnlockableItemSprites.Length,
				BackgroundSkinIconSprites.Length,
				EffectIconSprites.Length);
		}

		public List<int> GetPurchasedItemsId()
		{
			List<int> result = new List<int>();
			foreach (var itemSkin in PurchasableItemSkins)
			{
				if (itemSkin.Unlocked)
					result.Add(itemSkin.Id);
			}

			return result;
		}

		public List<int> GetNotPurchasedItemsId()
		{
			List<int> result = new List<int>();
			foreach (var itemSkin in PurchasableItemSkins)
			{
				if (!itemSkin.Unlocked)
					result.Add(itemSkin.Id);
			}

			return result;
		}

		public List<UnlockableItemSkin> GetNotUnlockedItems()
		{
			List<UnlockableItemSkin> result = new List<UnlockableItemSkin>();
			foreach (var itemSkin in UnlockableItemSkins)
			{
				if (!itemSkin.Unlocked)
					result.Add(itemSkin);
			}

			return result;
		}

		public List<ItemSkin> GetUnlockedItems()
		{
			List<ItemSkin> result = new List<ItemSkin>();
			foreach (var itemSkin in UnlockableItemSkins)
			{
				if (itemSkin.Unlocked)
					result.Add(itemSkin);
			}

			foreach (var itemSkin in PurchasableItemSkins)
			{
				if (itemSkin.Unlocked)
					result.Add(itemSkin);
			}

			return result;
		}

		public void AddPriorityItem(ItemSkin item)
		{
			var temp = PriorityItems;
			temp.Add(item);
			PriorityItems = temp;
		}

		public void RemovePriorityItem(ItemSkin item)
		{
			var temp = PriorityItems;
			temp.Remove(item);
			PriorityItems = temp;
		}

		public void AddUnlockedItemSkin(int id)
		{
			foreach (var itemSkin in UnlockableItemSkins)
			{
				if (itemSkin.Id != id)
					continue;

				itemSkin.Unlocked = true;
				PriorityItems.Add(itemSkin);
				break;
			}
		}

		public bool HavePurchased(int id) => PurchasableItemSkins[id].Unlocked;
		public bool IsUsing(int id) => PurchasableItemSkins[id].InUse;

		public Sprite GetItemSprite(ItemSkin itemSkin)
		{
			return itemSkin is UnlockableItemSkin
				? UnlockableItemSprites[itemSkin.Id]
				: PurchasableItemSprites[itemSkin.Id];
		}

		public Sprite GetPurchasableItemSprite(int id) => PurchasableItemSprites[id];
		public Sprite GetUnlockableItemSprite(int id) => UnlockableItemSprites[id];
		public Sprite GetShelfSkinIcon(int id) => BackgroundSkinIconSprites[id];
		public Sprite GetEffectIcon(int id) => EffectIconSprites[id];


		public Sprite GetSkinIconSprite(Skin skin)
		{
			bool isPurchasableItem = skin is ItemSkin && !(skin is UnlockableItemSkin);
			if (isPurchasableItem)
				return PurchasableItemSprites[skin.Id];


			return BackgroundSkinIconSprites[skin.Id];
		}
	}

	[Serializable]
	public class SkinData
	{
		public ItemSkin[] PurchasableItemSkins;
		public UnlockableItemSkin[] UnlockableItemSkins;
		public Skin[] BackgroundSkins;
		public Skin[] Effects;

		public Skin BackgroundSkinInUse;
		public Skin EffectInUse;

		public ItemSkin[] PriorityItems = Array.Empty<ItemSkin>();

		public SkinData(int purchasableItemCount, int unlockableItemsCount, int shelfSkinsCount, int effectsCount)
		{
			PurchasableItemSkins = new ItemSkin[purchasableItemCount];
			for (int i = 0; i < purchasableItemCount; i++)
				PurchasableItemSkins[i] = new ItemSkin(i);

			UnlockableItemSkins = new UnlockableItemSkin[purchasableItemCount];
			for (int i = 0; i < unlockableItemsCount; i++)
				UnlockableItemSkins[i] = new UnlockableItemSkin(i);

			BackgroundSkins = new Skin[shelfSkinsCount];
			for (int i = 0; i < shelfSkinsCount; i++)
				BackgroundSkins[i] = new Skin(i);

			Effects = new Skin[effectsCount];
			for (int i = 0; i < effectsCount; i++)
				Effects[i] = new Skin(i);
		}
	}

	[Serializable]
	public class ItemSkin : Skin
	{
		public ItemSkin(int id) : base(id)
		{
		}
	}

	[Serializable]
	public class UnlockableItemSkin : ItemSkin
	{
		public UnlockableItemSkin(int id) : base(id)
		{
			InUse = true;
		}
	}

	[Serializable]
	public class Skin
	{
		public int Id;
		public bool Unlocked;
		public bool InUse;

		public Skin(int id)
		{
			Id = id;
			Unlocked = false;
			InUse = false;
		}
	}
}