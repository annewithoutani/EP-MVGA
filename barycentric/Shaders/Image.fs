uniform sampler2D iChannel0;
uniform sampler2D iChannel1;
uniform sampler2D iChannel2;
uniform float iTime;
uniform vec2 iResolution;
uniform vec4 iMouse;
#define EPS  .01
#define PI 3.14159265359
#define COL0 vec3(.2, .35, .55)
#define COL1 vec3(.9, .43, .34)
#define COL2 vec3(.96, .66, .13)
#define COL3 vec3(0.0)
#define COL4 vec3(0.99,0.1,0.09)
 

/*  Install  Istructions

sudo apt-get install g++ cmake git
 sudo apt-get install libsoil-dev libglm-dev libassimp-dev libglew-dev libglfw3-dev libxinerama-dev libxcursor-dev
libxi-dev libfreetype-dev libgl1-mesa-dev xorg-dev

git clone https://github.com/JoeyDeVries/LearnOpenGL.git*/

float df_circ(in vec2 p, in vec2 c, in float r)
{
    return abs(r - length(p - c));
}

// Find the intersection of "p" onto "ab".
vec2 intersect (vec2 p, vec2 a, vec2 b)
{
    // Calculate the unit vector from "a" to "b".
    vec2 ba = normalize(b - a);

    // Calculate the intersection of p onto "ab" by
    // calculating the dot product between the unit vector
    // "ba" and the direction vector from "a" to "p", then
    // this value is multiplied by the unit vector "ab"
    // fired from the point "a".
    return a + ba * dot(ba, p - a);
}


// Visual line for debugging purposes.
bool line (vec2 p, vec2 a, vec2 b)
{
    // Direction from a to b.
    vec2 ab = normalize(b - a);

    // Direction from a to the pixel.
    vec2 ap = p - a;

    // Find the intersection of the pixel on to vector
    // from a to b, calculate the distance between the
    // pixel and the intersection point, then compare
    // that distance to the line width.
    return length((a + ab * dot(ab, ap)) - p) < 0.0025;
}

float df_line(in vec2 p, in vec2 a, in vec2 b)
{
    vec2 pa = p - a, ba = b - a;
        float h = clamp(dot(pa,ba) / dot(ba,ba), 0., 1.);
        return length(pa - ba * h);
}

float sharpen(in float d, in float w)
{
    float e = 1. / min(iResolution.y , iResolution.x);
    return 1. - smoothstep(-e, e, d - w);
}

vec3 bary(in vec2 a, in vec2 b, in vec2 c, in vec2 p)
{
    float denominator = ((b.x*c.y - c.x*b.y) + (b.y - c.y)*a.x + (c.x - b.x)*a.y);
    float baryA = ((b.x*c.y - c.x*b.y) + (b.y - c.y)*p.x + (c.x - b.x)*p.y) / denominator;
    float baryB = ((c.x*a.y - a.x*c.y) + (c.y - a.y)*p.x + (a.x - c.x)*p.y) / denominator;
    float baryC = ((a.x*b.y - b.x*a.y) + (a.y - b.y)*p.x + (b.x - a.x)*p.y) / denominator;
    
    return vec3(baryA, baryB, baryC);
}

bool test(in vec2 a, in vec2 b, in vec2 c, in vec2 p, inout vec3 barycoords)
{
    barycoords = bary(vec2(a.x, a.y),
                      vec2(b.x, b.y),
                      vec2(c.x, c.y),
                      vec2(p.x, p.y));

    return barycoords.x > 0. && barycoords.y > 0. && barycoords.z > 0.;
}

int get_section(in vec2 a, in vec2 b, in vec2 c, in vec2 p) {
    vec3 barycoords = bary(vec2(a.x, a.y),
                           vec2(b.x, b.y),
                           vec2(c.x, c.y),
                           vec2(p.x, p.y));
    if(!(barycoords.x < 0 || barycoords.y < 0 || barycoords.z < 0))
        return 0; //dentro do triangulo
    if(!(barycoords.x < 0 || barycoords.y < 0) && barycoords.z < 0)
        return 1; //fora ab
    if(!(barycoords.x < 0 || barycoords.z < 0) && barycoords.y < 0)
        return 2; //fora ac
    if(!(barycoords.y < 0 || barycoords.z < 0) && barycoords.x < 0)
        return 3; //fora bc
    if(!(barycoords.x < 0) && barycoords.y < 0 && barycoords.z < 0)
        return 4; //fora a
    if(!(barycoords.y < 0) && barycoords.z < 0 && barycoords.x < 0)
        return 5; //fora b
    if(!(barycoords.z < 0) && barycoords.y < 0 && barycoords.x < 0)
        return 6; //fora c
}

