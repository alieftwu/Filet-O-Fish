using UnityEngine;

public class RandomWoodCreak : MonoBehaviour
{
    public AudioSource audioSource;public AudioClip[] creakSounds;public float minInterval=5f;public float maxInterval=15f;

    private float nextCreakTime=0f;

    void Start()
    {
        if(audioSource==null)audioSource=GetComponent<AudioSource>();
        if(creakSounds==null||creakSounds.Length==0){Debug.LogError("No creak sounds assigned!");return;}
        ScheduleNextCreak();
    }

    void Update()
    {
        if(Time.time>=nextCreakTime){PlayRandomCreak();ScheduleNextCreak();}
    }

    void PlayRandomCreak()
    {
        if(creakSounds.Length==0)return;
        int randomIndex=Random.Range(0,creakSounds.Length);
        audioSource.clip=creakSounds[randomIndex];
        if(audioSource.clip!=null)
        {
            audioSource.volume=Random.Range(0.3f,0.5f);
            audioSource.pitch=Random.Range(0.9f,1.1f);
            audioSource.Play();
        }
    }

    void ScheduleNextCreak()
    {
        nextCreakTime=Time.time+Random.Range(minInterval,maxInterval);
    }
}
