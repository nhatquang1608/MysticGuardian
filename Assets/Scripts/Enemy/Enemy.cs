using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public static event Action OnDecreaseHealth;
    public static Action<Enemy> OnEndReached;
    
    private Vector3 currentPointPosition;
    private Vector3 lastPointPosition;
    private SpriteRenderer spriteRenderer;
    private int currentWaypointIndex;
    public EnemyHealth enemyHealth;

    public bool poisoned;
    public float timePoisoned;
    public float moveSpeed;
    [SerializeField] public float moveDefault;
    [SerializeField] private float moveSlow;
    [SerializeField] private Waypoints waypoints;
    public ObjectPooler objectPooler;
    [SerializeField] private GameController gameController;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyHealth = GetComponent<EnemyHealth>();
        waypoints = GameObject.Find("Spawner").GetComponent<Waypoints>();
        gameController = GameObject.Find("GameController").GetComponent<GameController>();

        currentPointPosition = waypoints.Points[currentWaypointIndex];
        lastPointPosition = transform.position;

        moveSlow = moveDefault - 0.5f;
        moveSpeed = moveDefault;
    }

    private void OnEnable()
    {}

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
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }
    }
}