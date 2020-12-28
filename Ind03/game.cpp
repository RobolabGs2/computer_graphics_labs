#include "game.h"
#include "helper.h"

#include <GL\glew.h>
#include <gl\freeglut.h>

#include <math.h>
#include <ctime>

//	*****************************************  //
//	**                Game                 **  //
//	*****************************************  //

Game::Game():
	graphics(&shaderManager),
	controller(*this)
{
	Init();
}

void Game::Tick(double dt)
{
	if (dt > 0.03)
		dt = 0.03;

	glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
	glEnable(GL_DEPTH_TEST);
	glClearDepth(1.0f);

	shaderManager.SetCamera(camera.Projection(), camera.Transform());

	world.Tick(dt);
	physics.Tick(dt);
	graphics.Tick(dt);
	controller.Tick(dt);

	glFlush();
	glutSwapBuffers();
}

void Game::ResizeWindow(int width, int height)
{
	glViewport(0, 0, width, height);
	camera.aspectRatio = width / (float)height;
}

//	*****************************************  //
//	**       Описание игрового мира	       **  //
//	*****************************************  //

void Game::Init()
{
	Entity* user = world.AddEntity({0, 10, 0}); {
		user->yAngle = 10;
		camera.parent = user;
		controller.AddCarUser(physics.AddDynamicCylinder(user, 1, 2));
	}

	Entity* car = world.AddEntity({ 0, -2, -3 }); {
		graphics.AddSimpleMesh(car, "box.obj");
		physics.AddDynamicCylinder(car, 1, 1);
	}

	Entity* floor = world.AddEntity({ 0, -50, 0 }); {
		Entity* mesh = world.AddTailEntity(floor, {0, 50, 0});
		graphics.AddSimpleMesh(mesh, "floor.obj");
		physics.AddStaticCube(floor, {100, 100, 100});
	}
}
