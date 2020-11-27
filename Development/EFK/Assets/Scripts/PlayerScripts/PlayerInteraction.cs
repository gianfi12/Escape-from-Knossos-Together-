using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float interactionDistance;
    [SerializeField] private float interactionAngle;
    [SerializeField] private GameObject interactiveText;
    private Transform _SelectedTarget;
    private bool _hasTargetSelected = false;
    private GameObject _instatiatedText;
    private PlayerControllerMap playerControllerMap;

    private void Awake()
    {
        playerControllerMap = GetComponent<PlayerControllerMap>();
        _instatiatedText = Instantiate(interactiveText);
        _instatiatedText.SetActive(false);
    }


    public void SelectInteractableTarget(List<Transform> visibleTargets) {
        Vector2 playerPosition = new Vector2(playerControllerMap.transform.position.x, playerControllerMap.transform.position.y);
        Vector2 playerDirection = new Vector2(playerControllerMap.Movement.x, playerControllerMap.Movement.y);

        Transform selectableTarget = visibleTargets.Where(t => Vector2.Distance(playerPosition, t.position) < interactionDistance)
                                                           .Where(t => Vector2.Angle(playerDirection, (new Vector2(t.position.x, t.position.y) - playerPosition).normalized) < interactionAngle)
                                                           .OrderBy(t => Vector2.Angle(playerDirection, (new Vector2(t.position.x, t.position.y) - playerPosition).normalized))
                                                           .FirstOrDefault();

        if (selectableTarget != null) {
            if (!_hasTargetSelected || selectableTarget.GetInstanceID() != _SelectedTarget.transform.GetInstanceID()) {
                Material shader;
                if (_hasTargetSelected) {
                    shader = _SelectedTarget.GetComponent<SpriteRenderer>().material;
                    shader.SetFloat("_Thickness", 0f);
                    _instatiatedText.SetActive(false);
                }
                
                shader = selectableTarget.GetComponent<SpriteRenderer>().material;
                shader.SetFloat("_Thickness", 5f);
                _instatiatedText.transform.SetParent(selectableTarget);
                _instatiatedText.transform.position =
                    selectableTarget.GetComponent<InteractableObject>().GetTopRight().position;
                _instatiatedText.SetActive(true);
                _SelectedTarget = selectableTarget;
                _hasTargetSelected = true;
            }
        }
        else if (_hasTargetSelected) {
            Material shader = _SelectedTarget.GetComponent<SpriteRenderer>().material;
            shader.SetFloat("_Thickness", 0f);
            _instatiatedText.SetActive(false);
            _hasTargetSelected = false;
        }
    }

    public void InteractWithTarget(GameObject player) {
        if (HasTargetSelected) SelectedTarget.GetComponent<InteractableObject>().Interact(player);
    }

    public Transform SelectedTarget => _SelectedTarget;

    public bool HasTargetSelected => _hasTargetSelected;
}
