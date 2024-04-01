using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace BossBehaviorMaker.Prefabs.Character
{
    [Serializable]
    public struct InputKeyCodes
    {
        [field:SerializeField] public KeyCode LeftInput { get; private set; }
        [field:SerializeField] public KeyCode RightInput { get; private set; }
        [field:SerializeField] public KeyCode UpInput { get; private set; }
        [field:SerializeField] public KeyCode DownInput { get; private set; }
    }
    
    public class TopDownCharacterController : MonoBehaviour
    {
        [SerializeField] private InputKeyCodes _inputs;
        [SerializeField] private Camera _camera;
        [SerializeField] private Animator _animator;
        [SerializeField] private float _characterSpeed = 0.1f;
        
        private void Update()
        {
            Move(GetInputs());
        }

        private Vector2 GetInputs()
        {
            Vector2 inputVector = new Vector2(0, 0);
            if (Input.GetKey(_inputs.LeftInput)) inputVector += new Vector2Int(-1, 0);
            if (Input.GetKey(_inputs.RightInput)) inputVector += new Vector2Int(1, 0);
            if (Input.GetKey(_inputs.DownInput)) inputVector += new Vector2Int(0, -1);
            if (Input.GetKey(_inputs.UpInput)) inputVector += new Vector2Int(0, 1);
            return inputVector;
        }

        private void Move(Vector2 inputVector)
        {
            //get the normalized world vector
            Vector2 inputVectorNormalized = inputVector.normalized;
            Vector3 worldVector = new Vector3(inputVectorNormalized.x, 0, inputVectorNormalized.y) * _characterSpeed;

            //transform its direction depending on the camera
            Vector3 baseCameraRotation = _camera.transform.rotation.eulerAngles;
            Vector3 baseCameraPosition = _camera.transform.position;
            _camera.transform.position = new Vector3(baseCameraPosition.x, transform.position.y, baseCameraPosition.z);
            _camera.transform.rotation = Quaternion.Euler(0,baseCameraRotation.y,baseCameraRotation.z);
            worldVector = _camera.transform.TransformDirection(worldVector);
            _camera.transform.position = baseCameraPosition;
            _camera.transform.rotation = Quaternion.Euler(baseCameraRotation);

            //apply it to the position and make the character looks where he is going
            transform.position += worldVector;
            transform.LookAt(transform.position + worldVector);

            //animate
            _animator.SetBool("IsWalking", inputVector.magnitude > 0.1f);
        }
    }
}
