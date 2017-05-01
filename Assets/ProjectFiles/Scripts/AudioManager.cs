using UnityEngine;

public class AudioManager : MonoBehaviour {
    private AudioSource Audio;

    public AudioClip Jump;
    public AudioClip CoinPickup;
    private GameObject Player;

    private void Awake()
    {
         
    }
    
	private void Start () {
        Player = GameObject.FindGameObjectWithTag("Player");
        Audio = Player.GetComponent<AudioSource>();
    }
	
	private void Update () {
		
	}

    public void PlaySoundEffect(AudioClip CurrentClip)
    {
        Audio.clip = CurrentClip;
        Audio.Play();
    }
}

/*
 AudioSource audioSource = gameObject.AddComponent<AudioSource>();
 audioSource.clip = Resources.Load(name) as AudioClip;
 audioSource.Play();
 */