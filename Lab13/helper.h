#pragma once

#include <GL\glew.h>

void shaderLog(unsigned int shader);
void checkOpenGLerror();
GLuint getAttribLocation(const char* name, GLuint program);
GLuint getUniformLocation(const char* name, GLuint program);
