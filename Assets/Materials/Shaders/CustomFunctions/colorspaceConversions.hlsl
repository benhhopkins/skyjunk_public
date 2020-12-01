// UNITY_SHADER_NO_UPGRADE
#ifndef RGBTOHSL_INCLUDED
#define RGBTOHSL_INCLUDED

float Epsilon = 1e-10;

void HSVtoHSL_float(in float3 hsv, out float3 hsl)
{
	hsl = hsv;
	hsl.z = hsv.z - (hsv.z * hsv.y) / 2;
	float delta = min(hsl.z, 1 - hsl.z);
	if(delta == 0)
		hsl.y = 0;
	else
		hsl.y = (hsv.z - hsl.z) / delta;
}

#endif // RGBTOHSL_INCLUDED