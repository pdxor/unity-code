using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class snappyPants : MonoBehaviour
{
	public UnityEvent triggerEvent;

    public void Invoke() {
        triggerEvent.Invoke();
        Debug.Log("event Invoked");
    }	

	private void OnTriggerEnter(Collider other)
	{
		Debug.Log("other.gameObject.name"+ other.gameObject.name);
		Debug.Log("other.gameObject.tag" + other.gameObject.tag);
		other.gameObject.transform.position = transform.position;
		other.gameObject.transform.SetParent(gameObject.transform);
		other.enabled = false;
		Invoke();

	}    
}
