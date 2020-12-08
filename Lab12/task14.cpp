
GLuint Program;
GLint Unif_angle;

double rotate_x = 0;
double rotate_y = 0;
double rotate_z = 0;

void checkOpenGLerror()
{
	GLenum errCode;
	if ((errCode = glGetError()) != GL_NO_ERROR)
		std::cout << "OpenGl error! - " << gluErrorString(errCode);
}
//! Функция печати лога шейдера
void shaderLog(unsigned int shader)
{
	int infologLen = 0;
	int charsWritten = 0;
	char* infoLog;
	glGetShaderiv(shader, GL_INFO_LOG_LENGTH, &infologLen);
	if (infologLen > 1)
	{
		infoLog = new char[infologLen];
		if (infoLog == NULL)
		{
			std::cout << "ERROR: Could not allocate InfoLog buffer\n";
			exit(1);
		}
		glGetShaderInfoLog(shader, infologLen, &charsWritten, infoLog);
		std::cout << "InfoLog: " << infoLog << "\n\n\n";
		delete[] infoLog;
	}
}

void initShader()
{
	const char* vsSource = R"(
		attribute vec3 coord;
		uniform	vec3 angle;

		void main() {
			float xsin = sin(angle.x);
			float xcos = cos(angle.x);
			float ysin = sin(angle.y);
			float ycos = cos(angle.y);
			float zsin = sin(angle.z);
			float zcos = cos(angle.z);
			vec3 rot_y = vec3(coord.x * ycos + coord.z * ysin,  coord.y, coord.z * ycos - coord.x * ysin);
			vec3 rot_x = vec3(rot_y.x,  rot_y.y * xcos + rot_y.z * xsin, rot_y.z * xcos - rot_y.y * xsin);
			vec3 rot_z = vec3(rot_x.x * zcos - rot_x.y * zsin,  rot_x.y * zcos + rot_x.x * zsin, rot_x.z);
			gl_Position = vec4(rot_z, 1.0);
			gl_FrontColor = gl_Color;
		}
	)";

	const char* fsSource = R"(
			void main() {
				gl_FragColor = gl_Color;
			};
		)";

	GLuint vShader = glCreateShader(GL_VERTEX_SHADER);
	glShaderSource(vShader, 1, &vsSource, NULL);
	glCompileShader(vShader);
	std::cout << "vertex shader \n";
	shaderLog(vShader);

	GLuint fShader = glCreateShader(GL_FRAGMENT_SHADER);
	glShaderSource(fShader, 1, &fsSource, NULL);
	glCompileShader(fShader);
	Program = glCreateProgram();
	glAttachShader(Program, fShader);
	glAttachShader(Program, vShader);
	glLinkProgram(Program);
	int link_ok;
	glGetProgramiv(Program, GL_LINK_STATUS, &link_ok);
	if (!link_ok)
	{
		std::cout << "error attach shaders \n";
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

void freeShader()
{
	glUseProgram(0);
	glDeleteProgram(Program);
}

void resizeWindow(int width, int height)
{
	glViewport(0, 0, width, height);
}

void render2()
{
	glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
	glClearDepth(1.0f);
	glEnable(GL_DEPTH_TEST);

	glLoadIdentity();
	glUseProgram(Program);
	double pi = 3.1415926535897932;
	glUniform3f(Unif_angle,
		(GLfloat)(rotate_x * pi / 180),
		(GLfloat)(rotate_y * pi / 180),
		(GLfloat)(rotate_z * pi / 180));

	glBegin(GL_QUADS); {

		glColor3f(0.0, 1.0, 1.0);
		glVertex3f(-0.5, -0.5, -0.5);
		glColor3f(0.0, 0.0, 1.0);
		glVertex3f(-0.5, 0.5, -0.5);
		glColor3f(0.0, 1.0, 0.0);
		glVertex3f(-0.5, 0.5, 0.5);
		glColor3f(1.0, 1.0, 0.0);
		glVertex3f(-0.5, -0.5, 0.5);

		glColor3f(1.0, 0.0, 1.0);
		glVertex3f(0.5, -0.5, -0.5);
		glColor3f(1.0, 0.0, 0.0);
		glVertex3f(0.5, 0.5, -0.5);
		glColor3f(1.0, 1.0, 1.0);
		glVertex3f(0.5, 0.5, 0.5);
		glColor3f(0.0, 0.0, 0.0);
		glVertex3f(0.5, -0.5, 0.5);

		glColor3f(0.0, 1.0, 1.0);
		glVertex3f(-0.5, -0.5, -0.5);
		glColor3f(0.0, 0.0, 1.0);
		glVertex3f(-0.5, 0.5, -0.5);
		glColor3f(1.0, 0.0, 0.0);
		glVertex3f(0.5, 0.5, -0.5);
		glColor3f(1.0, 0.0, 1.0);
		glVertex3f(0.5, -0.5, -0.5);

		glColor3f(1.0, 1.0, 0.0);
		glVertex3f(-0.5, -0.5, 0.5);
		glColor3f(0.0, 1.0, 0.0);
		glVertex3f(-0.5, 0.5, 0.5);
		glColor3f(1.0, 1.0, 1.0);
		glVertex3f(0.5, 0.5, 0.5);
		glColor3f(0.0, 0.0, 0.0);
		glVertex3f(0.5, -0.5, 0.5);

		glColor3f(0.0, 1.0, 1.0);
		glVertex3f(-0.5, -0.5, -0.5);
		glColor3f(1.0, 0.0, 1.0);
		glVertex3f(0.5, -0.5, -0.5);
		glColor3f(0.0, 0.0, 0.0);
		glVertex3f(0.5, -0.5, 0.5);
		glColor3f(1.0, 1.0, 0.0);
		glVertex3f(-0.5, -0.5, 0.5);

		glColor3f(0.0, 0.0, 1.0);
		glVertex3f(-0.5, 0.5, -0.5);
		glColor3f(1.0, 0.0, 0.0);
		glVertex3f(0.5, 0.5, -0.5);
		glColor3f(1.0, 1.0, 1.0);
		glVertex3f(0.5, 0.5, 0.5);
		glColor3f(0.0, 1.0, 0.0);
		glVertex3f(-0.5, 0.5, 0.5);
	}glEnd();

	glUseProgram(0);
	checkOpenGLerror();
	glFlush();
	glutSwapBuffers();
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

int main(int argc, char** argv)
{
	glutInit(&argc, argv);
	glutInitDisplayMode(GLUT_DEPTH | GLUT_RGBA | GLUT_ALPHA | GLUT_DOUBLE);
	glutInitWindowSize(600, 600);
	glutCreateWindow("Simple shaders");
	glClearColor(0, 0, 1, 0);
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
	initShader();
	glutReshapeFunc(resizeWindow);
	glutDisplayFunc(render2);
	glutSpecialFunc(specialKeys);
	glutMainLoop();
	freeShader();
}