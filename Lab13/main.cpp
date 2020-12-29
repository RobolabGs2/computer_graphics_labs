#include "world.h"

#include <gl/glew.h>
#include <gl/freeglut.h>
#include <iostream>
#include <time.h>
#include <chrono>

GLfloat rotate_x = 0;
GLfloat rotate_y = 0;
GLfloat coord_y = 0;
GLfloat rotate_z = 0;

Point currentLocation{0, -5, 0};
World world;

void resizeWindow(int width, int height)
{
	glViewport(0, 0, width, height);
	world.aspectRatio = static_cast<GLfloat>(width) / height;
}

Entity* flyCube = nullptr;

void specialKeys(int key, int x, int y)
{
	switch (key)
	{
	case GLUT_KEY_UP: rotate_x = -5;
		break;
	case GLUT_KEY_DOWN: rotate_x = 5;
		break;
	case GLUT_KEY_RIGHT: rotate_y = 5;
		break;
	case GLUT_KEY_LEFT: rotate_y = -5;
		break;
	case GLUT_KEY_HOME: coord_y = -1;
		break;
	case GLUT_KEY_END: coord_y = 1;
		break;
	case GLUT_KEY_PAGE_UP: rotate_z += 5;
		break;
	case GLUT_KEY_PAGE_DOWN: rotate_z += -5;
		break;
	case GLUT_KEY_INSERT: world.lightOn[1] = !world.lightOn[1];break;
	case GLUT_KEY_CTRL_L: world.lightOn[0] = !world.lightOn[0];break;
	case GLUT_KEY_CTRL_R: world.lightOn[2] = !world.lightOn[2];break;
	}
	world.rotation = Matrix::YRotation(rotate_z / 300);
	auto reflectedDirection = (Point{rotate_y / 10, coord_y, rotate_x / 10} * world.rotation);
	reflectedDirection.x = -reflectedDirection.x;
	world.location = Matrix::Move(currentLocation = (currentLocation + reflectedDirection));
	glutPostRedisplay();
	rotate_x = 0;
	rotate_y = 0;
	coord_y = 0;
}
const auto floorWidth = 100;


void InitWorld()
{
	world.projection = Matrix::Projection(0, 0, 1);
	world.lightOn.push_back(true);
	world.lightOn.push_back(false);
	world.lightOn.push_back( true);
	world.lightPos = { {0,10,50},{0,10,0}, {0, 11, 0} };
	auto CubeMesh = TriangleMesh("cube.obj").ToMesh();
	auto HouseMesh = TriangleMesh("House.obj").ToMesh();
	auto LanternMesh = TriangleMesh("lantern.obj").ToMesh();

	Entity* tank = new CompositeEntity<TriangleEntity>(world, "Tank.obj");
	tank->Rotate(Matrix::YRotation(1));
	tank->Move({ -15, 0.5, 0 });
	
	Entity* ctcube = new CompositeEntity<TriangleEntity>(world, "Gun.obj");
	ctcube->Rotate(Matrix::YRotation(1));
	ctcube->Move({-5, 1, 0});
	
	Entity* lantern = new CompositeEntity<TriangleEntity>(world, "lantern.obj");
	lantern->Move({ 16, 0, 0 });
	
	Entity* floor = new CompositeEntity<ColorTexturedEntity>(world, CubeMesh);
	floor->Move({0, -50, 0});
	floor->Scale(floorWidth);
	
	Entity* blinn = new CompositeEntity<TriangleEntity>(world, "Bus.obj");
	blinn->Rotate(Matrix::YRotation(1));
	blinn->Move({5, 0.5, -5});

	flyCube = new CompositeEntity<ColorTexturedEntity>(world, CubeMesh);
	
	auto distanseBetweenHouses = 40;
	for(auto i = 2.5f-floorWidth / 2.0f; i<=floorWidth/2.0f; i+=distanseBetweenHouses)
	{
		for (auto j = 2.5f-floorWidth / 2.0f; j <= floorWidth/2.0f; j += distanseBetweenHouses)
		{
			
			Entity* house = nullptr;
			if (rand() % 2 == 0) {
				house = new CompositeEntity<ColorTexturedEntity>(world, HouseMesh);
			}
			else {
				house = new CompositeEntity<TexturedTexturedEntity>(world, HouseMesh);
			}
			house->Move({ i, 7.5, j });
			world.Add(house);
		}
	}
	
	
	Entity* other = new CompositeEntity<TriangleEntity>(world, CubeMesh);
	other->Rotate(Matrix::XRotation(1));
	other->Rotate(Matrix::YRotation(1));

	world.Add(tank);
	world.Add(flyCube);
	world.Add(lantern);
	world.Add(ctcube);
	world.Add(floor);
	world.Add(blinn);
	world.Add(other);
}

float last_dt = 20;
namespace chn = std::chrono;

void timf(int value)
{
	glutTimerFunc(20, timf, 0);
	glutPostRedisplay();
}

Point location{ 0, 20, 0 };

float mod(float a, int b)
{
	return a - static_cast<int>(a / b)*b;
}

bool divEven(float a, int b)
{
	return static_cast<int>(a / b) % 2 == 0;
}

void DrawWorld()
{
	static chn::steady_clock::time_point old_time = chn::steady_clock::now();
	chn::steady_clock::time_point new_time = chn::steady_clock::now();
	last_dt = (chn::duration_cast<chn::duration<double, std::ratio<1>>>(new_time - old_time)).count()*10;
	float m = mod(last_dt, floorWidth);
	auto x = (m - floorWidth / 2.0f)*(divEven(last_dt, floorWidth)?1:-1);
	world.lightPos[2].x = x;
	flyCube->position = Matrix::Move(x, 10, 0);
	world.Draw();
}

int main(int argc, char** argv)
{
	//setlocale(LC_ALL, "russian");
	glutInit(&argc, argv);
	glutInitDisplayMode(GLUT_RGBA | GLUT_ALPHA | GLUT_DOUBLE);
	glutInitWindowSize(1920, 1080);
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
	glutTimerFunc(20, timf, 0);
	glutMainLoop();
}
