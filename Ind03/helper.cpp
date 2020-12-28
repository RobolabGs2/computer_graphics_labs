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
