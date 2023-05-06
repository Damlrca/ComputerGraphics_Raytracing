#version 330 core
in vec4 glPosition;
out vec4 color;

//#define DOUBLE_SIDED_TRIANGLES
#define DOUBLE_SIDED_RECTANGLES
#define DOUBLE_SIDED_PENTAGONS
//#define DOUBLE_SIDED_PLANES

#define EPSILON 0.001
#define BIG 1000000.0
const int DEFAULT = 1;
const int LIGHT = 2;
const int GLASS = 3;
const int CHECKBOARD = 4;
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
    //vec3 v2, v3;
    int MaterialId;
    //
    vec3 norm;
    vec3 sp1;
    vec3 sp2;
    float d;
};

struct SRectangle {
    //vec3 v1, v3;
    vec3 v2, v4;
    int MaterialId;
    //
    vec3 norm;
    vec3 sp1, sp2, sp3, sp4;
};

struct SPentagon {
    //vec3 v1, v3;
    vec3 v2, v4, v5;
    int MaterialId;
    //
    vec3 norm;
    vec3 sp1, sp2, sp3, sp4, sp5;
};

struct SDodecahedron {
    int id_first;
    int id_shell;
};

struct SPlane {
    vec3 v1;
    //vec3 v2, v3;
    int MaterialId;
    //
    vec3 norm;
    //vec3 sp1;
    //vec3 sp2;
    //float d;
};

uniform STriangle triangles[12];
uniform int triangles_used;
uniform SSphere spheres[4];
uniform int spheres_used;
uniform SRectangle rectangles[12];
uniform int rectangles_used;
uniform SPentagon pentagons[12];
uniform int pentagons_used;
uniform SDodecahedron dodecahedra[1];
uniform int dodecahedra_used;
uniform SPlane planes[1];
uniform int planes_used;

bool IntersectSphere(SRay ray, int i, out float time) {
    time = -1;
    ray.origin -= spheres[i].center;
    float A = dot(ray.direction, ray.direction);
    float B = dot(ray.direction, ray.origin);
    float C = dot(ray.origin, ray.origin) - spheres[i].radius * spheres[i].radius;
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
    float NdotRay = dot(triangles[i].norm, ray.direction);
#ifdef DOUBLE_SIDED_TRIANGLES
    if (abs(NdotRay) < EPSILON)
        return false;
#else
    if (NdotRay > -EPSILON)
        return false;
#endif
    float t = dot(triangles[i].norm, triangles[i].v1 - ray.origin) / NdotRay;
    if (t < 0)
        return false;
    
    vec3 P = ray.origin + t * ray.direction;
    vec3 c = P - triangles[i].v1;

    float u = dot(triangles[i].sp1, c);
    if (u < 0 || u > triangles[i].d)
        return false;
    float v = dot(triangles[i].sp2, c);
    if (v < 0 || v + u > triangles[i].d)
        return false;

    time = t;
    return true;
}

bool IntersectRectangle(SRay ray, int i, out float time) {
    float NdotRay = dot(rectangles[i].norm, ray.direction);
#ifdef DOUBLE_SIDED_RECTANGLES
    if (abs(NdotRay) < EPSILON)
        return false;
#else
    if (NdotRay > -EPSILON)
        return false;
#endif
    float t = dot(rectangles[i].norm, rectangles[i].v2 - ray.origin) / NdotRay;
    if (t < 0)
        return false;

    vec3 P = ray.origin + t * ray.direction;

    vec3 temp = P - rectangles[i].v2;
    if (dot(rectangles[i].sp1, temp) < 0) return false;
    if (dot(rectangles[i].sp2, temp) < 0) return false;
    temp = P - rectangles[i].v4;
    if (dot(rectangles[i].sp3, temp) < 0) return false;
    if (dot(rectangles[i].sp4, temp) < 0) return false;

    time = t;
    return true;
}

bool IntersectPentagon(SRay ray, int i, out float time) {
    float NdotRay = dot(pentagons[i].norm, ray.direction);
#ifdef DOUBLE_SIDED_PENTAGONS
    if (abs(NdotRay) < EPSILON)
        return false;
#else
    if (NdotRay > -EPSILON)
        return false;
#endif
    float t = dot(pentagons[i].norm, pentagons[i].v2 - ray.origin) / NdotRay;
    if (t < 0)
        return false;

    vec3 P = ray.origin + t * ray.direction;

    vec3 temp = P - pentagons[i].v2;
    if (dot(pentagons[i].sp1, temp) < 0) return false;
    if (dot(pentagons[i].sp2, temp) < 0) return false;
    temp = P - pentagons[i].v4;
    if (dot(pentagons[i].sp3, temp) < 0) return false;
    if (dot(pentagons[i].sp4, temp) < 0) return false;
    if (dot(pentagons[i].sp5, P - pentagons[i].v5) < 0) return false;

    time = t;
    return true;
}

