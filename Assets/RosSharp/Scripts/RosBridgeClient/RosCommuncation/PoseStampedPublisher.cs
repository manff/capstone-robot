/*
© Siemens AG, 2017-2018
Author: Dr. Martin Bischoff (martin.bischoff@siemens.com)

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at
<http://www.apache.org/licenses/LICENSE-2.0>.
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

// Added allocation free alternatives
// UoK , 2019, Odysseas Doumas (od79@kent.ac.uk / odydoum@gmail.com)

using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class PoseStampedPublisher : UnityPublisher<MessageTypes.Geometry.TwistStamped>
    {
        public Transform PublishedTransform;
        public string FrameId = "Unity";

        public HandSync handSync;

        private float lastXPos;
        private float lastYPos;
        private float lastZPos;
        private float lastXAng;
        private float lastYAng;
        private float lastZAng;

        private float deltaX;
        private float deltaY;
        private float deltaZ;
        private float deltaXAng;
        private float deltaYAng;
        private float deltaZAng;

        private MessageTypes.Geometry.TwistStamped message;

        protected override void Start()
        {
            lastXPos = 0;
            base.Start();
            InitializeMessage();
        }

        private void FixedUpdate()
        {
            UpdateMessage();
        }

        private void InitializeMessage()
        {
            message = new MessageTypes.Geometry.TwistStamped
            {
                header = new MessageTypes.Std.Header()
                {
                    frame_id = FrameId
                }
            };
        }

        private void UpdateMessage()
        {
            message.header.Update();
            GetGeometryLinear(PublishedTransform.position.Unity2Ros(), message.twist.linear);
            GetGeometryQuaternion(PublishedTransform.rotation.eulerAngles.Unity2Ros(), message.twist.angular);

            Publish(message);
        }

        private void GetGeometryLinear(Vector3 position, MessageTypes.Geometry.Vector3 geometryLinear)
        {
            if (handSync.rButtonHand)
            {
                deltaX = (position.y - lastYPos) / Time.deltaTime * 10;
                deltaY = -(position.x - lastXPos) / Time.deltaTime * 10;
                deltaZ = (position.z - lastZPos) / Time.deltaTime * 10;

                if (deltaX != 0)
                {
                    geometryLinear.x = deltaX;
                }
                if (deltaY != 0)
                {
                    geometryLinear.y = deltaY;
                }
                if (deltaZ != 0)
                {
                    geometryLinear.z = deltaZ;
                }

                lastXPos = position.x;
                lastYPos = position.y;
                lastZPos = position.z;
            } else
            {
                geometryLinear.x = 0;
                geometryLinear.y = 0;
                geometryLinear.z = 0;
            }
        }

        private void GetGeometryQuaternion(Vector3 eulerAngles, MessageTypes.Geometry.Vector3 geometryAngular)
        {
            if (handSync.rButtonHand)
            {
                deltaXAng = -(eulerAngles.y - lastYAng) / Time.deltaTime * (float)3.1415 / (float)180 * 10;
                deltaYAng = (eulerAngles.x - lastXAng) / Time.deltaTime * (float)3.1415 / (float)180 * 10;
                deltaZAng = -(eulerAngles.z - lastZAng) / Time.deltaTime * (float)3.1415 / (float)180 * 10;

                if (deltaXAng != 0)
                {
                    geometryAngular.x = deltaXAng;
                }
                if (deltaYAng != 0)
                {
                    geometryAngular.y = deltaYAng;
                }
                if (deltaZAng != 0)
                {
                    geometryAngular.z = deltaZAng;
                }

                print(deltaXAng);

                lastXAng = eulerAngles.x;
                lastYAng = eulerAngles.y;
                lastZAng = eulerAngles.z;
            } else
            {
                geometryAngular.x = 0;
                geometryAngular.y = 0;
                geometryAngular.z = 0;
            }
        }

    }
}
