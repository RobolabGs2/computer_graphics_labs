#include "helper.h"
#include <GL\glew.h>
#include <iostream>
#include <exception>

void shaderLog(unsigned int shader)
{
#ifdef _DEBUG
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
#endif	// _DEBUG
}

void checkOpenGLerror()
{
	GLenum errCode;
	if ((errCode = glGetError()) != GL_NO_ERROR)
#ifdef _DEBUG
		std::cout << "checkOpenGLerror: OpenGl error! - " << gluErrorString(errCode);
#else	// _DEBUG
		throw std::exception((std::string("OpenGl error! - ") + (char*)gluErrorString(errCode)).c_str());
#endif	// _DEBUG
}

GLuint getAttribLocation(const char* name, GLuint program)
{
	GLuint result = glGetAttribLocation(program, name);
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

GLuint getUniformLocation(const char* name, GLuint program)
{
	GLuint result = glGetUniformLocation(program, name);
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
