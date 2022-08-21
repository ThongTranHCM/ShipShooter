using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//using AssetBundles;

//====================================================================================================
/// <summary>
/// Scene loader.
/// </summary>
//====================================================================================================
public class SceneLoader : MonoBehaviour
{
	#region Static Variables
	private static SceneLoader instance = null;

	public static bool isLoading{
		get{
			if(Controller.gameObject.activeSelf){
				return true;
			}
			return false;
		}
	}

	#endregion
	public SceneLoaderController controller;
	public GraphicRaycaster gpRaycaster;
	public bool playFirstTime {get;set;}

	//----------------------------------------------------------------------------------------------------
	/// <summary>
	/// Awake this instance.
	/// </summary>
	//----------------------------------------------------------------------------------------------------
	public void Awake ()
	{
		if (instance != null && instance != this) {
			Destroy (gameObject);
			return;
		}
		instance = this;
		playFirstTime = true;

		GameObject.DontDestroyOnLoad (instance.gameObject);
		Application.backgroundLoadingPriority = ThreadPriority.High;
	}

	//----------------------------------------------------------------------------------------------------
	/// <summary>
	/// Gets the instance.
	/// </summary>
	/// <value>The instance.</value>
	//----------------------------------------------------------------------------------------------------
	private static SceneLoader Instance {
		get {
			if (instance == null) {
				instance = FindObjectOfType<SceneLoader> ();
				GameObject.DontDestroyOnLoad (instance.gameObject);
			}
			return instance;
		}
	}
		
	public static SceneLoaderController Controller {
		get {
			return Instance.controller;
		}
	}

	#region LoadScene Normal
	public static Coroutine LoadLevel (string name)
	{
		return Instance.StartCoroutine (GetRoutineLoadNormal (name));
	}

	static IEnumerator GetRoutineLoadNormal (string name)
    {
        Controller.Show();
        yield return null;
        var asyncLoad = SceneManager.LoadSceneAsync(name, LoadSceneMode.Single);
        while (!asyncLoad.isDone)
        {
            Controller.mySliderLoading.value = Mathf.Max(Controller.mySliderLoading.value, asyncLoad.progress * 0.6f);// Khi load Scene MainMenu lan dau tien, Progress tang dan sau do reset ve 0
            yield return null;
        }
        yield return Resources.UnloadUnusedAssets();

        // Wait for 1 frame so MainMenuScreenController.instance is assigned
        yield return Yielder.Get(1);

        if (name.Equals(Constants.SCENENAME_GamePlay) || name.Equals(Constants.SCENENAME_GamePlay_Tutorial))
        {
            if (GamePlayManager.Instance != null)
            {
                while (!GamePlayManager.Instance.canShowScene)
                {
                    yield return null;
                }
            }
        }
        else if (name.Equals(Constants.SCENENAME_MainMenu))
        {
            if (MainMenuController.instance != null)
            {
                while (!MainMenuController.instance.canShowScene)
                {
                    yield return null;
                }
            }
        }
        else
        {
            yield return Yielder.Get(1);
        }
        yield return new WaitUntil(()=> Controller.showFinished);
		yield return null;
        Controller.Hide();
        yield return new WaitUntil(() => Controller.hideFinished);
        yield return Yielder.Get(0.2f);
        Controller.gameObject.SetActive(false);
        Instance.gpRaycaster.enabled = false;
    }
	#endregion
}
