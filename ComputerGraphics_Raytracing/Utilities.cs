using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

namespace ComputerGraphics_Raytracing
{
    public class Camera
    {
        public Vector3 position;
        public Vector3 view;
        public Vector3 up;
        public Vector3 right;
        public Vector2 scale;
    }

    public static class InitializeDefaultScene
    {
        public static readonly int DEFAULT = 1;
        public static readonly int LIGHT = 2;
        public static readonly int GLASS = 3;

        public static void InitializeCamera(Shaders shaders, Camera camera)
        {
            shaders.Uniform3("uCamera.position", camera.position);
            shaders.Uniform3("uCamera.view", camera.view);
            shaders.Uniform3("uCamera.up", camera.up);
            shaders.Uniform3("uCamera.right", camera.right);
            shaders.Uniform2("uCamera.scale", camera.scale);
        }

        public static void InitializeScene(Shaders shaders)
        {
            // TRIANGLES (clockwise order of vectors!!!)
            shaders.Uniform1("triangles_used", 12);

            // left wall: triangles 0, 1
            shaders.Uniform3("triangles[0].v1", -5.0f, -5.0f, -5.0f);
            shaders.Uniform3("triangles[0].v2", -5.0f, 5.0f, -5.0f);
            shaders.Uniform3("triangles[0].v3", -5.0f, 5.0f, 5.0f);
            shaders.Uniform1("triangles[0].MaterialId", 1);

            shaders.Uniform3("triangles[1].v1", -5.0f, -5.0f, -5.0f);
            shaders.Uniform3("triangles[1].v2", -5.0f, 5.0f, 5.0f);
            shaders.Uniform3("triangles[1].v3", -5.0f, -5.0f, 5.0f);
            shaders.Uniform1("triangles[1].MaterialId", 1);

            // right wall: triangles 2, 3
            shaders.Uniform3("triangles[2].v1", 5.0f, -5.0f, 5.0f);
            shaders.Uniform3("triangles[2].v2", 5.0f, 5.0f, 5.0f);
            shaders.Uniform3("triangles[2].v3", 5.0f, 5.0f, -5.0f);
            shaders.Uniform1("triangles[2].MaterialId", 3);

            shaders.Uniform3("triangles[3].v1", 5.0f, -5.0f, 5.0f);
            shaders.Uniform3("triangles[3].v2", 5.0f, 5.0f, -5.0f);
            shaders.Uniform3("triangles[3].v3", 5.0f, -5.0f, -5.0f);
            shaders.Uniform1("triangles[3].MaterialId", 3);

            // down wall: triangles 4, 5
            shaders.Uniform3("triangles[4].v1", 5.0f, -5.0f, -5.0f);
            shaders.Uniform3("triangles[4].v2", -5.0f, -5.0f, -5.0f);
            shaders.Uniform3("triangles[4].v3", -5.0f, -5.0f, 5.0f);
            shaders.Uniform1("triangles[4].MaterialId", 0);

            shaders.Uniform3("triangles[5].v1", 5.0f, -5.0f, -5.0f);
            shaders.Uniform3("triangles[5].v2", -5.0f, -5.0f, 5.0f);
            shaders.Uniform3("triangles[5].v3", 5.0f, -5.0f, 5.0f);
            shaders.Uniform1("triangles[5].MaterialId", 0);

            // up wall: triangles 6, 7
            shaders.Uniform3("triangles[6].v1", -5.0f, 5.0f, -5.0f);
            shaders.Uniform3("triangles[6].v2", 5.0f, 5.0f, -5.0f);
            shaders.Uniform3("triangles[6].v3", 5.0f, 5.0f, 5.0f);
            shaders.Uniform1("triangles[6].MaterialId", 0);

            shaders.Uniform3("triangles[7].v1", -5.0f, 5.0f, -5.0f);
            shaders.Uniform3("triangles[7].v2", 5.0f, 5.0f, 5.0f);
            shaders.Uniform3("triangles[7].v3", -5.0f, 5.0f, 5.0f);
            shaders.Uniform1("triangles[7].MaterialId", 0);

            // back wall: triangles 8, 9
            shaders.Uniform3("triangles[8].v1", -5.0f, -5.0f, 5.0f);
            shaders.Uniform3("triangles[8].v2", -5.0f, 5.0f, 5.0f);
            shaders.Uniform3("triangles[8].v3", 5.0f, 5.0f, 5.0f);
            shaders.Uniform1("triangles[8].MaterialId", 2);

            shaders.Uniform3("triangles[9].v1", -5.0f, -5.0f, 5.0f);
            shaders.Uniform3("triangles[9].v2", 5.0f, 5.0f, 5.0f);
            shaders.Uniform3("triangles[9].v3", 5.0f, -5.0f, 5.0f);
            shaders.Uniform1("triangles[9].MaterialId", 2);

            // front wall: triangles 10, 11
            shaders.Uniform3("triangles[10].v1", 5.0f, -5.0f, -5.0f);
            shaders.Uniform3("triangles[10].v2", -5.0f, 5.0f, -5.0f);
            shaders.Uniform3("triangles[10].v3", -5.0f, -5.0f, -5.0f);
            shaders.Uniform1("triangles[10].MaterialId", 0);

            shaders.Uniform3("triangles[11].v1", 5.0f, -5.0f, -5.0f);
            shaders.Uniform3("triangles[11].v2", 5.0f, 5.0f, -5.0f);
            shaders.Uniform3("triangles[11].v3", -5.0f, 5.0f, -5.0f);
            shaders.Uniform1("triangles[11].MaterialId", 0);

            // SPHERES
            shaders.Uniform1("spheres_used", 3);

            // big sphere: sphere 0
            shaders.Uniform3("spheres[0].center", -1.0f, -1.0f, -2.0f);
            shaders.Uniform1("spheres[0].radius", 2.0f);
            shaders.Uniform1("spheres[0].MaterialId", 4);

            // small sphere: sphere 1
            shaders.Uniform3("spheres[1].center", 2.0f, 1.0f, 2.0f);
            shaders.Uniform1("spheres[1].radius", 1.0f);
            shaders.Uniform1("spheres[1].MaterialId", 6);

            // light sphere: sphere 2
            shaders.Uniform3("spheres[2].center", 0.0f, 2.0f, -4.0f);
            shaders.Uniform1("spheres[2].radius", 0.2f);
            shaders.Uniform1("spheres[2].MaterialId", 5);
        }