float df_bounds(in vec2 uv, in vec2 p, in vec2 a, in vec2 b, in vec2 c, in vec3 barycoords)
{
    float cp = 0.;


    float c0 = sharpen(df_circ(uv, p, (.03 + cos(15.*iTime) *.01)), EPS * 1.);


    return cp;
}


vec3 globalColor (in vec2 uv, in vec2 a, in vec2 b, in vec2 c)
{
    vec3 r=vec3(1.0);

    return r;
}

float dist_01(vec2 p,float r)
{
    float d = length(p);
    return smoothstep(r,r+0.01,d);
}



void main()
{
    float ar = iResolution.x / iResolution.y;
        vec2 mc=vec2(0.0); //coordensdss do mouse
        vec2 uv = (gl_FragCoord.xy / iResolution.xy * 2. - 1.) * vec2(ar, 1.);
        //coordenada normalizada do pixel
        mc = (iMouse.xy    / iResolution.xy * 2. - 1.) * vec2(ar, 1.);
        //mc fica normalizada

    vec2 a = vec2( .73,  .75); //vermelho
    vec2 b = vec2(-.85,  .15); //verde
    vec2 c = vec2( .25, -.75); //amarelo
    vec3 barycoords;

    bool t0 = test(a, b, c, mc, barycoords);
    float l = 0.1;
    //df_bounds(uv, mc, a, b, c, barycoords);

    bool t1 = test(a, b, c, uv, barycoords);
    vec3 r = globalColor(uv, a, b, c);
    bool testcc = false;//t1;
    vec3 color=vec3(0.0);
    // Visual debug lines and points.
    if (line(uv, a, b))
        color = vec3(1.0, 1.0, 0.0); //lado ab (azul)
    if (line(uv, b, c))
        color = vec3(1.0, 0.0, 1.0); //lado bc (verde)
    if (line(uv, c, a))
        color = vec3(0.0, 1.0, 1.0); //lado ac (vermelho)
    if (df_circ(uv, a, EPS)<0.5*EPS) 
        color = vec3(0.0, 1.0, 0.0); //ponto a (magenta)
    if (df_circ(uv, b, EPS)<0.5*EPS)
        color = vec3(1.0, 0.0, 0.0); //ponto b (ciano)
    if (df_circ(uv, c, EPS)<0.5*EPS)
        color = vec3(0.0, 0.0, 1.0); //ponto c (amarelo)

    //if(!(barycoords.x < 0 || barycoords.y < 0 || barycoords.z < 0)) {
    //    color = vec3(barycoords.x, barycoords.y, barycoords.z);
    //}
    int ms = get_section(a, b, c, mc);
    int ps = get_section(a, b, c, uv);

    if(ms == ps)
        color = vec3(0.0, 1.0, 0.5);

    vec3 col = l > 0. ? ( vec3(1)-color) : (t1 ? r : (t0 ? COL3+color : COL2-color));

        /*vec3 c1=vec3(1.0,0.4,0.1);
        vec3 c2=vec3(1.0,0.8,0.1);
        vec2 p =gl_FragCoord.xy/iResolution.xy;
        p.x*=iResolution.x/iResolution.y;
        vec3 col = mix(c1,c2,p.y);
        vec2 q =p-vec2(0.35,0.7);
        float r =0.1;
        r+=0.045*cos(atan(q.x,q.y)*13-30.0*q.x +sin(iTime*3.14159));
        r+=0.01*sin(atan(q.x,q.y)*120.0);
        col*=dist_01(q,r);

        vec2 x = p-vec2(0.8, 0.2+0.4*cos(2.0*PI*iTime*0.0125));
        r=0.2;
        r+=0.01*cos(atan(x.x,x.y)*300.0);
        col+=vec3((1-dist_01(x,r))*0.25);

        r=0.012;
        r+=0.002*cos(q.y*150);
        r+=exp(-40.0*p.y);
        col*=1.0-(1.0-smoothstep(r,r+EPS,abs(q.x-0.075*sin(q.y*3.0))))*(1.0-smoothstep(0.0,0.1,q.y));
        */


        // float f= 0.1+smoothstep(0,0.6,0.6-x.y);
        gl_FragColor = vec4(col, 1);
}
