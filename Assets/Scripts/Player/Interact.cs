using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : MonoBehaviour
{
    #region member fields

    [SerializeField]
    AudioClip click, pop;

    [SerializeField]
    LayerMask interactMask;

    // Debug only
    [SerializeField]
    bool debug;

    Path lastPath;
    Camera mainCam;
    Tile currentTile;
    Character selectedCharacter;
    Pathfinder pathfinder;

    #endregion

    void Start()
    {
        mainCam = gameObject.GetComponent<Camera>();

        if(pathfinder == null)
        {
            pathfinder = GameObject.Find("Pathfinder").GetComponent<Pathfinder>();
        }
    }

    void Update()
    {
        Clear();
        MouseUpdate();
    }

    private void MouseUpdate()
    {
        if (!Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 200f, interactMask))
        {
            return;
        }

        currentTile = hit.transform.GetComponent<Tile>();
        InspectTile();
    }

    private void InspectTile()
    {
        // Alter cost by right clicking
        if(Input.GetMouseButtonUp(1))
        {
            currentTile.ModifyCost();
            return;
        }

        if(currentTile.Occupied)
        {
            InspectCharacter();
        }
        else
        {
            NavigateToTile();
        }
    }

    private void InspectCharacter()
    {
        if(currentTile.occupyingCharacter.Moving)
        {
            return;
        }

        currentTile.Highlight();

        if(Input.GetMouseButtonDown(0))
        {
            SelectCharacter();
        }
    }

    private void Clear()
    {
        if(currentTile == null || currentTile.Occupied == false)
        {
            return;
        }

        currentTile.ClearHighLight();
        currentTile = null;
    }

    private void SelectCharacter()
    {
        selectedCharacter = currentTile.occupyingCharacter;
        GetComponent<AudioSource>().PlayOneShot(pop);
    }

    private void NavigateToTile()
    {
        if(selectedCharacter == null || selectedCharacter.Moving == true)
        {
            return;
        }

        if(RetrievePath(out Path newPath))
        {
            if(Input.GetMouseButton(0))
            {
                GetComponent<AudioSource>().PlayOneShot(click);
                selectedCharacter.StartMove(newPath);
                selectedCharacter = null;
            }
        }
    }

    bool RetrievePath(out Path path)
    {
        path = pathfinder.FindPath(selectedCharacter.characterTile, currentTile);

        if(path == null || path == lastPath)
        {
            return false;
        }

        // Debug only
        if(debug)
        {
            ClearLastPath();
            DebugNewPath(path);
            lastPath = path;
        }

        return true;
    }

    // Debug only
    void ClearLastPath()
    {
        if(lastPath == null)
        {
            return;
        }

        foreach(Tile tile in lastPath.tiles)
        {
            tile.ClearText();
        }
    }

    // Debug only
    void DebugNewPath(Path path)
    {
        foreach(Tile t in path.tiles)
        {
            t.DebugCostText();
        }
    }
}
