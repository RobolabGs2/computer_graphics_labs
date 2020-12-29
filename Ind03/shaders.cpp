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
		out vec3 fragPosition;
		out vec2 tex;

		void main() {
			vec4 b = vec4(coord, 1.0) * transform * camera;
			fragPosition = b.xyz;
			gl_Position = b * projection;
			vec4 k = vec4(normal, 1.0) * transform * camera - vec4(0.0, 0.0, 0.0, 1.0) * transform * camera;
			fragNormal = k.xyz;
			tex = texture;
		}
	);

	const char* simpleTexture = TO_STRING(
		#version 330 core\n
		in vec3 fragNormal;
		in vec3 fragPosition;
		in vec2 tex;

		struct Spotlight
		{
			float lx;
			float ly;
			float lz;
			float d;
			float vx;
			float vy;
			float vz;
			float a;
		};

		uniform lightness
		{
			Spotlight[1] spotlight;
		};

		uniform int light_count;

		uniform sampler2D texmap;
		uniform sampler2D texmap2;
		uniform vec3 fill_color;

		uniform float texmap_k;
		uniform float texmap2_k;
		uniform float fill_color_k;


		out vec4 FragColor;

		vec2 Blinn(vec3 norm_v, vec3 light_v) {
			vec3 eye = vec3(0.0, 0.0, 1.0);
			light_v = normalize(light_v);
			norm_v = normalize(norm_v);

			float diff_cos = dot(-light_v, norm_v);
			float diffuse = max(0, diff_cos);

			vec3 lv = -light_v - eye;
			vec3 h = lv / length(lv);
			float ref_cos = max(0, dot(norm_v, h));
			float reflection = pow(ref_cos, 100);

			return vec2(diffuse, reflection);
		}

		void main() {
			vec3 texture_color = vec3(0, 0, 0);

			if (texmap_k > 0) texture_color += texmap_k * texture(texmap, tex).xyz;
			if (texmap2_k > 0) texture_color += texmap2_k * texture(texmap2, tex).xyz;
			if (fill_color_k > 0) texture_color += fill_color_k * fill_color;

			vec2 result = vec2(0, 0);

			for (int i = 0; i < light_count; ++i)
			{
				vec3 l_vector = normalize(fragPosition - vec3(spotlight[i].lx, spotlight[i].ly, spotlight[i].lz));
				vec3 l_direction = vec3(spotlight[i].vx, spotlight[i].vy, spotlight[i].vz);
				vec2 phong = Blinn(fragNormal, l_vector);
				float l_cos = dot(l_vector, l_direction);
				if (l_cos > spotlight[i].a)
					result += phong * mix(1 - (1 - l_cos) / (1 - spotlight[i].a), 1, spotlight[i].d);
			}

			FragColor = vec4(texture_color * (result.x + 0.1) + vec3(1, 1, 1) * result.y, 1.0);
		}
	);


	//	*****************************************  //
	//	**              Gouraud                **  //
	//	*****************************************  //

	const char* gouraudVertex = TO_STRING(
		#version 330 core\n
		in vec3 coord;
		in vec3 normal;
		in vec2 texture;

		uniform mat4 camera;
		uniform mat4 projection;
		uniform mat4 transform;

		struct Spotlight
		{
			float lx;
			float ly;
			float lz;
			float d;
			float vx;
			float vy;
			float vz;
			float a;
		};

		uniform lightness
		{
			Spotlight[1] spotlight;
		};

		uniform int light_count;

		out float illumination;
		out float glare;
		out vec2 tex;

		vec2 Blinn(vec3 norm_v, vec3 light_v) {
			vec3 eye = vec3(0.0, 0.0, 1.0);
			light_v = normalize(light_v);
			norm_v = normalize(norm_v);

			float diff_cos = dot(-light_v, norm_v);
			float diffuse = max(0, diff_cos);

			vec3 lv = -light_v - eye;
			vec3 h = lv / length(lv);
			float ref_cos = max(0, dot(norm_v, h));
			float reflection = pow(ref_cos, 100);

			return vec2(diffuse, reflection);
		}

		void main() {
			vec4 b = vec4(coord, 1.0) * transform * camera;
			vec3 fragPosition = b.xyz;
			gl_Position = b * projection;
			vec4 k = vec4(normal, 1.0) * transform * camera - vec4(0.0, 0.0, 0.0, 1.0) * transform * camera;
			vec3 fragNormal = k.xyz;
			tex = texture;

			vec2 result = vec2(0, 0);

			for (int i = 0; i < light_count; ++i)
			{
				vec3 l_vector = normalize(fragPosition - vec3(spotlight[i].lx, spotlight[i].ly, spotlight[i].lz));
				vec3 l_direction = vec3(spotlight[i].vx, spotlight[i].vy, spotlight[i].vz);
				vec2 phong = Blinn(fragNormal, l_vector);
				float l_cos = dot(l_vector, l_direction);
				if (l_cos > spotlight[i].a)
					result += phong * mix(1 - (1 - l_cos) / (1 - spotlight[i].a), 1, spotlight[i].d);
			}
			illumination = result.x;
			glare = result.x;
		}
	);

	const char* gouraudTexture = TO_STRING(
		#version 330 core\n
		in float illumination;
		in float glare;
		in vec2 tex;

		uniform sampler2D texmap;
		uniform sampler2D texmap2;
		uniform vec3 fill_color;

		uniform float texmap_k;
		uniform float texmap2_k;
		uniform float fill_color_k;

		out vec4 FragColor;

		void main() {
			vec3 texture_color = vec3(0, 0, 0);

			if (texmap_k > 0) texture_color += texmap_k * texture(texmap, tex).xyz;
			if (texmap2_k > 0) texture_color += texmap2_k * texture(texmap2, tex).xyz;
			if (fill_color_k > 0) texture_color += fill_color_k * fill_color;

			FragColor = vec4(texture_color * (illumination + 0.1) + vec3(1, 1, 1) * glare, 1.0);
		}
	);
}
