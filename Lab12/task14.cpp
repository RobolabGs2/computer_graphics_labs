
GLuint Program;

GLint Unif_color;
double rotate_x = 0;
double rotate_y = 0;
double rotate_z = 0;

void checkOpenGLerror()
{
	GLenum errCode;
	if ((errCode = glGetError()) != GL_NO_ERROR)
		std::cout << "OpenGl error! - " << gluErrorString(errCode);
}

void initShader()
{
	const char* fsSource1 =
		"uniform vec4 color;\n"
		"void main() {\n"
		" gl_FragColor = color;\n"
		"}\n";
	const char* fsSource =
		"uniform vec4 color;\n"
		"void main() {\n"
		" gl_FragColor = gl_Color;\n"
		"}\n";
	GLuint fShader;
	fShader = glCreateShader(GL_FRAGMENT_SHADER);
	glShaderSource(fShader, 1, &fsSource, NULL);
	glCompileShader(fShader);
	Program = glCreateProgram();
	glAttachShader(Program, fShader);
	glLinkProgram(Program);
	int link_ok;
	glGetProgramiv(Program, GL_LINK_STATUS, &link_ok);
	if (!link_ok)
	{
		std::cout << "error attach shaders \n";
		return;
	}
	const char* unif_name = "color";
	Unif_color = glGetUniformLocation(Program, unif_name);
	if (Unif_color == -1)
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
	glRotatef(rotate_x, 1.0, 0.0, 0.0);
	glRotatef(rotate_y, 0.0, 1.0, 0.0);
	glRotatef(rotate_z, 0.0, 0.0, 1.0);
	glUseProgram(Program);
	static float red[4] = { 1.0f, 0.0f, 0.0f, 1.0f };
	glUniform4fv(Unif_color, 1, red);

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