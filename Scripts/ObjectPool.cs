using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField]
    private GameObject[] objectPrefabs;

    private List<GameObject> pooledObjects = new List<GameObject>();

    public GameObject GetObject(string type)
    {
        foreach(GameObject go in pooledObjects)
        {
            if (go.name == type && !go.activeInHierarchy) //verifica se o nome do monstro da lista é o mesmo monstro gerado e verifica se ele não ta ativo
            {
                go.SetActive(true);
                return go;
            }
        }
        for (int i = 0; i < objectPrefabs.Length; i++) //instancia um objeto 
        {
            if (objectPrefabs[i].name == type)
            {
                GameObject newObject = Instantiate(objectPrefabs[i]);
                pooledObjects.Add(newObject); //adiciona o monstro a uma lista

                newObject.name = type;
                return newObject;
            }
        }
        return null;
    }

    public void ReleaseObject(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }
}
