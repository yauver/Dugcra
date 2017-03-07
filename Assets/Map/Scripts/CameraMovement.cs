using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

    Camera cam;
    public World fog;

	// Use this for initialization
	void Start () {
        cam = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        horizontal = horizontal * 5 * Time.deltaTime;
        vertical = vertical * 5 * Time.deltaTime;

        Vector3 position = new Vector3(transform.position.x + horizontal, transform.position.y + vertical, transform.position.z);
        
        transform.position = position;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector2 rayPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
            RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero);
            WorldPos pos = EditTerrain.GetBlockPos(hit);
            if (hit.collider != null && hit.collider.GetComponent<Grid>())
            {
                fog.SetTile(pos.x, pos.y, new GridTile(GridTile.TileTypes.Empty));
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            cam.orthographicSize = 5;
        }
        if (Input.GetMouseButtonUp(1))
        {
            cam.orthographicSize = 8;
        }
    }
}
