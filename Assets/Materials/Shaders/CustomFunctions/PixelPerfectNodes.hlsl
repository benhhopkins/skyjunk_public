// UNITY_SHADER_NO_UPGRADE
#ifndef PIXELPERFECTNODES_INCLUDED
#define PIXELPERFECTNODES_INCLUDED

void PixelPoint_float(float2 UV, float2 Position, out float Out) {
    float2 f = UV - Position;
    float2 ddxUV = ddx(UV);
    float2 ddyUV = ddy(UV);
	
	float InvD = 1/(ddxUV.x*ddyUV.y-ddxUV.y*ddyUV.x);
	float tx = (ddyUV.y*f.x-ddyUV.x*f.y)*InvD;
	float ty = (-ddxUV.y*f.x+ddxUV.x*f.y)*InvD;
    Out = (tx > -0.5 && tx <= 0.5) && (ty > -0.5 && ty <= 0.5) ? 1 : 0;
}

void PixelCircle_float(float2 UV, float2 Center, float Radius, float Width, out float Out) {
    float2 f = UV - Center;
    float2 ddxUV = ddx(UV);
    float2 ddyUV = ddy(UV);
	float r2 = Radius * Radius;
	
	float2 fx1, fx2, fy1, fy2;
	fx1 = f - (Width / 2)*ddxUV;
	fx2 = f + (Width / 2)*ddxUV;
	fy1 = f - (Width / 2)*ddyUV;
	fy2 = f + (Width / 2)*ddyUV;
	
    Out = ((dot(fx1, fx1) - r2) * (dot(fx2, fx2) - r2) <= 0 || (dot(fy1, fy1) - r2) * (dot(fy2, fy2) - r2) <= 0) ? 1 : 0;
}

#endif // PIXELPERFECTNODES_INCLUDED