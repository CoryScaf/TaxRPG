using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class abandon : MonoBehaviour
{
    public void AbandonRun(){
        FindObjectOfType<GameManager>().CompleteRun();
    }
}

