using UnityEngine;
using Main.Interfaces;
using Random = UnityEngine.Random;

namespace Main.Audio
{
    sealed class UnityAudioPlayer : MonoBehaviour, IAudioPlayer
    {
        [Header("【BGM】")]
        [SerializeField] AudioClip _bgmClip = default;

        [Header("【Drumming】")]
        [SerializeField] AudioClip _drummingClip = default;

        [Header("【Jingle】")]
        [SerializeField] AudioClip _gameStartClip = default;
        [SerializeField] AudioClip _gameOverClip = default;
        [SerializeField] AudioClip _getBananaClip = default;

        AudioSource _bgmSource;
        AudioSource _drummingSource;
        AudioSource _jingleSource;

        public void Initialize()
        {
            _bgmSource = AddAudioSource(gameObject, true);
            _drummingSource = AddAudioSource(gameObject, true);
            _jingleSource = AddAudioSource(gameObject, true);
            gameObject.hideFlags = HideFlags.HideInHierarchy;
        }

        public void PlayBGM()
        {
            _bgmSource.clip = _bgmClip;
            _bgmSource.time = 0f;
            _bgmSource.Play();
        }

        public void StopBGM() => _bgmSource.Stop();

        public void PlayDrumming()
            => _drummingSource.PlayOneShot(_drummingClip);

        public void PlayGetBanana()
            => _jingleSource.PlayOneShot(_getBananaClip);

        public void PlayGameStart()
            => _jingleSource.PlayOneShot(_gameStartClip);

        public void PlayGameOver()
            => _jingleSource.PlayOneShot(_gameOverClip);


        static AudioSource AddAudioSource(in GameObject target, in bool loop, in bool playOnAwake = false)
        {
            var source = target.AddComponent<AudioSource>();
            source.playOnAwake = playOnAwake;
            source.loop = loop;
            return source;
        }
    }
}
