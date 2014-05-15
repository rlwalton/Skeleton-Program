using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using Microsoft.Kinect;


namespace ConsoleApplication6
{
    class Program
    {
        private KinectSensor sensor;
        private static Skeleton[] skeletons;
        public event EventHandler<SkeletonFrameReadyEventArgs> SkeletonFrameReady;

        static void Main(string[] args)
        {
            getSensors();
            DateTime start = DateTime.Now;
            for (int i = 1; i < 10000000; i++)
            {
                if ((DateTime.Now - start).TotalSeconds >= 60)
                    break;
                System.Threading.Thread.Sleep(2000);

                Console.WriteLine("Position? " + getPos()); 
                PrintJoints(skeletons);

            }
        }

        public static void getSensors()
        {
            KinectSensor sensor = null;
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    sensor = potentialSensor;
                    break;
                }
            }

            if (sensor != null)
            {
                // Turn on the skeleton stream to receive skeleton frames
                sensor.SkeletonStream.Enable();

                // Add an event handler to be called whenever there is new color frame data

                sensor.SkeletonFrameReady += SensorSkeletonFrameReady;

                // Start the sensor!
                try
                {
                    sensor.Start();
                }
                catch (IOException)
                {
                    sensor = null;
                }
               
            }
        }

        public static void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {

            skeletons = new Skeleton[0];
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }
            }
           // PrintJoints(skeletons);
        }



        //for @param joint, prints out joint type and its position
        private static void PrintJoint(Joint joint)
        {
            //If we can't find join or point is inferred, don't print
            if (joint.TrackingState == JointTrackingState.NotTracked ||
                        joint.TrackingState == JointTrackingState.Inferred)
                return;
            //print joint type and position
            Console.WriteLine(joint.JointType + " ---> " + "( " + joint.Position.X + ", " + joint.Position.Y + ", " + joint.Position.Z + " )" );
        } //end PrintJoint


        //prints out all joints of each skeleton in @param skeletons
        private static void PrintJoints(Skeleton[] skeletons)
        {
            //attempts to print out all joints detected from each skeleton in Skeleton[] skeletons
            foreach (Skeleton skeleton in skeletons)
            {
               // PrintJoint(skeleton.Joints[JointType.AnkleLeft]);
               // PrintJoint(skeleton.Joints[JointType.AnkleRight]);

                PrintJoint(skeleton.Joints[JointType.ElbowLeft]);
                PrintJoint(skeleton.Joints[JointType.ElbowRight]);

               // PrintJoint(skeleton.Joints[JointType.FootLeft]);
              //  PrintJoint(skeleton.Joints[JointType.FootRight]);

                PrintJoint(skeleton.Joints[JointType.HandLeft]);
                PrintJoint(skeleton.Joints[JointType.HandRight]);

                PrintJoint(skeleton.Joints[JointType.Head]);

               // PrintJoint(skeleton.Joints[JointType.HipCenter]);
               // PrintJoint(skeleton.Joints[JointType.HipLeft]);
               // PrintJoint(skeleton.Joints[JointType.HipRight]);

                //PrintJoint(skeleton.Joints[JointType.KneeLeft]);
                //PrintJoint(skeleton.Joints[JointType.KneeRight]);

                PrintJoint(skeleton.Joints[JointType.ShoulderCenter]);
                PrintJoint(skeleton.Joints[JointType.ShoulderLeft]);
                PrintJoint(skeleton.Joints[JointType.ShoulderRight]);

                PrintJoint(skeleton.Joints[JointType.Spine]);

                PrintJoint(skeleton.Joints[JointType.WristLeft]);
                PrintJoint(skeleton.Joints[JointType.WristRight]);
                Console.WriteLine();
            }//end foreach
        } //end PrintJoints

        static int getPos()
        {
            Skeleton skeleton = skeletons[0];
            //obtain all coordinates of right shoulder position
            float shoulderRightX = skeleton.Joints[JointType.ShoulderRight].Position.X;
            float shoulderRightY = skeleton.Joints[JointType.ShoulderRight].Position.Y;
            float shoulderRightZ = skeleton.Joints[JointType.ShoulderRight].Position.Z;
            //obtain all coordinates of left shoulder position
            float shoulderLeftX = skeleton.Joints[JointType.ShoulderLeft].Position.X;
            float shoulderLeftY = skeleton.Joints[JointType.ShoulderLeft].Position.Y;
            float shoulderLeftZ = skeleton.Joints[JointType.ShoulderLeft].Position.Z;
            //obtain all coordinates of right elbow position
            float elbowRightX = skeleton.Joints[JointType.ElbowRight].Position.X;
            float elbowRightY = skeleton.Joints[JointType.ElbowRight].Position.Y;
            float elbowRightZ = skeleton.Joints[JointType.ElbowRight].Position.Z;
            //obtain all coordinates of left elbow position
            float elbowLeftX = skeleton.Joints[JointType.ElbowLeft].Position.X;
            float elbowLeftY = skeleton.Joints[JointType.ElbowLeft].Position.Y;
            float elbowLeftZ = skeleton.Joints[JointType.ElbowLeft].Position.Z;
            //obtain all coordinates of right wrist position
            float wristRightX = skeleton.Joints[JointType.WristRight].Position.X;
            float wristRightY = skeleton.Joints[JointType.WristRight].Position.Y;
            float wristRightZ = skeleton.Joints[JointType.WristRight].Position.Z;
            //obtain all coordinates of left wrist position
            float wristLeftX = skeleton.Joints[JointType.WristLeft].Position.X;
            float wristLeftY = skeleton.Joints[JointType.WristLeft].Position.Y;
            float wristLeftZ = skeleton.Joints[JointType.WristLeft].Position.Z;
            //obtain all coordinates of head position
            float headX = skeleton.Joints[JointType.Head].Position.X;
            float headY = skeleton.Joints[JointType.Head].Position.Y;
            float headZ = skeleton.Joints[JointType.Head].Position.Z;

            int result = 0;

            if (shoulderRightX == shoulderLeftX)
                result = 0;
            //test for postion 1: arms straight out from sides and level with shoulder
            //return 1 if position is held
            else if (compareRange(shoulderLeftY, elbowLeftY, wristLeftY) && compareRange(shoulderRightY, elbowRightY, wristRightY)
                && compareRange(shoulderLeftZ, elbowLeftZ, wristLeftZ) && compareRange(shoulderRightZ, elbowRightZ, wristRightZ))
                result = 1;
            //test for position 2: elbow angle is 90 degrees, upper arms alligned with shoulders, hands are at head level
            //return 2 if position is held
            else if (compareRange(elbowLeftX, wristLeftX) && compareRange(elbowRightX, wristRightX)
                && compareRange(shoulderLeftY, elbowLeftY) && compareRange(shoulderRightY, elbowRightY)
                && compareRange(wristRightZ, elbowRightZ, shoulderRightZ) && compareRange(wristLeftZ, elbowLeftZ, shoulderLeftZ)
                && (wristRightY - elbowRightY) > 0)
                result = 2;
            //test for position 3: arms held straight up
            //return 3 if position is held
            else if (compareRange(elbowLeftX, wristLeftX, shoulderLeftX) && compareRange(elbowRightX, wristRightX, shoulderRightX)
                && (wristLeftY > headY))
                result = 3;
            //test for position 4: elbow angle is 90 degrees, upper arms alligned with shoulders, hands are at waist level
            //return 4 if position is held
            else if (compareRange(elbowLeftX, wristLeftX) && compareRange(elbowRightX, wristRightX)
               && compareRange(shoulderLeftY, elbowLeftY) && compareRange(shoulderRightY, elbowRightY)
               && compareRange(wristRightZ, elbowRightZ, shoulderRightZ) && compareRange(wristLeftZ, elbowLeftZ, shoulderLeftZ)
               && (wristRightY - elbowRightY) < 0)
                result = 4;

            //test for position 5: arms are straight and down at sides
            else if (compareRange(elbowLeftX, wristLeftX, shoulderLeftX) && compareRange(elbowRightX, wristRightX, shoulderRightX)
                && (wristLeftY < shoulderLeftY))
                result = 5;

            return result;
          
        }

        static bool compareRange( float pos1, float pos2 , float pos3 ) {
            //if all three positions are within range, returns true
            return (compareRange(pos1, pos2) && compareRange(pos2, pos3) && compareRange(pos3, pos1));
            
        }

        static bool compareRange(float pos1, float pos2)
        {
            //if positions are within a range of .07, returns true
            // if (pos1 == pos2 && pos1 == 0)
            //    return false;
            if ((pos1 <= pos2 + .1) && (pos1 >= pos2 - .1))
                return true;
            else return false;
        }   
    }
}


