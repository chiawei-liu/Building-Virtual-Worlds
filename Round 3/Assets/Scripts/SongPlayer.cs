using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TMPro.SpriteAssetUtilities;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class SongPlayer : MonoBehaviour
{
    private float dspSongTime;
    public float songPos;
    private Line linePlaying;
    [SerializeField] private Metronome metronome;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public IEnumerator PlayLine(Line line)
    {
        GameManager.Instance.voiceGroups.ForEach(s =>
        {
            s.selectable = false;
            s.SetSelectingProgress(0);
            s.Sleeping();
        });
        linePlaying = line;
        GameManager.Instance.sheetScreen.SetupBackgroundBars(line);
        GameManager.Instance.sheetScreen.SetIndicator(0);
        for (int i = 0; i < line.voices.Count; i++)
        {
            GameManager.Instance.voiceGroups[i].expectedVoice = line.voices[i];
            GameManager.Instance.voiceGroups[i].audioSource.clip = Resources.Load<AudioClip>(line.voices[i].assetName);
        }
        GetComponent<AudioSource>().PlayOneShot((GameManager.Instance.metronome));
        metronome.Play(8, GameManager.Instance.metronome.length);
        // dspSongTime = (float) AudioSettings.dspTime;
        // yield return new WaitUntil(() => (float) AudioSettings.dspTime - dspSongTime > 2.5);
        // GameManager.Instance.VoiceGroups.ForEach(s =>
        // {
        //     s.selectable = false;
        // });
        yield return new WaitUntil(() => !GetComponent<AudioSource>().isPlaying);
        GameManager.Instance.pianoAnimator.SetBool("Kid", true);
        GameManager.Instance.voiceGroups.ForEach(s => s.selectable = true);
        for (int i = 0; i < line.voices.Count; i++)
        {
            GameManager.Instance.voiceGroups[i].audioSource.Play();
            GameManager.Instance.voiceGroups[i].audioSource.volume = 0;
        }
        dspSongTime = (float)AudioSettings.dspTime;
        songPos = 0;
        GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>(line.bgmAssetName));
        yield return StartCoroutine(UpdateIndicator());
        
        GameManager.Instance.voiceGroups.ForEach(s => {
            s.selectable = false;
            s.SetSelectingProgress(0);
            s.Sleeping();
        });
        GameManager.Instance.pianoAnimator.SetBool("Kid", false);

        yield return new WaitForSeconds(1);
        GameManager.Instance.sheetScreen.Clean();
    }

    public void TurnOnVoice(Selectable selectable)
    {
        selectable.selectable = false;
        selectable.GetComponent<AudioSource>().volume = 1;
        selectable.playerVoice.startPoints.Add(songPos);
        selectable.Song();
        var bar = GameManager.Instance.sheetScreen.StartBar(selectable.index, songPos / linePlaying.audioLength);
        bar.transform.GetChild(0).gameObject.SetActive(false);
        bar.GetComponent<Image>().sprite = null;
        var nextEndPoint = selectable.expectedVoice.endPoints.FirstOrDefault(p => p > songPos);
        var nextStartPoint = selectable.expectedVoice.startPoints.FirstOrDefault(p => p > songPos);
        if (nextEndPoint == 0)
        {
            nextEndPoint = linePlaying.audioLength;
            if (nextStartPoint == 0)
            {
                nextStartPoint = linePlaying.audioLength;
            }
        }
        if (nextStartPoint == 0 || nextStartPoint > nextEndPoint)
        {
            selectable.Song();
            StartCoroutine(UpdateVoice(selectable, bar, nextEndPoint));
        }
        else
        {
            selectable.WrongSong();
            StartCoroutine(UpdateVoice(selectable, bar, nextEndPoint, nextStartPoint));
        }
        selectable.playerVoice.endPoints.Add(nextEndPoint);
    }

    private void TurnOffVoice(Selectable selectable)
    {
        selectable.GetComponent<AudioSource>().volume = 0;
        selectable.StopAnimation();
        selectable.SetSelectingProgress(0);
        selectable.Sleeping();
    }

    private IEnumerator UpdateVoice(Selectable selectable, GameObject bar, float nextEndPoint)
    {
        while (songPos < nextEndPoint)
        {
            GameManager.Instance.sheetScreen.UpdateBar(selectable.index, songPos / linePlaying.audioLength, bar);
            yield return null;
        }
        // end
        TurnOffVoice(selectable);
        selectable.selectable = true;
    }
    
    private IEnumerator UpdateVoice(Selectable selectable, GameObject bar, float nextEndPoint, float nextStartPoint)
    {
        var turned = false;
        while (songPos < nextEndPoint)
        {
            if (!turned)
            {
                if (songPos > nextStartPoint)
                {
                    selectable.Song();
                    turned = true;
                }
            }
            GameManager.Instance.sheetScreen.UpdateBar(selectable.index, songPos / linePlaying.audioLength, bar);
            yield return null;
        }
        // end
        TurnOffVoice(selectable);
        selectable.selectable = true;
    }

    private IEnumerator UpdateIndicator()
    {
        while (songPos < linePlaying.audioLength)
        {
            songPos = (float)AudioSettings.dspTime - dspSongTime;
            //Debug.Log(songPos);
            GameManager.Instance.sheetScreen.SetIndicator(songPos / linePlaying.audioLength);
            yield return null;
        }
    }

    public IEnumerator Replay(List<Line> lines, List<Line> voiceRecordLines)
    {
        GetComponent<AudioSource>().PlayOneShot((GameManager.Instance.metronome));
        GameManager.Instance.pianoAnimator.SetBool("Kid", false);
        GameManager.Instance.voiceGroups.ForEach(selectable => selectable.selectable = false);
        yield return new WaitUntil(() => !GetComponent<AudioSource>().isPlaying);
        for (var j = 0; j < lines.Count; j++)
        {
            GameManager.Instance.voiceGroups.ForEach(v => v.SetSelectingProgress(0));
            voiceRecordLines[j].audioLength = lines[j].audioLength;
            voiceRecordLines[j].spriteName = lines[j].spriteName;
            // GameManager.Instance.SheetScreen.SetupBackgroundBars(voiceRecordLines[j]);
            for (int i = 0; i < lines[j].voices.Count; i++)
            {
                GameManager.Instance.voiceGroups[i].SetSelectingProgress(0);
                GameManager.Instance.voiceGroups[i].audioSource.clip = Resources.Load<AudioClip>(lines[j].voices[i].assetName);
                GameManager.Instance.voiceGroups[i].audioSource.Play();
                GameManager.Instance.voiceGroups[i].audioSource.volume = 0;
                GameManager.Instance.voiceGroups[i].Song();
            }
            GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>(lines[j].bgmAssetName));
            GameManager.Instance.pianoAnimator.SetBool("Kid", true);

            dspSongTime = (float) AudioSettings.dspTime;
            songPos = 0;
            // StartCoroutine(UpdateIndicator());

            var startPointPointers = Enumerable.Repeat(0, voiceRecordLines[j].voices.Count).ToList();
            var endPointPointers = Enumerable.Repeat(0, voiceRecordLines[j].voices.Count).ToList();
            while (songPos < lines[j].audioLength)
            {
                songPos = (float) AudioSettings.dspTime - dspSongTime;
                for (var k = 0; k < voiceRecordLines[j].voices.Count; k++)
                {
                    if (startPointPointers[k] != voiceRecordLines[j].voices[k].startPoints.Count)
                    {
                        if (songPos > voiceRecordLines[j].voices[k].startPoints[startPointPointers[k]])
                        {
                            GameManager.Instance.voiceGroups[k].audioSource.volume = 1;
                            GameManager.Instance.voiceGroups[k].SetSelectingProgress(1);
                            GameManager.Instance.voiceGroups[k].StartAnimation();
                            startPointPointers[k]++;
                            continue;
                        }
                    }

                    if (endPointPointers[k] == voiceRecordLines[j].voices[k].endPoints.Count) continue;
                    if (!(songPos > voiceRecordLines[j].voices[k].endPoints[endPointPointers[k]])) continue;
                    GameManager.Instance.voiceGroups[k].audioSource.volume = 0;
                    GameManager.Instance.voiceGroups[k].SetSelectingProgress(0);
                    GameManager.Instance.voiceGroups[k].StopAnimation();
                    endPointPointers[k]++;
                    continue;
                }
                yield return null;
            }
            // GameManager.Instance.SheetScreen.Clean();
        }
        for (var i = 0; i < lines[0].voices.Count; i++)
        {
            GameManager.Instance.voiceGroups[i].StopAnimation();
            // GameManager.Instance.VoiceGroups[i].Sleeping();
        }
        GameManager.Instance.pianoAnimator.SetBool("Kid", false);
    }
}
