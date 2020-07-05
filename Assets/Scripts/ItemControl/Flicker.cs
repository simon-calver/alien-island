using UnityEngine;
using System.Collections;

public class Flicker : MonoBehaviour
{
    public float MaxReduction;
    public float MaxIncrease;
    public float maxXMovement;
    public float maxYMovement;
    public float RateDamping;
    public float Strength;
    public bool _flickering;
    public float _baseIntensity;

    private UnityEngine.Experimental.Rendering.Universal.Light2D lightSource;
    private Vector3 positionNoise;
    private Vector3 lightPosition;

    //public void Reset()
    //{
    //    MaxReduction = 0.2f;
    //    MaxIncrease = 0.2f;
    //    RateDamping = 0.1f;
    //    Strength = 300;
    //}

    public void Start()
    {
        lightSource = this.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>();
        if (lightSource == null)
        {
            Debug.LogError("Flicker script must have a Light Component on the same GameObject.");
            return;
        }
        _baseIntensity = lightSource.intensity;
        lightPosition = lightSource.transform.localPosition;
        //Debug.Log(lightPosition);
        StartCoroutine(DoFlicker());
    }

    void Update()
    {
        if (!_flickering)
        {
            StartCoroutine(DoFlicker());
        }
    }

    private IEnumerator DoFlicker()
    {
        _flickering = true;
        lightSource.intensity = Mathf.Lerp(lightSource.intensity, Random.Range(_baseIntensity - MaxReduction, _baseIntensity + MaxIncrease), Strength * Time.deltaTime);
        positionNoise = new Vector3(Random.Range(-maxXMovement, maxXMovement), Random.Range(-maxYMovement, maxYMovement), 0f);
        lightSource.transform.localPosition = Vector3.Lerp(lightSource.transform.localPosition, lightPosition + positionNoise, Strength * Time.deltaTime);
        yield return new WaitForSeconds(RateDamping);
        _flickering = false;
    }
}