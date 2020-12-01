// UNITY_SHADER_NO_UPGRADE
#ifndef DITHERSAMPLERINCLUDE_INCLUDED
#define DITHERSAMPLERINCLUDE_INCLUDED

void DitherSamplerFunction_float(float steps, float3 In, float3 Dither, out float3 OutColor)
{
    In += Dither / (2.0 * steps) + 1.0 / (2.0 * steps);
    OutColor = floor(In * steps) / steps;
}

#endif // DITHERSAMPLERINCLUDE_INCLUDED