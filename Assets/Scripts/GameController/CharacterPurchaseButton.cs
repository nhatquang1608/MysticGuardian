using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterPurchaseButton : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private float delta = 0.5f;
    [SerializeField] private bool canPurchase;
    [SerializeField] private Vector3 characterPosition;
    [SerializeField] private GameObject demoPrefab;
    [SerializeField] private GameObject characterPrefab;
    [SerializeField] private GameController gameController;

    private void Awake()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(gameController.coins < characterPrefab.GetComponent<CharacterController>().price) return;
        else if(gameController.characterDemo) return;
        
        gameController.characterDemo = Instantiate(demoPrefab);
        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(eventData.position);
        touchPosition.z = 0;
        gameController.characterDemo.transform.position = touchPosition;
    }

    public void OnDrag (PointerEventData eventData)
    {
        if(!gameController.characterDemo) return;

        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(eventData.position);
        touchPosition.z = 0;
        gameController.characterDemo.transform.position = touchPosition;
        canPurchase = false;

        foreach(GameObject place in gameController.listPlaces)
        {
            if(Vector2.Distance(gameController.characterDemo.transform.position, place.transform.position) < delta)
            {
                canPurchase = true;
                gameController.characterDemo.transform.position = place.transform.position;
                characterPosition = place.transform.position;
            }
        }
    }

    public void OnPointerUp (PointerEventData eventData)
    {
        if(gameController.characterDemo)
        {
            if(!canPurchase) Destroy(gameController.characterDemo);
            else
            {
                GameObject character = Instantiate(characterPrefab);
                character.transform.position = characterPosition;
                Destroy(gameController.characterDemo);
            }
        }
    }
}
