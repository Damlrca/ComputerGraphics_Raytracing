#version 330 core
in vec4 glPosition;
out vec4 color;

#define EPSILON 0.001
#define BIG 1000000.0
const int DEFAULT = 1;
const int LIGHT = 2;
const int GLASS = 3;
uniform int MAX_DEPTH;

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

struct STriangleNORMS {
    vec3 norm;
    vec3 norm1;
    vec3 norm2;
    vec3 norm3;
};

uniform STriangle triangles[50];
uniform STriangleNORMS triangles_norms[50];
uniform int triangles_used;
uniform SSphere spheres[10];
uniform int spheres_used;

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

bool IntersectTriangle(SRay ray, int i, out float time) {
    vec3 v1 = triangles[i].v1;
    vec3 v2 = triangles[i].v2;
    vec3 v3 = triangles[i].v3;
    time = -1;
    //vec3 A = v2 - v1;
    //vec3 B = v3 - v1;
    //vec3 N = cross(A, B);
    vec3 N = triangles_norms[i].norm;
    float NdotRayDirection = dot(N, ray.direction);
    if (NdotRayDirection > -EPSILON) // triangle is transparent at one side
        return false;
    float t = dot(N, v1 - ray.origin) / NdotRayDirection;
    if (t < 0)
        return false;
    vec3 P = ray.origin + t * ray.direction;
    //vec3 C;
    //vec3 edge1 = v2 - v1;
    //vec3 VP1 = P - v1;
    //C = cross(triangles_norms[i].norm1, P - v1);
    if (dot(triangles_norms[i].norm1, P - v1) < 0)
        return false;
    //vec3 edge2 = v3 - v2;
    //vec3 VP2 = P - v2;
    //C = cross(edge2, VP2);
    if (dot(triangles_norms[i].norm2, P - v2) < 0)
        return false;
    //vec3 edge3 = v1 - v3;
    //vec3 VP3 = P - v3;
    //C = cross(edge3, VP3);
    if (dot(triangles_norms[i].norm3, P - v3) < 0)
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
    vec3 color;
    vec4 lightCoeffs; // ambient, diffuse and specular coeffs
    float reflectionCoef;
    float refractionCoef;
    float refractionIndex;
    int MaterialType;
};

uniform SLight uLight;
uniform SMaterial materials[10];

