#pragma once

#include "geometry.h"

#include <gl/glew.h>

class ShaderManager
{
public:
	enum ShaderType {
		SIMPLE = 0,
		ShaderCount
	};

private:
	Matrix cameraMatrix = Matrix::Ident();
	Matrix projectionMatrix = Matrix::Ident();
	Matrix transformMatrix = Matrix::Ident();

	struct ShaderProgram
	{
		GLuint unifCamera;
		GLuint unifProjection;
		GLuint unifTransform;
		GLuint program;
	};

	ShaderProgram programs[ShaderCount];

	void InitShader(ShaderType type ,const char* vShaderSource, const char* fShaderSource);

	ShaderManager(const ShaderManager&) = delete;
	ShaderManager(ShaderManager&&) = delete;
	ShaderManager& operator=(const ShaderManager&) = delete;
	ShaderManager& operator=(ShaderManager&&) = delete;

public:
	ShaderManager();

	void SetCamera(const Matrix& projection, const Matrix& transform);
	void SetTransform(const Matrix& transform);
	GLuint GetShader(ShaderType type);
	GLuint AttribLocation(const char* name, ShaderType program);
	GLuint UnifLocation(const char* name, ShaderType program);
	void Run(int meshSize);

	void UseShader(ShaderType program);
	void SetUniform(GLuint location, const Matrix& matrix);
	void SetTexture0(GLuint location, GLuint texture);
	void SetTexture1(GLuint location, GLuint texture);

	void EnableArrayBuffer(GLuint vbo, GLuint location, int elemSize);
	void DisableArrayBuffer(GLuint location);

	~ShaderManager();
};
