using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Player {

    public UIController ui;
        
    public override void DecideOnShot()
    {
        //Change player's position to be looking down at house
        transform.position = gc.gState.topPosition.transform.position;
        transform.rotation = gc.gState.topPosition.transform.rotation;
        ui.SetCanvasCamera(ui.weightCanvas, cam);
        ui.weightCanvas.gameObject.SetActive(true);
    }
}
