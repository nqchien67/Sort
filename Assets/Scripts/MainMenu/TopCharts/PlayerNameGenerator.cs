using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MainMenu.TopCharts
{
	public class PlayerNameGenerator
	{
		private string[] _basicNames;
		private NameByCountry[] _nameByCountries;

		private Sprite[] _nationalFlagAvatars;
		private Sprite[] _animalFlagAvatars;
		private Sprite[] _humanFlagAvatars;

		public PlayerNameGenerator(Sprite[] nationalFlagAvatars, Sprite[] animalFlagAvatars, Sprite[] humanFlagAvatars)
		{
			_nationalFlagAvatars = nationalFlagAvatars;
			_animalFlagAvatars = animalFlagAvatars;
			_humanFlagAvatars = humanFlagAvatars;

			FakePlayerNameData data = LoadFakePlayerNameData();

			_basicNames = data.BasicNames;
			_nameByCountries = data.NameByCountries;
		}

		private FakePlayerNameData LoadFakePlayerNameData()
		{
			var data = Resources.Load<TextAsset>("fake_player_names");
			return JsonUtility.FromJson<FakePlayerNameData>(data.text);
		}

		public PlayerData GetRandomName()
		{
			const float basicNameRatio = 0.4f;
			const float autoGenNameRatio = 0.2f;

			float randomValue = Random.value;
			if (randomValue < autoGenNameRatio)
				return GenerateDefaultNamePlayer();

			if (randomValue < basicNameRatio)
				return GenerateBasicNamePlayer();

			return GenerateNameByCountryPlayer();
		}

		private PlayerData GenerateDefaultNamePlayer()
		{
			string avatarName = Random.Range(0, 3) switch
			{
				0 => GetRandomElement(_humanFlagAvatars).name,
				1 => GetRandomElement(_animalFlagAvatars).name,
				2 => GetRandomElement(_nationalFlagAvatars).name,
				_ => ""
			};

			return new PlayerData(GenerateDefaultName(), avatarName);
		}

		private PlayerData GenerateBasicNamePlayer()
		{
			string name = GetRandomElement(_basicNames);

			string avatarName = Random.value < 0.5f
				? GetRandomElement(_humanFlagAvatars).name
				: GetRandomElement(_animalFlagAvatars).name;

			return new PlayerData(name, avatarName);
		}

		private PlayerData GenerateNameByCountryPlayer()
		{
			NameByCountry nameByCountry = GetRandomElement(_nameByCountries);
			string firstname = GetRandomElement(nameByCountry.Firstname);
			string surname = GetRandomElement(nameByCountry.Surname);
			string number = Random.Range(0, 10).ToString() + Random.Range(0, 10);
			string name = $"{firstname} {surname} {number}";

			bool useAnimalAvatar = Random.value < 0.4f;
			string avatarName = useAnimalAvatar ? GetRandomElement(_animalFlagAvatars).name : nameByCountry.Country;

			return new PlayerData(name, avatarName);
		}

		private static T GetRandomElement<T>(T[] array)
		{
			int randomIndex = Random.Range(0, array.Length);
			return array[randomIndex];
		}

		public static string GenerateDefaultName()
		{
			StringBuilder result = new StringBuilder(10);
			result.Append("User_");
			for (int i = 0; i < 5; i++)
				result.Append(Random.Range(0, 10));

			return result.ToString();
		}
	}

	[Serializable]
	public class FakePlayerNameData
	{
		public string[] BasicNames;
		public NameByCountry[] NameByCountries;
	}

	[Serializable]
	public class NameByCountry
	{
		public string Country;
		public string[] Firstname;
		public string[] Surname;
	}
}