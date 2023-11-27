using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    private HashSet<Collider2D> _interactables = new();
    private Collider2D _closest;

    private void Update()
    {
        _closest = TakeClosest();

        #region INPUT HANDLER
        if (Input.GetKeyDown(Controls.Get(InputAction.Up)) || Input.GetKeyDown(Controls.GetAlt(InputAction.Up)))
            OnInteract();

        #endregion
    }

    private Collider2D TakeClosest()
    {
        if (_interactables.Count == 0)
            return null;

        if (_interactables.Count == 1)
        {
            foreach (Collider2D interactable in _interactables)
                return interactable;
        }

        Vector3 position = transform.position;
        Collider2D closest = null;
        float minDist = float.MaxValue;
        foreach (Collider2D interactable in _interactables)
        {
            float dist = Vector2.Distance(interactable.transform.position, position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = interactable;
            }
        }
        return closest;
    }

    private void OnInteract()
    {
        if (_closest is null)
            return;
        _closest.GetComponent<Interactable>().Interact(this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Interactable"))
            _interactables.Add(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Interactable"))
            _interactables.Remove(other);
    }
}
