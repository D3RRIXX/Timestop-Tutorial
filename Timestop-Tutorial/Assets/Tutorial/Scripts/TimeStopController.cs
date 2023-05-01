using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Volume), typeof(AudioSource))]
public class TimeStopController : MonoBehaviour
{
	[SerializeField] private int _timeStopDuration = 5;
	[SerializeField] private float _targetDistortion = 0.7f;
	[SerializeField] private float _stopTransitionDuration = 2f;
	[SerializeField] private float _resumeTransitionDuration = 1.75f;
	[SerializeField] private AudioClip _timeStopClip;
	[SerializeField] private AudioClip _timeResumeClip;
	
	private VolumeProfile _volume;
	private AudioSource _audioSource;

	private bool _stoppedTime;
	public static event Action<bool> TimeStopToggled; 

	private LensDistortion _lensDistortion;
	private ColorAdjustments _colorAdjustments;

	private void Awake()
	{
		_volume = GetComponent<Volume>().profile;
		_audioSource = GetComponent<AudioSource>();

		_volume.TryGet(out _colorAdjustments);
		_volume.TryGet(out _lensDistortion);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.T) && !_stoppedTime)
			StopTime();
	}

	private void StopTime()
	{
		_audioSource.PlayOneShot(_timeStopClip);

		SetStoppedTime(true);
		
		Sequence sequence = DOTween.Sequence();
		sequence.Append(GetDistortionSequence(_stopTransitionDuration));
		sequence.Insert(0f, GetHueShift(_colorAdjustments.hueShift.max, _stopTransitionDuration));
		sequence.AppendCallback(() => _colorAdjustments.saturation.value = _colorAdjustments.saturation.min);
		sequence.OnComplete(() => StartCoroutine(ResumeTimeOnFinish()));
	}

	private void SetStoppedTime(bool value)
	{
		_stoppedTime = value;
		TimeStopToggled?.Invoke(value);
	}

	private IEnumerator ResumeTimeOnFinish()
	{
		yield return new WaitForSeconds(_timeStopDuration);

		ResumeTime();
	}

	private void ResumeTime()
	{
		_audioSource.PlayOneShot(_timeResumeClip);

		Sequence sequence = DOTween.Sequence();
		sequence.Append(GetDistortionSequence(_resumeTransitionDuration));
		sequence.InsertCallback(0f, () => _colorAdjustments.saturation.value = 0f);
		sequence.Insert(0f, GetHueShift(0f, _resumeTransitionDuration));
		sequence.OnComplete(() => SetStoppedTime(false));
	}

	private Tween GetHueShift(float endValue, float duration)
		=> DOTween.To(() => _colorAdjustments.hueShift.value, value => _colorAdjustments.hueShift.value = value, endValue, duration);

	private Sequence GetDistortionSequence(float transitionDuration)
	{
		Sequence sequence = DOTween.Sequence();
		float GetDistortion() => _lensDistortion.intensity.value;
		void SetDistortion(float value) => _lensDistortion.intensity.value = value;

		float duration = transitionDuration / 2.5f;
		sequence.Append(DOTween.To(GetDistortion, SetDistortion, _targetDistortion, duration));
		sequence.Append(DOTween.To(GetDistortion, SetDistortion, -_targetDistortion, duration / 2f));
		sequence.Append(DOTween.To(GetDistortion, SetDistortion, 0f, duration));

		return sequence;
	}
}
