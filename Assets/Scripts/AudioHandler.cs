using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioHandler : MonoBehaviour
{
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private AudioClip[] musicClips;

    [SerializeField] private AudioClip monsterRoar;
    [SerializeField] private AudioClip monsterInRoom;

    [SerializeField] private GameObject player;
    [SerializeField] private GameObject monster;

    private PlayerRoomDeterminer playerRoom;
    private MonsterBehavior monsterBehavior;
    private bool monsterWasInRoom;

    private void Awake()
    {
        musicAudioSource.clip = RandomMusicSelector();
        StartCoroutine(MusicRotater());

        playerRoom = player.GetComponent<PlayerRoomDeterminer>();
        monsterBehavior = monster.GetComponent<MonsterBehavior>();
    }

    private void Update()
    {
        PlayerInSameRoomAsMonster();
    }


    private IEnumerator MusicRotater()
    {
        while (true)
        {
            musicAudioSource.Play();
            yield return new WaitForSeconds(musicAudioSource.clip.length);

            Debug.Log("Audio finished!");
            musicAudioSource.clip = RandomMusicSelector();
        }
    }

    private AudioClip RandomMusicSelector()
    {
        int index = Random.Range(0, musicClips.Length);
        return musicClips[index];
    }

    private void PlayerInSameRoomAsMonster()
    {
        bool isInSameRoom = playerRoom.roomIndex == monsterBehavior.currentRoomIndex;

        if (isInSameRoom && !monsterWasInRoom)
        {
            sfxAudioSource.clip = monsterInRoom;
            sfxAudioSource.Play();
            monsterWasInRoom = true;
        }
        else if (!isInSameRoom)
        {
            monsterWasInRoom = false;
        }
    }

    public void PlayMonsterRoar()
    {
        sfxAudioSource.clip = monsterRoar;
        sfxAudioSource.Play();
    }
}
