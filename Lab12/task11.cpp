#include <Windows.h>
#include <GL\glew.h>
#include <GL\freeglut.h>
#include <iostream>

using namespace std;



GLuint Program;

GLint Attrib_vertex;

GLint Unif_color;
GLint Unif_angle;

float rotate_x = 0.0f;
float rotate_y = 0.0f;
float rotate_z = 0.0f;

void checkOpenGLerror() {
	GLenum errCode;
	if ((errCode = glGetError()) != GL_NO_ERROR)
		std::cout << "OpenGl error! - " << gluErrorString(errCode) << "\n";
}


void freeShader()
{
	
	glUseProgram(0);
	
	glDeleteProgram(Program);
}
void resizeWindow(int width, int height)
{
	glViewport(0, 0, width, height);
}

void specialKeys(int key, int x, int y) {

	switch (key) {
	case GLUT_KEY_UP: rotate_x += 5; break;
	case GLUT_KEY_DOWN: rotate_x -= 5; break;
	case GLUT_KEY_RIGHT: rotate_y += 5; break;
	case GLUT_KEY_LEFT: rotate_y -= 5; break;
	case GLUT_KEY_PAGE_UP: rotate_z += 5; break;
	case GLUT_KEY_PAGE_DOWN: rotate_z -= 5; break;
	}
	glutPostRedisplay();
}


void renderTriangle()
{
	glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

	glLoadIdentity();
	
	//glRotatef(rotate_z, 0.0, 0.0, 1.0);
	
	glUseProgram(Program);
	glUniform1f(Unif_angle, rotate_z);
	glBegin(GL_TRIANGLES);
	glColor3f(1.0, 0.0, 0.0);
	glVertex2f(-0.5f, -0.5f);
	glColor3f(0.0, 1.0, 0.0);
	glVertex2f(0.5f, -0.5f);
	glColor3f(0.0, 0.0, 1.0);
	glVertex2f(0.0f, 0.5f);
	glEnd();

	glFlush();

	
	glUseProgram(0);
	checkOpenGLerror();
	glutSwapBuffers();
}


void initShader()
{
	const char* vsSource = R"(
			attribute vec2 coord;
			uniform float angle;
			
			mat2 rot(in float a) {
				return mat2(cos(a), sin(a), -sin(a), cos(a));
			}
			
			void main() {
				 vec2 pos = rot(3.14*angle/100)*coord;
				 gl_Position = vec4(pos, 0.0, 1.0);
				 gl_FrontColor = gl_Color;
			}
		)";
	const char* fsSource = R"(
			void main() {
				gl_FragColor = gl_Color;
			}
		)";
	
	GLuint  vShader, fShader;
	
	vShader = glCreateShader(GL_VERTEX_SHADER);
	
	glShaderSource(vShader, 1, &vsSource, NULL);
	
	glCompileShader(vShader);
	int compile_ok;
	glGetShaderiv(vShader, GL_COMPILE_STATUS, &compile_ok);
	
	fShader = glCreateShader(GL_FRAGMENT_SHADER);
	
	glShaderSource(fShader, 1, &fsSource, NULL);
	
	glCompileShader(fShader);
	int compile_ok2;
	glGetShaderiv(fShader, GL_COMPILE_STATUS, &compile_ok2);
	if (!compile_ok || !compile_ok2)
	{
		std::cout << "error compile shaders \n";
		int loglen;
		glGetShaderiv(fShader, GL_INFO_LOG_LENGTH, &loglen);
		int realLogLen;
		char* log = new char[loglen];
		glGetShaderInfoLog(fShader, loglen, &realLogLen, log);
		std::cout << log << "\n";
		delete[] log;
	}
	
	Program = glCreateProgram();
	glAttachShader(Program, vShader);
	glAttachShader(Program, fShader);
	
	glLinkProgram(Program);
	
	int link_ok;
	glGetProgramiv(Program, GL_LINK_STATUS, &link_ok);
	if (!link_ok)
	{
		std::cout << "error attach shaders \n";
	}
	
	const char* attr_name = "coord";
	Attrib_vertex = glGetAttribLocation(Program, attr_name);
	if (Attrib_vertex == -1)
	{
		std::cout << "could not bind attrib " << attr_name << std::endl;
		return;
	}
	
	const char* unif_name = "angle";
	Unif_angle = glGetUniformLocation(Program, unif_name);
	if (Unif_angle == -1)
	{
		std::cout << "could not bind uniform " << unif_name << std::endl;
		return;
	}
	checkOpenGLerror();
}

int main(int argc, char** argv)
{
	setlocale(LC_ALL, "");
	glutInit(&argc, argv);
	glutInitDisplayMode(GLUT_DEPTH | GLUT_RGBA | GLUT_ALPHA | GLUT_DOUBLE);
	glutInitWindowSize(800, 800);
	glutCreateWindow("Simple shaders");
	glClearColor(0.25, 0.25, 0.25, 0);
	
	GLenum glew_status = glewInit();
	if (GLEW_OK != glew_status)
	{
		
		std::cout << "Error: " << glewGetErrorString(glew_status) << "\n";
		return 1;
	}
	
	if (!GLEW_VERSION_2_0)
	{
		
		std::cout << "No support for OpenGL 2.0 found\n";
		return 1;
	}
	
	glutReshapeFunc(resizeWindow);
	initShader();
	glutDisplayFunc(renderTriangle);
	glutSpecialFunc(specialKeys);
	glutMainLoop();
	freeShader();
}