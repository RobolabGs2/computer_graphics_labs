#include "helper.h"
#include <GL\glew.h>
#include <iostream>
#include <exception>

void shaderLog(unsigned int shader)
{
	int infologLen = 0;
	int charsWritten = 0;
	char* infoLog;
	glGetShaderiv(shader, GL_INFO_LOG_LENGTH, &infologLen);
	if (infologLen > 1)
	{
		infoLog = new char[infologLen];
		glGetShaderInfoLog(shader, infologLen, &charsWritten, infoLog);
		std::cout << "shaderLog: InfoLog: " << infoLog << "\n\n\n";
		delete[] infoLog;
	}
}

void checkOpenGLerror()
{
	const GLenum errCode = glGetError();
	if (errCode != GL_NO_ERROR)
		std::cout << "checkOpenGLerror: OpenGl error! - " << gluErrorString(errCode);
}

GLuint getAttribLocation(const char* name, GLuint program)
{
	GLuint result = glGetAttribLocation(program, name);
	if (result == -1)
	{
		std::cout << "could not bind attrib " << name << std::endl;
	}
	return result;
}

GLuint getUniformLocation(const char* name, GLuint program)
{
	GLuint result = glGetUniformLocation(program, name);
	if (result == -1)
	{
		std::cout << "could not bind uniform " << name << std::endl;
	}
	return result;
}
