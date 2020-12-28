#include "shaders.h"

#define TO_STRING(x) #x

namespace Shaders
{
	const char* simpleVertex = TO_STRING(
		#version 330 core\n
		in vec3 coord;
		in vec3 normal;
		in vec2 texture;

		uniform mat4 camera;
		uniform mat4 projection;
		uniform mat4 transform;

		out vec3 fragNormal;
		out vec3 light;
		out vec2 tex;

		void main() {
			vec4 b = vec4(coord, 1.0) * transform * camera* projection;
			gl_Position = b;
			vec4 k = vec4(normal, 1.0) * transform * camera - vec4(0.0, 0.0, 0.0, 1.0) * transform * camera;
			fragNormal = k.xyz;
			vec4 l = vec4(0.0, -1.0, 1.0, 1.0);
			light = (l * camera - vec4(0.0, 0.0, 0.0, 1.0) * camera).xyz;
			tex = texture;
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
}
