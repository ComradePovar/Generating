using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace Generating
{
    public class View
    {
        public Vector3 position;
        /// <summary>
        /// In radians, += clockwise
        /// </summary>
        public float rotation;
        /// <summary>
        /// 1 - no zoom
        /// 2 - 2x zoom
        /// </summary>
        public float zoom;

        public View(Vector3 startPosition, float startZoom = 1.0f, float startRotation = 0.0f)
        {
            this.position = startPosition;
            this.rotation = startRotation;
            this.zoom = startZoom;
        }

        public void Update()
        {

        }

        public void Transform()
        {
            Matrix4 transform = Matrix4.Identity;

            transform = Matrix4.Mult(transform, Matrix4.CreateTranslation(position.X, position.Y, position.Z));
            transform = Matrix4.Mult(transform, Matrix4.CreateRotationZ(rotation));
            transform = Matrix4.Mult(transform, Matrix4.CreateScale(zoom, zoom, 1.0f));

            GL.MultMatrix(ref transform);
        }
    }
}
