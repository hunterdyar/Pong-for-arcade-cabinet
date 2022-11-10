using System.Collections;
using System.Collections.Generic;
using Pong;
using UnityEngine;

public class OuterBoundsCreator : MonoBehaviour
{
    [SerializeField] private PhysicsMaterial2D _physicsMaterial2D;

    [SerializeField] private PlayerData _scoreWhenHitTopPlayerData;

    [SerializeField] private PlayerData _scoreWhenHitBottomPlayerData;
    // Start is called before the first frame update
    void Awake()
    {
        CreateOuterBounds();
    }

    //This code is ugly, don't copy it for other things.
    //but it works. I can refactor it later. I probably wont bother.
    private void CreateOuterBounds()
    {
        float thick = 1;
        float extra = 1;
        var bounds = Utility.GetXYScreenBoundsInWorldSpace();
        Vector2 center = bounds.center;
        Vector2 size = bounds.size;
        Vector3 topRight = new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y + bounds.extents.y, 0);
        Vector3 bottomLeft = new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y - bounds.extents.y, 0);
        Vector3 topLeft = new Vector3(bottomLeft.x, topRight.y, 0);
        Vector3 bottomRight = new Vector3(topRight.x, bottomLeft.y,0);
        var horizontalSize = new Vector3(topRight.x - bottomLeft.x+extra, thick, 1);
        var verticalSize = new Vector3(thick, topRight.y - bottomLeft.y+extra);

        transform.position = center;
        
        GameObject top = new GameObject();
        top.transform.SetParent(transform);
        top.transform.position = center;
        top.name = "Top";
        top.layer = gameObject.layer;
        var box = top.AddComponent<BoxCollider2D>();
        box.sharedMaterial = _physicsMaterial2D;
        top.AddComponent<ScoreZone>().SetScoringPlayer(_scoreWhenHitTopPlayerData);
        var offset = new Vector3(0, thick / 2, 0);
        var bCenter = Vector3.Lerp(topRight, topLeft, 0.5f)+offset;
        box.offset = bCenter;
        box.size = horizontalSize;

        GameObject bottom = new GameObject();
        bottom.transform.SetParent(transform);
        bottom.transform.position = center;
        bottom.name = "Bottom";
        bottom.layer = gameObject.layer;
        box = bottom.AddComponent<BoxCollider2D>();
        box.sharedMaterial = _physicsMaterial2D;
        bottom.AddComponent<ScoreZone>().SetScoringPlayer(_scoreWhenHitBottomPlayerData);
        offset = new Vector3(0, -thick / 2, 0);
        bCenter = Vector3.Lerp(bottomLeft, bottomRight, 0.5f) + offset;
        box.offset = bCenter;
        box.size = horizontalSize;

        GameObject right = new GameObject();
        right.transform.SetParent(transform);
        right.transform.position = center;
        right.name = "Right";
        right.layer = gameObject.layer;
        box = right.AddComponent<BoxCollider2D>();
        box.sharedMaterial = _physicsMaterial2D;
        offset = new Vector3(thick/2,0, 0);
        bCenter = Vector3.Lerp(topRight, bottomRight, 0.5f) + offset;
        box.offset = bCenter;
        box.size = verticalSize;

        GameObject left = new GameObject();
        left.transform.SetParent(transform);
        left.transform.position = center;
        left.name = "Left";
        left.layer = gameObject.layer;
        box = left.AddComponent<BoxCollider2D>();
        box.sharedMaterial = _physicsMaterial2D;
        offset = new Vector3(-thick / 2, 0, 0);
        bCenter = Vector3.Lerp(topLeft, bottomLeft, 0.5f) + offset;
        box.offset = bCenter;
        box.size = verticalSize;
        
    }


}
