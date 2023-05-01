using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Volume), typeof(AudioSource))]
public class TimeStopController : MonoBehaviour
{
	[SerializeField] private float _targetDistortion = 0.7f;
	[SerializeField] private float _transitionDuration = 2f;
	[SerializeField] private AudioClip _timeStopClip;
	
	private VolumeProfile _volume;
	private AudioSource _audioSource;

	private bool _stoppedTime;
	
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
		if (Input.GetKeyDown(KeyCode.T) && _stoppedTime)
			StopTime();
	}

	private void StopTime()
	{
		_audioSource.PlayOneShot(_timeStopClip);

		_stoppedTime = true;
		
		Sequence sequence = DOTween.Sequence();
		sequence.Append(GetDistortionSequence());
		sequence.Insert(0f, GetHueShift(_colorAdjustments.hueShift.max, _transitionDuration));
		sequence.AppendCallback(() => _colorAdjustments.saturation.value = _colorAdjustments.saturation.min);
	}

	private Tween GetHueShift(float endValue, float duration)
		=> DOTween.To(() => _colorAdjustments.hueShift.value, value => _colorAdjustments.hueShift.value = value, endValue, duration);

	private Sequence GetDistortionSequence()
	{
		Sequence sequence = DOTween.Sequence();
		float GetDistortion() => _lensDistortion.intensity.value;
		void SetDistortion(float value) => _lensDistortion.intensity.value = value;

		float duration = _transitionDuration / 2.5f;
		sequence.Append(DOTween.To(GetDistortion, SetDistortion, _targetDistortion, duration));
		sequence.Append(DOTween.To(GetDistortion, SetDistortion, -_targetDistortion, duration / 2f));
		sequence.Append(DOTween.To(GetDistortion, SetDistortion, 0f, duration));

		return sequence;
	}
}
