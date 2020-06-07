using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveRoof : MonoBehaviour
{






    //private Transform playerTrans = null;

    // Start is called before the first frame update
    void Start()
    {
        // The position of the player is used to determine if a roof is see-through
        //playerTransform = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {

        //if (Vector3.Distance(playerTrans.position, this.transform.position) < openDist)

        //this.GetComponent<SpriteRenderer>().color = spriteColours[XNA[XNA_index]];

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("AAh");
        // What has enterd the line of sight trigger
        if (other.transform.tag == "Player")// | other.transform.tag == "Mob")
        {
            StartCoroutine(FadeTo(0.0f, 1.0f));
            //Debug.Log("OOh");
            //target = other.transform;
            //m_IsTargetInRange = true;
            //Debug.Log(m_IsTargetInRange);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform.tag == "Player")// | other.transform.tag == "Mob") //(other.transform == target)
        {
            StartCoroutine(FadeTo(1.0f, 1.0f));
            //Debug.Log("GGRRR");
            //m_IsTargetInRange = false;
            //Debug.Log(m_IsTargetInRange);

        }
    }

    IEnumerator FadeTo(float aValue, float aTime)
    {
        
        float alpha = transform.GetComponent<SpriteRenderer>().color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Debug.Log(t);
            Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, aValue, t));
            transform.GetComponent<SpriteRenderer>().color = newColor;
            yield return null;
        }
    }


}
