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
    public GameObject characterPrefab;
    [SerializeField] private Places places;
    [SerializeField] private GameObject available;
    [SerializeField] private GameObject unavailable;
    [SerializeField] private GameController gameController;

    private void Awake()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
    }

    public void SetAvailable(bool isAvailable)
    {
        available.SetActive(isAvailable);
        unavailable.SetActive(!isAvailable);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(gameController.coins < characterPrefab.GetComponent<CharacterController>().price) return;
        else if(gameController.characterDemo) return;
        
        gameController.characterDemo = Instantiate(demoPrefab);
        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(eventData.position);
        touchPosition.z = 0;
        gameController.characterDemo.transform.position = touchPosition;
        SoundManager.Instance.PlaySound(SoundManager.Instance.selectedCharacterSound);
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
            if(Vector2.Distance(gameController.characterDemo.transform.position, place.transform.position) < delta &&
            !place.GetComponent<Places>().isOccupied)
            {
                canPurchase = true;
                gameController.characterDemo.transform.position = place.transform.position;
                characterPosition = place.transform.position;
                places = place.GetComponent<Places>();
            }
        }
    }

    public void OnPointerUp (PointerEventData eventData)
    {
        if(gameController.characterDemo)
        {
            if(!canPurchase) 
            {
                Destroy(gameController.characterDemo);
                places = null;
                SoundManager.Instance.PlaySound(SoundManager.Instance.cancelCharacterSound);
            }
            else
            {
                GameObject character = Instantiate(characterPrefab);
                character.transform.position = characterPosition;
                places.isOccupied = true;
                SoundManager.Instance.PlaySound(SoundManager.Instance.placedCharacterSound);
                Destroy(gameController.characterDemo);
            }
        }
    }
}
