using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource[] sources;
    private int currentSource = 0;
    private double nextLoopTime;

    public void PlayMusic(MusicScriptableObject musicObject)
    {
        nextLoopTime = AudioSettings.dspTime + 0.1;
        sources[currentSource].clip = musicObject.track;
        sources[currentSource].PlayScheduled(nextLoopTime);
        sources[0].volume = sources[1].volume = 1;

        currentSource = 1 - currentSource;

        StopAllCoroutines();
        if (!musicObject.doesLoop) return;

        PlayMusicLoopPoint(musicObject, (double)sources[1 - currentSource].clip.samples / sources[1 - currentSource].clip.frequency);
    }

    public void StopMusic()
    {
        sources[0].Stop();
        sources[1].Stop();
        StopAllCoroutines();
    }

    public IEnumerator FadeMusicOut(float fadeTime)
    {
        float timer = fadeTime;

        while(timer > 0)
        {
            sources[currentSource].volume = timer / fadeTime;
            timer -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        sources[currentSource].volume = 0;
    }

    private void PlayMusicLoopPoint(MusicScriptableObject musicObject, double trackLength)
    {
        nextLoopTime += trackLength;

        sources[currentSource].clip = musicObject.track;
        sources[currentSource].PlayScheduled(nextLoopTime);
        sources[currentSource].time = musicObject.loopPointSeconds;

        currentSource = 1 - currentSource;
        StartCoroutine(LoopTimer(musicObject));
    }

    private IEnumerator LoopTimer(MusicScriptableObject musicObject)
    {
        yield return new WaitWhile(() => AudioSettings.dspTime < nextLoopTime);

        double previousTrackLength = (double)sources[1 - currentSource].clip.samples / sources[1 - currentSource].clip.frequency;
        double currentTrackLength = previousTrackLength - musicObject.loopPointSeconds;

        PlayMusicLoopPoint(musicObject, currentTrackLength);
    }
}
