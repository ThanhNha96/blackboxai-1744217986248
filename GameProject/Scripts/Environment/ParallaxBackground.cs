using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxLayer
    {
        public Transform transform;
        [Range(0f, 1f)]
        public float parallaxEffect;
        public bool infiniteHorizontal;
        public float offsetX;
        public Sprite[] variations;
        public Color tintColor = Color.white;
    }

    public ParallaxLayer[] layers;
    public float smoothing = 1f;

    private Transform cameraTransform;
    private Vector3 previousCameraPosition;
    private float[] layerWidth;

    void Start()
    {
        cameraTransform = Camera.main.transform;
        previousCameraPosition = cameraTransform.position;
        layerWidth = new float[layers.Length];

        // Initialize layers
        for (int i = 0; i < layers.Length; i++)
        {
            if (layers[i].transform != null)
            {
                // Set random variation if available
                if (layers[i].variations != null && layers[i].variations.Length > 0)
                {
                    SpriteRenderer spriteRenderer = layers[i].transform.GetComponent<SpriteRenderer>();
                    if (spriteRenderer != null)
                    {
                        spriteRenderer.sprite = layers[i].variations[Random.Range(0, layers[i].variations.Length)];
                        spriteRenderer.color = layers[i].tintColor;
                    }
                }

                // Store layer width for infinite scrolling
                SpriteRenderer renderer = layers[i].transform.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    layerWidth[i] = renderer.bounds.size.x;
                }
            }
        }
    }

    void LateUpdate()
    {
        Vector3 deltaMovement = cameraTransform.position - previousCameraPosition;

        for (int i = 0; i < layers.Length; i++)
        {
            if (layers[i].transform != null)
            {
                float parallax = (previousCameraPosition.x - cameraTransform.position.x) * layers[i].parallaxEffect;
                float backgroundTargetPosX = layers[i].transform.position.x + parallax;

                Vector3 backgroundTargetPos = new Vector3(backgroundTargetPosX, layers[i].transform.position.y, layers[i].transform.position.z);
                layers[i].transform.position = Vector3.Lerp(layers[i].transform.position, backgroundTargetPos, smoothing * Time.deltaTime);

                // Handle infinite scrolling
                if (layers[i].infiniteHorizontal)
                {
                    float distance = cameraTransform.position.x - layers[i].transform.position.x;
                    if (Mathf.Abs(distance) >= layerWidth[i])
                    {
                        float offsetX = (distance > 0) ? layerWidth[i] : -layerWidth[i];
                        layers[i].transform.position = new Vector3(layers[i].transform.position.x + offsetX, layers[i].transform.position.y, layers[i].transform.position.z);
                    }
                }
            }
        }

        previousCameraPosition = cameraTransform.position;
    }
}
