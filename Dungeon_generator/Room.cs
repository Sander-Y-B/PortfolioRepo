using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    private RoomManager roomManager;
    public List<GameObject> doorWays = new List<GameObject>();
    public GameObject[] doorRoomSpawners;
    public bool spawnDone;

    public List<GameObject> monstersInRoom = new List<GameObject>();
    public List<GameObject> blockers = new List<GameObject>();

    private void Start()
    {
        roomManager = GameObject.FindGameObjectWithTag("ScriptManager").GetComponent<RoomManager>();
    }

    private void Update()
    {
        if (roomManager.monsterCount <= 0)
        {
            foreach (GameObject block in blockers)
            {
                Destroy(block);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !spawnDone)
        {
            roomManager.monsterCount = monstersInRoom.Count;
            spawnDone = true;
            roomManager.roomBlocked.Play();

            foreach (GameObject door in doorWays)
            {
                if (door.GetComponent<DoorwaySc>().isConnected)
                {
                    blockers.Add(Instantiate(roomManager.doorBlock, door.transform));
                }
            }
            foreach (GameObject monster in monstersInRoom)
            {
                monster.SetActive(true);
            }
        }
    }

}