        public static void InitializeLight(Shaders shaders)
        {
            shaders.Uniform3("uLight.position", 0.0f, 2.0f, -4.0f);
        }

        public static void InitializeMaterials(Shaders shaders)
        {
            // 0: Gray wall
            shaders.Uniform3("materials[0].color", 0.5f, 0.5f, 0.5f);
            shaders.Uniform4("materials[0].lightCoeffs", 0.4f, 0.9f, 0.6f, 10.0f);
            shaders.Uniform1("materials[0].reflectionCoef", 0.0f);
            shaders.Uniform1("materials[0].refractionCoef", 0.0f);
            shaders.Uniform1("materials[0].refractionIndex", 0.0f);
            shaders.Uniform1("materials[0].MaterialType", DEFAULT);

            // 1: Red wall
            shaders.Uniform3("materials[1].color", 1.0f, 0.0f, 0.0f);
            shaders.Uniform4("materials[1].lightCoeffs", 0.4f, 0.9f, 0.6f, 10.0f);
            shaders.Uniform1("materials[1].reflectionCoef", 0.5f);
            shaders.Uniform1("materials[1].refractionCoef", 0.0f);
            shaders.Uniform1("materials[1].refractionIndex", 0.0f);
            shaders.Uniform1("materials[1].MaterialType", DEFAULT);

            // 2: Cyan wall
            shaders.Uniform3("materials[2].color", 0.0f, 1.0f, 1.0f);
            shaders.Uniform4("materials[2].lightCoeffs", 0.4f, 0.9f, 0.6f, 10.0f);
            shaders.Uniform1("materials[2].reflectionCoef", 0.0f);
            shaders.Uniform1("materials[2].refractionCoef", 0.0f);
            shaders.Uniform1("materials[2].refractionIndex", 0.0f);
            shaders.Uniform1("materials[2].MaterialType", DEFAULT);

            // 3: Green wall
            shaders.Uniform3("materials[3].color", 0.0f, 1.0f, 0.0f);
            shaders.Uniform4("materials[3].lightCoeffs", 0.4f, 0.9f, 0.6f, 10.0f);
            shaders.Uniform1("materials[3].reflectionCoef", 0.5f);
            shaders.Uniform1("materials[3].refractionCoef", 0.0f);
            shaders.Uniform1("materials[3].refractionIndex", 0.0f);
            shaders.Uniform1("materials[3].MaterialType", DEFAULT);

            // 4: Blue material
            shaders.Uniform3("materials[4].color", 0.0f, 0.0f, 1.0f);
            shaders.Uniform4("materials[4].lightCoeffs", 0.4f, 0.9f, 0.6f, 10.0f);
            shaders.Uniform1("materials[4].reflectionCoef", 0.1f);
            shaders.Uniform1("materials[4].refractionCoef", 0.0f);
            shaders.Uniform1("materials[4].refractionIndex", 0.0f);
            shaders.Uniform1("materials[4].MaterialType", DEFAULT);

            // 5: Light
            shaders.Uniform3("materials[5].color", 10.0f, 10.0f, 10.0f);
            shaders.Uniform4("materials[5].lightCoeffs", 0.0f, 0.0f, 0.0f, 0.0f);
            shaders.Uniform1("materials[5].reflectionCoef", 0.0f);
            shaders.Uniform1("materials[5].refractionCoef", 0.0f);
            shaders.Uniform1("materials[5].refractionIndex", 0.0f);
            shaders.Uniform1("materials[5].MaterialType", LIGHT);

            // 6: Glass
            shaders.Uniform3("materials[6].color", 1.0f, 1.0f, 1.0f);
            shaders.Uniform4("materials[6].lightCoeffs", 0.0f, 0.0f, 0.6f, 10.0f);
            shaders.Uniform1("materials[6].reflectionCoef", 0.02f);
            shaders.Uniform1("materials[6].refractionCoef", 0.98f);
            shaders.Uniform1("materials[6].refractionIndex", 1.47f);
            shaders.Uniform1("materials[6].MaterialType", GLASS);
        }
    }
}
