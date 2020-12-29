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

void Game::Init()
{
	GenerateCubes();
	Entity* user = world.AddEntity({0, 10, 0}); {
		user->yAngle = 0;
		Entity* cameraLocation = world.AddTailEntity(user, { 0, 3, -5}); {
			cameraLocation->xAngle = 10;
			camera.parent = cameraLocation;
		}

		Entity* mesh = world.AddTailEntity(user, { 0, 0, 0 }); {
			mesh->yAngle = 180;
			graphics.AddSimpleColorMesh(mesh, "car.obj");
		}

		Entity* light = world.AddTailEntity(user, { 0, 0, 0}); {
			light->yAngle = 0;
		}
		SpotLight* headlight = illumination.AddSpotLight(light, 30, 0);

		DynamicCylinder* body = physics.AddDynamicCylinder(user, 0.6, 0.6); {
			controller.AddCarUser(body, headlight);
			body->friction = { 0.1, 0, 0.01 };
		}
	}


	Entity* car = world.AddEntity({ 0, 10, 3 }); {
		physics.AddDynamicCylinder(car, 0.6, 0.6);

		Entity* light = world.AddTailEntity(car, { 0, 0, 0 }); {
			light->yAngle = 0;
			illumination.AddSpotLight(light, 30, 0);
		}

		Entity* mesh = world.AddTailEntity(car, { 0, 0, 0 }); {
			mesh->yAngle = 180;
			graphics.AddSimpleColorMesh(mesh, "car.obj");
		}
	}

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
