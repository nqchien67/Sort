using MoreMountains.NiceVibrations;
using UnityEngine;

namespace Audio
{
	public class AudioController : SingletonCore<AudioController>
	{
		private bool isSfxMute, isMusicMute, isVibrationOff;
		[SerializeField] private AudioSource sfxSource, musicSource;

		public bool IsSfxMute => isSfxMute;

		public bool Music
		{
			set
			{
				PlayerPrefs.SetInt("music_audio", value ? 1 : 0);
				isMusicMute = !value;
				if (isMusicMute) musicSource.Stop();
				else musicSource.Play();
			}
			get => PlayerPrefs.GetInt("music_audio", 1) == 1;
		}

		public bool SFX
		{
			set
			{
				PlayerPrefs.SetInt("sfx_audio", value ? 1 : 0);
				isSfxMute = !value;
			}
			get => PlayerPrefs.GetInt("sfx_audio", 1) == 1;
		}

		public bool Vibration
		{
			get => PlayerPrefs.GetInt("vibrations", 1) == 1;
			set
			{
				PlayerPrefs.SetInt("vibrations", value ? 1 : 0);
				isVibrationOff = !value;
			}
		}

		private void Start()
		{
			ResetSound();
			DontDestroyOnLoad(gameObject);
		}

		public void ResetSound()
		{
			isMusicMute = !Music;
			isSfxMute = !SFX;
		}

		public void PlaySfx(AudioClip clip)
		{
			if (!isSfxMute || clip != null)
				sfxSource.PlayOneShot(clip);
		}

		public void PlaySfxLoop(AudioClip clip)
		{
			if (!isSfxMute)
			{
				sfxSource.clip = clip;
				sfxSource.Play();
				sfxSource.loop = true;
			}
		}

		public void StopLoopSfx()
		{
			sfxSource.loop = false;
			sfxSource.Stop();
		}

		public void PlayMusic(AudioClip clip, bool isLoop)
		{
			musicSource.clip = clip;
			musicSource.loop = isLoop;
			if (!isMusicMute)
				musicSource.Play();
		}

		public void StopMusic()
		{
			musicSource.Stop();
		}

		public void Vibrate()
		{
			if (Vibration)
				MMVibrationManager.Vibrate();
		}

		public void PauseMusic()
		{
			musicSource.Pause();
		}

		public void UnPauseMusic()
		{
			musicSource.UnPause();
		}
	}
}