bool Raytrace(SRay ray, float final, inout SIntersection intersect) {
    bool result = false;
    float test;
    intersect.time = final;
    //calculate intersect with spheres
    for (int i = 0; i < spheres_used; i++) {
        if (IntersectSphere(ray, spheres[i], test) && test < intersect.time) {
            intersect.time = test;
            intersect.point = ray.origin + ray.direction * test;
            intersect.normal = normalize(intersect.point - spheres[i].center);
            intersect.MaterialId = spheres[i].MaterialId;
            result = true;
        }
    }
    //calculate intersect with triangles
    for (int i = 0; i < triangles_used; i++) {
        if(IntersectTriangle(ray, i, test) && test < intersect.time) {
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
    if (Raytrace(shadowRay, distanceLight, shadowIntersect) && materials[shadowIntersect.MaterialId].MaterialType != LIGHT)
        shadowing = 0.1f;
    return shadowing;
}

vec3 Phong(SIntersection intersect, SLight currLight, float shadow, vec3 origin) {
    vec3 light = normalize(currLight.position - intersect.point);
    float diffuse = max(dot(light, intersect.normal), 0.0f);
    vec3 view = normalize(intersect.point - origin);
    vec3 reflected = reflect(view, intersect.normal);
    vec4 lightCoeffs = materials[intersect.MaterialId].lightCoeffs;
    vec3 color = materials[intersect.MaterialId].color;
    float specular = pow(max(dot(reflected, light), 0.0f), lightCoeffs.w);
    int unit = shadow == 1.0f ? 1 : 0;
    return lightCoeffs.x * color +
           lightCoeffs.y * diffuse * color * shadow +
           lightCoeffs.z * specular * unit;
}

struct STracingRay {
    SRay ray;
    float contribution;
    int depth;
};

STracingRay stack[11];
int stack_id = -1;

void pushRay(STracingRay trRay) {
    stack_id++;
    stack[stack_id] = trRay;
}

bool isEmpty() {
    return stack_id == -1;
}

STracingRay popRay() {
    return stack[stack_id--];
}

void INIT() {
    /*for (int i = 0; i < triangles_used; i++) {
        vec3 v1 = triangles[i].v1;
        vec3 v2 = triangles[i].v2;
        vec3 v3 = triangles[i].v3;
        triangles_norms[i].norm = cross(v2 - v1, v3 - v1);
        triangles_norms[i].norm1 = cross(triangles_norms[i].norm, v2 - v1);
        triangles_norms[i].norm2 = cross(triangles_norms[i].norm, v3 - v2);
        triangles_norms[i].norm3 = cross(triangles_norms[i].norm, v1 - v3);
    }*/
}

void main()
{
    //INIT();
    float final = BIG;
    SRay ray = GenerateRay();
    SIntersection intersect;
    intersect.time = BIG;
    vec3 resultColor = vec3(0.0f, 0.0f, 0.0f);

    STracingRay trRay = STracingRay(ray, 1.0f, 0);
    pushRay(trRay);
    while(!isEmpty()) {
        STracingRay trRay = popRay();
        ray = trRay.ray;
        SIntersection intersect;
        final = BIG;
        if (Raytrace(ray, final, intersect)) {
            SMaterial material = materials[intersect.MaterialId];
            switch (material.MaterialType) {
                case DEFAULT: {
                    float diffuse_contrib = trRay.contribution * (1.0f - material.reflectionCoef);
                    if (diffuse_contrib > EPSILON) {
                        float shadowing = Shadow(uLight, intersect);
                        resultColor += diffuse_contrib * Phong(intersect, uLight, shadowing, trRay.ray.origin);
                    }
                    if (trRay.depth >= MAX_DEPTH)
                        break;

                    float mirror_contrib = trRay.contribution * material.reflectionCoef;
                    if (mirror_contrib > EPSILON) {
                        vec3 reflectDirection = reflect(ray.direction, intersect.normal);
                        STracingRay reflectRay = STracingRay(SRay(intersect.point + reflectDirection * EPSILON, reflectDirection), mirror_contrib, trRay.depth + 1);
                        pushRay(reflectRay);
                    }
                    break;
                }
                case LIGHT: {
                    resultColor += trRay.contribution * material.color;
                    break;
                }
                case GLASS: {
                    float diffuse_contrib = trRay.contribution; // for specular light
                    float shadowing = Shadow(uLight, intersect);
                    resultColor += diffuse_contrib * Phong(intersect, uLight, shadowing, trRay.ray.origin);
                    if (trRay.depth >= MAX_DEPTH)
                        break;
                    
                    float eta = 1.0f / material.refractionIndex;
                    if (dot(ray.direction, intersect.normal) >= 0.0f) {
                        intersect.normal = -intersect.normal;
                        eta = material.refractionIndex / 1.0f;
                    }

                    float refract_contrib = trRay.contribution * material.refractionCoef;
                    if (refract_contrib > EPSILON) {
                        vec3 refractDirection = refract(ray.direction, intersect.normal, eta);
                        STracingRay refractRay = STracingRay(SRay(intersect.point + refractDirection * EPSILON, refractDirection), refract_contrib, trRay.depth + 1);
                        pushRay(refractRay);
                    }

                    float mirror_contrib = trRay.contribution * material.reflectionCoef;
                    if (mirror_contrib > EPSILON) {
                        vec3 reflectDirection = reflect(ray.direction, intersect.normal);
                        STracingRay reflectRay = STracingRay(SRay(intersect.point + reflectDirection * EPSILON, reflectDirection), mirror_contrib, trRay.depth + 1);
                        pushRay(reflectRay);
                    }
                    break;
                }
            }
        }
    }

    color = vec4(resultColor, 1.0);
}
