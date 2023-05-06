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

    public static class SceneInitializer
    {
        public static readonly int DEFAULT = 1;
        public static readonly int LIGHT = 2;
        public static readonly int GLASS = 3;
        public static readonly int CHECKBOARD = 4;

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

        private static void UniformRectangle(Shaders shaders, int id, Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, int MaterialId)
        {
            //shaders.Uniform3($"rectangles[{id}].v1", v1);
            shaders.Uniform3($"rectangles[{id}].v2", v2);
            //shaders.Uniform3($"rectangles[{id}].v3", v3);
            shaders.Uniform3($"rectangles[{id}].v4", v4);
            Vector3 norm = Vector3.Cross(v2 - v1, v4 - v1);
            norm.Normalize();
            shaders.Uniform3($"rectangles[{id}].norm", norm);
            shaders.Uniform3($"rectangles[{id}].sp1", Vector3.Cross(norm, v2 - v1));
            shaders.Uniform3($"rectangles[{id}].sp2", Vector3.Cross(norm, v3 - v2));
            shaders.Uniform3($"rectangles[{id}].sp3", Vector3.Cross(norm, v4 - v3));
            shaders.Uniform3($"rectangles[{id}].sp4", Vector3.Cross(norm, v1 - v4));
            shaders.Uniform1($"rectangles[{id}].MaterialId", MaterialId);
        }

        private static void UniformPentagon(Shaders shaders, int id, Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, Vector3 v5, int MaterialId)
        {
            //shaders.Uniform3($"pentagons[{id}].v1", v1);
            shaders.Uniform3($"pentagons[{id}].v2", v2);
            //shaders.Uniform3($"pentagons[{id}].v3", v3);
            shaders.Uniform3($"pentagons[{id}].v4", v4);
            shaders.Uniform3($"pentagons[{id}].v5", v5);
            Vector3 norm = Vector3.Cross(v2 - v1, v3 - v1);
            norm.Normalize();
            shaders.Uniform3($"pentagons[{id}].norm", norm);
            shaders.Uniform3($"pentagons[{id}].sp1", Vector3.Cross(norm, v2 - v1));
            shaders.Uniform3($"pentagons[{id}].sp2", Vector3.Cross(norm, v3 - v2));
            shaders.Uniform3($"pentagons[{id}].sp3", Vector3.Cross(norm, v4 - v3));
            shaders.Uniform3($"pentagons[{id}].sp4", Vector3.Cross(norm, v5 - v4));
            shaders.Uniform3($"pentagons[{id}].sp5", Vector3.Cross(norm, v1 - v5));
            shaders.Uniform1($"pentagons[{id}].MaterialId", MaterialId);
        }

        private static void UniformPlane(Shaders shaders, int id, Vector3 v1, Vector3 v2, Vector3 v3, int MaterialId)
        {
            shaders.Uniform3($"planes[{id}].v1", v1);
            //shaders.Uniform3($"planes[{id}].v2", v2);
            //shaders.Uniform3($"planes[{id}].v3", v3);
            Vector3 norm = Vector3.Cross(v2 - v1, v3 - v1);
            norm.Normalize();
            shaders.Uniform3($"planes[{id}].norm", norm);

            //Matrix3 m = new Matrix3(v2 - v1, v3 - v1, norm);
            //shaders.Uniform1($"planes[{id}].d", m.Determinant);
            //shaders.Uniform3($"planes[{id}].sp1", m[1, 1] * m[2, 2] - m[1, 2] * m[2, 1],
            //                                        -m[1, 0] * m[2, 2] + m[1, 2] * m[2, 0],
            //                                         m[1, 0] * m[2, 1] - m[1, 1] * m[2, 0]);
            //shaders.Uniform3($"planes[{id}].sp2", -m[0, 1] * m[2, 2] + m[0, 2] * m[2, 1],
            //                                          m[0, 0] * m[2, 2] - m[0, 2] * m[2, 0],
            //                                         -m[0, 0] * m[2, 1] + m[0, 1] * m[2, 0]);
            shaders.Uniform1($"planes[{id}].MaterialId", MaterialId);
        }

        public static void InitializeDefaultScene(Shaders shaders)
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

            // clockwise order of vectors!!!

            // TRIANGLES
            shaders.Uniform1("triangles_used", 12);

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

            // RECTANGLES
            shaders.Uniform1("rectangles_used", 0);

            // PENTAGONS
            shaders.Uniform1("pentagons_used", 0);

            UniformPentagon(shaders, 0, Dodecahedron[0], Dodecahedron[1], Dodecahedron[12], Dodecahedron[5], Dodecahedron[13], 7);
            UniformPentagon(shaders, 1, Dodecahedron[0], Dodecahedron[14], Dodecahedron[9], Dodecahedron[15], Dodecahedron[1], 7);
            UniformPentagon(shaders, 2, Dodecahedron[2], Dodecahedron[19], Dodecahedron[8], Dodecahedron[18], Dodecahedron[3], 7);
            UniformPentagon(shaders, 3, Dodecahedron[2], Dodecahedron[3], Dodecahedron[17], Dodecahedron[4], Dodecahedron[16], 7);

            UniformPentagon(shaders, 4, Dodecahedron[4], Dodecahedron[5], Dodecahedron[12], Dodecahedron[6], Dodecahedron[16], 7);
            UniformPentagon(shaders, 5, Dodecahedron[4], Dodecahedron[17], Dodecahedron[11], Dodecahedron[13], Dodecahedron[5], 7);
            UniformPentagon(shaders, 6, Dodecahedron[6], Dodecahedron[7], Dodecahedron[19], Dodecahedron[2], Dodecahedron[16], 7);
            UniformPentagon(shaders, 7, Dodecahedron[6], Dodecahedron[12], Dodecahedron[1], Dodecahedron[15], Dodecahedron[7], 7);

            UniformPentagon(shaders, 8, Dodecahedron[8], Dodecahedron[19], Dodecahedron[7], Dodecahedron[15], Dodecahedron[9], 7);
            UniformPentagon(shaders, 9, Dodecahedron[8], Dodecahedron[9], Dodecahedron[14], Dodecahedron[10], Dodecahedron[18], 7);
            UniformPentagon(shaders, 10, Dodecahedron[10], Dodecahedron[11], Dodecahedron[17], Dodecahedron[3], Dodecahedron[18], 7);
            UniformPentagon(shaders, 11, Dodecahedron[10], Dodecahedron[14], Dodecahedron[0], Dodecahedron[13], Dodecahedron[11], 7);

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

            // dodecahedron shell: sphere 3
            shaders.Uniform3("spheres[3].center", x, y, z);
            shaders.Uniform1("spheres[3].radius", (new Vector3(n * sinPi10)).Length);
            shaders.Uniform1("spheres[3].MaterialId", 0);

            // DODECAHEDRA
            shaders.Uniform1("dodecahedra_used", 1);

            // dodecahedron 0
            shaders.Uniform1("dodecahedra[0].id_first", 0);
            shaders.Uniform1("dodecahedra[0].id_shell", 3);

            // PLANES
            shaders.Uniform1("planes_used", 0);

            // LIGHT
            shaders.Uniform3("uLight.position", 0.5f, 3.0f, -4.0f);
        }

        public static void InitializeScene2(Shaders shaders)
        {
            Vector3[] Cube = new Vector3[]
            {
                new Vector3(-2.5f, -2.5f, -2.5f),
                new Vector3(-2.5f, -2.5f, 2.5f),
                new Vector3(-2.5f, 2.5f, -2.5f),
                new Vector3(-2.5f, 2.5f, 2.5f),
                new Vector3(2.5f, -2.5f, -2.5f),
                new Vector3(2.5f, -2.5f, 2.5f),
                new Vector3(2.5f, 2.5f, -2.5f),
                new Vector3(2.5f, 2.5f, 2.5f)
            };
            Matrix3 r0;
            Matrix3.CreateRotationY((float)Math.PI / 8, out r0);
            Vector3 d0 = new Vector3(10.0f, -2.49f, 0.0f);
            for (int i = 0; i < Cube.Length; i++)
            {
                Cube[i] = r0 * Cube[i] + d0;
            }

            Vector3[] Prism = new Vector3[]
            {
                new Vector3(-1.5f, -1.5f, -0.75f),
                new Vector3(-1.5f, -1.5f, 0.75f),
                new Vector3(-1.5f, 1.5f, -0.75f),
                new Vector3(-1.5f, 1.5f, 0.75f),
                new Vector3(1.5f, -1.5f, -0.75f),
                new Vector3(1.5f, -1.5f, 0.75f),
                new Vector3(1.5f, 1.5f, -0.75f),
                new Vector3(1.5f, 1.5f, 0.75f)
            };
            Matrix3 r1;
            Matrix3.CreateRotationX((float)Math.PI / 10, out r1);
            Matrix3 r2;
            Matrix3.CreateRotationY((float)Math.PI / 21, out r2);
            Matrix3 r3;
            Matrix3.CreateRotationZ((float)Math.PI / 17, out r3);
            for (int i = 0; i < Prism.Length; i++)
            {
                Prism[i] = r1 * r2 * r3 * Prism[i];
            }

            float phi = (float)((Math.Sqrt(5) + 1) / 2);
            float n = 5.5f;
            float sinPi10 = (float)Math.Sin(Math.PI / 10);
            Vector3[] Dodecahedron = new Vector3[]
            {
                new Vector3(-(n / (phi + 1)) / 2, 0, -n / 2),
                new Vector3((n / (phi + 1)) / 2, 0, -n / 2),
                new Vector3((n / (phi + 1)) / 2, 0, n / 2),
                new Vector3(-(n / (phi + 1)) / 2, 0, n / 2),
                new Vector3(0, -n / 2, (n / (phi + 1)) / 2),
                new Vector3(0, -n / 2, -(n / (phi + 1)) / 2),
                new Vector3(n / 2, -(n / (phi + 1)) / 2, 0),
                new Vector3(n / 2, (n / (phi + 1)) / 2, 0),
                new Vector3(0, n / 2, (n / (phi + 1)) / 2),
                new Vector3(0, n / 2, -(n / (phi + 1)) / 2),
                new Vector3(-n / 2, (n / (phi + 1)) / 2, 0),
                new Vector3(-n / 2, -(n / (phi + 1)) / 2, 0),
                new Vector3(n * sinPi10, -n * sinPi10, -n * sinPi10),
                new Vector3(-n * sinPi10, -n * sinPi10, -n * sinPi10),
                new Vector3(-n * sinPi10, n * sinPi10, -n * sinPi10),
                new Vector3(n * sinPi10, n * sinPi10, -n * sinPi10),
                new Vector3(n * sinPi10, -n * sinPi10, n * sinPi10),
                new Vector3(-n * sinPi10, -n * sinPi10, n * sinPi10),
                new Vector3(-n * sinPi10, n * sinPi10, n * sinPi10),
                new Vector3(n * sinPi10, n * sinPi10, n * sinPi10)
            };
            Matrix3 r4;
            Matrix3.CreateRotationX((float)Math.PI / 7, out r4);
            Matrix3 r5;
            Matrix3.CreateRotationY((float)Math.PI / 25, out r5);
            Matrix3 r6;
            Matrix3.CreateRotationZ((float)Math.PI / 5, out r6);
            Vector3 d1 = new Vector3(-7.5f, -2.0f, 0.0f);
            for (int i = 0; i < Dodecahedron.Length; i++)
            {
                Dodecahedron[i] = r4 * r5 * r6 * Dodecahedron[i] + d1;
            }

            // clockwise order of vectors!!!

            // TRIANGLES
            shaders.Uniform1("triangles_used", 0);

            // RECTANGLES
            shaders.Uniform1("rectangles_used", 12);

            // Prism
            UniformRectangle(shaders, 0, Prism[2], Prism[0], Prism[1], Prism[3], 10);
            UniformRectangle(shaders, 1, Prism[7], Prism[5], Prism[4], Prism[6], 10);
            UniformRectangle(shaders, 2, Prism[0], Prism[4], Prism[5], Prism[1], 10);
            UniformRectangle(shaders, 3, Prism[6], Prism[2], Prism[3], Prism[7], 10);
            UniformRectangle(shaders, 4, Prism[3], Prism[1], Prism[5], Prism[7], 10);
            UniformRectangle(shaders, 5, Prism[0], Prism[2], Prism[6], Prism[4], 10);

            // Cube prism
            UniformRectangle(shaders, 6, Cube[2], Cube[0], Cube[1], Cube[3], 6);
            UniformRectangle(shaders, 7, Cube[7], Cube[5], Cube[4], Cube[6], 6);
            UniformRectangle(shaders, 8, Cube[0], Cube[4], Cube[5], Cube[1], 6);
            UniformRectangle(shaders, 9, Cube[6], Cube[2], Cube[3], Cube[7], 6);
            UniformRectangle(shaders, 10, Cube[3], Cube[1], Cube[5], Cube[7], 6);
            UniformRectangle(shaders, 11, Cube[0], Cube[2], Cube[6], Cube[4], 6);

            // PENTAGONS
            shaders.Uniform1("pentagons_used", 0);

            UniformPentagon(shaders, 0, Dodecahedron[0], Dodecahedron[1], Dodecahedron[12], Dodecahedron[5], Dodecahedron[13], 11);
            UniformPentagon(shaders, 1, Dodecahedron[0], Dodecahedron[14], Dodecahedron[9], Dodecahedron[15], Dodecahedron[1], 11);
            UniformPentagon(shaders, 2, Dodecahedron[2], Dodecahedron[19], Dodecahedron[8], Dodecahedron[18], Dodecahedron[3], 11);
            UniformPentagon(shaders, 3, Dodecahedron[2], Dodecahedron[3], Dodecahedron[17], Dodecahedron[4], Dodecahedron[16], 11);

            UniformPentagon(shaders, 4, Dodecahedron[4], Dodecahedron[5], Dodecahedron[12], Dodecahedron[6], Dodecahedron[16], 11);
            UniformPentagon(shaders, 5, Dodecahedron[4], Dodecahedron[17], Dodecahedron[11], Dodecahedron[13], Dodecahedron[5], 11);
            UniformPentagon(shaders, 6, Dodecahedron[6], Dodecahedron[7], Dodecahedron[19], Dodecahedron[2], Dodecahedron[16], 11);
            UniformPentagon(shaders, 7, Dodecahedron[6], Dodecahedron[12], Dodecahedron[1], Dodecahedron[15], Dodecahedron[7], 11);

            UniformPentagon(shaders, 8, Dodecahedron[8], Dodecahedron[19], Dodecahedron[7], Dodecahedron[15], Dodecahedron[9], 11);
            UniformPentagon(shaders, 9, Dodecahedron[8], Dodecahedron[9], Dodecahedron[14], Dodecahedron[10], Dodecahedron[18], 11);
            UniformPentagon(shaders, 10, Dodecahedron[10], Dodecahedron[11], Dodecahedron[17], Dodecahedron[3], Dodecahedron[18], 11);
            UniformPentagon(shaders, 11, Dodecahedron[10], Dodecahedron[14], Dodecahedron[0], Dodecahedron[13], Dodecahedron[11], 11);

            // SPHERES
            shaders.Uniform1("spheres_used", 2);

            // light sphere: sphere 0
            shaders.Uniform3("spheres[0].center", 0.5f, 5.0f, -4.0f);
            shaders.Uniform1("spheres[0].radius", 0.2f);
            shaders.Uniform1("spheres[0].MaterialId", 5);

            // blue sky sphere: sphere 1
            shaders.Uniform3("spheres[1].center", 0.0f, 0.0f, 0.0f);
            shaders.Uniform1("spheres[1].radius", 100.0f);
            shaders.Uniform1("spheres[1].MaterialId", 8);

            // dodecahedron shell: sphere 2
            shaders.Uniform3("spheres[2].center", d1);
            shaders.Uniform1("spheres[2].radius", (new Vector3(n * sinPi10)).Length);
            shaders.Uniform1("spheres[2].MaterialId", 0);

            // DODECAHEDRA
            shaders.Uniform1("dodecahedra_used", 1);

            // dodecahedron 0
            shaders.Uniform1("dodecahedra[0].id_first", 0);
            shaders.Uniform1("dodecahedra[0].id_shell", 2);

            // PLANES
            shaders.Uniform1("planes_used", 1);
            UniformPlane(shaders, 0, new Vector3(5.0f, -5.0f, -5.0f), new Vector3(-5.0f, -5.0f, -5.0f), new Vector3(-5.0f, -5.0f, 5.0f), 9);

            // LIGHT
            shaders.Uniform3("uLight.position", 0.5f, 5.0f, -4.0f);
        }

        public static void InitializeMaterials(Shaders shaders)
        {
            // 0: Gray wall
            shaders.Uniform3("materials[0].color", 0.75f, 0.75f, 0.75f);
            shaders.Uniform4("materials[0].lightCoeffs", 0.4f, 0.5f, 0.25f, 10.0f);
            shaders.Uniform1("materials[0].reflectionCoef", 0.0f);
            shaders.Uniform1("materials[0].refractionIndex", 1.0f);
            shaders.Uniform1("materials[0].MaterialType", DEFAULT);

            // 1: Red wall
            shaders.Uniform3("materials[1].color", 1.0f, 0.0f, 0.0f);
            shaders.Uniform4("materials[1].lightCoeffs", 0.9f, 0.4f, 0.01f, 5.0f);
            shaders.Uniform1("materials[1].reflectionCoef", 0.5f);
            shaders.Uniform1("materials[1].refractionIndex", 1.0f);
            shaders.Uniform1("materials[1].MaterialType", DEFAULT);

            // 2: Cyan wall
            shaders.Uniform3("materials[2].color", 0.0f, 1.0f, 1.0f);
            shaders.Uniform4("materials[2].lightCoeffs", 0.4f, 0.9f, 0.6f, 7.0f);
            shaders.Uniform1("materials[2].reflectionCoef", 0.0f);
            shaders.Uniform1("materials[2].refractionIndex", 1.0f);
            shaders.Uniform1("materials[2].MaterialType", DEFAULT);

            // 3: Green wall
            shaders.Uniform3("materials[3].color", 0.0f, 1.0f, 0.0f);
            shaders.Uniform4("materials[3].lightCoeffs", 0.9f, 0.4f, 0.01f, 5.0f);
            shaders.Uniform1("materials[3].reflectionCoef", 0.5f);
            shaders.Uniform1("materials[3].refractionIndex", 1.0f);
            shaders.Uniform1("materials[3].MaterialType", DEFAULT);

            // 4: Blue material
            shaders.Uniform3("materials[4].color", 0.0f, 0.0f, 1.0f);
            shaders.Uniform4("materials[4].lightCoeffs", 0.5f, 0.8f, 0.4f, 5.0f);
            shaders.Uniform1("materials[4].reflectionCoef", 0.05f);
            shaders.Uniform1("materials[4].refractionIndex", 1.0f);
            shaders.Uniform1("materials[4].MaterialType", DEFAULT);

            // 5: Light
            shaders.Uniform3("materials[5].color", 10.0f, 10.0f, 10.0f);
            shaders.Uniform4("materials[5].lightCoeffs", 0.0f, 0.0f, 0.0f, 0.0f);
            shaders.Uniform1("materials[5].reflectionCoef", 0.0f);
            shaders.Uniform1("materials[5].refractionIndex", 1.0f);
            shaders.Uniform1("materials[5].MaterialType", LIGHT);

            // 6: Green Glass
            shaders.Uniform3("materials[6].color", 0.4f, 0.05f, 0.2f); // absorb
            shaders.Uniform4("materials[6].lightCoeffs", 0.0f, 0.0f, 0.4f, 7.5f);
            shaders.Uniform1("materials[6].reflectionCoef", 0.05f);
            shaders.Uniform1("materials[6].refractionIndex", 1.35f);
            shaders.Uniform1("materials[6].MaterialType", GLASS);

            // 7: Purple material
            shaders.Uniform3("materials[7].color", 0.53f, 0.05f, 0.95f);
            shaders.Uniform4("materials[7].lightCoeffs", 0.35f, 0.7f, 0.4f, 20.0f);
            shaders.Uniform1("materials[7].reflectionCoef", 0.4f);
            shaders.Uniform1("materials[7].refractionIndex", 1.0f);
            shaders.Uniform1("materials[7].MaterialType", DEFAULT);

            // 8: Second Blue material
            shaders.Uniform3("materials[8].color", 0.0f, 0.0f, 1.0f);
            shaders.Uniform4("materials[8].lightCoeffs", 0.5f, 0.8f, 0.4f, 5.0f);
            shaders.Uniform1("materials[8].reflectionCoef", 0.0f);
            shaders.Uniform1("materials[8].refractionIndex", 1.0f);
            shaders.Uniform1("materials[8].MaterialType", DEFAULT);

            // 9: White material (for plane)
            shaders.Uniform3("materials[9].color", 1.0f, 1.0f, 1.0f);
            shaders.Uniform4("materials[9].lightCoeffs", 0.4f, 0.5f, 0.25f, 10.0f);
            shaders.Uniform1("materials[9].reflectionCoef", 0.0f);
            shaders.Uniform1("materials[9].refractionIndex", 1.0f);
            shaders.Uniform1("materials[9].MaterialType", CHECKBOARD);

            // 10: Red-Purple Glass
            shaders.Uniform3("materials[10].color", 0.03f, 0.42f, 0.24f); // absorb
            shaders.Uniform4("materials[10].lightCoeffs", 0.0f, 0.0f, 0.4f, 7.5f);
            shaders.Uniform1("materials[10].reflectionCoef", 0.05f);
            shaders.Uniform1("materials[10].refractionIndex", 1.35f);
            shaders.Uniform1("materials[10].MaterialType", GLASS);

            // 11: Blue Glass
            shaders.Uniform3("materials[11].color", 0.21f, 0.34f, 0.01f); // absorb
            shaders.Uniform4("materials[11].lightCoeffs", 0.0f, 0.0f, 0.4f, 7.5f);
            shaders.Uniform1("materials[11].reflectionCoef", 0.05f);
            shaders.Uniform1("materials[11].refractionIndex", 1.35f);
            shaders.Uniform1("materials[11].MaterialType", GLASS);
        }
    }
}
