using System;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;

namespace Generating
{
    public class Camera
    {
        private static Camera instance;
        public Vector3 Eye;
        private Vector3 target;
        private Vector3 upVector;
        public float Facing { get; set; }
        public float Pitch { get; set; }
        private int prevMouseX;
        private int prevMouseY;
        internal Game scene { get; set; }

        public float MovementSpeed { get; set; }
        public float RotationSpeed { get; set; }
        public Matrix4 ModelView;
        public Matrix4 Normal;
        public Matrix4 Projection;

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
            target = Vector3.UnitZ;
            upVector = Vector3.UnitY;
            MovementSpeed = 5.5f;
            RotationSpeed = .001f;

            ModelView = Matrix4.LookAt(Eye, target, upVector);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref ModelView);
        }

        public void OnResize(int width, int height)
        {
            Projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4f, width / (float)height, 1.0f, 10000.0f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref Projection);
        }

        public void Translate(float x, float y, float z)
        {
            Eye.X += x;
            Eye.Y += y;
            Eye.Z += z;
        }

        public void UpdateView(MouseState mouse, KeyboardState keyboard)
        {
            if (mouse.X < prevMouseX)
            {
                Facing += RotationSpeed * (prevMouseX - mouse.X);
                prevMouseX = mouse.X;
            }
            else if (mouse.X > prevMouseX)
            {
                Facing -= RotationSpeed * (mouse.X - prevMouseX);
                prevMouseX = mouse.X;
            }
            if (mouse.Y > prevMouseY && Pitch >= -MathHelper.PiOver2)
            {
                Pitch -= RotationSpeed * (mouse.Y - prevMouseY);
                prevMouseY = mouse.Y;
            }
            else if (mouse.Y < prevMouseY && Pitch <= MathHelper.PiOver2)
            {
                Pitch += RotationSpeed * (prevMouseY - mouse.Y);
                prevMouseY = mouse.Y;
            }
            
            if (keyboard[Key.W])
                Translate(MovementSpeed * (float)Math.Sin(Facing), 0, MovementSpeed * (float)Math.Cos(Facing));
            if (keyboard[Key.S])
                Translate(-MovementSpeed * (float)Math.Sin(Facing), 0, -MovementSpeed * (float)Math.Cos(Facing));
            if (keyboard[Key.A])
                Translate(MovementSpeed * (float)Math.Sin(Facing + MathHelper.PiOver2), 0, MovementSpeed * (float)Math.Cos(Facing + MathHelper.PiOver2));
            if (keyboard[Key.D])
                Translate(-MovementSpeed * (float)Math.Sin(Facing + MathHelper.PiOver2), 0, -MovementSpeed * (float)Math.Cos(Facing + MathHelper.PiOver2));
            if (keyboard[Key.KeypadPlus])
            {
                scene.tiling += 0.1f;
                Console.WriteLine("tiling "+scene.tiling);
            }
            if (keyboard[Key.KeypadMinus])
            {
                scene.tiling -= 0.1f;
                Console.WriteLine("tiling "+scene.tiling);
            }
            if (keyboard[Key.Plus])
            {
                scene.wave += 0.0005f;
                Console.WriteLine("wave " + scene.wave);
            }
            if (keyboard[Key.Minus])
            {
                scene.wave -= 0.0005f;
                Console.WriteLine("wave " + scene.wave);
            }
            if (keyboard[Key.K])
            {
                scene.waterSpeed += 0.001f;
                Console.WriteLine("time " + scene.waterSpeed);
            }
            if (keyboard[Key.L])
            {
                scene.waterSpeed -= 0.001f;
                Console.WriteLine("time " + scene.waterSpeed);
            }
            if (keyboard[Key.LShift])
                Translate(0, MovementSpeed, 0);
            if (keyboard[Key.LControl])
                Translate(0, -MovementSpeed, 0);
            Update();
        }
        public void Update()
        {

            ModelView = Matrix4.LookAt(Eye, Eye + new Vector3((float)Math.Sin(Facing), (float)Math.Sin(Pitch), (float)Math.Cos(Facing)),
                                               upVector);
            Normal = Matrix4.Transpose(Matrix4.Invert(ModelView));

        }
    }
}
