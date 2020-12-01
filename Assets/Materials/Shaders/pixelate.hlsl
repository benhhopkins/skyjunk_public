// UNITY_SHADER_NO_UPGRADE
#ifndef DITHERSAMPLERINCLUDE_INCLUDED
#define DITHERSAMPLERINCLUDE_INCLUDED

void UVDither_float(float3 In, float2 uv, float scale)
{
    In += Dither / (2.0 * steps) + In / (2.0 * steps);
    
    float3 remapped = pow(abs(In), .7);
    
    remapped = floor(remapped * steps) / steps;
    OutColor = remapped * remapped + 0.01;
}

#endif // DITHERSAMPLERINCLUDE_INCLUDED