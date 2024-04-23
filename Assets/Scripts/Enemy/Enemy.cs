using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public static event Action OnDecreaseHealth;
    public static Action<Enemy> OnEndReached;

    public enum EnemyType
    {
        Snail,
        Chameleon,
        Pig,
        Rino,
        Bat
    }

    public EnemyType enemyType;
    
    private Vector3 currentPointPosition;
    private Vector3 lastPointPosition;
    [SerializeField] private Vector3 initScale;
    private SpriteRenderer spriteRenderer;
    private int currentWaypointIndex;
    public EnemyHealth enemyHealth;

    public bool poisoned;
    public float timePoisoned;
    public float moveSpeed;
    [SerializeField] public float moveDefault;
    [SerializeField] private float moveSlow;
    [SerializeField] private Waypoints waypoints;
    [SerializeField] private Spawner spawner;
    public ObjectPooler objectPooler;
    [SerializeField] private GameController gameController;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyHealth = GetComponent<EnemyHealth>();
        spawner = GameObject.Find("Spawner").GetComponent<Spawner>();
        gameController = GameObject.Find("GameController").GetComponent<GameController>();

        if(spawner.listWaypoints.Length != 1)
        {
            System.Random random = new System.Random();
            int randomIndex = random.Next(spawner.listWaypoints.Length);
            waypoints = spawner.listWaypoints[randomIndex];
        }
        else
        {
            waypoints = spawner.listWaypoints[0];
        }

        currentPointPosition = waypoints.Points[currentWaypointIndex];
        lastPointPosition = transform.position;

        initScale = transform.localScale;

        moveSlow = moveDefault - 0.5f;
        moveSpeed = moveDefault;
    }

    private void Update()
    {
        if(gameController.isGameOver) return;
        Move();
        Rotate();
        if(poisoned)
        {
            timePoisoned -= Time.deltaTime;
            if(timePoisoned <= 0)
            {
                timePoisoned = 0;
                moveSpeed = moveDefault;
                poisoned = false;
            }
        }

        if(currentPointPositionReached()) UpdateCurrentPointIndex();
    }

    public void Poisoned()
    {
        poisoned = true;
        timePoisoned = 5;
        moveSpeed = moveSlow;
    }

    private void UpdateCurrentPointIndex()
    {
        int lastWaypointIndex = waypoints.Points.Length - 1;
        if(currentWaypointIndex < lastWaypointIndex)
        {
            currentWaypointIndex++;
            currentPointPosition = waypoints.Points[currentWaypointIndex];
        }
        else
        {
            EndPointReached();
        }
    }

    private void EndPointReached()
    {
        OnEndReached?.Invoke(this);
        enemyHealth.ResetHealth();
        objectPooler.ReturnToPool(gameObject);

        OnDecreaseHealth?.Invoke();
    }

    private bool currentPointPositionReached()
    {
        float distanceToNextPointPosition = (transform.position - currentPointPosition).magnitude;
        if(distanceToNextPointPosition < 0.1f)
        {
            lastPointPosition = transform.position;
            return true;
        }

        return false;
    }

    private void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, currentPointPosition, moveSpeed * Time.deltaTime);
    }

    private void Rotate()
    {
        if(currentPointPosition.x > lastPointPosition.x)
        {
            // spriteRenderer.flipX = false;
            transform.localScale = initScale;
        }
        else
        {
            // spriteRenderer.flipX = true;
            transform.localScale = new Vector3(-initScale.x, initScale.y, initScale.z);
        }
    }
}
