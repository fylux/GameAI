using UnityEngine;

public abstract class SelectBox : MonoBehaviour {

    public Select select;

	protected abstract void OnTriggerEnter (Collider other);

	protected abstract void OnTriggerExit (Collider other);

}
