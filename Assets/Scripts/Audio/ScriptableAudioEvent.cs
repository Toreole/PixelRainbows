using UnityEditor;
using UnityEngine;

namespace Audio
{
	/// <summary>
	/// Scriptable Object that grouping multiple audio clips into one sfx which is playable at run and edit-time.
	/// </summary>
	[CreateAssetMenu(fileName = "New Audio Event", menuName = "PixelRainbows/Audio Event", order = -50)]
	public class ScriptableAudioEvent : ScriptableObject
	{
		[Header("Clips")] [Tooltip("The audio clips used for the sfx")] [SerializeField]
		private AudioClip[] _clips;

		[Tooltip("Toggles the sfx to loop")][SerializeField]
		private bool _loop;

		[Header("Volume")]
		[Tooltip("Toggles whether the sfx is played at a fixed or a random range volume")]
		[SerializeField]
		private bool _randomizeVolume;

		[Tooltip("The fixed volume at which the sfx will be played")] [Range(0.33f, 2)] [SerializeField]
		private float _volume;
		
		[Tooltip("Minimum/maximum volume when using a random value")] [SerializeField] [MinMaxFloat(0.33f,2)]
		private MinMaxFloat _minMaxVolume = default;

		[Header("Pitch")] [Tooltip("Toggles whether the sfx is played at a fixed or a random range pitch")] [SerializeField]
		private bool _randomizePitch;

		[Tooltip("The fixed pitch at which the sfx will be played")] [Range(0.25f, 2)] [SerializeField]
		private float _pitch;

		[Tooltip("Minimum/maximum pitch when using a random value")] [SerializeField] [MinMaxFloat(0.25f, 2)]
		private MinMaxFloat _minMaxPitch = default;

		// Old, replaced by the above minmaxfloat
		// [Tooltip("Minimum pitch when using a random value")] [SerializeField] 
		// private float _minPitch;
		// [Tooltip("Maximum pitch when using a random value")] [SerializeField] 
		// private float _maxPitch;

		public bool RandomizeVolume => _randomizeVolume;
		public bool RandomizePitch => _randomizePitch;

		/// <summary>
		/// Plays SFX on the given audio source.
		/// </summary>
		/// <param name="source">The audio source to play the event on</param>
		public void Play(AudioSource source)
		{
			if ((_clips.Length == 0) || (source == null))
			{
				return;
			}
			
			// pick a random audio clip from the array
			source.clip = _clips[Random.Range(0, _clips.Length)];
			// Use either fixed or randomized volume
			source.volume = _randomizeVolume ? Random.Range(_minMaxVolume.MinValue, _minMaxVolume.MaxValue) : _volume;
			// Use either fixed or randomized pitch
			source.pitch = _randomizePitch ? Random.Range(_minMaxPitch.MinValue, _minMaxPitch.MaxValue) : _pitch;
			// Use either looped or unlooped clips
			if (_clips.Length == 1)
				source.loop = _loop ? source.loop = true : source.loop = false;
			
			source.Play();
			
		}
	}
}

