using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float interactionDistance;
    [SerializeField] private float interactionAngle;
    [SerializeField] private GameObject textBubble;
    private Vector3 textBubbleOffset;

    private Transform _SelectedTarget;
    private bool _hasTargetSelected = false;
    public bool canChangeLastInteractableObejct = true;
    private GameObject _instantiatedText;
    private PlayerControllerMap playerControllerMap;

    private void Awake()
    {
        playerControllerMap = GetComponent<PlayerControllerMap>();
        _instantiatedText = Instantiate(textBubble);
        _instantiatedText.SetActive(false);
        textBubbleOffset = _instantiatedText.GetComponentInChildren<SpriteRenderer>().bounds.extents / 2;
    }


    public void SelectInteractableTarget(List<Transform> visibleTargets) {
        Vector2 playerPosition = new Vector2(playerControllerMap.transform.position.x, playerControllerMap.transform.position.y);
        Vector2 playerDirection = new Vector2(playerControllerMap.Movement.x, playerControllerMap.Movement.y);

        Transform selectableTarget = visibleTargets.Where(t => Vector2.Distance(playerPosition, t.position) < interactionDistance)
                                                           .Where(t => Vector2.Angle(playerDirection, (new Vector2(t.position.x, t.position.y) - playerPosition).normalized) < interactionAngle)
                                                           .OrderBy(t => Vector2.Angle(playerDirection, (new Vector2(t.position.x, t.position.y) - playerPosition).normalized))
                                                           .FirstOrDefault();
        InteractableObject interactableObject;
        if (selectableTarget != null && canChangeLastInteractableObejct &&
            (interactableObject = selectableTarget.GetComponent<InteractableObject>()) != null) {
            if ((!_hasTargetSelected || selectableTarget.GetInstanceID() != _SelectedTarget.transform.GetInstanceID()))
            {
                Material material;
                if (_hasTargetSelected)
                {
                    material = _SelectedTarget.GetComponent<SpriteRenderer>().material;
                    material.SetFloat("_Thickness", 0f);
                    _instantiatedText.SetActive(false);
                }

                material = selectableTarget.GetComponent<SpriteRenderer>().material;
                material.SetFloat("_Thickness", 20f);

                _instantiatedText.transform.position = interactableObject.GetTextPosition() + textBubbleOffset;
                _instantiatedText.GetComponentInChildren<TMPro.TextMeshPro>().text = interactableObject.InteractiveText;
                _instantiatedText.SetActive(true);
                _SelectedTarget = selectableTarget;
                _hasTargetSelected = true;
            }
        }

        
        if (selectableTarget == null && canChangeLastInteractableObejct && _hasTargetSelected)
        {
            Material material = _SelectedTarget.GetComponent<SpriteRenderer>().material;
            material.SetFloat("_Thickness", 0f);
            _instantiatedText.SetActive(false);
            _hasTargetSelected = false;
        }


    }

    public void InteractWithTarget(GameObject player) {
        if (_hasTargetSelected) SelectedTarget.GetComponent<InteractableObject>().Interact(player);
    }

    public Transform SelectedTarget => _SelectedTarget;

    public bool HasTargetSelected => _hasTargetSelected;
}
