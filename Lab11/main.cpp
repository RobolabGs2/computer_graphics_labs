#include "game.h"

#include <Windows.h>
#include <gl\freeglut.h>

#include <time.h>
#include <iostream>
#include <chrono>


namespace chn = std::chrono;

Game* game = nullptr;
double last_dt = 20;

void GlobalTick() {
	static chn::steady_clock::time_point old_time = chn::steady_clock::now();
	chn::steady_clock::time_point new_time = chn::steady_clock::now();
	last_dt = (chn::duration_cast<chn::duration<double, std::ratio<1>>>(new_time - old_time)).count();
	game->Tick(last_dt);
	old_time = new_time;
}

void Reshape(int width, int height)
{
	game->ResizeWindow(width, height);
}

template<bool value>
void KeyEvent(unsigned char key, int x, int y)
{
	switch (key)
	{	
	case 'q':	game->keys.headlamp = value; return;
	case 'e':	game->keys.lantern = value; game->controller.on = false; return;
	case 'a':	game->keys.left = value; return;
	case 'd':	game->keys.right = value; return;
	case 'w':	game->keys.up = value; return;
	case 's':	game->keys.down = value; return;
	case 'Q':	game->keys.headlamp = value; return;
	case 'E':	game->keys.lantern = value; game->controller.on = false; return;
	case 'A':	game->keys.left = value; return;
	case 'D':	game->keys.right = value; return;
	case 'W':	game->keys.up = value; return;
	case 'S':	game->keys.down = value; return;
	case ' ':	game->keys.space = value; return;
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

void timf(int value)
{
	glutTimerFunc(20, timf, 0);
	glutPostRedisplay();
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
	game = new Game();
	game->gamemode = false;
	glutIdleFunc(NULL);
	glutDisplayFunc(GlobalTick);
	glutReshapeFunc(Reshape);
	glutKeyboardFunc(KeyDown);
	glutKeyboardUpFunc(KeyUp);
	glutTimerFunc(20, timf, 0); // Set up timer for 40ms, about 25 fps
	glutMainLoop();
	delete game;
}
