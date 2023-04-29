#version 330 core
in vec4 glPosition;
out vec4 color;

#define EPSILON 0.001
#define BIG 1000000.0
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

bool IntersectSphere(SRay ray, SSphere sphere, out float time) {
    time = -1;
    ray.origin -= sphere.center;
    float A = dot(ray.direction, ray.direction);
    float B = dot(ray.direction, ray.origin);
    float C = dot(ray.origin, ray.origin) - sphere.radius * sphere.radius;
    float D = B * B - A * C;
    if (D > 0.0f) {
        D = sqrt(D);
        float t1 = (-B - D) / A;
        float t2 = (-B + D) / A;
        if (t1 < 0.0f && t2 < 0.0f)
            return false;
        time = min(t1, t2);
        if (time < 0.0f)
            time = max(t1, t2);
        return true;
    }
    return false;
}

bool IntersectTriangle(SRay ray, STriangle triangle, out float time) {
    vec3 v1 = triangle.v1;
    vec3 v2 = triangle.v2;
    vec3 v3 = triangle.v3;
    time = -1;
    vec3 A = v2 - v1;
    vec3 B = v3 - v1;
    vec3 N = cross(A, B);
    float NdotRayDirection = dot(N, ray.direction);
    if (NdotRayDirection > -EPSILON) // triangle is transparent at one side
        return false;
    float t = dot(N, v1 - ray.origin) / NdotRayDirection;
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
    if (dot(N, C) < -EPSILON)
        return false;
    vec3 edge3 = v1 - v3;
    vec3 VP3 = P - v3;
    C = cross(edge3, VP3);
    if (dot(N, C) < -EPSILON)
        return false;
    time = t;
    return true;
}

struct SIntersection{
    float time;
    vec3 point;
    vec3 normal;
    int MaterialId;
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
    float test;
    intersect.time = final;
    //calculate intersect with spheres
    for (int i = 0; i < spheres.length(); i++) {
        if (IntersectSphere(ray, spheres[i], test) && test < intersect.time) {
            intersect.time = test;
            intersect.point = ray.origin + ray.direction * test;
            intersect.normal = normalize(intersect.point - spheres[i].center);
            intersect.MaterialId = spheres[i].MaterialId;
            result = true;
        }
    }
    //calculate intersect with triangles
    for (int i = 0; i < triangles.length(); i++) {
        if(IntersectTriangle(ray, triangles[i], test) && test < intersect.time) {
            intersect.time = test;
            intersect.point = ray.origin + ray.direction * test;
            intersect.normal = normalize(cross(triangles[i].v2 - triangles[i].v1, triangles[i].v3 - triangles[i].v1));
            intersect.MaterialId = triangles[i].MaterialId;
            result = true;
        }
    }
    return result;
}

float Shadow(SLight currLight, SIntersection intersect) {
    float shadowing = 1.0;
    vec3 direction = normalize(currLight.position - intersect.point);
    float distanceLight = distance(currLight.position, intersect.point);
    SRay shadowRay = SRay(intersect.point + direction * EPSILON, direction);
    SIntersection shadowIntersect;
    if (Raytrace(shadowRay, 0, distanceLight, shadowIntersect)) {
        //shadowing = 0.0f;
        shadowing = 0.2f;
        //shadowing = shadowIntersect.time / distanceLight;
    }
    return shadowing;
}

vec3 Phong(SIntersection intersect, SLight currLight, float shadow) {
    vec3 light = normalize(currLight.position - intersect.point);
    float diffuse = max(dot(light, intersect.normal), 0.0);
    vec3 view = normalize(intersect.point - uCamera.position);
    vec3 reflected = reflect(view, intersect.normal);
    vec4 lightCoeffs = materials[intersect.MaterialId].lightCoeffs;
    vec3 color = materials[intersect.MaterialId].color;
    float specular = pow(max(dot(reflected, light), 0.0), lightCoeffs.w);
    return lightCoeffs.x * color +
           lightCoeffs.y * diffuse * color * shadow +
           lightCoeffs.z * specular * shadow;
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
            SMaterial material = materials[intersect.MaterialId];
            switch(material.MaterialType) {
                case DIFFUSE_REFLECTION: {
                    float shadowing = Shadow(uLight, intersect);
                    resultColor += trRay.contribution * Phong ( intersect, uLight, shadowing );
                    break;
                }
                case MIRROR_REFLECTION: {
                    if(material.reflectionCoef < 1) {
                        float contribution = trRay.contribution * (1 - material.reflectionCoef);
                        float shadowing = Shadow(uLight, intersect);
                        resultColor += contribution * Phong(intersect, uLight, shadowing);
                    }
                    vec3 reflectDirection = reflect(ray.direction, intersect.normal);
                    // creare reflection ray
                    float contribution = trRay.contribution * material.reflectionCoef;
                    STracingRay reflectRay = STracingRay( SRay(intersect.point + reflectDirection * EPSILON, reflectDirection), contribution, trRay.depth + 1);
                    if (reflectRay.depth > 6)
                        break;
                    pushRay(reflectRay);
                    break;
                }
            } // switch
        } // if (Raytrace(ray, start, final, intersect))
    } // while(!isEmpty())

    color = vec4(resultColor, 1.0);
}
