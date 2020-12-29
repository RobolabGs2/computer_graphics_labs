#include "shaders.h"
#include <GL\glut.h>
#include <fstream>

#define TO_STRING(x) MakeShader(#x)

std::string MakeShader(std::string main)
{
	static std::string common;
	if (common.length() == 0)
	{
		std::ifstream commonFile("common.glsl", std::ios::in);
		if (!commonFile.is_open())
			throw std::exception("Can't open common shader file!");
		common.assign((std::istreambuf_iterator<char>(commonFile)),
			(std::istreambuf_iterator<char>()));
	}
	return common + std::string(main);
}

namespace Shaders
{
	std::string simpleVertex = TO_STRING(
		in vec3 coord;
		in vec3 normal;
		in vec2 texture;
		uniform mat4 transform;
		uniform mat4 camera;
		uniform mat4 projection;
		uniform PointLight[LIGHT_COUNT] lights;

		out vec3 fragNormal;
		out PointLight[LIGHT_COUNT] light;
		out vec2 tex;

		void main() {
			vec4 b = vec4(coord, 1.0) * transform * camera;
			gl_Position = b * projection;
			vec4 k = vec4(normal, 1.0) * transform * camera - vec4(0.0, 0.0, 0.0, 1.0) * transform * camera;
			fragNormal = k.xyz;
			int activeLights = 0;
			for (int i = 0; i < LIGHT_COUNT; ++i) {
				if (lights[i].on) {
					light[activeLights].position = (b - vec4(lights[i].position, 1.0) * camera).xyz;
					activeLights = activeLights + 1;
				}
			}
			tex = texture;
		}
	);

	std::string colorTextureVertex = TO_STRING(
		in vec3 coord;
		in vec3 normal;
		in vec2 texture;
		in vec3 vcolor;
		uniform mat4 transform;
		uniform mat4 camera;
		uniform mat4 projection;
		uniform PointLight[LIGHT_COUNT] lights;

		out vec3 fragNormal;
		out vec3 frontColor;
		out PointLight[LIGHT_COUNT] light;
		out vec2 tex;

		void main() {
			vec4 b = vec4(coord, 1.0) * transform * camera;
			gl_Position = b * projection;
			vec4 k = vec4(normal, 1.0) * transform * camera - vec4(0.0, 0.0, 0.0, 1.0) * transform * camera;
			fragNormal = k.xyz;
			int activeLights = 0;
			for (int i = 0; i < LIGHT_COUNT; ++i) {
				if (lights[i].on) {
					light[activeLights].position = (b - vec4(lights[i].position, 1.0) * camera).xyz;
					activeLights = activeLights + 1;
				}
			}
			tex = texture;
			frontColor = vcolor;
		}
	);

	std::string simplePhong = TO_STRING(
		in vec3 fragNormal;
		in PointLight[LIGHT_COUNT] light;
		uniform vec3 color;
		uniform int activeLights;
		out vec4 FragColor;
	
		void main()
		{
			FragColor = vec4(Blinn(fragNormal, light, color, activeLights), 1.0);
		}
	);

	std::string simpleTexture = TO_STRING(
		in vec3 fragNormal;
		in PointLight[LIGHT_COUNT] light;
		in vec2 tex;
		uniform sampler2D texmap;
		uniform int activeLights;
		out vec4 FragColor;

		void main() {
			FragColor = vec4(Blinn(fragNormal, light, texture(texmap, tex).xyz, activeLights), 1.0);
		}
	);

	std::string colorTexture = TO_STRING(
		in vec3 fragNormal;
		in vec3 frontColor;
		in PointLight[LIGHT_COUNT] light;
		in vec2 tex;
		uniform sampler2D texmap;	
		uniform int activeLights;
		out vec4 FragColor;

		void main() {
			float k = 0.3;
			FragColor = vec4(Blinn(fragNormal, light,
				mix(texture(texmap, tex).xyz, frontColor*0.5, k), activeLights), 1.0);
		}
	);

	std::string textureTexture = TO_STRING(
		in vec3 fragNormal;
		uniform int activeLights;
		in vec2 tex;

		uniform sampler2D texmap;
		uniform sampler2D mixtex;
		uniform float intensity;
		in PointLight[LIGHT_COUNT] light;

		out vec4 FragColor;

		void main() {
			FragColor = vec4(Blinn(fragNormal, light, mix(
				texture(texmap, tex).xyz, 
				texture(mixtex, tex).xyz,
				intensity), activeLights), 1.0);
		}
	);
}