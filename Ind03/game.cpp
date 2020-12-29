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
	illumination(&shaderManager),
	controller(*this)
{
	Init();
}

void Game::Tick(double dt)
{
	if (dt > 0.03)
		dt = 0.03;

	if (!camera.parent->alive)
	{
		AddUserCar({ 0, 2, 10 });
		if (gamemode)
		{
			if (game_bus->alive) game_bus->user = user;
			if (game_tank->alive) game_tank->user = user;
			if (game_gun->alive) game_gun->user = user;
		}
	}

	if (gamemode) {
		if (!game_bus->alive) AddBus({ (float)(rand() % 100 - 50), 2 , float(rand() % 100 - 50) });
		if (!game_tank->alive) AddTank({ (float)(rand() % 100 - 50), 2 , float(rand() % 100 - 50) });
		if (!game_gun->alive) AddGun({ (float)(rand() % 100 - 50), 2 , float(rand() % 100 - 50) });
	}

	glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
	glEnable(GL_DEPTH_TEST);
	glClearDepth(1.0f);

	shaderManager.SetCamera(camera.Projection(), camera.Transform());

	illumination.Tick(dt);
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

Entity* Game::AddCar(Point location, float rotation)
{
	Entity* car = world.AddEntity(location); {
		car->yAngle = rotation;

		Entity* mesh = world.AddTailEntity(car, {0, 0, 0}); {
			mesh->yAngle = 180;
			graphics.AddSimpleColorMesh(mesh, "car.obj");
		}
		controller.AddFallSuicidal(car);
	}
	return car;
}

Entity* Game::AddTank(Point location)
{
	Entity* car = world.AddEntity(location); {
		DynamicCylinder* body = physics.AddDynamicCylinder(car, 0.7, 0.7);
		graphics.AddSimpleColorMesh(car, "Tank.obj");
		if (gamemode) game_tank = controller.AddTank(body, user);
		controller.AddFallSuicidal(car);
	}
	return car;
}


Entity* Game::AddGun(Point location)
{
	Entity* car = world.AddEntity(location); {

		graphics.AddSimpleColorMesh(car, "Gun.obj");
		DynamicCylinder* body = physics.AddDynamicCylinder(car, 0.7, 0.7);
		if (gamemode) game_gun = controller.AddGun(body, user);
		controller.AddFallSuicidal(car);
	}
	return car;
}


Entity* Game::AddBus(Point location)
{
	Entity* car = world.AddEntity(location); {

		graphics.AddSimpleColorMesh(car, "Bus.obj");
		DynamicCylinder* body = physics.AddDynamicCylinder(car, 0.7, 1.1);
		if (gamemode) game_bus = controller.AddBus(body, user);
		controller.AddFallSuicidal(car);
	}
	return car;
}

Entity* Game::AddUserCar(Point location)
{
	Entity* car = AddCar(location, 0); {
		Entity* lightPoint = world.AddTailEntity(car, { 0, 0, 0 });
		SpotLight* headlight = illumination.AddSpotLight(lightPoint, 30, 0);

		DynamicCylinder* body = physics.AddDynamicCylinder(car, 0.6, 0.6); {
			controller.AddCarUser(body, headlight);
			body->friction = { 0.1, 0, 0.01 };
		}

		Entity* cameraPoint = world.AddTailEntity(car, { 0, 3, -5 }); {
			cameraPoint->xAngle = 10;
			camera.parent = cameraPoint;
		}

		user = car;
	}
	return car;
}

//	*****************************************  //
//	**       Описание игрового мира	       **  //
//	*****************************************  //

void Game::GenerateCubes()
{
	Entity* textured2 = world.AddEntity({ 20, 2, 30 }); {
		graphics.AddSimpleTexture2Mesh(textured2, "box.obj");
		physics.AddStaticCube(textured2, { 4, 4, 4 });
	}

	Entity* texturedTextured = world.AddEntity({ 20, 2, 20 }); {
		graphics.AddSimpleTextureTextureMesh(texturedTextured, "box.obj");
		physics.AddStaticCube(texturedTextured, { 4, 4, 4 });
	}

	Entity* textured = world.AddEntity({ 20, 2, 10 }); {
		graphics.AddSimpleTextureMesh(textured, "box.obj");
		physics.AddStaticCube(textured, { 4, 4, 4 });
	}

	Entity* texturedColor = world.AddEntity({ 20, 2, 0 }); {
		graphics.AddSimpleColorTextureMesh(texturedColor, "box.obj");
		physics.AddStaticCube(texturedColor, { 4, 4, 4 });
	}

	Entity* color = world.AddEntity({ 20, 2, -10 }); {
		graphics.AddSimpleColorMesh(color, "box.obj");
		physics.AddStaticCube(color, { 4, 4, 4 });
	}

	Entity* gtextured2 = world.AddEntity({ -20, 2, 30 }); {
		graphics.AddSimpleTexture2Mesh(gtextured2, "box.obj", ShaderManager::GOURAUD);
		physics.AddStaticCube(gtextured2, { 4, 4, 4 });
	}

	Entity* gtexturedTextured = world.AddEntity({ -20, 2, 20 }); {
		graphics.AddSimpleTextureTextureMesh(gtexturedTextured, "box.obj", ShaderManager::GOURAUD);
		physics.AddStaticCube(gtexturedTextured, { 4, 4, 4 });
	}

	Entity* gtextured = world.AddEntity({ -20, 2, 10 }); {
		graphics.AddSimpleTextureMesh(gtextured, "box.obj", ShaderManager::GOURAUD);
		physics.AddStaticCube(gtextured, { 4, 4, 4 });
	}


	Entity* gtexturedColor = world.AddEntity({ -20, 2, 0 }); {
		graphics.AddSimpleColorTextureMesh(gtexturedColor, "box.obj", ShaderManager::GOURAUD);
		physics.AddStaticCube(gtexturedColor, { 4, 4, 4 });
	}

	Entity* gcolor = world.AddEntity({ -20, 2, -10 }); {
		graphics.AddSimpleColorMesh(gcolor, "box.obj", ShaderManager::GOURAUD);
		physics.AddStaticCube(gcolor, { 4, 4, 4 });
	}
}

Entity* Game::AddBullet(Point location, float yAngle, float speed, float v_speed)
{
	Entity* bullet = world.AddEntity(location); {
		bullet->yAngle = yAngle;
		graphics.AddSimpleTextureMesh(bullet, "bullet.obj");

		controller.AddSuicidal(bullet, 5);
		DynamicCylinder* body = physics.AddDynamicCylinder(bullet, 0.2, 0.4); {
			double radYAngle = yAngle * 2 * PI / 360;
			body->velocity.z = -speed * std::cos(radYAngle);
			body->velocity.x = -speed * std::sin(radYAngle);
			body->velocity.y = v_speed;
			body->friction = { 0, 0, 0 };
			controller.AddBullet(body);
		}
	}
	return bullet;
}

void Game::Init()
{
	GenerateCubes();
	AddUserCar({0, 0, 0});

	Entity* car2 = AddTank({ 5,2, -10 });
	Entity* gun = AddGun({ 5,2, -15 });
	Entity* bus = AddBus({ 5,2, -20 });

	Entity* floor = world.AddEntity({ 0, -50, 0 }); {
		Entity* mesh = world.AddTailEntity(floor, { 0, 50, 0 });
		graphics.AddSimpleTextureMesh(mesh, "floor.obj");
		physics.AddStaticCube(floor, { 100, 100, 100 });
	}

	Entity* light1 = world.AddEntity({ 0, 20, 0 }); {
		SpotLight* spotlight = illumination.AddSpotLight(light1);
		controller.AddLightContriller(spotlight);
	}

	Entity* platform = world.AddEntity({ 0, 0, -45 }); {
		platform->yAngle = 90;
		graphics.AddSimpleTextureMesh(platform, "platform.obj");
		physics.AddStaticCube(platform, { 100, 8, 20 });
	}

	for (int i = 0; i < 8; ++i)
	{
		GLfloat idx = (GLfloat)i;
		Entity* textured1 = world.AddEntity({ idx + 20, idx / 2 - 1.99f, -25 - idx }); {
			graphics.AddSimpleTextureMesh(textured1, "box.obj");
			physics.AddStaticCube(textured1, { 4, 4, 4 });
		}

		Entity* textured2 = world.AddEntity({ idx - 20, (8 - idx) / 2 - 1.99f, -25 + idx - 8 }); {
			graphics.AddSimpleTextureMesh(textured2, "box.obj");
			physics.AddStaticCube(textured2, { 4, 4, 4 });
		}
	}
}
