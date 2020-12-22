#include "world.h"

#include <gl/glew.h>
#include <gl/freeglut.h>
#include <iostream>

double rotate_x = 0;
double rotate_y = 0;
double rotate_z = 0;

void resizeWindow(int width, int height)
{
	glViewport(0, 0, width, height);
}

World world;
void specialKeys(int key, int x, int y) {
	switch (key) {
	case GLUT_KEY_UP: rotate_x += 5; break;
	case GLUT_KEY_DOWN: rotate_x -= 5; break;
	case GLUT_KEY_RIGHT: rotate_y += 5; break;
	case GLUT_KEY_LEFT: rotate_y -= 5; break;
	case GLUT_KEY_PAGE_UP: rotate_z += 5; break;
	case GLUT_KEY_PAGE_DOWN: rotate_z -= 5; break;
	}
	world.location = Matrix::Move({ (GLfloat)rotate_y / 10 ,  0,(GLfloat)rotate_x / 10 });
	world.rotation = Matrix::YRotation((GLfloat)rotate_z / 300);
	glutPostRedisplay();
}


void InitWorld()
{
	world.projection = Matrix::Projection(0, 0, 1);

	Entity* t_cube = new TexturedEntity(world, "cube.obj");
	t_cube->Rotate(Matrix::XRotation(1));
	t_cube->Rotate(Matrix::YRotation(1));
	t_cube->Move({5, 0, 0});

	Entity* apple = new TriangleEntity(world, "apple.obj");
	apple->Rotate(Matrix::XRotation(1));
	apple->Rotate(Matrix::YRotation(1));
	apple->Move({ -5, 0, 0 });

	Entity* ctcube = new ColorTexturedEntity(world, "cube.obj");
	ctcube->Rotate(Matrix::XRotation(1));
	ctcube->Rotate(Matrix::YRotation(1));
	ctcube->Move({ -5, 0, 5 });

	Entity* textex = new TexturedTexturedEntity(world, "cube.obj");
	textex->Rotate(Matrix::XRotation(1));
	textex->Rotate(Matrix::YRotation(1));
	textex->Move({ -5, 0, -5 });

	Entity* blinn = new TexturedBlinnEntity(world, "cube.obj");
	blinn->Rotate(Matrix::XRotation(1));
	blinn->Rotate(Matrix::YRotation(1));
	blinn->Move({ 5, 0, -5 });

	Entity* toon = new TexturedToonEntity(world, "cube.obj");
	toon->Rotate(Matrix::XRotation(1));
	toon->Rotate(Matrix::YRotation(1));
	toon->Move({ 5, 0, 5 });

	Entity* other = new TexturedOtherEntity(world, "cube.obj");
	other->Rotate(Matrix::XRotation(1));
	other->Rotate(Matrix::YRotation(1));

	world.Add(apple);
	world.Add(t_cube);
	world.Add(ctcube);
	world.Add(textex);
	world.Add(blinn);
	world.Add(toon);
	world.Add(other);
}

void DrawWorld()
{
	world.Draw();
}

int main(int argc, char** argv)
{
	//setlocale(LC_ALL, "russian");
	glutInit(&argc, argv);
	glutInitDisplayMode(GLUT_RGBA | GLUT_ALPHA | GLUT_DOUBLE);
	glutInitWindowSize(600, 600);
	glutCreateWindow("Simple shaders");
	// ќб€зательно перед инициализацией шейдеров
	GLenum glew_status = glewInit();
	if (GLEW_OK != glew_status)
	{
		// GLEW не проинициализировалась
		std::cout << "Error: " << glewGetErrorString(glew_status) << "\n";
		return 1;
	}
	// ѕровер€ем доступность OpenGL 2.0
	if (!GLEW_VERSION_2_0)
	{
		// OpenGl 2.0 оказалась не доступна
		std::cout << "No support for OpenGL 2.0 found\n";
		return 1;
	}
	// »нициализаци€
	InitWorld();
	glClearColor(0, 0, 0, 0);
	glutReshapeFunc(resizeWindow);
	glutDisplayFunc(DrawWorld);
	glutSpecialFunc(specialKeys);
	glutMainLoop();
}
