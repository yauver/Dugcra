using UnityEngine;
using UnityEngine.SceneManagement;

public class Change_scene : MonoBehaviour {

	public void ChangeToScene (int sceneToChangeTo) {
        SceneManager.LoadScene(sceneToChangeTo);
    }
}
