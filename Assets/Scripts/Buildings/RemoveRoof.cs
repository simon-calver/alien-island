using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveRoof : MonoBehaviour
{
    private Transform playerTransform;// = null;

    // Start is called before the first frame update
    void Start()
    {
        // The position of the player is used to determine if a roof is see-through
        playerTransform = GameObject.Find("Player").transform;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // What has enterd the line of sight trigger
        if (other.transform.tag == "Player")// | other.transform.tag == "Mob")
        {
            StartCoroutine(FadeTo(0.0f, 1.0f));
            StartCoroutine(ZoomCamera(0.75f, 1.0f));
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform.tag == "Player")// | other.transform.tag == "Mob") //(other.transform == target)
        {
            StartCoroutine(FadeTo(1.0f, 1.0f));
            StartCoroutine(ZoomCamera(1.5f, 1.0f));

        }
    }

    IEnumerator FadeTo(float aValue, float aTime)
    {
        float alpha = transform.GetChild(0).GetComponent<SpriteRenderer>().color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, aValue, t));
            foreach (Transform child in transform)
            {
                child.GetComponent<SpriteRenderer>().color = newColor;
            }
            yield return null;
        }
    }

    IEnumerator ZoomCamera(float zValue, float zTime)
    {
        float orthographic_size = Camera.main.orthographicSize;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / zTime)
        {
            Camera.main.orthographicSize = orthographic_size + t * (zValue-orthographic_size);
            yield return null;
        }
    }

}
