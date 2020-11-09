
using System;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float interactionDistance;
    [SerializeField] private LayerMask interactionLayer;
    [SerializeField] private GameObject interactiveText;
    private Transform _previousInteraction;
    private bool _hasPreviousValue = false;
    private GameObject _instatiatedText;
    private Collider2D _playerCollider;

    private void Awake()
    {
        _playerCollider = transform.GetComponent<Collider2D>();
    }

    private void Update()
    {
        Vector2 origin = new Vector2(transform.position.x, transform.position.y);
        Vector3 direction3D = GetComponent<PlayerControllerMap>().Movement;
        Vector2 direction = new Vector2(direction3D.x, direction3D.y);
        //RaycastHit2D hit = Physics2D.Raycast(origin, direction, interactionDistance, interactionLayer);
        Bounds bounds = transform.GetComponent<Collider2D>().bounds;
        RaycastHit2D hit = Physics2D.CircleCast(origin, bounds.extents.y/2, direction, interactionDistance, interactionLayer);
        if (hit)
        {
            Transform trans = hit.transform;
            if (trans.name == "Chest")
            {
                if (!_hasPreviousValue || trans.GetInstanceID() != _previousInteraction.transform.GetInstanceID())
                {
                    Material shader = hit.transform.GetComponent<SpriteRenderer>().material;
                    shader.SetFloat("_Thickness",5f);
                    
                    Destroy(_instatiatedText);
                    _instatiatedText = Instantiate(interactiveText);
                    _instatiatedText.transform.parent = trans;
                    _instatiatedText.transform.position = trans.position + new Vector3(13.5f,-2.7f,0f);
                    _previousInteraction = hit.transform;
                    _hasPreviousValue = true;
                }
            }
        }else if (_instatiatedText)
        {
            Material shader = _previousInteraction.GetComponent<SpriteRenderer>().material;
            shader.SetFloat("_Thickness",0f);
            Destroy(_instatiatedText);
            _hasPreviousValue = false;
        }
    }
}
