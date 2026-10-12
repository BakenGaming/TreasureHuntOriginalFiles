using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NewMapGenerator : MonoBehaviour
{
    public static event Action OnGameStarted;
    public static event Action OnGameRestarted;
    public static event Action OnRoomGenerated;
    [SerializeField] private List<Transform> mapPoints;
    [SerializeField] private Transform[] possibleStartingPositions;
    [SerializeField] private Transform[] possibleEndingPositions;
    [SerializeField] private float moveAmount = 1, minX, maxX, maxY;
    [SerializeField] private int maxRows;
    [SerializeField] private Transform currentMapPoint, mapBegin, mapEnd;

    private float timer=0, timeBetweenTrys = 0f;
    private Dictionary<GameObject, bool> mapPointDictionary = new Dictionary<GameObject, bool>();
    private List<GameObject> currentMapPoints = new List<GameObject>();
    private List<GameObject> mainPathPoints = new List<GameObject>();

    private GameObject startPoint, currentGenerationPoint, endPoint;
    private Vector2 boxSize = new Vector2(.1f, .1f);
    private bool generationOver = false, pathRoomsFull = false, nonPathRoomsFull = false, resetLevel, complete;
    private int empty, coin, enemy, trap, potion, emptyCount, coinCount, enemyCount, trapCount, potionCount, rowIndex;

    private int direction, lastSetRoom, roomCounter;

    private void OnEnable() 
    {
        MapPoint.OnRoomSet += IncreaseRoomCount; 
        MapPoint.OnEmptyReplaced += ReplaceRoomCount;   
    }

    private void OnDisable() 
    {
        MapPoint.OnRoomSet -= IncreaseRoomCount;    
        MapPoint.OnEmptyReplaced -= ReplaceRoomCount;
    }

    public void TESTBREAKALLROOMS()
    {
        foreach (GameObject point in currentMapPoints)
        {
            point.GetComponent<MapPoint>().TESTBreakPoint();
        }
    }

    public void GenerateMap(MinerRank rank) 
    {
        ClearVariables();

        coin = UnityEngine.Random.Range(rank.minNumberOfCoins, rank.maxNumberOfCoins);
        enemy = UnityEngine.Random.Range(rank.minNumberOfEnemies, rank.maxNumberOfEnemies);
        trap = UnityEngine.Random.Range(rank.minNumberOfTraps, rank.maxNumberOfTraps);
        potion = UnityEngine.Random.Range(rank.minNumberOfPotions, rank.maxNumberOfPotions);
        empty = 42 - (coin + enemy + trap + potion);
        foreach (Transform point in mapPoints)
        {
            GameObject newPoint = Instantiate(GameAssets.i.mapPoint, point.position, Quaternion.identity);
            newPoint.transform.SetParent(point);
            mapPointDictionary.Add(newPoint, false);
            currentMapPoints.Add(newPoint);
        }
        direction = UnityEngine.Random.Range(1,6);
        SetStartPoint();

    }

    public void ClearRooms(MinerRank rank)
    {   
        resetLevel = true;
        GameManager.i.PauseGame();
        foreach (GameObject point in currentMapPoints)
        {
            point.GetComponent<MapPoint>().Clear();
            Destroy(point);
        }
        GenerateMap(rank);
    }

    private void ClearVariables()
    {
        complete = false;
        generationOver = false;
        pathRoomsFull = false;
        nonPathRoomsFull = false;
        rowIndex = 0;
        enemyCount = 0;
        potionCount = 0;
        trapCount = 0;
        coinCount = 0;
        emptyCount = 0;
        coin = 0;
        trap = 0;
        enemy = 0;
        potion = 0;
        empty = 0;
        startPoint = null;
        endPoint = null;
        currentGenerationPoint = null;
        currentMapPoints.Clear();
        mapPointDictionary.Clear();
        mainPathPoints.Clear();
        mainPathPoints = new List<GameObject>();
        currentMapPoints = new List<GameObject>();
        mapPointDictionary = new Dictionary<GameObject, bool>();
    }

    private void Update() 
    {
        if(!generationOver && timer <= 0)
        {
            Move();
            timer = timeBetweenTrys;
        }
        else timer -= Time.deltaTime;

        if(generationOver && !pathRoomsFull && timer <= 0)
        {
            FillPathRooms();
            timer = timeBetweenTrys;
        }
        else timer -= Time.deltaTime;

        if(generationOver && pathRoomsFull && !nonPathRoomsFull && timer <= 0)
        {
            FillNonPathRooms();
            timer = timeBetweenTrys;
        }
        else timer -= Time.deltaTime;

        if(generationOver && pathRoomsFull && nonPathRoomsFull && !complete)
        {
            if(!resetLevel) 
            {
                OnGameStarted?.Invoke();
                complete = true;
                resetLevel = false;
                GameManager.i.UnPauseGame();
            }
            else 
            {
                OnGameRestarted?.Invoke();
                complete = true;
                resetLevel = false;
                GameManager.i.UnPauseGame();
            }
        }
    }

    public void SetStartPoint()
    {
        Transform randPos = possibleStartingPositions[UnityEngine.Random.Range(0, possibleStartingPositions.Length)];
        Collider2D collider = Physics2D.OverlapBox(randPos.position, boxSize, 0);
        startPoint = collider.gameObject;
        startPoint.GetComponent<MapPoint>().SetStart();
        mapPointDictionary[startPoint] = true;
        mainPathPoints.Add(startPoint);
        currentGenerationPoint = startPoint;
        IncreaseRoomCount("empty");
    }

    public void SetEndPoint(GameObject point)
    {
        endPoint = point;
        endPoint.GetComponent<MapPoint>().SetEndPoint();
        generationOver = true;
        currentMapPoint.transform.position = mapBegin.position;
        //FillOutRooms();
    }

    public void Move()
    {
        if(direction == 1) //move right
        {
            if(currentGenerationPoint.transform.position.x < maxX)
            {
                Vector2 newPos = new Vector2(currentGenerationPoint.transform.position.x + moveAmount,
                    currentGenerationPoint.transform.position.y); 
                currentMapPoint.transform.position = newPos; 
                Collider2D collider = Physics2D.OverlapBox(newPos, boxSize, 0);
                currentGenerationPoint = collider.gameObject;
                mapPointDictionary[currentGenerationPoint] = true;

                currentGenerationPoint.GetComponent<MapPoint>().SetMainPath(); 
                mainPathPoints.Add(currentGenerationPoint);
                IncreaseRoomCount("empty");

                direction = UnityEngine.Random.Range(1,6);
                if(direction != 1) direction = 5;
            }
            else direction = 5;
        } 
        else if (direction == 2) //move left
        {
            if(currentGenerationPoint.transform.position.x > minX)
            {
                Vector2 newPos = new Vector2(currentGenerationPoint.transform.position.x - moveAmount,
                    currentGenerationPoint.transform.position.y); 
                currentMapPoint.transform.position = newPos;
                Collider2D collider = Physics2D.OverlapBox(newPos, boxSize, 0);
                currentGenerationPoint = collider.gameObject;
                mapPointDictionary[currentGenerationPoint] = true;
                currentGenerationPoint.GetComponent<MapPoint>().SetMainPath(); 
                mainPathPoints.Add(currentGenerationPoint);
                IncreaseRoomCount("empty");

                direction = UnityEngine.Random.Range(2,6);
                if(direction != 2) direction = 5;
            }
            else direction = 5;
        }
        else if (direction == 3 || direction == 4 || direction == 5) //move down
        {
            if(currentGenerationPoint.transform.position.y > maxY)
            {
                Vector2 newPos = new Vector2(currentGenerationPoint.transform.position.x,
                    currentGenerationPoint.transform.position.y - moveAmount); 
                currentMapPoint.transform.position = newPos; 
                Collider2D collider = Physics2D.OverlapBox(newPos, boxSize, 0);
                currentGenerationPoint = collider.gameObject; 
                mapPointDictionary[currentGenerationPoint] = true;
                currentGenerationPoint.GetComponent<MapPoint>().SetMainPath(); 
                mainPathPoints.Add(currentGenerationPoint);
                IncreaseRoomCount("empty");

                direction = UnityEngine.Random.Range(1,6);              
            }
            else 
            {
                SetEndPoint(currentGenerationPoint);
            }

        }
    }

    private void FillOutRooms()
    {
        foreach (GameObject point in currentMapPoints)
        {
            if(mapPointDictionary.TryGetValue(point, out bool setStatus))
            {
                if(setStatus == false)
                {
                    point.GetComponent<MapPoint>().SetMax(empty, coin, enemy, trap, potion, this);
                    currentMapPoint.transform.position = point.transform.position;
                    point.GetComponent<MapPoint>().SetRoom(emptyCount, coinCount, enemyCount, trapCount, potionCount);
                    mapPointDictionary[point] = true;
                }  
            }
        }
        
        foreach (GameObject point in mainPathPoints)
        {
            point.GetComponent<MapPoint>().SetMax(empty, coin, enemy, trap, potion, this);
            currentMapPoint.transform.position = point.transform.position;
            point.GetComponent<MapPoint>().SetNonHazardRoom(emptyCount, coinCount, enemyCount, potionCount);
            mapPointDictionary[point] = true;
        }
        //GameManager.i.StartFadeIn();
    }

    private void FillPathRooms()
    {
        if(currentMapPoint.transform.position.x <= maxX)
        {
            Collider2D collider = Physics2D.OverlapBox(currentMapPoint.transform.position, boxSize, 0);
            GameObject point = collider.gameObject;
            if(mapPointDictionary.TryGetValue(point, out bool setStatus))
            {
                if(point.GetComponent<MapPoint>().GetIsStart())
                {
                    OnRoomGenerated?.Invoke();
                    Vector2 newPos = new Vector2(currentMapPoint.transform.position.x + moveAmount,
                        currentMapPoint.transform.position.y); 
                    currentMapPoint.transform.position = newPos;
                }
                else if (point.GetComponent<MapPoint>().GetIsTreasure())
                {
                    OnRoomGenerated?.Invoke();
                    Vector2 newPos = new Vector2(currentMapPoint.transform.position.x + moveAmount,
                        currentMapPoint.transform.position.y); 
                    currentMapPoint.transform.position = newPos;
                }  
                else if(setStatus == true)
                {
                    point.GetComponent<MapPoint>().SetMax(empty, coin, enemy, trap, potion, this);
                    if(point.GetComponent<MapPoint>().SetNonHazardRoom(emptyCount, coinCount, enemyCount, potionCount))
                    {
                        Vector2 newPos = new Vector2(currentMapPoint.transform.position.x + moveAmount,
                            currentMapPoint.transform.position.y); 
                        currentMapPoint.transform.position = newPos; 
                        OnRoomGenerated?.Invoke();                           
                    }
                }
                else
                {
                    Vector2 newPos = new Vector2(currentMapPoint.transform.position.x + moveAmount,
                        currentMapPoint.transform.position.y); 
                    currentMapPoint.transform.position = newPos;
                }
            }
        }
        else
        {
            rowIndex++;
            if(rowIndex < maxRows)
            {
                Vector2 newPos = new Vector2(mapBegin.position.x,
                    currentMapPoint.transform.position.y - 1); 
                currentMapPoint.transform.position = newPos;                 
            }                
            else 
            {
                pathRoomsFull = true;    
                currentMapPoint.transform.position = mapBegin.position;
                rowIndex = 0;
            }
        }
    }

    private void FillNonPathRooms()
    {
        if(currentMapPoint.transform.position.x <= maxX)
        {
            Collider2D collider = Physics2D.OverlapBox(currentMapPoint.transform.position, boxSize, 0);
            GameObject point = collider.gameObject;
            if(mapPointDictionary.TryGetValue(point, out bool setStatus))
            {
                if(setStatus == false)
                {
                    point.GetComponent<MapPoint>().SetMax(empty, coin, enemy, trap, potion, this);
                    if(point.GetComponent<MapPoint>().SetRoom(emptyCount, coinCount, enemyCount, trapCount, potionCount))
                    {
                        mapPointDictionary[point] = true;
                        OnRoomGenerated?.Invoke();
                        Vector2 newPos = new Vector2(currentMapPoint.transform.position.x + moveAmount,
                            currentMapPoint.transform.position.y); 
                        currentMapPoint.transform.position = newPos;
                    }
                } 
                else
                {
                    Vector2 newPos = new Vector2(currentMapPoint.transform.position.x + moveAmount,
                        currentMapPoint.transform.position.y); 
                    currentMapPoint.transform.position = newPos;
                }
            }

        }
        else
        {
            rowIndex++;
            if(rowIndex < maxRows)
            {
                Vector2 newPos = new Vector2(mapBegin.position.x,
                    currentMapPoint.transform.position.y - 1); 
                currentMapPoint.transform.position = newPos;                 
            }                
            else 
            {
                nonPathRoomsFull = true;
                GameManager.i.StartFadeIn();
            }
        }

    }

    
    public bool CheckRandomRoom(int room)
    {
        if(room == lastSetRoom) 
        {
            lastSetRoom = room;
            return false;
        }
        else 
        {
            lastSetRoom = room;
            return true;
        }
    }

    private void IncreaseRoomCount(string room)
    {
        switch(room)
        {
            case "empty":
                emptyCount++;
                break;
            case "coin":
                coinCount++;
                break;
            case "potion":
                potionCount++;
                break;
            case "trap":
                trapCount++;
                break;
            case "enemy":
                enemyCount++;
                break;
            default:
                Debug.Log("String Mismatch on IncreaseRoomCount");
                break;
        }
    }

    private void ReplaceRoomCount(string room)
    {
        switch(room)
        {
            case "coin":
                coinCount++;
                emptyCount--;
                break;
            case "potion":
                potionCount++;
                emptyCount--;
                break;
            case "trap":
                trapCount++;
                emptyCount--;
                break;
            case "enemy":
                enemyCount++;
                emptyCount--;
                break;
            case "empty":
                emptyCount--;
                emptyCount++;
                break;
            default:
                Debug.Log("String Mismatch on IncreaseRoomCount");
                break;
        }
    }
    public int GetRoomCount(string room)
    {
        switch(room)
        {
            case "empty":
                return empty;
            case "coin":
                return coin;
            case "potion":
                return potion;
            case "trap":
                return trap;
            case "enemy":
                return enemy;
            default:
                Debug.Log("String Mismatch on GetRoomCount");
                return -99;
        }
    }
    public GameObject GetStartingPoint(){ return startPoint; }
}