bool IntersectPlane(SRay ray, int i, out float time) {
    float NdotRay = dot(planes[i].norm, ray.direction);
#ifdef DOUBLE_SIDED_PLANES
    if (abs(NdotRay) < EPSILON)
        return false;
#else
    if (NdotRay > -EPSILON)
        return false;
#endif
    float t = dot(planes[i].norm, planes[i].v1 - ray.origin) / NdotRay;
    if (t < 0)
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
    float refractionIndex;
    int MaterialType;
};

uniform SLight uLight;
uniform SMaterial materials[12];

bool Raytrace(SRay ray, float final, inout SIntersection intersect) {
    bool result = false;
    float test;
    intersect.time = final;
    // spheres
    for (int i = 0; i < spheres_used; i++) {
        if (IntersectSphere(ray, i, test) && test < intersect.time) {
            intersect.time = test;
            intersect.point = ray.origin + ray.direction * test;
            intersect.normal = normalize(intersect.point - spheres[i].center);
            intersect.MaterialId = spheres[i].MaterialId;
            result = true;
        }
    }
    // triangles
    for (int i = 0; i < triangles_used; i++) {
        if(IntersectTriangle(ray, i, test) && test < intersect.time) {
            intersect.time = test;
            intersect.point = ray.origin + ray.direction * test;
            intersect.normal = triangles[i].norm;
            intersect.MaterialId = triangles[i].MaterialId;
            result = true;
        }
    }
    // rectangles
    for (int i = 0; i < rectangles_used; i++) {
        if(IntersectRectangle(ray, i, test) && test < intersect.time) {
            intersect.time = test;
            intersect.point = ray.origin + ray.direction * test;
            intersect.normal = rectangles[i].norm;
            intersect.MaterialId = rectangles[i].MaterialId;
            result = true;
        }
    }
    // pentagons
    for (int i = 0; i < pentagons_used; i++) {
        if(IntersectPentagon(ray, i, test) && test < intersect.time) {
            intersect.time = test;
            intersect.point = ray.origin + ray.direction * test;
            intersect.normal = pentagons[i].norm;
            intersect.MaterialId = pentagons[i].MaterialId;
            result = true;
        }
    }
    // dodecahderons
    for (int j = 0; j < dodecahedra_used; j++) {
        if (IntersectSphere(ray, dodecahedra[j].id_shell, test))
            for (int i = dodecahedra[j].id_first; i < dodecahedra[j].id_first + 12; i++) {
                if(IntersectPentagon(ray, i, test) && test < intersect.time) {
                    intersect.time = test;
                    intersect.point = ray.origin + ray.direction * test;
                    intersect.normal = pentagons[i].norm;
                    intersect.MaterialId = pentagons[i].MaterialId;
                    result = true;
                }
            }
    }
    // planes
    for (int i = 0; i < planes_used; i++) {
        if(IntersectPlane(ray, i, test) && test < intersect.time) {
            intersect.time = test;
            intersect.point = ray.origin + ray.direction * test;
            intersect.normal = planes[i].norm;
            intersect.MaterialId = planes[i].MaterialId;
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

vec3 Checkboard(vec3 color, vec2 xy) {
    int x = int(ceil(xy.x));
    int y = int(ceil(xy.y));
    if ((x + y) % 2 == 1)
        color *= 0.25f;
    else
        color *= 0.75f;
    return color;
}

vec3 Phong(SIntersection intersect, SLight currLight, float shadow, vec3 origin) {
    vec3 light = normalize(currLight.position - intersect.point);
    float diffuse = max(dot(light, intersect.normal), 0.0f);
    vec3 view = normalize(intersect.point - origin);
    vec3 reflected = reflect(view, intersect.normal);
    vec4 lightCoeffs = materials[intersect.MaterialId].lightCoeffs;
    vec3 color = (materials[intersect.MaterialId].MaterialType != CHECKBOARD ? materials[intersect.MaterialId].color : Checkboard(materials[intersect.MaterialId].color, intersect.point.xz));
    float specular = pow(max(dot(reflected, light), 0.0f), lightCoeffs.w);
    int unit = shadow == 1.0f ? 1 : 0;
    return lightCoeffs.x * color +
           lightCoeffs.y * diffuse * color * shadow +
           lightCoeffs.z * specular * unit;
}

// Schlick’s Fresnel approximation
// adoptation of https://blog.demofox.org/2017/01/09/raytracing-reflection-refraction-fresnel-total-internal-reflection-and-beers-law/
float Fresnel(float n1, float n2, float dp, int MaterialId) {
    float r0 = (n1 - n2) / (n1 + n2);
    r0 *= r0;
    float cosX = abs(dp);
    if (n1 > n2) {
        float n = n1 / n2;
        float sinT2 = n * n * (1.0f - cosX * cosX);
        // Total internal reflection
        if (sinT2 > 1.0)
            return 1.0;
        cosX = sqrt(1.0-sinT2);
    }
    float x = 1.0f - cosX;
    float ret = r0 + (1.0f - r0) * x * x * x * x * x;

    ret = materials[MaterialId].reflectionCoef + (1.0 - materials[MaterialId].reflectionCoef) * ret;
    return ret;
}

struct STracingRay {
    SRay ray;
    float contribution;
    int depth;
    vec3 absorb; // for Beer's law
};

STracingRay stack[21];
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

void main()
{
    float final = BIG;
    SRay ray = GenerateRay();
    SIntersection intersect;
    intersect.time = BIG;
    vec3 resultColor = vec3(0.0f, 0.0f, 0.0f);

    STracingRay trRay = STracingRay(ray, 1.0f, 0, vec3(1.0f));
    pushRay(trRay);
    while(!isEmpty()) {
        STracingRay trRay = popRay();
        ray = trRay.ray;
        SIntersection intersect;
        final = BIG;
        if (Raytrace(ray, final, intersect)) {
            SMaterial material = materials[intersect.MaterialId];
            switch (material.MaterialType) {
                case DEFAULT:
                case CHECKBOARD: {
                    float diffuse_contrib = trRay.contribution * (1.0f - material.reflectionCoef);
                    if (diffuse_contrib > EPSILON) {
                        float shadowing = Shadow(uLight, intersect);
                        resultColor += diffuse_contrib * Phong(intersect, uLight, shadowing, trRay.ray.origin) * trRay.absorb;
                    }
                    if (trRay.depth >= MAX_DEPTH)
                        break;

                    float dp = dot(ray.direction, intersect.normal);
                    if (dp >= 0.0f) {
                        intersect.normal = -intersect.normal;
                    }

                    float mirror_contrib = trRay.contribution * material.reflectionCoef;
                    if (mirror_contrib > EPSILON) {
                        vec3 reflectDirection = reflect(ray.direction, intersect.normal);
                        STracingRay reflectRay = STracingRay(SRay(intersect.point + reflectDirection * EPSILON, reflectDirection), mirror_contrib, trRay.depth + 1, trRay.absorb);
                        pushRay(reflectRay);
                    }
                } break;
                case LIGHT: {
                    resultColor += trRay.contribution * material.color * trRay.absorb;
                } break;
                case GLASS: {
                    float diffuse_contrib = trRay.contribution; // for specular light (in Phong)
                    float shadowing = Shadow(uLight, intersect);
                    resultColor += diffuse_contrib * Phong(intersect, uLight, shadowing, trRay.ray.origin) * trRay.absorb;
                    if (trRay.depth >= MAX_DEPTH)
                        break;
                    
                    vec3 absorb = trRay.absorb;
                    float n1 = 1.0f;
                    float n2 = material.refractionIndex;
                    float dp = dot(ray.direction, intersect.normal);
                    float eta = 1.0f / material.refractionIndex;
                    if (dp >= 0.0f) {
                        intersect.normal = -intersect.normal;
                        eta = material.refractionIndex / 1.0f;
                        n1 = material.refractionIndex;
                        n2 = 1.0f;
                        absorb *= exp(-material.color * intersect.time); // Beer's law
                    }

                    float fresnel = Fresnel(n1, n2, dp, intersect.MaterialId);

                    float refract_contrib = trRay.contribution * (1.0f - fresnel);
                    if (refract_contrib > EPSILON) {
                        vec3 refractDirection = refract(ray.direction, intersect.normal, eta);
                        STracingRay refractRay = STracingRay(SRay(intersect.point + refractDirection * EPSILON, refractDirection), refract_contrib, trRay.depth + 1, absorb);
                        pushRay(refractRay);
                    }

                    float mirror_contrib = trRay.contribution * fresnel;
                    if (mirror_contrib > EPSILON) {
                        vec3 reflectDirection = reflect(ray.direction, intersect.normal);
                        STracingRay reflectRay = STracingRay(SRay(intersect.point + reflectDirection * EPSILON, reflectDirection), mirror_contrib, trRay.depth + 1, absorb);
                        pushRay(reflectRay);
                    }
                } break;
            }
        }
    }

    color = vec4(resultColor, 1.0);
}
