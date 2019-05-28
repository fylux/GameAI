using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenes : MonoBehaviour
{
    public void PVC(){
		SceneManager.LoadScene(2);
	}
	public void CVC(){
		SceneManager.LoadScene(1);
	}
}
