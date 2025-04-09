using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SetupMainMenu : MonoBehaviour
{
    public GameObject startButtonPrefab;
    public GameObject continueButtonPrefab;
    public GameObject customizeButtonPrefab;
    public GameObject collectionButtonPrefab;
    public GameObject settingsButtonPrefab;

    void Start()
    {
        StartCoroutine(SetupButtons());
    }

    IEnumerator SetupButtons()
    {
        float delay = 0.1f;
        
        yield return CreateButtonWithAnimation(startButtonPrefab, new Vector3(0, 200, 0));
        yield return CreateButtonWithAnimation(continueButtonPrefab, new Vector3(0, 100, 0));
        yield return CreateButtonWithAnimation(customizeButtonPrefab, new Vector3(0, 0, 0));
        yield return CreateButtonWithAnimation(collectionButtonPrefab, new Vector3(0, -100, 0));
        yield return CreateButtonWithAnimation(settingsButtonPrefab, new Vector3(0, -200, 0));
    }

    IEnumerator CreateButtonWithAnimation(GameObject buttonPrefab, Vector3 position)
    {
        GameObject button = Instantiate(buttonPrefab, transform);
        button.transform.localPosition = position;
        button.transform.localScale = Vector3.zero;
        
        float elapsed = 0;
        float duration = 0.2f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            button.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, progress);
            yield return null;
        }
        
        button.transform.localScale = Vector3.one;
        yield return new WaitForSeconds(0.1f);
    }
}
