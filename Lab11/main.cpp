#include "world.h"

#include <Windows.h>
#include <gl\freeglut.h>

#include <time.h>
#include <iostream>
#include <chrono>


namespace chn = std::chrono;

World world;

void GlobalTick() {
	static chn::steady_clock::time_point old_time = chn::steady_clock::now();
	chn::steady_clock::time_point new_time = chn::steady_clock::now();
	world.Tick((chn::duration_cast<chn::duration<double, std::ratio<1>>>(new_time - old_time)).count());
	old_time = new_time;
}

void Reshape(int width, int height)
{
	world.ResizeWindow(width, height);
}

template<bool value>
void KeyEvent(unsigned char key, int x, int y)
{
	switch (key)
	{
	case 'a':	world.keys.left = value; return;
	case 'd':	world.keys.right = value; return;
	case 'w':	world.keys.up = value; return;
	case 's':	world.keys.down = value; return;
	case 'A':	world.keys.left = value; return;
	case 'D':	world.keys.right = value; return;
	case 'W':	world.keys.up = value; return;
	case 'S':	world.keys.down = value; return;
	}
}

void KeyDown(unsigned char key, int x, int y)
{
	KeyEvent<true>(key, x, y);
}

void KeyUp(unsigned char key, int x, int y)
{
	KeyEvent<false>(key, x, y);
}

#ifndef _DEBUG
int WINAPI WinMain(
	_In_ HINSTANCE hInstance,
	_In_opt_ HINSTANCE hPrevInstance,
	_In_ LPSTR lpCmdLine,
	_In_ int nShowCmd
)
{
	int argc = 0;
	char** argv = nullptr;
#else
int main(int argc, char** argv)
{
#endif // _DEBUG
	glutInit(&argc, argv);
	glutInitWindowPosition(100, 100);
	glutInitWindowSize(500, 500);
	glutInitDisplayMode(GLUT_RGBA | GLUT_DOUBLE);
	glutCreateWindow("My window");

	glutIdleFunc(GlobalTick);
	glutDisplayFunc(GlobalTick);
	glutReshapeFunc(Reshape);
	glutKeyboardFunc(KeyDown);
	glutKeyboardUpFunc(KeyUp);
	glutMainLoop();
}
