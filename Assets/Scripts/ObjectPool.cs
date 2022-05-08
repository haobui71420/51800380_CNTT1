using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;
    public List<GameObject> objects;
    public GameObject ball;
    [SerializeField]
    private int amount = 81;
	private void Awake()
	{
        instance = this;
        objects = new List<GameObject>();
        GameObject tmp;

        for (int i = 0; i < amount; i++)
        {
            tmp = Instantiate(ball);
            tmp.SetActive(false);
            objects.Add(tmp);
        }
    }
	void Start()
    {
        
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < amount; i++)
        {
            if(!objects[i].activeInHierarchy)
            {
                return objects[i];
			}
		}

        return null;
	}
}
