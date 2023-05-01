using UnityEngine;

namespace Tutorial.Scripts
{
	[RequireComponent(typeof(AudioSource))]
	public class TimeStoppableAudioSource : TimeStoppableObject
	{
		private AudioSource _audioSource;

		protected override void Awake()
		{
			base.Awake();
			_audioSource = GetComponent<AudioSource>();
		}

		protected override void OnTimeStopToggled(bool value)
		{
			if (value)
				_audioSource.Pause();
			else
				_audioSource.UnPause();
		}
	}
}