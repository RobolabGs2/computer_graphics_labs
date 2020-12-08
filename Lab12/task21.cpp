
double rotate_x = 0;
double rotate_y = 0;
double rotate_z = 0;


//! Переменные с индентификаторами ID
//! ID шейдерной программы
GLuint Program;
//! ID атрибута
GLint Attrib_vertex;
GLint Attrib_vcolor;
//! ID юниформ переменной цвета
GLint Unif_angle;
//! ID Vertex Buffer Object
GLuint Vertex_VBO;
//! ID Color Buffer Object
GLuint Color_VBO;
//! Вершина
struct vertex
{
	GLfloat x;
	GLfloat y;
	GLfloat z;
};
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
//! Проверка ошибок OpenGL, если есть то вывод в консоль тип ошибки
void checkOpenGLerror()
{
	GLenum errCode;
	if ((errCode = glGetError()) != GL_NO_ERROR)
		std::cout << "OpenGl error! - " << gluErrorString(errCode);
}
//! Инициализация шейдеров
void initShader()
{
	//! Исходный код шейдеров
	const char* vsSource = R"(
		attribute vec3 coord;
		attribute vec3 vcolor;
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
			gl_FrontColor = vec4(vcolor, 1.0);
		}
	)";

	GLuint vShader = glCreateShader(GL_VERTEX_SHADER);
	glShaderSource(vShader, 1, &vsSource, NULL);
	glCompileShader(vShader);
	std::cout << "vertex shader \n";
	shaderLog(vShader);

	Program = glCreateProgram();
	glAttachShader(Program, vShader);
	glLinkProgram(Program);
	int link_ok;
	glGetProgramiv(Program, GL_LINK_STATUS, &link_ok);
	if (!link_ok)
	{
		std::cout << "error attach shaders \n";
		return;
	}

	const char* attr_name = "coord";
	Attrib_vertex = glGetAttribLocation(Program, attr_name);
	if (Attrib_vertex == -1)
	{
		std::cout << "could not bind attrib " << attr_name << std::endl;
		return;
	}

	attr_name = "vcolor";
	Attrib_vcolor = glGetAttribLocation(Program, attr_name);
	if (Attrib_vcolor == -1)
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

void initVBO()
{
	vertex vertexes[] = {
		{-0.5, -0.5, -0.5},	{-0.5, 0.5, -0.5},	{-0.5, 0.5, 0.5},	{-0.5, -0.5, 0.5},
		{0.5, -0.5, -0.5},	{0.5, 0.5, -0.5},	{0.5, 0.5, 0.5},	{0.5, -0.5, 0.5},
		{-0.5, -0.5, -0.5}, {-0.5, 0.5, -0.5},	{0.5, 0.5, -0.5},	{0.5, -0.5, -0.5},
		{-0.5, -0.5, 0.5},	{-0.5, 0.5, 0.5},	{0.5, 0.5, 0.5},	{0.5, -0.5, 0.5},
		{-0.5, -0.5, -0.5},	{0.5, -0.5, -0.5},	{0.5, -0.5, 0.5},	{-0.5, -0.5, 0.5},
		{-0.5, 0.5, -0.5},	{0.5, 0.5, -0.5},	{0.5, 0.5, 0.5},	{-0.5, 0.5, 0.5},
	};

	vertex colors[] = {
		{0.0, 1.0, 1.0},	{0.0, 0.0, 1.0},	{0.0, 1.0, 0.0},	{1.0, 1.0, 0.0},
		{1.0, 0.0, 1.0},	{1.0, 0.0, 0.0},	{1.0, 1.0, 1.0},	{0.0, 0.0, 0.0},
		{0.0, 1.0, 1.0},	{0.0, 0.0, 1.0},	{1.0, 0.0, 0.0},	{1.0, 0.0, 1.0},
		{1.0, 1.0, 0.0},	{0.0, 1.0, 0.0},	{1.0, 1.0, 1.0},	{0.0, 0.0, 0.0},
		{0.0, 1.0, 1.0},	{1.0, 0.0, 1.0},	{0.0, 0.0, 0.0},	{1.0, 1.0, 0.0},
		{0.0, 0.0, 1.0},	{1.0, 0.0, 0.0},	{1.0, 1.0, 1.0},	{0.0, 1.0, 0.0},
	};
	glGenBuffers(1, &Vertex_VBO);
	glBindBuffer(GL_ARRAY_BUFFER, Vertex_VBO);
	glBufferData(GL_ARRAY_BUFFER, sizeof(vertexes), vertexes, GL_STATIC_DRAW);
	glGenBuffers(1, &Color_VBO);
	glBindBuffer(GL_ARRAY_BUFFER, Color_VBO);
	glBufferData(GL_ARRAY_BUFFER, sizeof(colors), colors, GL_STATIC_DRAW);
	checkOpenGLerror();
}

void freeShader()
{
	glUseProgram(0);
	glDeleteProgram(Program);
}
//! Освобождение буфера
void freeVBO()
{
	glBindBuffer(GL_ARRAY_BUFFER, 0);
	glDeleteBuffers(1, &Vertex_VBO);
	glDeleteBuffers(1, &Color_VBO);
}
void resizeWindow(int width, int height)
{
	glViewport(0, 0, width, height);
}
//! Отрисовка
void render()
{
	glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
	glClearDepth(1.0f);
	glEnable(GL_DEPTH_TEST);

	//! Устанавливаем шейдерную программу текущей
	glUseProgram(Program);
	double pi = 3.1415926535897932;
	glUniform3f(Unif_angle,
		(GLfloat)(rotate_x * pi / 180),
		(GLfloat)(rotate_y * pi / 180),
		(GLfloat)(rotate_z * pi / 180));

	glEnableVertexAttribArray(Attrib_vertex);
	glEnableVertexAttribArray(Attrib_vcolor);

	glBindBuffer(GL_ARRAY_BUFFER, Vertex_VBO);
	glVertexAttribPointer(Attrib_vertex, 3, GL_FLOAT, GL_FALSE, 0, 0);
	glBindBuffer(GL_ARRAY_BUFFER, Color_VBO);
	glVertexAttribPointer(Attrib_vcolor, 3, GL_FLOAT, GL_FALSE, 0, 0);


	glBindBuffer(GL_ARRAY_BUFFER, 0);
	glDrawArrays(GL_QUADS, 0, 24);
	glDisableVertexAttribArray(Attrib_vertex);
	glDisableVertexAttribArray(Attrib_vcolor);

	glUseProgram(0);
	checkOpenGLerror();
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
	glutInitDisplayMode(GLUT_RGBA | GLUT_ALPHA | GLUT_DOUBLE);
	glutInitWindowSize(600, 600);
	glutCreateWindow("Simple shaders");
	//! Обязательно перед инициализацией шейдеров
	GLenum glew_status = glewInit();
	if (GLEW_OK != glew_status)
	{
		//! GLEW не проинициализировалась
		std::cout << "Error: " << glewGetErrorString(glew_status) << "\n";
		return 1;
	}
	//! Проверяем доступность OpenGL 2.0
	if (!GLEW_VERSION_2_0)
	{
		//! OpenGl 2.0 оказалась не доступна
		std::cout << "No support for OpenGL 2.0 found\n";
		return 1;
	}
	//! Инициализация
	glClearColor(0, 0, 0, 0);
	initVBO();
	initShader();
	glutReshapeFunc(resizeWindow);
	glutDisplayFunc(render);
	glutSpecialFunc(specialKeys);
	glutMainLoop();
	//! Освобождение ресурсов
	freeShader();
	freeVBO();
}
