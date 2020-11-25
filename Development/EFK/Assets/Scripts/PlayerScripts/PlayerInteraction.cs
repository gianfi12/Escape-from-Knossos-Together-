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
    private Collider2D _playerCollider;

    private void Awake()
    {
        _playerCollider = transform.GetComponent<Collider2D>();
    }


    public void SelectInteractableTarget(List<Transform> visibleTargets) {
        PlayerControllerMap playerControllerMap = GetComponent<PlayerControllerMap>();
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
                    Destroy(_instatiatedText);
                }

                shader = selectableTarget.GetComponent<SpriteRenderer>().material;
                shader.SetFloat("_Thickness", 5f);
                Destroy(_instatiatedText);
                _instatiatedText = Instantiate(interactiveText);
                _instatiatedText.transform.SetParent(selectableTarget);
                _instatiatedText.transform.position = selectableTarget.position + new Vector3(13.5f, -2.7f, 0f);
                _SelectedTarget = selectableTarget;
                _hasTargetSelected = true;
            }
        }
        else if (_hasTargetSelected) {
            Material shader = _SelectedTarget.GetComponent<SpriteRenderer>().material;
            shader.SetFloat("_Thickness", 0f);
            Destroy(_instatiatedText);
            _hasTargetSelected = false;
        }
    }

    public void InteractWithTarget(GameObject player) {
        if (HasTargetSelected) SelectedTarget.GetComponent<InteractableObject>().Interact(player);
    }

    public Transform SelectedTarget => _SelectedTarget;

    public bool HasTargetSelected => _hasTargetSelected;
}
