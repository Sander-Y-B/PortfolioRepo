using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour
{
    //Prefabs
    public GameObject[] roomPrefabs;
    public GameObject doorBlock;
    public GameObject finalRoom;
    public GameObject bossDoor;

    //Counters
    public int minimumRoomCount = 5;
    private int currentRoomIndex;
    private int currNewRoomIndex;
    private int totalRooms;
    [HideInInspector] public int monsterCount;

    //Spawn info
    public Vector3 RoomOffset;
    public Vector3 rayDoorOffset;
    [SerializeField] float rayDoorLenght = 20f;
    [SerializeField] float roomRayLenght = 10f;
    private RaycastHit hit;
    private GameObject finalDoorway;

    //Ref info
    public GameObject loadingUI;
    public GameObject playerUI;
    private GameObject player;

    //Lists
    public List<GameObject> currentRooms = new List<GameObject>();
    List<GameObject> newRooms = new List<GameObject>();
    List<GameObject> emptyDoorWays = new List<GameObject>();
    public List<GameObject> totalRoomList = new List<GameObject>();
   
    //Bools
    private bool isGenerating = true;
    private bool hasFinalRoom = false;

    //sounds
    public AudioSource roomBlocked;
    public AudioSource roomUnblocked;



    void Start()
    {
        loadingUI.SetActive(true); 
        StartCoroutine(GenerateDungeon());
        player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerBehaviour>().moveAllow = false;
    }

    void Update()
    {
        if (!isGenerating)
        {
            StopCoroutine(GenerateDungeon());
            StartCoroutine(GenerateDungeon());
            isGenerating = true;
        }
    }

    public IEnumerator GenerateDungeon()
    {
        if (totalRooms <= minimumRoomCount)
        {
            foreach (GameObject room in currentRooms)
            {
                foreach (GameObject door in room.GetComponent<Room>().doorRoomSpawners) 
                {
                    RoomSpawn(door);
                    yield return new WaitForSeconds(0.01f);
                }
            }
            ListsUpdate();
            isGenerating = false;
        }
        else
        {
            if (currentRoomIndex < totalRoomList.Count)
            {
                StartCoroutine(DoorwayConnector());
            }
            else if (currentRoomIndex == totalRoomList.Count)
            {
                FinalRoomSpawn();
            }
        }
    }

    IEnumerator DoorwayConnector()
    {
        foreach (GameObject doorway in totalRoomList[currentRoomIndex].GetComponent<Room>().doorWays)
        {
            if (Physics.Raycast(doorway.transform.position - rayDoorOffset, doorway.transform.forward, out hit, rayDoorLenght) && doorway.tag == "Door")
            {
                doorway.GetComponent<DoorwaySc>().isConnected = true;
                if (hit.transform.tag == "Door" && !hit.transform.GetComponent<DoorwaySc>().isConnected || hit.transform.tag == "WallPiece")
                {
                    hit.transform.GetComponent<DoorwaySc>().isConnected = true;
                    hit.transform.gameObject.SetActive(false);
                    yield return new WaitForEndOfFrame();
                }
            }
            else
            {
                EmptyDoorway(doorway);
                yield return new WaitForEndOfFrame();
            }
        }
        currentRoomIndex++;
        isGenerating = false;
    }

    void RoomSpawn(GameObject door)
    {
        if (!Physics.Raycast(door.transform.position - rayDoorOffset, door.transform.forward, roomRayLenght))
        {
            newRooms.Add(Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Length)], door.transform.position, door.transform.rotation, door.transform));
            newRooms[currNewRoomIndex].transform.localPosition += RoomOffset;
            currNewRoomIndex++;
            totalRooms++;
        }
    }

    void ListsUpdate()
    {
        currentRooms.Clear();
        for (int i = 0; i < newRooms.Count; i++)
        {
            GameObject room = newRooms[i];
            currentRooms.Add(room);
            totalRoomList.Add(room);
        }
        currentRoomIndex = 0;
        currNewRoomIndex = 0;
        newRooms.Clear();
    }
    
    void EmptyDoorway(GameObject doorway)
    {
        emptyDoorWays.Add(doorway);
        if (doorway.tag != "WallPiece")
        {
            Instantiate(doorBlock, doorway.transform.position, doorway.transform.rotation, doorway.transform);
        }
    }

    void FinalRoomSpawn()
    {
        finalDoorway = emptyDoorWays[Random.Range(0, emptyDoorWays.Count)];
        if (!hasFinalRoom)
        {
            hasFinalRoom = true;
            totalRoomList.Add(Instantiate(finalRoom, finalDoorway.transform.position, finalDoorway.transform.rotation, finalDoorway.transform));
            totalRooms++;
            totalRoomList[totalRooms].transform.localPosition += RoomOffset;

            foreach (GameObject door in totalRoomList[totalRooms].GetComponent<Room>().doorWays)
            {
                if (Physics.Raycast(door.transform.position - rayDoorOffset, door.transform.forward, out hit, 5))
                {
                    totalRoomList[totalRooms].transform.SetParent(totalRoomList[0].transform, true);
                    hit.transform.gameObject.SetActive(false);
                    Instantiate(bossDoor, door.transform.position, door.transform.rotation, totalRoomList[totalRooms].transform);
                    Destroy(door);
                }
            }
            FinishedLoading();
        }
    }

    void FinishedLoading()
    {
        loadingUI.SetActive(false);
        playerUI.SetActive(true);
        player.GetComponent<PlayerBehaviour>().moveAllow = true;
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

}
