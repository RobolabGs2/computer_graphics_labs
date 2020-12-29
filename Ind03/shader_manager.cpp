#include "shader_manager.h"
#include "shaders.h"
#include "helper.h"

#include <iostream>


ShaderManager::ShaderManager()
{
	InitShader(SIMPLE, Shaders::simpleVertex, Shaders::simpleTexture);
	InitShader(GOURAUD, Shaders::gouraudVertex, Shaders::gouraudTexture);
}

void ShaderManager::SetCamera(const Matrix& projection, const Matrix& transform)
{
	cameraMatrix = transform;
	projectionMatrix = projection;
}

void ShaderManager::SetTransform(const Matrix& transform)
{
	transformMatrix = transform;
}

void ShaderManager::InitShader(ShaderType type, const char* vShaderSource, const char* fShaderSource)
{
	GLuint vShader = glCreateShader(GL_VERTEX_SHADER);
	glShaderSource(vShader, 1, &vShaderSource, NULL);
	glCompileShader(vShader);
	shaderLog(vShader);

	GLuint fShader = glCreateShader(GL_FRAGMENT_SHADER);
	glShaderSource(fShader, 1, &fShaderSource, NULL);
	glCompileShader(fShader);
	shaderLog(fShader);

	GLuint program = glCreateProgram();
	glAttachShader(program, vShader);
	glAttachShader(program, fShader);
	glLinkProgram(program);
	int link_ok;
	glGetProgramiv(program, GL_LINK_STATUS, &link_ok);
	if (!link_ok)
	{
		std::cout << "error attach shaders " << type << std::endl;
		return;
	}
	programs[type].program = program;
	programs[type].unifCamera = UnifLocation("camera", type);
	programs[type].unifProjection = UnifLocation("projection", type);
	programs[type].unifTransform = UnifLocation("transform", type);
	programs[type].unifLightCount = UnifLocation("light_count", type);
	programs[type].unifLightBlock = UnifBlockLocation("lightness", type);

	checkOpenGLerror();
}

void ShaderManager::Initlight(const RawSpotLight* light, int count)
{
	if (lightBlockIndex != -1)
	{
		glDeleteBuffers(1, &lightBlockIndex);
	}

	glGenBuffers(1, &lightBlockIndex);
	glBindBuffer(GL_UNIFORM_BUFFER, lightBlockIndex);
	glBufferData(GL_UNIFORM_BUFFER, count * sizeof(RawSpotLight), light, GL_STATIC_DRAW);
	glBindBuffer(GL_UNIFORM_BUFFER, 0);
	checkOpenGLerror();

	lightCount = count;
}

GLuint ShaderManager::GetShader(ShaderType type)
{
	return programs[type].program;
}

GLuint ShaderManager::AttribLocation(const char* name, ShaderType program)
{
	GLuint result = glGetAttribLocation(GetShader(program), name);
	if (result == -1)
	{
#ifdef _DEBUG
		std::cout << "could not bind attrib " << name << std::endl;
#else	// _DEBUG
		throw std::exception((std::string("could not bind attrib ") + name).c_str());
#endif	// _DEBUG
	}
	return result;
}

GLuint ShaderManager::UnifLocation(const char* name, ShaderType program)
{
	GLuint result = glGetUniformLocation(GetShader(program), name);
	if (result == -1)
	{
#ifdef _DEBUG
		std::cout << "could not bind uniform " << name << std::endl;
#else	// _DEBUG
		throw std::exception((std::string("could not bind uniform ") + name).c_str());
#endif	// _DEBUG
	}
	return result;
}

GLuint ShaderManager::UnifBlockLocation(const char* name, ShaderType program)
{
	GLuint result = glGetUniformBlockIndex(GetShader(program), name);
	if (result == -1)
	{
#ifdef _DEBUG
		std::cout << "could not bind uniform " << name << std::endl;
#else	// _DEBUG
		throw std::exception((std::string("could not bind uniform ") + name).c_str());
#endif	// _DEBUG
	}
	return result;
}

void ShaderManager::SetUniform(GLuint location, const Matrix& matrix)
{
	glUniformMatrix4fv(location, 1, GL_TRUE, matrix.Data());
	checkOpenGLerror();
}

void ShaderManager::SetUniform(GLuint location, const Color& color)
{
	glUniform3fv(location, 1, &color.r);
	checkOpenGLerror();
}

void ShaderManager::SetUniform(GLuint location, int value)
{
	glUniform1i(location, value);
	checkOpenGLerror();
}

void ShaderManager::SetUniform(GLuint location, GLfloat value)
{
	glUniform1f(location, value);
	checkOpenGLerror();
}

void ShaderManager::SetTexture0(GLuint location, GLuint texture)
{
	glActiveTexture(GL_TEXTURE0);
	glBindTexture(GL_TEXTURE_2D, texture);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
	glUniform1i(location, 0);
	checkOpenGLerror();
}

void ShaderManager::SetTexture1(GLuint location, GLuint texture)
{
	glActiveTexture(GL_TEXTURE1);
	glBindTexture(GL_TEXTURE_2D, texture);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
	glUniform1i(location, 1);
	checkOpenGLerror();
}

void ShaderManager::UseShader(ShaderType program)
{
	glUseProgram(programs[program].program);
	SetUniform(programs[program].unifCamera, cameraMatrix);
	SetUniform(programs[program].unifProjection, projectionMatrix);
	SetUniform(programs[program].unifTransform, transformMatrix);
	glBindBufferBase(GL_UNIFORM_BUFFER, programs[program].unifLightBlock, lightBlockIndex);
	SetUniform(programs[program].unifLightCount, lightCount);
	checkOpenGLerror();
}

void ShaderManager::Run(int meshSize)
{
	glBindBuffer(GL_ARRAY_BUFFER, 0);
	glDrawArrays(GL_TRIANGLES, 0, meshSize);
	checkOpenGLerror();
}

void ShaderManager::EnableArrayBuffer(GLuint vbo, GLuint location, int elemSize)
{
	glEnableVertexAttribArray(location);
	glBindBuffer(GL_ARRAY_BUFFER, vbo);
	glVertexAttribPointer(location, elemSize, GL_FLOAT, GL_FALSE, 0, 0);
	checkOpenGLerror();
}

void ShaderManager::DisableArrayBuffer(GLuint location)
{
	glDisableVertexAttribArray(location);
	checkOpenGLerror();
}

ShaderManager::~ShaderManager()
{
	glUseProgram(0);
	for (int i = 0; i < ShaderCount; ++i)
		glDeleteProgram(programs[i].program);
}
