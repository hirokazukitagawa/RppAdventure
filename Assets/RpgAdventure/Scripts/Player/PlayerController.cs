using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RppAdventure
{
    public class PlayerController : MonoBehaviour
    {
        public float maxForwardSpeed = 8.0f;
        public float rotationSpeed;

        private PlayerInput m_PlayerInput;
        private CharacterController m_ChController;
        private Camera m_MainCamera;
        private Vector3 m_Movement;
        private Animator m_Animator;
        private readonly int m_HashForwardSpeed = Animator.StringToHash("ForwardSpeed");
        private float m_DesiredForwardSpeed;
        private float m_ForwardSpeed;

        private void Awake()
        {
            m_ChController = GetComponent<CharacterController>();
            m_PlayerInput = GetComponent<PlayerInput>();
            m_MainCamera = Camera.main;
            m_Animator = GetComponent<Animator>();
        }
        private void FixedUpdate()
        {
            ComputeMovement();

            
        }

        private void ComputeMovement()
        {
            Vector3 moveInput = m_PlayerInput.MoveInput.normalized;
            m_DesiredForwardSpeed = moveInput.magnitude * maxForwardSpeed;

            m_ForwardSpeed = Mathf.MoveTowards(
                m_ForwardSpeed,
                m_DesiredForwardSpeed,
                Time.deltaTime);

            m_Animator.SetFloat(m_HashForwardSpeed, m_ForwardSpeed);
        }
    }
}
