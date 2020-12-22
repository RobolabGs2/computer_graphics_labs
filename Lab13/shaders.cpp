#include "shaders.h"
#include <GL\glut.h>

#define TO_STRING(x) #x

namespace Shaders
{
	const char* simpleVertex = TO_STRING(
		#version 330 core\n
		in vec3 coord;
		in vec3 normal;
		in vec2 texture;
		uniform mat4 transform;
		uniform mat4 camera;
		uniform mat4 projection;

		out vec3 fragNormal;
		out vec3 light;
		out vec2 tex;

		void main() {
			vec4 b = vec4(coord, 1.0) * transform * camera;
			vec4 r = b * projection;
			r.z = b.z / 10000.0;
			gl_Position = r;
			vec4 k = vec4(normal, 1.0) * transform * camera - vec4(0.0, 0.0, 0.0, 1.0) * transform * camera;
			fragNormal = k.xyz;
			vec4 l = vec4(0.0, -1.0, 1.0, 1.0);
			light = (l * camera - vec4(0.0, 0.0, 0.0, 1.0) * camera).xyz;
			tex = texture;
		}
	);

	const char* colorTextureVertex = TO_STRING(
		#version 330 core\n
		in vec3 coord;
		in vec3 normal;
		in vec2 texture;
		in vec3 vcolor;
		uniform mat4 transform;
		uniform mat4 camera;
		uniform mat4 projection;

		out vec3 fragNormal;
		out vec3 frontColor;
		out vec3 light;
		out vec2 tex;

		void main() {
			vec4 b = vec4(coord, 1.0) * transform * camera;
			vec4 r = b * projection;
			r.z = b.z / 10000.0;
			gl_Position = r;
			vec4 k = vec4(normal, 1.0) * transform * camera - vec4(0.0, 0.0, 0.0, 1.0) * transform * camera;
			fragNormal = k.xyz;
			vec4 l = vec4(0.0, -1.0, 1.0, 1.0);
			light = (l * camera - vec4(0.0, 0.0, 0.0, 1.0) * camera).xyz;
			tex = texture;
			frontColor = vcolor;
		}
	);

	const char* simplePhong = TO_STRING(
		#version 330 core\n
		in vec3 fragNormal;
		in vec3 light;

		out vec4 FragColor;
		
		vec3 Phong(vec3 norm_v, vec3 light_v, vec3 color_v)
		{
			light_v = normalize(light_v);
			norm_v = normalize(norm_v);

			float diff_cos = dot(-light_v, norm_v);
			float diffuse = max(0, diff_cos);

			vec3 ref_light = reflect(-light_v, norm_v);
			float ref_cos = max(0, dot(ref_light, vec3(0.0, 0.0, 1.0)));
			float reflection = pow(ref_cos, 100);

			return vec3(reflection, reflection, reflection)
				+ color_v * diffuse
				+ color_v * 0.1;
		}

		void main()
		{
			FragColor = vec4(Phong(fragNormal, light, vec3(1.0, 1.0, 1.0)), 1.0);
		}
	);

	const char* simpleTexture = TO_STRING(
		#version 330 core\n
		in vec3 fragNormal;
		in vec3 light;
		in vec2 tex;
		uniform sampler2D texmap;
		out vec4 FragColor;

		vec3 Phong(vec3 norm_v, vec3 light_v, vec3 color_v)
		{
			light_v = normalize(light_v);
			norm_v = normalize(norm_v);

			float diff_cos = dot(-light_v, norm_v);
			float diffuse = max(0, diff_cos);

			vec3 ref_light = reflect(-light_v, norm_v);
			float ref_cos = max(0, dot(ref_light, vec3(0.0, 0.0, 1.0)));
			float reflection = pow(ref_cos, 100);

			return vec3(reflection, reflection, reflection)
				+ color_v * diffuse
				+ color_v * 0.1;
		}

		void main() {
			FragColor = vec4(Phong(fragNormal, light, texture(texmap, tex).xyz), 1.0);
		}
	);

	const char* colorTexture = TO_STRING(
		#version 330 core\n
		in vec3 fragNormal;
		in vec3 frontColor;
		in vec3 light;
		in vec2 tex;
		uniform sampler2D texmap;
		out vec4 FragColor;

		vec3 Phong(vec3 norm_v, vec3 light_v, vec3 color_v)
		{
			light_v = normalize(light_v);
			norm_v = normalize(norm_v);

			float diff_cos = dot(-light_v, norm_v);
			float diffuse = max(0, diff_cos);

			vec3 ref_light = reflect(-light_v, norm_v);
			float ref_cos = max(0, dot(ref_light, vec3(0.0, 0.0, 1.0)));
			float reflection = pow(ref_cos, 100);

			return vec3(reflection, reflection, reflection)
				+ color_v * diffuse
				+ color_v * 0.1;
		}

		void main() {
			float k = 0.5;
			FragColor = vec4(Phong(fragNormal, light,
				mix(texture(texmap, tex).xyz, frontColor, k)), 1.0);
		}
	);

