using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Console : MonoBehaviour {

    [SerializeField]
    Text consoleText;

    [SerializeField]
    int maxLines = 2;

    int nLines;
    public static Console singleton;

	void Start () {
        singleton = this;
        consoleText.text = "";
        nLines = 0;
	}
	
	public void Log(string str) {
        consoleText.text += str + '\n';
        nLines++;

        if (nLines > maxLines) {
            consoleText.text = consoleText.text.Substring(consoleText.text.IndexOf('\n') + 1);
            nLines--;
        }
    }
}
