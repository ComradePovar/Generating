using System;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;

namespace Generating
{
    public class Camera
    {
        private static Camera instance;
        private Vector3 eye;
        private Vector3 target;
        private Vector3 upVector;
        private float facing;
        private float pitch;

        public Vector3 Eye
        {
            get
            {
                return eye;
            }
            set
            {
                eye = value;
            }
        }
        public Vector3 Target
        {
            get
            {
                return target;
            }
            set
            {
                target = value;
            }
        }
        public Vector3 UpVector
        {
            get
            {
                return upVector;
            }
            set
            {
                upVector = value;
            }
        }
        public float MovementSpeed { get; set; }
        public float RotationSpeed { get; set; }
        public static Camera Instance
        {
            get
            {
                if (instance == null)
                    instance = new Camera();
                return instance;
            }
        }

        private Camera()
        {
            Eye = Vector3.Zero;
            Target = Vector3.UnitZ;
            UpVector = Vector3.UnitY;
            MovementSpeed = 1.0f;
            RotationSpeed = 0.1f;

            Matrix4 modelView = Matrix4.LookAt(Eye, Target, UpVector);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelView);
        }

        public void Translate(float x, float y, float z)
        {
            eye.X += x;
            eye.Y += y;
            eye.Z += z;
        }

        public void UpdateView(MouseState mouse, KeyboardState keyboard)
        {
            if (mouse[MouseButton.Left])
            {
                if (keyboard[Key.W] && pitch < MathHelper.PiOver2)
                    pitch += RotationSpeed;
                if (keyboard[Key.S] && pitch > -MathHelper.PiOver2)
                    pitch -= RotationSpeed;
                if (keyboard[Key.A])
                    facing += RotationSpeed;
                if (keyboard[Key.D])
                    facing -= RotationSpeed;
            }
            else
            {
                if (keyboard[Key.W])
                    Translate(MovementSpeed * (float)Math.Sin(facing), 0, MovementSpeed * (float)Math.Cos(facing));
                if (keyboard[Key.S])
                    Translate(-MovementSpeed * (float)Math.Sin(facing), 0, -MovementSpeed * (float)Math.Cos(facing));
                if (keyboard[Key.A])
                    Translate(MovementSpeed * (float)Math.Sin(facing + MathHelper.PiOver2), 0, MovementSpeed * (float)Math.Cos(facing + MathHelper.PiOver2));
                if (keyboard[Key.D])
                    Translate(-MovementSpeed * (float)Math.Sin(facing + MathHelper.PiOver2), 0, -MovementSpeed * (float)Math.Cos(facing + MathHelper.PiOver2));
            }
            if (keyboard[Key.LShift])
                Translate(0, MovementSpeed, 0);
            if (keyboard[Key.LControl])
                Translate(0, -MovementSpeed, 0);

            Matrix4 modelView = Matrix4.LookAt(Eye, Eye + new Vector3((float)Math.Sin(facing), (float)Math.Sin(pitch), (float)Math.Cos(facing)),
                                               upVector);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelView);
        }
    }
}
