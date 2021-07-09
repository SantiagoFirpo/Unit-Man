using UnitMan.Source.Utilities;
using UnitMan.Source.Utilities.ObserverSystem;
using UnityEngine;

namespace UnitMan.Source.Management
{
    public sealed class AudioManagerSingle : MonoBehaviour
    {
        public static AudioManagerSingle Instance {get; private set; }

        private AudioSource[] _tracks;

        private Observer _resetObserver;

        [SerializeField] private AudioCollection _audioCollection;


        public bool IsTrackPlaying(int trackNumber) {
            return _tracks[trackNumber].isPlaying;
        }

        public enum AudioEffectType
        {
            Munch, EatGhost, Death, Intermission, Siren,
            Fleeing,
            Retreating, IntroMusic
        }

        private void OnEnable() {
            if (Instance != null) Destroy(gameObject);

            Instance = this;
            _resetObserver = new Observer(ResetTracks);
            
            SessionManagerSingle.Instance.resetEmitter.Attach(_resetObserver);
            _tracks = GetComponents<AudioSource>();
            

        }
        private void ResetTracks()
        {
            foreach (AudioSource track in _tracks)
            {
                track.Stop();
            }
        }

        private void OnDisable()
        {
            SessionManagerSingle.Instance.resetEmitter.Detach(_resetObserver);
        }

        public void PlayClip(AudioEffectType effectType, int trackNumber, bool loop)
        {
            AudioSource source = _tracks[trackNumber];

            source.clip = effectType switch {
                AudioEffectType.Munch => _audioCollection.munch,
                AudioEffectType.EatGhost => _audioCollection.eatGhost,
                AudioEffectType.Death => _audioCollection.death,
                AudioEffectType.Intermission => _audioCollection.intermission,
                AudioEffectType.Siren => _audioCollection.siren,
                AudioEffectType.Fleeing => _audioCollection.powerPellet,
                AudioEffectType.Retreating => _audioCollection.retreating,
                AudioEffectType.IntroMusic => _audioCollection.introMusic,
                _ => source.clip
            };
            source.Play();
            source.loop = loop;


        }
    }
}