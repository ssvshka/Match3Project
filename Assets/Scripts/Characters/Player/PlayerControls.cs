using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    [SerializeField] private GameObject upperLeftNose;
    [SerializeField] private GameObject upperRightNose;
    [SerializeField] private GameObject lowerLeftNose;
    [SerializeField] private GameObject lowerRightNose;

    private bool isLowerPosition;
    private Vector2 moveValue = Vector2.zero;
    private PlayerInputActions playerControls;
    private InputAction moveNose;
    private InputAction fire;

    private void Awake()
    {
        playerControls = new PlayerInputActions();
    }

    private void OnEnable()
    {
        moveNose = playerControls.Player.Move;
        moveNose.Enable();
        moveNose.performed += Move;
        fire = playerControls.Player.Fire;
        fire.Enable();
        fire.performed += Fire;
    }

    private void OnDisable()
    {
        moveNose.Disable();
        fire.Disable();
    }

    private void Move(InputAction.CallbackContext value)
    {
        moveValue = value.ReadValue<Vector2>();
        if (isLowerPosition)
            if (moveValue == Vector2.left)
            {
                lowerLeftNose.SetActive(true);
                DeactivateNose(lowerRightNose);
            }
            else if (moveValue == Vector2.right)
            {
                lowerRightNose.SetActive(true);
                DeactivateNose(lowerLeftNose);
            }
            else if (moveValue == Vector2.up)
            {
                if (lowerLeftNose.activeInHierarchy)
                {
                    upperLeftNose.SetActive(true);
                    DeactivateNose(lowerLeftNose);
                }
                else
                {
                    upperRightNose.SetActive(true);
                    DeactivateNose(lowerRightNose);
                }
                isLowerPosition = false;
            }

        if (!isLowerPosition)
            if (moveValue == Vector2.left)
            {
                upperLeftNose.SetActive(true);
                DeactivateNose(upperRightNose);
            }
            else if (moveValue == Vector2.right)
            {
                upperRightNose.SetActive(true);
                DeactivateNose(upperLeftNose);
            }
            else if (moveValue == Vector2.down)
            {
                if (upperLeftNose.activeInHierarchy)
                {
                    lowerLeftNose.SetActive(true);
                    DeactivateNose(upperLeftNose);
                }
                else
                {
                    lowerRightNose.SetActive(true);
                    DeactivateNose(upperRightNose);
                }
                isLowerPosition = true;
            }
    }

    private void DeactivateNose(GameObject nose) => nose.SetActive(false);

    private void Fire(InputAction.CallbackContext value)
    {
        Debug.Log("fire");
    }
        //private void ActivateNose(bool isLower, Vector2 vector, GameObject nose)
        //{

        //}
    }
