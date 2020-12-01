// UNITY_SHADER_NO_UPGRADE
#ifndef SNOISE_3D
#define SNOISE_3D
#include "snoise_all.hlsl"

void snoise3d_float(float3 v, out float Out) { 
	Out = snoise3d(v);
}

#endif // SNOISE_3D