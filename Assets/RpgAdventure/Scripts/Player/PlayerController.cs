using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RppAdventure
{
    public class PlayerController : MonoBehaviour
    { 

        public float maxForwardSpeed = 8.0f;
        public float rotationSpeed;
        public float m_MaxRotationSpeed = 1200;
        public float m_MinRotationSpeed = 800;
        public float gravity = 20.0f;
        public static PlayerController Instance
        {
            get
            {
                return s_Instance;
            }
        }

        const float k_Acceleration = 20.0f;
        const float k_Deceleration = 35.0f;

        private PlayerInput m_PlayerInput;
        private CharacterController m_ChController;
        private CameraController  m_CameraController;
        private Vector3 m_Movement;
        private Animator m_Animator;
        private readonly int m_HashForwardSpeed = Animator.StringToHash("ForwardSpeed");
        private float m_DesiredForwardSpeed;
        private float m_ForwardSpeed;
        private float m_VerticalSpeed;
        private Quaternion m_CameraRotation;
        private Quaternion m_TargetRotation;
        private static PlayerController s_Instance;

        private void Awake()
        {
            m_ChController = GetComponent<CharacterController>();
            m_PlayerInput = GetComponent<PlayerInput>();
            m_CameraController = Camera.main.GetComponent<CameraController>();
            m_Animator = GetComponent<Animator>();

            s_Instance = this;
        }
        private void FixedUpdate()
        {
            ComputeForwardMovement();
            ComputeVirticalMovement();
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

        private void OnAnimatorMove()
        {
            Vector3 movement = m_Animator.deltaPosition;
            movement += m_VerticalSpeed * Vector3.up * Time.fixedDeltaTime;
            m_ChController.Move(movement);
        }

        private void ComputeVirticalMovement()
        {
            m_VerticalSpeed = -gravity;
        }

        private void ComputeForwardMovement()
        {
            Vector3 moveInput = m_PlayerInput.MoveInput.normalized;
            m_DesiredForwardSpeed = moveInput.magnitude * maxForwardSpeed;

            float accekeration = m_PlayerInput.IsMoveInput ? k_Acceleration : k_Deceleration;

            m_ForwardSpeed = Mathf.MoveTowards(
                m_ForwardSpeed,
                m_DesiredForwardSpeed,
                Time.deltaTime * accekeration);

            m_Animator.SetFloat(m_HashForwardSpeed, m_ForwardSpeed);
        }

        private void ComputeRotation()
        {
            Vector3 moveInput = m_PlayerInput.MoveInput.normalized;
            Vector3 cameraDirection = Quaternion.Euler(
                0,
                m_CameraController.PlayerCam.m_XAxis.Value,
                0
                ) * Vector3.forward;

            Quaternion targetRotation;

            if (Mathf.Approximately(Vector3.Dot(moveInput, Vector3.forward), -1.0f))
            {
                targetRotation = Quaternion.LookRotation(-cameraDirection);
            }
            else
            {
                Quaternion movementRotation = Quaternion.FromToRotation(Vector3.forward, moveInput);
                targetRotation = Quaternion.LookRotation(movementRotation * cameraDirection);
            }

            m_TargetRotation = targetRotation;
        }
    }
}
