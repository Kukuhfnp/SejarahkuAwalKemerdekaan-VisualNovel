using System.Collections;
using System.Collections.Generic;
using System.IO.Enumeration;
using UnityEngine;

public class TestFile : MonoBehaviour
{
    [SerializeField] private TextAsset fileName;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Run());
    }

    IEnumerator Run()
    {
        List<string> lines = FileManager.ReadTextAsset(fileName,false);

        // foreach(string line in lines)
        // {
        //     //Debug.Log(line);
        // }
        yield return null;
    }
}