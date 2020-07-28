/* 
    ------------------- Code Monkey -------------------
    
    Thank you for downloading the Code Monkey Utilities
    I hope you find them useful in your projects
    If you have any questions use the contact form
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */
 
using System;
using System.Collections.Generic;
using UnityEngine;

    public class CameraFollow : MonoBehaviour {

        private Func<Vector3> GetCameraFollowPositionFunc;

        public void Setup(Func<Vector3> GetCameraFollowPositionFunc, Func<float> GetCameraZoomFunc, bool teleportToFollowPosition, bool instantZoom) {
            this.GetCameraFollowPositionFunc = GetCameraFollowPositionFunc;
           

            if (teleportToFollowPosition) {
                Vector3 cameraFollowPosition = GetCameraFollowPositionFunc();
                cameraFollowPosition.z = transform.position.z;
            transform.position = new Vector3(Mathf.Clamp(cameraFollowPosition.x, -5.5f, 5.5f), Mathf.Clamp(cameraFollowPosition.y, -8f,8f), transform.position.z);

        }

          
        }

       

        public void SetCameraFollowPosition(Vector3 cameraFollowPosition) {
            SetGetCameraFollowPositionFunc(() => cameraFollowPosition);
        }

        public void SetGetCameraFollowPositionFunc(Func<Vector3> GetCameraFollowPositionFunc) {
            this.GetCameraFollowPositionFunc = GetCameraFollowPositionFunc;
        }

      
 

        private void Update() {
            HandleMovement();
        }

        private void HandleMovement() {
            if (GetCameraFollowPositionFunc == null) return;
            Vector3 cameraFollowPosition = GetCameraFollowPositionFunc();
            cameraFollowPosition.z = transform.position.z;

            Vector3 cameraMoveDir = (cameraFollowPosition - transform.position).normalized;
            float distance = Vector3.Distance(cameraFollowPosition, transform.position);
            float cameraMoveSpeed = 3f;

            if (distance > 0) {
            Vector3 newCameraPosition = transform.position + cameraMoveDir * distance * cameraMoveSpeed * Time.deltaTime;

            float distanceAfterMoving = Vector3.Distance(newCameraPosition, cameraFollowPosition);

                if (distanceAfterMoving > distance) {
                    // Overshot the target
                    newCameraPosition = cameraFollowPosition;
                }

            transform.position = transform.position = new Vector3(Mathf.Clamp(newCameraPosition.x, -5.5f, 5.5f), Mathf.Clamp(newCameraPosition.y, -8f, 8f), transform.position.z);
           
        }
        }

      
    }

