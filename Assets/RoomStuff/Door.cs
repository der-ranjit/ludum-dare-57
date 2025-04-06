using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Check if game is done
            if (GameManager.Instance.IsRoomComplete()) {
                Debug.Log("Room is complete, proceeding to next room");
                GameManager.Instance.ProceedToNextRoom();
            }
        }       
    }
}
