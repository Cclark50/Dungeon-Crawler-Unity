using System;
using UnityEngine;

public class CoreGame : MonoBehaviour
{
    public static CoreGame Instance;
    public DirectionResolver _directionResolver;

    private void Awake()
    {
        CreateSingleton();
        _directionResolver = new DirectionResolver();
    }

    void CreateSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        
    }
}