#version 330 core
in vec4 glPosition;
out vec4 color;

#define EPSILON 0.001
#define BIG 1000000.0
const int DIFFUSE = 1;
const int REFLECTION = 2;
const int REFRACTION = 3;
const int DIFFUSE_REFLECTION = 1;
const int MIRROR_REFLECTION = 2;

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

uniform STriangle triangles[12];
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

bool IntersectTriangle (SRay ray, vec3 v1, vec3 v2, vec3 v3, out float time) {
    time = -1;
    vec3 A = v2 - v1;
    vec3 B = v3 - v1;
    vec3 N = cross(A, B);
    float NdotRayDirection = dot(N, ray.direction);
    if (NdotRayDirection < EPSILON)
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
    vec3 position;
};

struct SMaterial {
    vec3 color; // diffuse color
    vec4 lightCoeffs; // ambient, diffuse and specular coeffs
    // 0 - non-reflection, 1 - mirror
    float reflectionCoef;
    float refractionCoef;
    int MaterialType;
};

uniform SLight uLight;
uniform SMaterial materials[6];

bool Raytrace(SRay ray, float start, float final, inout SIntersection intersect) {
    bool result = false;
    float test = start;
    intersect.time = final;
    //calculate intersect with spheres
    for (int i = 0; i < spheres.length(); i++) {
        SSphere sphere = spheres[i];
        if (IntersectSphere(sphere, ray, start, final, test) && test < intersect.time) {
            intersect.time = test; intersect.point = ray.origin + ray.direction * test;
            intersect.normal = normalize(intersect.point - spheres[i].center);
            SMaterial material = materials[sphere.MaterialId];
            intersect.color = material.color;
            intersect.lightCoeffs = material.lightCoeffs;
            intersect.reflectionCoef = material.reflectionCoef;
            intersect.refractionCoef = material.refractionCoef;
            intersect.MaterialType = material.MaterialType;
            result = true;
        }
    }
    //calculate intersect with triangles
    for (int i = 0; i < triangles.length(); i++) {
        STriangle triangle = triangles[i];
        if(IntersectTriangle(ray, triangle.v1, triangle.v2, triangle.v3, test) && test < intersect.time) {
            intersect.time = test; intersect.point = ray.origin + ray.direction * test;
            intersect.normal = normalize(cross(triangle.v1 - triangle.v2, triangle.v3 - triangle.v2));
            SMaterial material = materials[triangle.MaterialId];
            intersect.color = material.color;
            intersect.lightCoeffs = material.lightCoeffs;
            intersect.reflectionCoef = material.reflectionCoef;
            intersect.refractionCoef = material.refractionCoef;
            intersect.MaterialType = material.MaterialType;
            result = true;
        }
    }
    return result;
}

float Shadow(SLight currLight, SIntersection intersect) {
    // Point is lighted
    float shadowing = 1.0;
    // Vector to the light source
    vec3 direction = normalize(currLight.position - intersect.point);
    // Distance to the light source
    float distanceLight = distance(currLight.position, intersect.point);
    // Generation shadow ray for this light source
    SRay shadowRay = SRay(intersect.point + direction * EPSILON, direction);
    // ...test intersection this ray with each scene object
    SIntersection shadowIntersect;
    shadowIntersect.time = BIG;
    // trace ray from shadow ray begining to light source position
    if (Raytrace(shadowRay, 0, distanceLight, shadowIntersect)) {
        // this light source is invisible in the intercection point
        shadowing = 0.0;
    }
    return shadowing;
}

vec3 Phong(SIntersection intersect, SLight currLight, float shadow) {
    int Unit = 0;
    vec3 light = normalize(currLight.position - intersect.point);
    float diffuse = max(dot(light, intersect.normal), 0.0);
    vec3 view = normalize(uCamera.position - intersect.point);
    vec3 reflected= reflect(-view, intersect.normal);
    float specular = pow(max(dot(reflected, light), 0.0), intersect.lightCoeffs.w);
    return intersect.lightCoeffs.x * intersect.color +
           intersect.lightCoeffs.y * diffuse * intersect.color * shadow +
           intersect.lightCoeffs.z * specular * Unit;
}

struct STracingRay {
    SRay ray;
    float contribution;
    int depth;
};

STracingRay arr[11];
int id = -1;
void pushRay(STracingRay trRay) {
    id++;
    arr[id] = trRay;
}
bool isEmpty() {
    return id == -1;
}
STracingRay popRay() {
    return arr[id--];
}

void main()
{
    /*SRay ray = GenerateRay();
    color = vec4(abs(ray.direction.xy), 0, 1.0);*/
    /*float start = 0;
    float final = BIG;
    SRay ray = GenerateRay();
    SIntersection intersect;
    intersect.time = BIG;
    vec3 resultColor = vec3(0,0,0);
    if (Raytrace(ray, start, final, intersect)) {
        resultColor = intersect.color;
    }
    color = vec4(resultColor, 1.0);*/
    float start = 0;
    float final = BIG;
    SRay ray = GenerateRay();
    SIntersection intersect;
    intersect.time = BIG;
    vec3 resultColor = vec3(0,0,0);

    STracingRay trRay = STracingRay(ray, 1, 0);
    pushRay(trRay);
    while(!isEmpty()) {
        STracingRay trRay = popRay();
        ray = trRay.ray;
        SIntersection intersect;
        intersect.time = BIG;
        start = 0;
        final = BIG;
        if (Raytrace(ray, start, final, intersect)) {
            switch(intersect.MaterialType) {
                case DIFFUSE_REFLECTION: {
                    float shadowing = Shadow(uLight, intersect);
                    resultColor += trRay.contribution * Phong ( intersect, uLight, shadowing );
                    break;
                }
                case MIRROR_REFLECTION: {
                    if(intersect.reflectionCoef < 1) {
                        float contribution = trRay.contribution * (1 - intersect.reflectionCoef);
                        float shadowing = Shadow(uLight, intersect);
                        resultColor += contribution * Phong(intersect, uLight, shadowing);
                    }
                    vec3 reflectDirection = reflect(ray.direction, intersect.normal);
                    // creare reflection ray
                    float contribution = trRay.contribution * intersect.reflectionCoef;
                    STracingRay reflectRay = STracingRay( SRay(intersect.point + reflectDirection * EPSILON, reflectDirection), contribution, trRay.depth + 1);
                    pushRay(reflectRay);
                    break;
                }
            } // switch
        } // if (Raytrace(ray, start, final, intersect))
    } // while(!isEmpty())

    color = vec4(resultColor, 1.0);
}
