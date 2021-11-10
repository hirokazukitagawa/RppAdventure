using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RppAdventure
{
    public class PlayerController : MonoBehaviour
    {
        public float speed;
        public float rotationSpeed;

        private PlayerInput m_PlayerInput;
        private CharacterController m_ChController;
        private Camera m_MainCamera;
        private Vector3 m_Movement;

        private void Awake()
        {
            m_ChController = GetComponent<CharacterController>();
            m_PlayerInput = GetComponent<PlayerInput>();
            m_MainCamera = Camera.main;
        }
        void FixedUpdate()
        {
            Vector3 moveInput = m_PlayerInput.MoveInput;

            Quaternion camRotation = m_MainCamera.transform.rotation;
            Vector3 targetDirection = camRotation * moveInput;
            targetDirection.y = 0;

            m_ChController.Move(targetDirection.normalized * speed * Time.fixedDeltaTime);
            m_ChController.transform.rotation = Quaternion.Euler(0, camRotation.eulerAngles.y, 0);

        }
    }
}
