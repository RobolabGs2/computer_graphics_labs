#version 330 core
#define LIGHT_COUNT 3

struct PointLight{
	vec3 position;
	bool on;
};

vec3 Blinn(vec3 norm_v,PointLight[LIGHT_COUNT]light,vec3 color_v,int activeLights)
{
	vec3 eye=vec3(0.,0.,1.);
	norm_v=normalize(norm_v);
	vec3 result=color_v*.1;
	for(int i=0;i<activeLights;++i){
		vec3 light_v=normalize(light[i].position);
		float diff_cos=dot(-light_v,norm_v);
		float diffuse=max(0,diff_cos);
		vec3 lv=-light_v-eye;
		vec3 h=lv/length(lv);
		float ref_cos=max(0,dot(norm_v,h));
		float reflection=pow(ref_cos,500);
		result=result+vec3(reflection,reflection,reflection)
		+color_v*diffuse;
	}
	return result;
}