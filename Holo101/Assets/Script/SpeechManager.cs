﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class SpeechManager : MonoBehaviour
{
    public  GameObject Container;
    public Material FirstCubeMaterial;

    private KeywordRecognizer _keywordRecognizer;
    private readonly Dictionary<string, Action> _keywords = new Dictionary<string, Action>();
    private readonly System.Random _random = new System.Random();

    // Use this for initialization
    private void Start()
    {
        _keywords.Add("Add cube", AddCube);
        _keywords.Add("Reset scene", ResetScene);
        _keywords.Add("Gravity", ToggleGravity);

        _keywordRecognizer = new KeywordRecognizer(_keywords.Keys.ToArray());
        _keywordRecognizer.OnPhraseRecognized += OnPhraseRecognized;
        _keywordRecognizer.Start();
    }

    private void ToggleGravity()
    {
        Cursor.UseGravity = !Cursor.UseGravity;
    }

    private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        Action keywordAction;
        if (_keywords.TryGetValue(args.text, out keywordAction))
        {
            keywordAction.Invoke();
        }
    }

    private void ResetScene()
    {
        if (Container == null)
        {
            return;
        }

        foreach (Transform t in Container.transform)
        {
            var cube = t.gameObject;
            Destroy(cube);
        }
    }

    private void AddCube()
    {
        if (Container == null
            || FirstCubeMaterial == null)
        {
            return;
        }

        var newCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        newCube.transform.SetParent(Container.transform, true);
        newCube.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

        newCube.transform.rotation = Quaternion.Euler(
            45f,
            0,
            45f);

        newCube.transform.localPosition = new Vector3(0f, 0f, 0f);

        var rigidBody = newCube.AddComponent<Rigidbody>();
        rigidBody.useGravity = false;

        var material = new Material(FirstCubeMaterial.shader);

        var color = new Color(
            (float)_random.NextDouble(),
            (float)_random.NextDouble(),
            (float)_random.NextDouble());

        material.SetColor("_Color", color);

        var newRenderer = newCube.GetComponent<Renderer>();
        newRenderer.material = material;
    }
}