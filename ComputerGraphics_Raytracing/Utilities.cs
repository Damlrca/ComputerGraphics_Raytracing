using System;
using OpenTK.Mathematics;

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

        private static void UniformTriangle(Shaders shaders, int id, Vector3 v1, Vector3 v2, Vector3 v3, int MaterialId)
        {
            shaders.Uniform3($"triangles[{id}].v1", v1);
            //shaders.Uniform3($"triangles[{id}].v2", v2);
            //shaders.Uniform3($"triangles[{id}].v3", v3);
            Vector3 norm = Vector3.Cross(v2 - v1, v3 - v1);
            norm.Normalize();
            shaders.Uniform3($"triangles[{id}].norm", norm);

            Matrix3 m = new Matrix3(v2 - v1, v3 - v1, norm);
            shaders.Uniform1($"triangles[{id}].d", m.Determinant);
            shaders.Uniform3($"triangles[{id}].sp1", m[1, 1] * m[2, 2] - m[1, 2] * m[2, 1],
                                                    -m[1, 0] * m[2, 2] + m[1, 2] * m[2, 0],
                                                     m[1, 0] * m[2, 1] - m[1, 1] * m[2, 0]);
            shaders.Uniform3($"triangles[{id}].sp2", -m[0, 1] * m[2, 2] + m[0, 2] * m[2, 1],
                                                      m[0, 0] * m[2, 2] - m[0, 2] * m[2, 0],
                                                     -m[0, 0] * m[2, 1] + m[0, 1] * m[2, 0]);
            shaders.Uniform1($"triangles[{id}].MaterialId", MaterialId);
        }

        private static void UniformPentagon(Shaders shaders, int id, Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, Vector3 v5, int MaterialId)
        {
            UniformTriangle(shaders, id, v1, v2, v3, MaterialId);
            UniformTriangle(shaders, id + 1, v1, v3, v4, MaterialId);
            UniformTriangle(shaders, id + 2, v1, v4, v5, MaterialId);
        }

        public static void InitializeScene(Shaders shaders)
        {
            Vector3[] Cube = new Vector3[]
            {
                new Vector3(-5.0f, -5.0f, -5.0f),
                new Vector3(-5.0f, -5.0f, 5.0f),
                new Vector3(-5.0f, 5.0f, -5.0f),
                new Vector3(-5.0f, 5.0f, 5.0f),
                new Vector3(5.0f, -5.0f, -5.0f),
                new Vector3(5.0f, -5.0f, 5.0f),
                new Vector3(5.0f, 5.0f, -5.0f),
                new Vector3(5.0f, 5.0f, 5.0f)
            };

            float phi = (float)((Math.Sqrt(5) + 1) / 2);
            float n = 4, x = 1.5f, y = -3.0f, z = -1.0f;
            float sinPi10 = (float)Math.Sin(Math.PI / 10);

            Vector3[] Dodecahedron = new Vector3[]
            {
                new Vector3(-(n / (phi + 1)) / 2 + x, y, -n / 2 + z),
                new Vector3((n / (phi + 1)) / 2 + x, y, -n / 2 + z),
                new Vector3((n / (phi + 1)) / 2 + x, y, n / 2 + z),
                new Vector3(-(n / (phi + 1)) / 2 + x, y, n / 2 + z),
                new Vector3(x, -n / 2 + y, (n / (phi + 1)) / 2 + z),
                new Vector3(x, -n / 2 + y, -(n / (phi + 1)) / 2 + z),
                new Vector3(n / 2 + x, -(n / (phi + 1)) / 2 + y, z),
                new Vector3(n / 2 + x, (n / (phi + 1)) / 2 + y, z),
                new Vector3(x, n / 2 + y, (n / (phi + 1)) / 2 + z),
                new Vector3(x, n / 2 + y, -(n / (phi + 1)) / 2 + z),
                new Vector3(-n / 2 + x, (n / (phi + 1)) / 2 + y, z),
                new Vector3(-n / 2 + x, -(n / (phi + 1)) / 2 + y, z),
                new Vector3(n * sinPi10 + x, -n * sinPi10 + y, -n * sinPi10 + z),
                new Vector3(-n * sinPi10 + x, -n * sinPi10 + y, -n * sinPi10 + z),
                new Vector3(-n * sinPi10 + x, n * sinPi10 + y, -n * sinPi10 + z),
                new Vector3(n * sinPi10 + x, n * sinPi10 + y, -n * sinPi10 + z),
                new Vector3(n * sinPi10 + x, -n * sinPi10 + y, n * sinPi10 + z),
                new Vector3(-n * sinPi10 + x, -n * sinPi10 + y, n * sinPi10 + z),
                new Vector3(-n * sinPi10 + x, n * sinPi10 + y, n * sinPi10 + z),
                new Vector3(n * sinPi10 + x, n * sinPi10 + y, n * sinPi10 + z)
            };

            // TRIANGLES (clockwise order of vectors!!!)
            shaders.Uniform1("triangles_used", 48);

            // CUBE
            // left wall: triangles 0, 1
            UniformTriangle(shaders, 0, Cube[0], Cube[2], Cube[3], 1);
            UniformTriangle(shaders, 1, Cube[0], Cube[3], Cube[1], 1);
            // right wall: triangles 2, 3
            UniformTriangle(shaders, 2, Cube[5], Cube[7], Cube[6], 3);
            UniformTriangle(shaders, 3, Cube[5], Cube[6], Cube[4], 3);
            // down wall: triangles 4, 5
            UniformTriangle(shaders, 4, Cube[4], Cube[0], Cube[1], 0);
            UniformTriangle(shaders, 5, Cube[4], Cube[1], Cube[5], 0);
            // up wall: triangles 6, 7
            UniformTriangle(shaders, 6, Cube[2], Cube[6], Cube[7], 0);
            UniformTriangle(shaders, 7, Cube[2], Cube[7], Cube[3], 0);
            // back wall: triangles 8, 9
            UniformTriangle(shaders, 8, Cube[1], Cube[3], Cube[7], 2);
            UniformTriangle(shaders, 9, Cube[1], Cube[7], Cube[5], 2);
            // front wall: triangles 10, 11
            UniformTriangle(shaders, 10, Cube[4], Cube[2], Cube[0], 0);
            UniformTriangle(shaders, 11, Cube[4], Cube[6], Cube[2], 0);

            // DODECAHEDRON
            // 1 Pentagon
            UniformPentagon(shaders, 12, Dodecahedron[0], Dodecahedron[1], Dodecahedron[12], Dodecahedron[5], Dodecahedron[13], 7);
            // 2 Pentagon
            UniformPentagon(shaders, 15, Dodecahedron[0], Dodecahedron[14], Dodecahedron[9], Dodecahedron[15], Dodecahedron[1], 7);
            // 3 Pentagon
            UniformPentagon(shaders, 18, Dodecahedron[2], Dodecahedron[19], Dodecahedron[8], Dodecahedron[18], Dodecahedron[3], 7);
            // 4 Pentagon
            UniformPentagon(shaders, 21, Dodecahedron[2], Dodecahedron[3], Dodecahedron[17], Dodecahedron[4], Dodecahedron[16], 7);
            // 5 Pentagon
            UniformPentagon(shaders, 24, Dodecahedron[4], Dodecahedron[5], Dodecahedron[12], Dodecahedron[6], Dodecahedron[16], 7);
            // 6 Pentagon
            UniformPentagon(shaders, 27, Dodecahedron[4], Dodecahedron[17], Dodecahedron[11], Dodecahedron[13], Dodecahedron[5], 7);
            // 7 Pentagon
            UniformPentagon(shaders, 30, Dodecahedron[6], Dodecahedron[7], Dodecahedron[19], Dodecahedron[2], Dodecahedron[16], 7);
            // 8 Pentagon
            UniformPentagon(shaders, 33, Dodecahedron[6], Dodecahedron[12], Dodecahedron[1], Dodecahedron[15], Dodecahedron[7], 7);
            // 9 Pentagon
            UniformPentagon(shaders, 36, Dodecahedron[8], Dodecahedron[19], Dodecahedron[7], Dodecahedron[15], Dodecahedron[9], 7);
            // 10 Pentagon
            UniformPentagon(shaders, 39, Dodecahedron[8], Dodecahedron[9], Dodecahedron[14], Dodecahedron[10], Dodecahedron[18], 7);
            // 11 Pentagon
            UniformPentagon(shaders, 42, Dodecahedron[10], Dodecahedron[11], Dodecahedron[17], Dodecahedron[3], Dodecahedron[18], 7);
            // 12 Pentagon
            UniformPentagon(shaders, 45, Dodecahedron[10], Dodecahedron[14], Dodecahedron[0], Dodecahedron[13], Dodecahedron[11], 7);

            // SPHERES
            shaders.Uniform1("spheres_used", 3);

            // big sphere: sphere 0
            shaders.Uniform3("spheres[0].center", -2.5f, 0.25f, -2.5f);
            shaders.Uniform1("spheres[0].radius", 1.5f);
            shaders.Uniform1("spheres[0].MaterialId", 4);

            // small sphere: sphere 1
            shaders.Uniform3("spheres[1].center", 2.0f, 1.0f, 2.0f);
            shaders.Uniform1("spheres[1].radius", 1.0f);
            shaders.Uniform1("spheres[1].MaterialId", 6);

            // light sphere: sphere 2
            shaders.Uniform3("spheres[2].center", 0.5f, 3.0f, -4.0f);
            shaders.Uniform1("spheres[2].radius", 0.2f);
            shaders.Uniform1("spheres[2].MaterialId", 5);
        }

        public static void InitializeLight(Shaders shaders)
        {
            shaders.Uniform3("uLight.position", 0.5f, 3.0f, -4.0f);
        }

        public static void InitializeMaterials(Shaders shaders)
        {
            // 0: Gray wall
            shaders.Uniform3("materials[0].color", 0.75f, 0.75f, 0.75f);
            shaders.Uniform4("materials[0].lightCoeffs", 0.4f, 0.5f, 0.25f, 10.0f);
            shaders.Uniform1("materials[0].reflectionCoef", 0.0f);
            shaders.Uniform1("materials[0].refractionCoef", 0.0f);
            shaders.Uniform1("materials[0].refractionIndex", 1.0f);
            shaders.Uniform1("materials[0].MaterialType", DEFAULT);

            // 1: Red wall
            shaders.Uniform3("materials[1].color", 1.0f, 0.0f, 0.0f);
            shaders.Uniform4("materials[1].lightCoeffs", 0.9f, 0.4f, 0.01f, 5.0f);
            shaders.Uniform1("materials[1].reflectionCoef", 0.5f);
            shaders.Uniform1("materials[1].refractionCoef", 0.0f);
            shaders.Uniform1("materials[1].refractionIndex", 1.0f);
            shaders.Uniform1("materials[1].MaterialType", DEFAULT);

            // 2: Cyan wall
            shaders.Uniform3("materials[2].color", 0.0f, 1.0f, 1.0f);
            shaders.Uniform4("materials[2].lightCoeffs", 0.4f, 0.9f, 0.6f, 7.0f);
            shaders.Uniform1("materials[2].reflectionCoef", 0.0f);
            shaders.Uniform1("materials[2].refractionCoef", 0.0f);
            shaders.Uniform1("materials[2].refractionIndex", 1.0f);
            shaders.Uniform1("materials[2].MaterialType", DEFAULT);

            // 3: Green wall
            shaders.Uniform3("materials[3].color", 0.0f, 1.0f, 0.0f);
            shaders.Uniform4("materials[3].lightCoeffs", 0.9f, 0.4f, 0.01f, 5.0f);
            shaders.Uniform1("materials[3].reflectionCoef", 0.5f);
            shaders.Uniform1("materials[3].refractionCoef", 0.0f);
            shaders.Uniform1("materials[3].refractionIndex", 1.0f);
            shaders.Uniform1("materials[3].MaterialType", DEFAULT);

            // 4: Blue material
            shaders.Uniform3("materials[4].color", 0.0f, 0.0f, 1.0f);
            shaders.Uniform4("materials[4].lightCoeffs", 0.5f, 0.8f, 0.4f, 5.0f);
            shaders.Uniform1("materials[4].reflectionCoef", 0.05f);
            shaders.Uniform1("materials[4].refractionCoef", 0.0f);
            shaders.Uniform1("materials[4].refractionIndex", 1.0f);
            shaders.Uniform1("materials[4].MaterialType", DEFAULT);

            // 5: Light
            shaders.Uniform3("materials[5].color", 10.0f, 10.0f, 10.0f);
            shaders.Uniform4("materials[5].lightCoeffs", 0.0f, 0.0f, 0.0f, 0.0f);
            shaders.Uniform1("materials[5].reflectionCoef", 0.0f);
            shaders.Uniform1("materials[5].refractionCoef", 0.0f);
            shaders.Uniform1("materials[5].refractionIndex", 1.0f);
            shaders.Uniform1("materials[5].MaterialType", LIGHT);

            // 6: Glass
            shaders.Uniform3("materials[6].color", 1.0f, 1.0f, 1.0f);
            shaders.Uniform4("materials[6].lightCoeffs", 0.0f, 0.0f, 0.4f, 7.5f);
            shaders.Uniform1("materials[6].reflectionCoef", 0.05f);
            shaders.Uniform1("materials[6].refractionCoef", 0.90f);
            shaders.Uniform1("materials[6].refractionIndex", 1.47f);
            shaders.Uniform1("materials[6].MaterialType", GLASS);

            // 7: Purple material
            shaders.Uniform3("materials[7].color", 0.53f, 0.05f, 0.95f);
            shaders.Uniform4("materials[7].lightCoeffs", 0.35f, 0.7f, 0.4f, 20.0f);
            shaders.Uniform1("materials[7].reflectionCoef", 0.4f);
            shaders.Uniform1("materials[7].refractionCoef", 0.0f);
            shaders.Uniform1("materials[7].refractionIndex", 1.0f);
            shaders.Uniform1("materials[7].MaterialType", DEFAULT);
        }
    }
}