	const char* textureTexture = TO_STRING(
		#version 330 core\n
		in vec3 fragNormal;
		in vec3 light;
		in vec2 tex;

		uniform sampler2D texmap;
		uniform sampler2D mixtex;
		uniform float intensity;

		out vec4 FragColor;

		vec3 Phong(vec3 norm_v, vec3 light_v, vec3 color_v)
		{
			light_v = normalize(light_v);
			norm_v = normalize(norm_v);

			float diff_cos = dot(-light_v, norm_v);
			float diffuse = max(0, diff_cos);

			vec3 ref_light = reflect(-light_v, norm_v);
			float ref_cos = max(0, dot(ref_light, vec3(0.0, 0.0, 1.0)));
			float reflection = pow(ref_cos, 100);

			return vec3(reflection, reflection, reflection)
				+ color_v * diffuse
				+ color_v * 0.1;
		}

		void main() {
			FragColor = vec4(Phong(fragNormal, light, mix(
				texture(texmap, tex).xyz, 
				texture(mixtex, tex).xyz,
				intensity)), 1.0);
		}
	);

	const char* blinnTexture = TO_STRING(
		#version 330 core\n
		in vec3 fragNormal;
		in vec3 light;
		in vec2 tex;
		uniform sampler2D texmap;
		out vec4 FragColor;

		vec3 Blinn(vec3 norm_v, vec3 light_v, vec3 color_v)
		{
			vec3 eye = vec3(0.0, 0.0, 1.0);
			light_v = normalize(light_v);
			norm_v = normalize(norm_v);

			float diff_cos = dot(-light_v, norm_v);
			float diffuse = max(0, diff_cos);

			vec3 lv = -light_v - eye;
			vec3 h = lv / length(lv);
			float ref_cos = max(0, dot(norm_v, h));
			float reflection = pow(ref_cos, 100);

			return vec3(reflection, reflection, reflection)
				+ color_v * diffuse
				+ color_v * 0.1;
		}

		void main() {
			FragColor = vec4(Blinn(fragNormal, light, texture(texmap, tex).xyz), 1.0);
		}
	);

	const char* toonTexture = TO_STRING(
		#version 330 core\n
		in vec3 fragNormal;
		in vec3 light;
		in vec2 tex;
		uniform sampler2D texmap;
		out vec4 FragColor;

		vec3 Toon(vec3 norm_v, vec3 light_v, vec3 color_v)
		{
			vec3 eye = vec3(0.0, 0.0, 1.0);
			light_v = normalize(light_v);
			norm_v = normalize(norm_v);

			float diffuse = max(0, dot(-light_v, norm_v));
			vec3 toon;
			if (diffuse < 0.04)
				toon = color_v * 0.1;
			else if (diffuse < 0.4)
				toon = color_v * 0.5;
			else if (diffuse < 0.7)
				toon = color_v;
			else
				toon = color_v * 1.3;
			return toon;
		}

		void main() {
			FragColor = vec4(Toon(fragNormal, light, texture(texmap, tex).xyz), 1.0);
		}
	);

	const char* otherTexture = TO_STRING(
		#version 330 core\n
		in vec3 fragNormal;
		in vec3 light;
		in vec2 tex;
		uniform sampler2D texmap;
		out vec4 FragColor;

		vec3 Other(vec3 norm_v, vec3 light_v, vec3 color_v)
		{
			float k = 0.8;
			vec3 eye = vec3(0.0, 0.0, 1.0);
			light_v = normalize(light_v);
			norm_v = normalize(norm_v);
			float d1 = pow(max(dot(norm_v, -light_v), 0.0), 1.0 + k);
			float d2 = pow(1.0 - dot(norm_v, eye), 1.0 - k);

			return color_v * d1 * d2;
		}

		void main() {
			FragColor = vec4(Other(fragNormal, light, texture(texmap, tex).xyz), 1.0);
		}
	);
}

/*vec3 Other(vec3 norm_v, vec3 light_v, vec3 color_v)
		{
			float r0 = 10;
			float specular = 10;
			vec3 eye = vec3(0.0, 0.0, 1.0);
			light_v = normalize(light_v);
			norm_v = normalize(norm_v);

			float diff_cos = dot(-light_v, norm_v);
			float diffuse = max(0, diff_cos);


			vec3 h = normalize(-light_v - eye);
			float nh = dot(norm_v, h);
			float nl = dot(norm_v, -light_v);
			float nv = dot(norm_v, -eye);
			float r2 = specular * specular;
			float nh2 = nh * nh;
			float ex = -(1 - nh2) / (nh2 * r2);
			float d = exp(ex) / (r2 * nh2 * nh2);

			float f = mix(pow(1.0 - nv, 5.0), 1.0, r0);
			float x = 2.0 * nh / dot(-eye, h);
			float g = min(1.0, min(x * nl, x * nv));

			float ct = d * f * g / nv;
			float reflection = max(0.0, ct);

			return vec3(1.0, 1.0, 1.0) * reflection
				+ color_v * diffuse
				+ color_v * 0.1;
		}
*/