using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RppAdventure
{
    public class PlayerController : MonoBehaviour
    {
        const float k_Acceleration = 20.0f;
        const float k_Deceleration = 35.0f;

        public float maxForwardSpeed = 8.0f;
        public float rotationSpeed;
        public float m_MaxRotationSpeed = 1200;
        public float m_MinRotationSpeed = 800;

        private PlayerInput m_PlayerInput;
        private CharacterController m_ChController;
        private CameraController  m_CameraController;
        private Vector3 m_Movement;
        private Animator m_Animator;
        private readonly int m_HashForwardSpeed = Animator.StringToHash("ForwardSpeed");
        private float m_DesiredForwardSpeed;
        private float m_ForwardSpeed;
        private Quaternion m_CameraRotation;
        private Quaternion m_TargetRotation;


        private void Awake()
        {
            m_ChController = GetComponent<CharacterController>();
            m_PlayerInput = GetComponent<PlayerInput>();
            m_CameraController = GetComponent<CameraController>();
            m_Animator = GetComponent<Animator>();
        }
        private void FixedUpdate()
        {
            ComputeMovement();
            ComputeRotation();

            if (m_PlayerInput.IsMoveInput)
            {
                float rotationSpeed = Mathf.Lerp(m_MaxRotationSpeed, m_MinRotationSpeed, m_ForwardSpeed/ m_DesiredForwardSpeed);
                m_TargetRotation = Quaternion.RotateTowards(
                    transform.rotation,
                    m_TargetRotation,
                    400 * Time.fixedDeltaTime);
                transform.rotation = m_TargetRotation;
            }
            
        }

        private void ComputeMovement()
        {
            Vector3 moveInput = m_PlayerInput.MoveInput.normalized;
            m_DesiredForwardSpeed = moveInput.magnitude * maxForwardSpeed;

            float accekeration = m_PlayerInput.IsMoveInput ? k_Acceleration : k_Deceleration;

            m_ForwardSpeed = Mathf.MoveTowards(
                m_ForwardSpeed,
                m_DesiredForwardSpeed,
                Time.deltaTime);

            m_Animator.SetFloat(m_HashForwardSpeed, m_ForwardSpeed);
        }

        private void ComputeRotation()
        {
            Vector3 moveInput = m_PlayerInput.MoveInput.normalized;
            Vector3 cameraDirection = Quaternion.Euler(
                0,
                m_CameraController.freeLookCamera.m_XAxis.Value,
                0
                ) * Vector3.forward;

            Quaternion movementRotation = Quaternion.FromToRotation(Vector3.forward, moveInput);
            Quaternion targetRotation = Quaternion.LookRotation(movementRotation * cameraDirection);

            m_TargetRotation = targetRotation;
        }
    }
}
