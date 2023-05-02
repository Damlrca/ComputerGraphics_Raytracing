using System;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

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
            shaders.Uniform3("triangles[0].v1", Cube[0]);
            shaders.Uniform3("triangles[0].v2", Cube[2]);
            shaders.Uniform3("triangles[0].v3", Cube[3]);
            shaders.Uniform1("triangles[0].MaterialId", 1);

            shaders.Uniform3("triangles[1].v1", Cube[0]);
            shaders.Uniform3("triangles[1].v2", Cube[3]);
            shaders.Uniform3("triangles[1].v3", Cube[1]);
            shaders.Uniform1("triangles[1].MaterialId", 1);

            // right wall: triangles 2, 3
            shaders.Uniform3("triangles[2].v1", Cube[5]);
            shaders.Uniform3("triangles[2].v2", Cube[7]);
            shaders.Uniform3("triangles[2].v3", Cube[6]);
            shaders.Uniform1("triangles[2].MaterialId", 3);

            shaders.Uniform3("triangles[3].v1", Cube[5]);
            shaders.Uniform3("triangles[3].v2", Cube[6]);
            shaders.Uniform3("triangles[3].v3", Cube[4]);
            shaders.Uniform1("triangles[3].MaterialId", 3);

            // down wall: triangles 4, 5
            shaders.Uniform3("triangles[4].v1", Cube[4]);
            shaders.Uniform3("triangles[4].v2", Cube[0]);
            shaders.Uniform3("triangles[4].v3", Cube[1]);
            shaders.Uniform1("triangles[4].MaterialId", 0);

            shaders.Uniform3("triangles[5].v1", Cube[4]);
            shaders.Uniform3("triangles[5].v2", Cube[1]);
            shaders.Uniform3("triangles[5].v3", Cube[5]);
            shaders.Uniform1("triangles[5].MaterialId", 0);

            // up wall: triangles 6, 7
            shaders.Uniform3("triangles[6].v1", Cube[2]);
            shaders.Uniform3("triangles[6].v2", Cube[6]);
            shaders.Uniform3("triangles[6].v3", Cube[7]);
            shaders.Uniform1("triangles[6].MaterialId", 0);

            shaders.Uniform3("triangles[7].v1", Cube[2]);
            shaders.Uniform3("triangles[7].v2", Cube[7]);
            shaders.Uniform3("triangles[7].v3", Cube[3]);
            shaders.Uniform1("triangles[7].MaterialId", 0);

            // back wall: triangles 8, 9
            shaders.Uniform3("triangles[8].v1", Cube[1]);
            shaders.Uniform3("triangles[8].v2", Cube[3]);
            shaders.Uniform3("triangles[8].v3", Cube[7]);
            shaders.Uniform1("triangles[8].MaterialId", 2);

            shaders.Uniform3("triangles[9].v1", Cube[1]);
            shaders.Uniform3("triangles[9].v2", Cube[7]);
            shaders.Uniform3("triangles[9].v3", Cube[5]);
            shaders.Uniform1("triangles[9].MaterialId", 2);

            // front wall: triangles 10, 11
            shaders.Uniform3("triangles[10].v1", Cube[4]);
            shaders.Uniform3("triangles[10].v2", Cube[2]);
            shaders.Uniform3("triangles[10].v3", Cube[0]);
            shaders.Uniform1("triangles[10].MaterialId", 0);

            shaders.Uniform3("triangles[11].v1", Cube[4]);
            shaders.Uniform3("triangles[11].v2", Cube[6]);
            shaders.Uniform3("triangles[11].v3", Cube[2]);
            shaders.Uniform1("triangles[11].MaterialId", 0);

            // DODECAHEDRON
            // 1 Pentagon
            shaders.Uniform3("triangles[12].v1", Dodecahedron[0]);
            shaders.Uniform3("triangles[12].v2", Dodecahedron[1]);
            shaders.Uniform3("triangles[12].v3", Dodecahedron[12]);
            shaders.Uniform1("triangles[12].MaterialId", 7);

            shaders.Uniform3("triangles[13].v1", Dodecahedron[0]);
            shaders.Uniform3("triangles[13].v2", Dodecahedron[12]);
            shaders.Uniform3("triangles[13].v3", Dodecahedron[5]);
            shaders.Uniform1("triangles[13].MaterialId", 7);

            shaders.Uniform3("triangles[14].v1", Dodecahedron[0]);
            shaders.Uniform3("triangles[14].v2", Dodecahedron[5]);
            shaders.Uniform3("triangles[14].v3", Dodecahedron[13]);
            shaders.Uniform1("triangles[14].MaterialId", 7);

            // 2 Pentagon
            shaders.Uniform3("triangles[15].v1", Dodecahedron[1]);
            shaders.Uniform3("triangles[15].v2", Dodecahedron[0]);
            shaders.Uniform3("triangles[15].v3", Dodecahedron[15]);
            shaders.Uniform1("triangles[15].MaterialId", 7);

            shaders.Uniform3("triangles[16].v1", Dodecahedron[15]);
            shaders.Uniform3("triangles[16].v2", Dodecahedron[0]);
            shaders.Uniform3("triangles[16].v3", Dodecahedron[9]);
            shaders.Uniform1("triangles[16].MaterialId", 7);

            shaders.Uniform3("triangles[17].v1", Dodecahedron[9]);
            shaders.Uniform3("triangles[17].v2", Dodecahedron[0]);
            shaders.Uniform3("triangles[17].v3", Dodecahedron[14]);
            shaders.Uniform1("triangles[17].MaterialId", 7);

            // 3 Pentagon
            shaders.Uniform3("triangles[18].v1", Dodecahedron[3]);
            shaders.Uniform3("triangles[18].v2", Dodecahedron[2]);
            shaders.Uniform3("triangles[18].v3", Dodecahedron[18]);
            shaders.Uniform1("triangles[18].MaterialId", 7);

            shaders.Uniform3("triangles[19].v1", Dodecahedron[18]);
            shaders.Uniform3("triangles[19].v2", Dodecahedron[2]);
            shaders.Uniform3("triangles[19].v3", Dodecahedron[8]);
            shaders.Uniform1("triangles[19].MaterialId", 7);

            shaders.Uniform3("triangles[20].v1", Dodecahedron[8]);
            shaders.Uniform3("triangles[20].v2", Dodecahedron[2]);
            shaders.Uniform3("triangles[20].v3", Dodecahedron[19]);
            shaders.Uniform1("triangles[20].MaterialId", 7);

            // 4 Pentagon
            shaders.Uniform3("triangles[21].v1", Dodecahedron[17]);
            shaders.Uniform3("triangles[21].v2", Dodecahedron[2]);
            shaders.Uniform3("triangles[21].v3", Dodecahedron[3]);
            shaders.Uniform1("triangles[21].MaterialId", 7);

            shaders.Uniform3("triangles[22].v1", Dodecahedron[4]);
            shaders.Uniform3("triangles[22].v2", Dodecahedron[2]);
            shaders.Uniform3("triangles[22].v3", Dodecahedron[17]);
            shaders.Uniform1("triangles[22].MaterialId", 7);

            shaders.Uniform3("triangles[23].v1", Dodecahedron[16]);
            shaders.Uniform3("triangles[23].v2", Dodecahedron[2]);
            shaders.Uniform3("triangles[23].v3", Dodecahedron[4]);
            shaders.Uniform1("triangles[23].MaterialId", 7);

            // 5 Pentagon
            shaders.Uniform3("triangles[24].v1", Dodecahedron[4]);
            shaders.Uniform3("triangles[24].v2", Dodecahedron[5]);
            shaders.Uniform3("triangles[24].v3", Dodecahedron[12]);
            shaders.Uniform1("triangles[24].MaterialId", 7);

            shaders.Uniform3("triangles[25].v1", Dodecahedron[4]);
            shaders.Uniform3("triangles[25].v2", Dodecahedron[12]);
            shaders.Uniform3("triangles[25].v3", Dodecahedron[6]);
            shaders.Uniform1("triangles[25].MaterialId", 7);

            shaders.Uniform3("triangles[26].v1", Dodecahedron[4]);
            shaders.Uniform3("triangles[26].v2", Dodecahedron[6]);
            shaders.Uniform3("triangles[26].v3", Dodecahedron[16]);
            shaders.Uniform1("triangles[26].MaterialId", 7);

            // 6 Pentagon
            shaders.Uniform3("triangles[27].v1", Dodecahedron[5]);
            shaders.Uniform3("triangles[27].v2", Dodecahedron[4]);
            shaders.Uniform3("triangles[27].v3", Dodecahedron[13]);
            shaders.Uniform1("triangles[27].MaterialId", 7);

            shaders.Uniform3("triangles[28].v1", Dodecahedron[13]);
            shaders.Uniform3("triangles[28].v2", Dodecahedron[4]);
            shaders.Uniform3("triangles[28].v3", Dodecahedron[11]);
            shaders.Uniform1("triangles[28].MaterialId", 7);

            shaders.Uniform3("triangles[29].v1", Dodecahedron[11]);
            shaders.Uniform3("triangles[29].v2", Dodecahedron[4]);
            shaders.Uniform3("triangles[29].v3", Dodecahedron[17]);
            shaders.Uniform1("triangles[29].MaterialId", 7);

            // 7 Pentagon
            shaders.Uniform3("triangles[30].v1", Dodecahedron[6]);
            shaders.Uniform3("triangles[30].v2", Dodecahedron[7]);
            shaders.Uniform3("triangles[30].v3", Dodecahedron[19]);
            shaders.Uniform1("triangles[30].MaterialId", 7);

            shaders.Uniform3("triangles[31].v1", Dodecahedron[6]);
            shaders.Uniform3("triangles[31].v2", Dodecahedron[19]);
            shaders.Uniform3("triangles[31].v3", Dodecahedron[2]);
            shaders.Uniform1("triangles[31].MaterialId", 7);

            shaders.Uniform3("triangles[32].v1", Dodecahedron[6]);
            shaders.Uniform3("triangles[32].v2", Dodecahedron[2]);
            shaders.Uniform3("triangles[32].v3", Dodecahedron[16]);
            shaders.Uniform1("triangles[32].MaterialId", 7);

            // 8 Pentagon
            shaders.Uniform3("triangles[33].v1", Dodecahedron[7]);
            shaders.Uniform3("triangles[33].v2", Dodecahedron[6]);
            shaders.Uniform3("triangles[33].v3", Dodecahedron[15]);
            shaders.Uniform1("triangles[33].MaterialId", 7);

            shaders.Uniform3("triangles[34].v1", Dodecahedron[15]);
            shaders.Uniform3("triangles[34].v2", Dodecahedron[6]);
            shaders.Uniform3("triangles[34].v3", Dodecahedron[1]);
            shaders.Uniform1("triangles[34].MaterialId", 7);

            shaders.Uniform3("triangles[35].v1", Dodecahedron[1]);
            shaders.Uniform3("triangles[35].v2", Dodecahedron[6]);
            shaders.Uniform3("triangles[35].v3", Dodecahedron[12]);
            shaders.Uniform1("triangles[35].MaterialId", 7);

            // 9 Pentagon
            shaders.Uniform3("triangles[36].v1", Dodecahedron[9]);
            shaders.Uniform3("triangles[36].v2", Dodecahedron[8]);
            shaders.Uniform3("triangles[36].v3", Dodecahedron[15]);
            shaders.Uniform1("triangles[36].MaterialId", 7);

            shaders.Uniform3("triangles[37].v1", Dodecahedron[15]);
            shaders.Uniform3("triangles[37].v2", Dodecahedron[8]);
            shaders.Uniform3("triangles[37].v3", Dodecahedron[7]);
            shaders.Uniform1("triangles[37].MaterialId", 7);

            shaders.Uniform3("triangles[38].v1", Dodecahedron[7]);
            shaders.Uniform3("triangles[38].v2", Dodecahedron[8]);
            shaders.Uniform3("triangles[38].v3", Dodecahedron[19]);
            shaders.Uniform1("triangles[38].MaterialId", 7);

            // 10 Pentagon
            shaders.Uniform3("triangles[39].v1", Dodecahedron[8]);
            shaders.Uniform3("triangles[39].v2", Dodecahedron[9]);
            shaders.Uniform3("triangles[39].v3", Dodecahedron[14]);
            shaders.Uniform1("triangles[39].MaterialId", 7);

            shaders.Uniform3("triangles[40].v1", Dodecahedron[8]);
            shaders.Uniform3("triangles[40].v2", Dodecahedron[14]);
            shaders.Uniform3("triangles[40].v3", Dodecahedron[10]);
            shaders.Uniform1("triangles[40].MaterialId", 7);

            shaders.Uniform3("triangles[41].v1", Dodecahedron[8]);
            shaders.Uniform3("triangles[41].v2", Dodecahedron[10]);
            shaders.Uniform3("triangles[41].v3", Dodecahedron[18]);
            shaders.Uniform1("triangles[41].MaterialId", 7);

            // 11 Pentagon
            shaders.Uniform3("triangles[42].v1", Dodecahedron[10]);
            shaders.Uniform3("triangles[42].v2", Dodecahedron[11]);
            shaders.Uniform3("triangles[42].v3", Dodecahedron[17]);
            shaders.Uniform1("triangles[42].MaterialId", 7);

            shaders.Uniform3("triangles[43].v1", Dodecahedron[10]);
            shaders.Uniform3("triangles[43].v2", Dodecahedron[17]);
            shaders.Uniform3("triangles[43].v3", Dodecahedron[3]);
            shaders.Uniform1("triangles[43].MaterialId", 7);

            shaders.Uniform3("triangles[44].v1", Dodecahedron[10]);
            shaders.Uniform3("triangles[44].v2", Dodecahedron[3]);
            shaders.Uniform3("triangles[44].v3", Dodecahedron[18]);
            shaders.Uniform1("triangles[44].MaterialId", 7);

            // 12 Pentagon
            shaders.Uniform3("triangles[45].v1", Dodecahedron[11]);
            shaders.Uniform3("triangles[45].v2", Dodecahedron[10]);
            shaders.Uniform3("triangles[45].v3", Dodecahedron[13]);
            shaders.Uniform1("triangles[45].MaterialId", 7);

            shaders.Uniform3("triangles[46].v1", Dodecahedron[13]);
            shaders.Uniform3("triangles[46].v2", Dodecahedron[10]);
            shaders.Uniform3("triangles[46].v3", Dodecahedron[0]);
            shaders.Uniform1("triangles[46].MaterialId", 7);

            shaders.Uniform3("triangles[47].v1", Dodecahedron[0]);
            shaders.Uniform3("triangles[47].v2", Dodecahedron[10]);
            shaders.Uniform3("triangles[47].v3", Dodecahedron[14]);
            shaders.Uniform1("triangles[47].MaterialId", 7);

            for (int i = 0; i < 48; i++)
            {
                Vector3 v1 = shaders.GetUniform($"triangles[{i}].v1");
                Vector3 v2 = shaders.GetUniform($"triangles[{i}].v2");
                Vector3 v3 = shaders.GetUniform($"triangles[{i}].v3");
                Vector3 norm = Vector3.Cross(v2 - v1, v3 - v1);
                norm.Normalize();
                shaders.Uniform3($"triangles_norms[{i}].norm", norm);
                shaders.Uniform3($"triangles_norms[{i}].norm1", Vector3.Cross(norm, v2 - v1));
                shaders.Uniform3($"triangles_norms[{i}].norm2", Vector3.Cross(norm, v3 - v2));
                shaders.Uniform3($"triangles_norms[{i}].norm3", Vector3.Cross(norm, v1 - v3));
            }

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

            // dode
            shaders.Uniform3("spheres[3].center", x, y, z);
            shaders.Uniform1("spheres[3].radius", (new Vector3(n * sinPi10)).Length);
            shaders.Uniform1("spheres[3].MaterialId", 0);
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
