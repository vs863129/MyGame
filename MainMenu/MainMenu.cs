using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [SerializeField] float Speed;
    [SerializeField] Transform CameraFollow;
    [Header("載入")]
    [SerializeField] Slider ProgressBar;
    [SerializeField] TextMeshProUGUI ProgressIndex;
    private void FixedUpdate()
    {
        CameraFollow.Translate(Speed * Time.fixedDeltaTime * Vector3.left, Space.World);
    }
    public void Play(int scenIndex)
    {
        StartCoroutine(LoadAsynchronously(scenIndex));
    }
    IEnumerator LoadAsynchronously(int scenIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(scenIndex);
        operation.allowSceneActivation = false;
        while (!operation.isDone)
        {
            yield return new WaitForSeconds(1);
            float progress = Mathf.Clamp01(operation.progress / .9f);
            ProgressBar.value =  progress;
            ProgressIndex.text =System.Math.Round( progress * 100f,1) + "%";
            if(progress==1)
            {
                yield return new WaitForSeconds(1);
                operation.allowSceneActivation = true;
            }
        }
    }
    public void Quit()
    {
        Application.Quit();
    }
}
