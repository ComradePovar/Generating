using OpenTK;

namespace Generating
{
    public class SceneParameters
    {
        public class TerrainParameters
        {
            public int Size { get; set; }
            public float Scale { get; set; }

            public int WindowWidth { get; set; }
            public int WindowHeight { get; set; }


            public float TerrainGenerationMin { get; set; }
            public float TerrainGenerationMax { get; set; }
            public float Roughness { get; set; }
            

            public Vector3 LightPosition { get; set; }
            public Vector3 LightColor { get; set; }
            public float AmbientIntensity { get; set; }
            public float SpecularIntensity { get; set; }
            public float SpecularPower { get; set; }

            public Vector4 FogColor { get; set; }
            public float FogDensity { get; set; }
            public float FogStart { get; set; }
            public float FogEnd { get; set; }
            public SceneObjects.FogType FogType { get; set; }

            public Vector3 WaterLightColor { get; set; }
            public float WaterSpecularIntensity { get; set; }
            public float WaterSpecularPower { get; set; }
            public float WaterWaveStrength { get; set; }
            public float WaterSpeed { get; set; }
        }

        public class CameraParameters
        {
            public Vector3 Eye { get; set; }
            public Vector3 Target { get; set; }
            public Vector3 UpVector { get; set; }
            public float MovementSpeed { get; set; }
            public float RotationSpeed { get; set; }
        }

        public TerrainParameters TerrainArgs { get; set; }
        public CameraParameters CameraArgs { get; set; }

        public SceneParameters()
        {
            TerrainArgs = new TerrainParameters();
            CameraArgs = new CameraParameters();
        }
    }
}