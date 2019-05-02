using UnityEngine;
using UnityEngine.UI;

public class Console : MonoBehaviour {

    [SerializeField]
    Text consoleTextObj;

    static Text consoleText;
    const int maxLines = 5;
    static int nLines;

	void Start () {
        consoleTextObj.text = "";
        nLines = 0;
        consoleText = consoleTextObj;
    }
	
	public static void Log(string str) {
        consoleText.text += "[" + Time.frameCount + "]" +  str + '\n';
        nLines++;

        if (nLines > maxLines) {
            consoleText.text = consoleText.text.Substring(consoleText.text.IndexOf('\n') + 1);
            nLines--;
        }
    }
}
