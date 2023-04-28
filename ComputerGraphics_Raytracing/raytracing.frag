#version 330 core
in vec4 glPosition;
out vec4 color;

#define EPSILON 0.001
#define BIG 1000000.0
const int DIFFUSE = 1;
const int REFLECTION = 2;
const int REFRACTION = 3;

struct SCamera {
    vec3 position;
    vec3 view;
    vec3 up;
    vec3 right;
    vec2 scale;
};

uniform SCamera uCamera;

struct SRay {
    vec3 origin;
    vec3 direction;
};

SRay GenerateRay() {
    vec2 coords = glPosition.xy * uCamera.scale;
    vec3 direction = uCamera.view + uCamera.right * coords.x + uCamera.up * coords.y;
    return SRay(uCamera.position, normalize(direction));
}

struct SSphere {
    vec3 center;
    float radius;
    int MaterialId;
};

struct STriangle {
    vec3 v1;
    vec3 v2;
    vec3 v3;
    int MaterialId;
};

uniform STriangle triangles[10];
uniform SSphere spheres[2];

bool IntersectSphere(SSphere sphere, SRay ray, float start, float final, out float time) {
    ray.origin -= sphere.center;
    float A = dot(ray.direction, ray.direction);
    float B = dot(ray.direction, ray.origin);
    float C = dot(ray.origin, ray.origin) - sphere.radius * sphere.radius;
    float D = B * B - A * C;
    if (D > 0.0) {
        D = sqrt(D);
        // time = min(max(0.0, (-B - D) / A), (-B + D) / A);
        float t1 = (-B - D) / A;
        float t2 = (-B + D) / A;
        if (t1 < 0 && t2 < 0)
            return false;
        if (min(t1, t2) < 0) {
            time = max(t1, t2);
            return true;
        }
        time = min(t1, t2);
        return true;
    }
    return false;
}

bool IntersectTriangle (SRay ray, vec3 v1, vec3 v2, vec3 v3, out float time ) {
    time = -1;
    vec3 A = v2 - v1;
    vec3 B = v3 - v1;
    vec3 N = cross(A, B);
    float NdotRayDirection = dot(N, ray.direction);
    if (abs(NdotRayDirection) < EPSILON)
        return false;
    float d = dot(N, v1);
    float t = -(dot(N, ray.origin) - d) / NdotRayDirection;
    if (t < 0)
        return false;
    vec3 P = ray.origin + t * ray.direction;
    vec3 C;
    vec3 edge1 = v2 - v1;
    vec3 VP1 = P - v1;
    C = cross(edge1, VP1);
    if (dot(N, C) < 0)
        return false;
    vec3 edge2 = v3 - v2;
    vec3 VP2 = P - v2;
    C = cross(edge2, VP2);
    if (dot(N, C) < 0)
        return false;
    vec3 edge3 = v1 - v3;
    vec3 VP3 = P - v3;
    C = cross(edge3, VP3);
    if (dot(N, C) < 0)
        return false;
    time = t;
    return true;
}

struct SIntersection{
    float time;
    vec3 point;
    vec3 normal;
    vec3 color;
    vec4 lightCoeffs; // ambient, diffuse and specular coeffs
    float reflectionCoef;
    float refractionCoef;
    int MaterialType;
};

struct SLight {
    vec3 Position;
};

struct SMaterial {
    //diffuse color
    vec3 Color;
    // ambient, diffuse and specular coeffs
    vec4 LightCoeffs;
    // 0 - non-reflection, 1 - mirror
    float ReflectionCoef;
    float RefractionCoef;
    int MaterialType;
};

SLight light;
SMaterial materials[6];

bool Raytrace(SRay ray, float start, float final, inout SIntersection intersect) {
    bool result = false;
    float test = start;
    intersect.time = final;
    //calculate intersect with spheres
    for(int i = 0; i < spheres.length(); i++) {
        //SSphere sphere = spheres[i];
        if (IntersectSphere(spheres[i], ray, start, final, test) && test < intersect.time) {
            intersect.time = test; intersect.point = ray.origin + ray.direction * test;
            intersect.normal = normalize(intersect.point - spheres[i].center);
            intersect.color = vec3(1,0,0);
            intersect.lightCoeffs = vec4(0,0,0,0);
            intersect.reflectionCoef = 0;
            intersect.refractionCoef = 0;
            intersect.MaterialType = 0;
            result = true;
        }
    }
    //calculate intersect with triangles
    for(int i = 0; i < triangles.length(); i++) {
        STriangle triangle = triangles[i];
        if(IntersectTriangle(ray, triangle.v1, triangle.v2, triangle.v3, test) && test < intersect.time) {
            intersect.time = test; intersect.point = ray.origin + ray.direction * test;
            intersect.normal = normalize(cross(triangle.v1 - triangle.v2, triangle.v3 - triangle.v2));
            intersect.color = vec3(1,0,0);
            intersect.lightCoeffs = vec4(0,0,0,0);
            intersect.reflectionCoef = 0;
            intersect.refractionCoef = 0;
            intersect.MaterialType = 0;
            result = true;
        }
    }
    return result;
}

/*
void initializeDefaultLightMaterials(out SLight light, out SMaterial materials[6]) {
    light.Position = vec3(0.0, 2.0, -4.0f);
    vec4 lightCoefs = vec4(0.4,0.9,0.0,512.0);
    materials[0].Color = vec3(0.0, 1.0, 0.0);
    materials[0].LightCoeffs = vec4(lightCoefs);
    materials[0].ReflectionCoef = 0.5;
    materials[0].RefractionCoef = 1.0;
    materials[0].MaterialType = DIFFUSE;
    materials[1].Color = vec3(0.0, 0.0, 1.0);
    materials[1].LightCoeffs = vec4(lightCoefs);
    materials[1].ReflectionCoef = 0.5;
    materials[1].RefractionCoef = 1.0;
    materials[1].MaterialType = DIFFUSE;
}
*/

void main()
{
    /*SRay ray = GenerateRay();
    color = vec4(abs(ray.direction.xy), 0, 1.0);*/
    float start = 0;
    float final = BIG;
    SRay ray = GenerateRay();
    SIntersection intersect;
    intersect.time = BIG;
    vec3 resultColor = vec3(0,0,0);
    if (Raytrace(ray, start, final, intersect)) {
        resultColor = vec3(1,0,0);
    }
    color = vec4(resultColor, 1.0);
}
