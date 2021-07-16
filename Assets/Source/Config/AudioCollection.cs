using UnityEngine;

namespace UnitMan.Source.Config
{
	[CreateAssetMenu(menuName = "Audio Collection", fileName = "Audio Collection")]
	public class AudioCollection : ScriptableObject
	{
		public AudioClip munch;
		public AudioClip eatGhost;
		public AudioClip death;
		public AudioClip intermission;
		public AudioClip siren;
		public AudioClip powerPellet;
		public AudioClip retreating;
		public AudioClip introMusic;
	}
}