#pragma once

#include "geometry.h"
#include "illumination.h"

#include <gl/glew.h>

struct RawSpotLight;

class ShaderManager
{
public:
	enum ShaderType {
		SIMPLE = 0,
		GOURAUD,
		ShaderCount
	};

	Matrix cameraMatrix = Matrix::Ident();
	Matrix projectionMatrix = Matrix::Ident();
	Matrix transformMatrix = Matrix::Ident();
private:

	struct ShaderProgram
	{
		GLuint unifCamera;
		GLuint unifProjection;
		GLuint unifTransform;
		GLuint unifLightCount;
		GLuint unifLightBlock;
		GLuint program;
	};

	GLuint	lightBlockIndex = -1;
	int		lightCount = -1;

	ShaderProgram programs[ShaderCount];

	void InitShader(ShaderType type ,const char* vShaderSource, const char* fShaderSource);

	ShaderManager(const ShaderManager&) = delete;
	ShaderManager(ShaderManager&&) = delete;
	ShaderManager& operator=(const ShaderManager&) = delete;
	ShaderManager& operator=(ShaderManager&&) = delete;

public:
	ShaderManager();

	void Initlight(const RawSpotLight* light, int count);
	void SetCamera(const Matrix& projection, const Matrix& transform);
	void SetTransform(const Matrix& transform);
	GLuint GetShader(ShaderType type);
	GLuint AttribLocation(const char* name, ShaderType program);
	GLuint UnifLocation(const char* name, ShaderType program);
	GLuint UnifBlockLocation(const char* name, ShaderType program);
	void Run(int meshSize);

	void UseShader(ShaderType program);
	void SetUniform(GLuint location, const Matrix& matrix);
	void SetUniform(GLuint location, const Color& color);
	void SetUniform(GLuint location, int value);
	void SetUniform(GLuint location, float value);
	void SetTexture0(GLuint location, GLuint texture);
	void SetTexture1(GLuint location, GLuint texture);

	void EnableArrayBuffer(GLuint vbo, GLuint location, int elemSize);
	void DisableArrayBuffer(GLuint location);

	~ShaderManager();
};
