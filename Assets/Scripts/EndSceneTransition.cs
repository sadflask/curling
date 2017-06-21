using UnityEngine;

public class EndSceneTransition : MonoBehaviour {

    public UIController ui;

	void Update()
    {
        if (Input.GetMouseButtonDown(0))
            ui.ToMain();
    }
}
