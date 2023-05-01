using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Volume))]
public class TimeStopController : MonoBehaviour
{
	[SerializeField] private float _targetDistortion = 0.7f;
	[SerializeField] private float _transitionDuration = 2f;
	
	private VolumeProfile _volume;
	private ColorAdjustments _colorAdjustments;
	private LensDistortion _lensDistortion;

	private void Awake()
	{
		_volume = GetComponent<Volume>().profile;
		
		_volume.TryGet(out _colorAdjustments);
		_volume.TryGet(out _lensDistortion);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.T))
			StopTime();
	}

	private void StopTime()
	{
		Sequence sequence = DOTween.Sequence();
		sequence.Append(GetDistortionSequence());
		sequence.Insert(0f, GetHueShift(_colorAdjustments.hueShift.max, _transitionDuration));
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
