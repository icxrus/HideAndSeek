using UnityEngine;

public class PlayerRoomDeterminer : MonoBehaviour
{
    public int roomIndex = 0;

    public bool IsPlayerInRoom(int currentRoomIndex)
    {
        if (roomIndex == currentRoomIndex)
            return true;
        else return false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Room0"))
            roomIndex = 0;
        else if (other.CompareTag("Room1"))
            roomIndex = 1;
        else if (other.CompareTag("Room2"))
            roomIndex = 2;
    }
}
